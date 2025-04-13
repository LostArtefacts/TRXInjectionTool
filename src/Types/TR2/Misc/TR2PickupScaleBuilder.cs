using System.Numerics;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types.TR2.Misc;

public class TR2PickupScaleBuilder : InjectionBuilder
{
    private class Target
    {
        public string Level { get; set; }
        public string BinName { get; set; }
        public Dictionary<TR2Type, float> Targets { get; set; }
    }

    private static readonly Dictionary<TR2Type, float> _defaultTargets = new()
    {
        [TR2Type.AutoAmmo_M_H] = 1.4f,
        [TR2Type.M16Ammo_M_H] = 1.4f,
        [TR2Type.Grenades_M_H] = 1.4f,
    };

    private static readonly Dictionary<TR2Type, TRAnimFrame> _animInfo = new()
    {
        [TR2Type.AutoAmmo_M_H] = new()
        {
            Bounds = new()
            {
                MinX = -56, MinY = -35, MinZ = -40,
                MaxX = 57, MaxY = 33, MaxZ = 43
            },
            OffsetX = -26,
            OffsetZ = -3,
        },
        [TR2Type.M16Ammo_M_H] = new()
        {
            Bounds = new()
            {
                MinX = -57, MinY = -29, MinZ = -50,
                MaxX = 50, MaxY = 8, MaxZ = 50
            },
            OffsetX = 22,
        },
        [TR2Type.Grenades_M_H] = new()
        {
            Bounds = new()
            {
                MinX = -60, MinY = -37, MinZ = -19,
                MaxX = 60, MaxY = 38, MaxZ = 22
            },
            OffsetZ = -18,
        },
    };

    private static readonly List<Target> _targets = new()
    {
        new()
        {
            Level = TR2LevelNames.GW,
            BinName = "common_pickup_meshes",
            Targets = new(),
        },
        new()
        {
            Level = TR2LevelNames.RIG,
            BinName = "rig_pickup_meshes",
            Targets = new()
            {
                [TR2Type.Key1_M_H] = 0.65f,
                [TR2Type.Key2_M_H] = 0.65f,
                [TR2Type.Key3_M_H] = 0.65f,
            },
        },
        new()
        {
            Level = TR2LevelNames.DA,
            BinName = "diving_pickup_meshes",
            Targets = new()
            {
                [TR2Type.Key1_M_H] = 0.65f,
                [TR2Type.Key4_M_H] = 0.65f,
            },
        },
        new()
        {
            Level = TR2LevelNames.DORIA,
            BinName = "wreck_pickup_meshes",
            Targets = new()
            {
                [TR2Type.Puzzle1_M_H] = 0.75f,
                [TR2Type.Key1_M_H] = 1.75f,
            },
        },
        new()
        {
            Level = TR2LevelNames.LQ,
            BinName = "living_pickup_meshes",
            Targets = new()
            {
                [TR2Type.Key1_M_H] = 1.75f,
            },
        },
        new()
        {
            Level = TR2LevelNames.DECK,
            BinName = "deck_pickup_meshes",
            Targets = new()
            {
                [TR2Type.Key4_M_H] = 1.75f,
            },
        },
        new()
        {
            Level = TR2LevelNames.MONASTERY,
            BinName = "barkhang_pickup_meshes",
            Targets = new()
            {
                [TR2Type.Puzzle1_M_H] = 0.5f,
                [TR2Type.Puzzle2_M_H] = 2.6f,
                [TR2Type.Key3_M_H] = 1.75f,
            },
        },
        new()
        {
            Level = TR2LevelNames.XIAN,
            BinName = "xian_pickup_meshes",
            Targets = new()
            {
                [TR2Type.Puzzle1_M_H] = 1.8f,
            },
        },
        new()
        {
            Level = TR2LevelNames.FLOATER,
            BinName = "floating_pickup_meshes",
            Targets = new()
            {
                [TR2Type.Puzzle1_M_H] = 1.0f,
                [TR2Type.Puzzle2_M_H] = 1.0f,
            },
        },
        new()
        {
            Level = TR2LevelNames.FOOLGOLD,
            BinName = "fools_pickup_meshes",
            Targets = new()
            {
                [TR2Type.Key1_M_H] = 0.65f,
                [TR2Type.Key4_M_H] = 0.65f,
            },
        },
        new()
        {
            Level = TR2LevelNames.FURNACE,
            BinName = "furnace_pickup_meshes",
            Targets = new()
            {
                [TR2Type.Puzzle2_M_H] = 2.6f,
            },
        },
    };

    public override List<InjectionData> Build()
    {
        List<InjectionData> result = new();

        foreach (Target target in _targets)
        {
            TR2Level level = _control2.Read($"Resources/{target.Level}");
            var typeTargets = target.Targets.Union(_defaultTargets);
            foreach (var (type, factor) in typeTargets)
            {
                Scale(type, level.Models[type], factor);
                Fix(target.Level, type, level.Models[type]);
            }

            _control2.Write(level, MakeOutputPath(TRGameVersion.TR2, $"Debug/{target.Level}"));

            InjectionData data = CreateData(level, target.BinName, typeTargets.Select(k => k.Key));
            result.Add(data);
        }

        return result;
    }

    private static void Scale(TR2Type type, TRModel model, float factor)
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

        TRAnimFrame frameInfo = _animInfo.ContainsKey(type)
            ? _animInfo[type]
            : new() { Bounds = model.GetBounds(), Rotations = new() { new() } };

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

    private static void Fix(string level, TR2Type type, TRModel model)
    {
        if (type == TR2Type.Grenades_M_H)
        {
            FixGrenades(model);
        }
        else if ((level == TR2LevelNames.RIG || level == TR2LevelNames.DA || level == TR2LevelNames.FOOLGOLD)
            && type >= TR2Type.Key1_M_H && type <= TR2Type.Key4_M_H)
        {
            FixYaw(model);
        }
        else if (level == TR2LevelNames.XIAN && type == TR2Type.Puzzle1_M_H)
        {
            FixYaw(model);
        }
        else if (level == TR2LevelNames.FLOATER && (type == TR2Type.Puzzle1_M_H || type == TR2Type.Puzzle2_M_H))
        {
            FixYaw(model);
        }
    }

    private static InjectionData CreateData(TR2Level level, string binName, IEnumerable<TR2Type> types)
    {
        TRDictionary<TR2Type, TRModel> models = new();
        foreach (TR2Type type in types)
        {
            models[type] = level.Models[type];
        }

        TR2TexturePacker packer = new(level);
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

    private static void FixGrenades(TRModel model)
    {
        // The OG tree is a mess and doesn't work well with scaling
        model.MeshTrees[0].OffsetX = -26;
        model.MeshTrees[0].OffsetY = 12;
        model.MeshTrees[0].OffsetZ = 0;
        model.MeshTrees[1].OffsetX = 23;
        model.MeshTrees[1].OffsetY = 12;
        model.MeshTrees[1].OffsetZ = 0;

        // Fix a broken face
        TRMesh mesh = model.Meshes[1];
        TRMeshFace texFace = mesh.TexturedRectangles.First();
        mesh.ColouredRectangles.ForEach(f => f.Texture = texFace.Texture);
        mesh.TexturedRectangles.AddRange(mesh.ColouredRectangles);
        mesh.ColouredRectangles.Clear();

        // The second grenade isn't identical to the first, so just clone
        model.Meshes[2] = mesh.Clone();
        model.Animations.SelectMany(a => a.Frames)
            .ToList()
            .ForEach(f => f.Rotations[2] = f.Rotations[1]);
    }

    private static void FixYaw(TRModel model)
    {
        // Make models face the correct way in the inventory
        model.Animations.SelectMany(a => a.Frames)
            .ToList()
            .ForEach(f => f.Rotations.ForEach(r => r.Y = (short)(r.Y == 0 ? 512 : 0)));
    }
}
