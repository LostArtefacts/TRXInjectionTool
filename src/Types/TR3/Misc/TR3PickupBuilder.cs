using System.Drawing;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types.TR3.Misc;

public class TR3PickupBuilder : InjectionBuilder
{
    private static readonly List<Target> _targets =
    [
        new()
        {
            Level = TR3LevelNames.JUNGLE,
            ObjectFixes = [],
        },
        new()
        {
            Level = TR3LevelNames.CRASH,
            ObjectFixes = new()
            {
                [TR3Type.Quest1_M_H] =
                [
                    (ctx) => FixYaw(ctx.Model),
                ],
            },
        },
        new()
        {
            Level = TR3LevelNames.ALDWYCH,
            ObjectFixes = new()
            {
                [TR3Type.Puzzle2_P] =
                [
                    (ctx) => FixYBounds(ctx.Model),
                ],
            },
        },
        new()
        {
            Level = TR3LevelNames.FISHES,
            ObjectFixes = new()
            {
                [TR3Type.Puzzle1_P] = [ (ctx) => FixCircuitBulbs(ctx.Model) ],
                [TR3Type.Puzzle4_P] = [ (ctx) => FixCircuitBulbs(ctx.Model) ],
                [TR3Type.Puzzle1_M_H] = [ (ctx) => FixCircuitBulbs(ctx.Model) ],
                [TR3Type.Puzzle4_M_H] = [ (ctx) => FixCircuitBulbs(ctx.Model) ],
            },
        },
    ];

    public override List<InjectionData> Build()
    {
        List<InjectionData> result = [.. FixOraDagger(), FixMenuArtefacts()];

        foreach (Target target in _targets)
        {
            var level = _control3.Read($"Resources/TR3/{target.Level}");

            foreach ((var type, var actions) in target.ObjectFixes)
            {
                var ctx = new TR3ModelContext(target.Level, type, level.Models[type]);
                foreach (var action in actions)
                {
                    action(ctx);
                }
            }

            var name = $"{(target.Level == TR3LevelNames.JUNGLE ? "common" : _tr3NameMap[target.Level])}_pickup_meshes";
            var data = CreateData(level, name, target.ObjectFixes.Keys);
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

    private static void FixYBounds(TRModel model)
    {
        foreach (var frame in model.Animations.SelectMany(a => a.Frames))
        {
            frame.Bounds.MinY = -1;
            frame.Bounds.MaxY = 0;
            frame.OffsetY = -1;
        }
    }

    private static IEnumerable<InjectionData> FixOraDagger()
    {
        // Bounding box is inaccurate, meaning it becomes embedded in the floor.
        foreach (var levelName in new[] { TR3LevelNames.PUNA, TR3LevelNames.WILLIE })
        {
            var level = _control3.Read($"Resources/TR3/{levelName}");
            level.Models = new()
            {
                [TR3Type.OraDagger_P] = level.Models[TR3Type.OraDagger_P],
            };

            foreach (var frame in level.Models.Values.SelectMany(m => m.Animations.SelectMany(a => a.Frames)))
            {
                frame.Bounds.MaxY += 75;
            }

            var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.General, $"{_tr3NameMap[levelName]}_pickup_meshes");
            data.FrameReplacements.AddRange(TRFrameReplacement.CreateFrom(level));
            yield return data;
        }
    }

    private static InjectionData FixMenuArtefacts()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.NEVADA}");
        level.Models = new()
        {
            [TR3Type.Infada_M_H] = level.Models[TR3Type.Infada_M_H],
            [TR3Type.Element115_M_H] = level.Models[TR3Type.Element115_M_H],
            [TR3Type.EyeOfIsis_M_H] = level.Models[TR3Type.EyeOfIsis_M_H],
            [TR3Type.OraDagger_M_H] = level.Models[TR3Type.OraDagger_M_H],
        };

        foreach (var frame in level.Models[TR3Type.OraDagger_M_H].Animations.SelectMany(a => a.Frames))
        {
            frame.Bounds.MaxY += 75;
        }

        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.General, "menu_artefacts");
        data.FrameReplacements.AddRange(TRFrameReplacement.CreateFrom(level));
        return data;
    }

    private static void FixCircuitBulbs(TRModel model)
    {
        var frame = model.Animations.First().Frames.First();
        frame.OffsetZ = 11;
        frame.Bounds.MinZ = -22;
        frame.Bounds.MaxZ = 21;
    }

    private static InjectionData CreateData(TR3Level level, string binName, IEnumerable<TR3Type> types)
    {
        FixCDPlayer(level);
        TRDictionary<TR3Type, TRModel> models = [];
        models[TR3Type.CDPlayer_M_H] = level.Models[TR3Type.CDPlayer_M_H];
        foreach (TR3Type type in types)
        {
            models[type] = level.Models[type];
        }

        TRFaceConverter.ConvertFlatFaces(level, [.. level.Palette16.Select(c => c.ToColor())]);

        TR3TexturePacker packer = new(level);
        var regions = packer.GetMeshRegions(models.Values.SelectMany(m => m.Meshes)).Values.SelectMany(v => v);
        List<TRObjectTexture> originalInfos = [.. level.ObjectTextures];

        List<Color> basePalette = [.. level.Palette.Select(c => c.ToTR1Color())];
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

    private static void FixCDPlayer(TR3Level level)
    {
        var mesh = level.Models[TR3Type.CDPlayer_M_H].Meshes[0];
        var verts = new ushort[] { 14, 19, 18, 15 };
        var face = mesh.TexturedFaces.First(f => f.Vertices.All(verts.Contains));
        var texInfo = level.ObjectTextures[face.Texture];
        var tile = new TRImage(level.Images16[texInfo.Atlas].Pixels);
        var img = tile.Export(texInfo.Bounds);
        img.Write((c, x, y) => c.A == 0 ? Color.FromArgb(224, 224, 216) : c);
        tile.Import(img, texInfo.Position);
        level.Images16[texInfo.Atlas].Pixels = tile.ToRGB555();
    }

    private class Target
    {
        public string Level { get; set; }
        public Dictionary<TR3Type, IEnumerable<Action<TR3ModelContext>>> ObjectFixes { get; set; }
    }

    private record TR3ModelContext(string Level, TR3Type Type, TRModel Model);
}
