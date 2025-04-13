using System.Numerics;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types.TR1.Misc;

public class TR1PickupScaleBuilder : InjectionBuilder
{
    private class Target
    {
        public string Level { get; set; }
        public string BinName { get; set; }
        public Dictionary<TR1Type, float> Targets { get; set; }
    }

    private static readonly List<Target> _targets = new()
    {
        new()
        {
            Level = TR1LevelNames.FOLLY,
            BinName = "folly_pickup_meshes",
            Targets = new()
            {
                [TR1Type.Key1_M_H] = 1.75f,
                [TR1Type.Key2_M_H] = 1.75f,
                [TR1Type.Key3_M_H] = 1.75f,
                [TR1Type.Key4_M_H] = 1.75f,
            },
        },
    };

    public override List<InjectionData> Build()
    {
        List<InjectionData> result = new();

        foreach (Target target in _targets)
        {
            TR1Level level = _control1.Read($"Resources/{target.Level}");
            foreach (var (type, factor) in target.Targets)
            {
                Scale(type, level.Models[type], factor);
            }

            _control1.Write(level, MakeOutputPath(TRGameVersion.TR1, $"Debug/{target.Level}"));

            InjectionData data = CreateData(level, target.BinName, target.Targets.Keys);
            result.Add(data);
        }

        return result;
    }

    private static void Scale(TR1Type type, TRModel model, float factor)
    {
        if (factor == 1.0f)
        {
            return;
        }

        TRBoundingBox box;
        foreach (TRMesh mesh in model.Meshes)
        {
            box = mesh.GetBounds();
            int dx = box.MaxX - (Math.Abs(box.MaxX) + Math.Abs(box.MinX)) / 2;
            int dy = box.MaxY - (Math.Abs(box.MaxY) + Math.Abs(box.MinY)) / 2;
            int dz = box.MaxZ - (Math.Abs(box.MaxZ) + Math.Abs(box.MinZ)) / 2;

            mesh.Vertices.ForEach(v =>
            {
                v.X -= (short)dx;
                v.Y -= (short)dy;
                v.Z -= (short)dz;

                Vector3 vec = new(v.X, v.Y, v.Z);
                vec *= factor;
                v.X = (short)vec.X;
                v.Y = (short)vec.Y;
                v.Z = (short)vec.Z;
            });

            mesh.SelfCalculateBounds();
        }

        TRAnimFrame frameInfo = new() { Bounds = model.GetBounds(), Rotations = new() { new() } };

        if (model.Animations.Count == 0)
        {
            model.Animations.Add(new()
            {
                Accel = new(),
                Speed = new(),
                FrameRate = 1,
                Frames = new()
                {
                    frameInfo
                },
            });
        }

        model.Animations.SelectMany(a => a.Frames)
            .ToList()
            .ForEach(f =>
            {
                f.Bounds = frameInfo.Bounds;
                f.OffsetX = frameInfo.OffsetX;
                f.OffsetY = frameInfo.OffsetY;
                f.OffsetZ = frameInfo.OffsetZ;
            });
    }

    private static InjectionData CreateData(TR1Level level, string binName, IEnumerable<TR1Type> types)
    {
        TRDictionary<TR1Type, TRModel> models = new();
        foreach (TR1Type type in types)
        {
            models[type] = level.Models[type];
        }

        TR1TexturePacker packer = new(level);
        var regions = packer.GetMeshRegions(models.Values.SelectMany(m => m.Meshes)).Values.SelectMany(v => v);
        List<TRObjectTexture> originalInfos = new(level.ObjectTextures);

        ResetLevel(level, 1);

        packer = new(level);
        packer.AddRectangles(regions);
        packer.Pack(true);

        level.Models = models;
        level.ObjectTextures.AddRange(regions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));
        models.Values
            .SelectMany(m => m.Meshes)
            .SelectMany(m => m.TexturedFaces)
            .ToList()
            .ForEach(f =>
            {
                f.Texture = (ushort)level.ObjectTextures.IndexOf(originalInfos[f.Texture]);
            });

        return InjectionData.Create(level, InjectionType.General, binName);
    }
}
