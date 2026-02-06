using System.Globalization;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types;

public static class LaraBackGunMeshEditor
{
    private const int _headMesh = 14; // LM_HEAD

    private static readonly Dictionary<Type, object> _laraLookup = new()
    {
        { typeof(TR1Type), TR1Type.Lara },
        { typeof(TR2Type), TR2Type.Lara },
        { typeof(TR3Type), TR3Type.Lara },
    };

    public static void AddBackGunMeshEdits<TType>(
        IDictionary<TType, TRModel> referenceModels,
        IDictionary<TType, TRModel> gunModels,
        IReadOnlyCollection<TType> backGunTypes,
        InjectionData data)
        where TType : struct, Enum
    {
        TType laraType = (TType)_laraLookup[typeof(TType)];

        if (!TryGetLaraTorsoToHeadNode(referenceModels, laraType, out var node))
        {
            throw new InvalidOperationException(
                "Lara torso/head node is missing; cannot generate back-gun mesh edits.");
        }

        foreach (var type in backGunTypes)
        {
            if (!gunModels.TryGetValue(type, out var sourceModel)
                && !referenceModels.TryGetValue(type, out sourceModel))
            {
                continue;
            }

            if (!TryBuildModelMeshEdits(type, sourceModel, node, out var meshEdits))
            {
                continue;
            }

            data.MeshEdits.AddRange(meshEdits);
        }
    }

    private static bool TryBuildModelMeshEdits<TType>(
        TType modelId,
        TRModel sourceModel,
        TRMeshTreeNode node,
        out List<TRMeshEdit> meshEdits)
        where TType : struct, Enum
    {
        meshEdits = [];
        if (!TryGetHeadRotation(sourceModel, out var rot))
        {
            return false;
        }

        var transformedModel = sourceModel.Clone();
        ApplyModelTransform(transformedModel, node, rot);

        int meshCount = Math.Min(sourceModel.Meshes.Count, transformedModel.Meshes.Count);
        for (int meshIndex = 0; meshIndex < meshCount; meshIndex++)
        {
            var sourceMesh = sourceModel.Meshes[meshIndex];
            var transformedMesh = transformedModel.Meshes[meshIndex];

            var meshEdit = new TRMeshEdit
            {
                ModelID = Convert.ToUInt32(modelId, CultureInfo.InvariantCulture),
                EnforcedType = TRObjectType.Game,
                MeshIndex = (short)meshIndex,
                Centre = sourceMesh.Centre,
                VertexEdits = [],
            };

            int count = Math.Min(sourceMesh.Vertices.Count, transformedMesh.Vertices.Count);
            for (int i = 0; i < count; i++)
            {
                var shift = CreateShift(sourceMesh.Vertices[i], transformedMesh.Vertices[i]);
                if (shift.X == 0 && shift.Y == 0 && shift.Z == 0)
                {
                    continue;
                }

                meshEdit.VertexEdits.Add(new TRVertexEdit
                {
                    Index = (short)i,
                    Change = shift,
                });
            }

            if (meshEdit.VertexEdits.Count > 0)
            {
                meshEdits.Add(meshEdit);
            }
        }

        return true;
    }

    private static void ApplyModelTransform(
        TRModel model,
        TRMeshTreeNode node,
        TRAnimFrameRotation rot)
    {
        foreach (var mesh in model.Meshes)
        {
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                mesh.Vertices[i] = ApplyHeadTransform(mesh.Vertices[i], node, rot);
            }
            mesh.SelfCalculateBounds();
        }
    }

    private static bool TryGetLaraTorsoToHeadNode<TType>(
        IDictionary<TType, TRModel> models,
        TType laraType,
        out TRMeshTreeNode node)
        where TType : struct, Enum
    {
        node = new();
        if (!models.TryGetValue(laraType, out var lara) || lara.MeshTrees.Count < _headMesh)
        {
            return false;
        }

        node = lara.MeshTrees[_headMesh - 1];
        return true;
    }

    private static bool TryGetHeadRotation(TRModel model, out TRAnimFrameRotation rotation)
    {
        rotation = new();
        if (model.Animations.Count == 0
            || model.Animations[0].Frames.Count == 0
            || model.Animations[0].Frames[0].Rotations.Count <= _headMesh)
        {
            return false;
        }

        rotation = model.Animations[0].Frames[0].Rotations[_headMesh];
        return true;
    }

    private static TRVertex ApplyHeadTransform(
        TRVertex vertex,
        TRMeshTreeNode node,
        TRAnimFrameRotation rotation)
    {
        const float angleToRadians = MathF.Tau / 1024.0f;
        double x = vertex.X;
        double y = vertex.Y;
        double z = vertex.Z;

        double sy = Math.Sin(rotation.Y * angleToRadians);
        double cy = Math.Cos(rotation.Y * angleToRadians);
        double sx = Math.Sin(rotation.X * angleToRadians);
        double cx = Math.Cos(rotation.X * angleToRadians);
        double sz = Math.Sin(rotation.Z * angleToRadians);
        double cz = Math.Cos(rotation.Z * angleToRadians);

        double x1 = x * cy + z * sy;
        double y1 = y;
        double z1 = z * cy - x * sy;

        double x2 = x1;
        double y2 = y1 * cx + z1 * sx;
        double z2 = z1 * cx - y1 * sx;

        double x3 = x2 * cz + y2 * sz;
        double y3 = y2 * cz - x2 * sz;
        double z3 = z2;

        return new TRVertex
        {
            X = ClampToInt16((int)Math.Round(x3 + node.OffsetX)),
            Y = ClampToInt16((int)Math.Round(y3 + node.OffsetY)),
            Z = ClampToInt16((int)Math.Round(z3 + node.OffsetZ)),
        };
    }

    private static short ClampToInt16(int value)
        => (short)Math.Clamp(value, short.MinValue, short.MaxValue);

    private static TRVertex CreateShift(TRVertex source, TRVertex target)
        => new()
        {
            X = ClampToInt16(target.X - source.X),
            Y = ClampToInt16(target.Y - source.Y),
            Z = ClampToInt16(target.Z - source.Z),
        };
}
