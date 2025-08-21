using System.Diagnostics;
using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;
using LM = TRLevelReader.Model;

namespace TRXInjectionTool.Util;

public static class TRModelExtensions
{
    public static bool IsEquivalent(this TRVertex vertex, TRVertex testVertex)
    {
        return vertex.X == testVertex.X && vertex.Y == testVertex.Y && vertex.Z == testVertex.Z;
    }

    public static TRBoundingBox GetBounds(this TRModel model)
    {
        TRBoundingBox box = new();
        if (model.Meshes.Count == 0)
        {
            return box;
        }

        foreach (TRMesh mesh in model.Meshes)
        {
            TRBoundingBox meshBox = mesh.GetBounds();
            box.MinX = Math.Min(box.MinX, meshBox.MinX);
            box.MinY = Math.Min(box.MinY, meshBox.MinY);
            box.MinZ = Math.Min(box.MinZ, meshBox.MinZ);
            box.MaxX = Math.Max(box.MaxX, meshBox.MaxX);
            box.MaxY = Math.Max(box.MaxY, meshBox.MaxY);
            box.MaxZ = Math.Max(box.MaxZ, meshBox.MaxZ);
        }
        return box;
    }

    public static TRBoundingBox GetBounds(this TRMesh mesh)
    {
        return new()
        {
            MinX = mesh.Vertices.Min(v => v.X),
            MaxX = mesh.Vertices.Max(v => v.X),
            MinY = mesh.Vertices.Min(v => v.Y),
            MaxY = mesh.Vertices.Max(v => v.Y),
            MinZ = mesh.Vertices.Min(v => v.Z),
            MaxZ = mesh.Vertices.Max(v => v.Z),
        };
    }

    public static void Scale(this TRMesh mesh, float scale)
    {
        mesh.Vertices.ForEach(v =>
        {
            v.X = (short)(v.X * scale);
            v.Y = (short)(v.Y * scale);
            v.Z = (short)(v.Z * scale);
        });
    }

    public static void SelfCalculateBounds(this TRMesh mesh)
    {
        TRBoundingBox box = mesh.GetBounds();

        mesh.Centre = new()
        {
            X = (short)((box.MinX + box.MaxX) / 2),
            Y = (short)((box.MinY + box.MaxY) / 2),
            Z = (short)((box.MinZ + box.MaxZ) / 2),
        };

        int xs = Math.Abs(box.MaxX - box.MinX);
        int ys = Math.Abs(box.MaxY - box.MinY);
        int zs = Math.Abs(box.MaxZ - box.MinZ);

        double inner = Math.Max(xs, Math.Max(ys, zs)) / 2d;
        double outer = Math.Sqrt(Math.Pow(xs, 2) + Math.Pow(ys, 2) + Math.Pow(zs, 2)) / 2d;
        mesh.CollRadius = (int)Math.Ceiling((inner + outer) / 2d);
    }

    public static void Rotate(this TRFace face, int rots)
    {
        for (int i = 0; i < rots; i++)
        {
            ushort first = face.Vertices[0];
            for (int j = 0; j < face.Vertices.Count - 1; j++)
            {
                face.Vertices[j] = face.Vertices[j + 1];
            }
            face.Vertices[^1] = first;
        }
    }

    public static void Serialize(this LM.TRModel model, TRLevelWriter writer, TRGameVersion version, bool isMeshOnly)
    {
        writer.Write((int)model.ID, TRObjectType.Game, version);
        writer.Write(model.NumMeshes);
        writer.Write(model.StartingMesh);
        writer.Write(model.MeshTree);
        writer.Write(isMeshOnly ? uint.MaxValue : model.FrameOffset);
        writer.Write(model.Animation);
    }

    public static void Serialize(this LM.TRSpriteSequence sequence, TRLevelWriter writer, TRGameVersion version)
    {
        int sceneryBase = version == TRGameVersion.TR1 ? (int)TR1Type.SceneryBase : (int)TR2Type.SceneryBase;
        TRObjectType type = sequence.SpriteID >= sceneryBase ? TRObjectType.Static2D : TRObjectType.Game;
        writer.Write(sequence.SpriteID, type, version);
        writer.Write(sequence.NegativeLength);
        writer.Write(sequence.Offset);
    }

    public static void Write(this TRLevelWriter writer, int objectID, TRObjectType objectType, TRGameVersion version)
    {
        if (objectType != TRObjectType.Game)
        {
            int sceneryBase = version == TRGameVersion.TR1 ? (int)TR1Type.SceneryBase : (int)TR2Type.SceneryBase;
            Debug.Assert(objectID >= sceneryBase);
            objectID -= sceneryBase;
        }

        writer.Write((int)objectType);
        writer.Write(objectID);
        if (objectType == TRObjectType.Game)
        {
            Guid guid = TRXGuid.Get(version, objectID);
            writer.Write(guid.ToByteArray());
        }
    }
}
