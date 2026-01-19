using System.Drawing;
using TRImageControl.Packing;
using TRImageControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Misc;

public class TR3PickupBuilder : InjectionBuilder
{
    private class Target
    {
        public string Level { get; set; }
        public string BinName { get; set; }
        public Dictionary<TR3Type, IEnumerable<Action<TR3ModelContext>>> ObjectFixes { get; set; }
    }

    private record TR3ModelContext(string Level, TR3Type Type, TRModel Model);

    public override List<InjectionData> Build()
    {
        List<InjectionData> result = new()
        {
            FixOraDagger(),
        };

        foreach (Target target in _targets)
        {
            TR3Level level = _control3.Read($"Resources/TR3/{target.Level}");

            foreach ((var type, var actions) in target.ObjectFixes)
            {
                var ctx = new TR3ModelContext(target.Level, type, level.Models[type]);
                foreach (Action<TR3ModelContext> action in actions)
                {
                    action(ctx);
                }
            }

            _control3.Write(level, MakeOutputPath(TRGameVersion.TR3, $"Debug/{target.Level}"));
            InjectionData data = CreateData(level, target.Level, target.BinName, target.ObjectFixes.Keys);
            result.Add(data);
        }

        return result;
    }

    private static void FixYaw(TRModel model)
    {
        // Make models face the correct way in the inventory
        foreach (var frame in model.Animations.SelectMany(a => a.Frames))
        {
            foreach (var r in frame.Rotations)
            {
                r.Y = (short)(r.Y == 0 ? 512 : 0);
            }
        }
    }

    private static readonly List<Target> _targets = new()
    {
        new()
        {
            Level = TR3LevelNames.CRASH,
            BinName = "crash_pickup_meshes",
            ObjectFixes = new()
            {
                [TR3Type.Quest1_M_H] = new Action<TR3ModelContext>[]
                {
                    (ctx) => FixYaw(ctx.Model),
                },
            },
        },
    };

    private static InjectionData FixOraDagger()
    {
        // Bounding box is inaccurate, meaning it becomes embedded in the floor.
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.PUNA}");
        level.Models = new()
        {
            [TR3Type.OraDagger_P] = level.Models[TR3Type.OraDagger_P],
            [TR3Type.OraDagger_M_H] = level.Models[TR3Type.OraDagger_M_H],
        };

        foreach (var frame in level.Models.Values.SelectMany(m => m.Animations.SelectMany(a => a.Frames)))
        {
            frame.Bounds.MaxY += 75;
        }

        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.General, "ora_dagger");
        data.FrameReplacements.AddRange(TRFrameReplacement.CreateFrom(level));
        return data;
    }

    private static InjectionData CreateData(TR3Level level, string levelName, string binName, IEnumerable<TR3Type> types)
    {
        TRDictionary<TR3Type, TRModel> models = new();
        foreach (TR3Type type in types)
        {
            models[type] = level.Models[type];
        }

        TR3TexturePacker packer = new(level);
        var regions = packer.GetMeshRegions(models.Values.SelectMany(m => m.Meshes)).Values.SelectMany(v => v);
        List<TRObjectTexture> originalInfos = new(level.ObjectTextures);

        List<Color> basePalette = new(level.Palette.Select(c => c.ToTR1Color()));
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

        GenerateImages8(level, basePalette);
        return InjectionData.Create(level, InjectionType.General, binName);
    }
}
