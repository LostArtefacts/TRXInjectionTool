using System.Diagnostics;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;
using TRXInjectionTool.Model;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types.TRX.Misc;

// Adds the models that the controls item of the inventory can show to every
// game, by name: the Sega Saturn pad, a keyboard, and the PlayStation pad that
// only Floating Islands carries. TR1 and TR2 each get their own keyboard, and
// TR3 and TR4 get the one from TR3, which has a finer texture.
public class ControlsBuilder : InjectionBuilder
{
    public override string ID => "controls_models";

    private const string _saturnName = "controls_option_saturn";
    private const string _keysName = "controls_option_keys";
    private const string _ps1Name = "controls_option_ps1";

    // Keys that hold the models in the base level until each game names them.
    private const TR3Type _saturnType = (TR3Type)1000;
    private const TR3Type _tr1KeysType = (TR3Type)1001;
    private const TR3Type _tr2KeysType = (TR3Type)1002;
    private const TR3Type _ps1Type = (TR3Type)1003;
    private const TR3Type _tr3KeysType = (TR3Type)1004;

    // The PlayStation pad leans back from the keyboard by this many degrees
    // about X, once it is turned to face the same way.
    private const double _ps1Lean = -8.09;

    public override List<InjectionData> Build()
    {
        var baseLevel = CreateBaseLevel();
        var images = baseLevel.Images16.Select(i => new TRImage(i.Pixels)).ToList();
        return
        [
            CreateData(CreateTR1Level(baseLevel), images),
            CreateData(CreateTR2Level(baseLevel), images),
            CreateData(CreateTR3Level(baseLevel), images),
            CreateTR4Data(baseLevel, images),
        ];
    }

    private static TR3Level CreateBaseLevel()
    {
        var caves = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        var stronghold = _control1.Read($"Resources/{TR1LevelNames.STRONGHOLD}");
        var wall = _control2.Read($"Resources/{TR2LevelNames.GW}");
        var floater = _control2.Read($"Resources/{TR2LevelNames.FLOATER}");
        var jungle = _control3.Read($"Resources/TR3/{TR3LevelNames.JUNGLE}");

        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.JUNGLE}");
        ResetLevel(level, 1);

        Import(level, _saturnType, caves.Models[TR1Type.DirectionKeys_M_H],
            new TR1TexturePacker(caves), caves.ObjectTextures);
        Import(level, _tr1KeysType, stronghold.Models[TR1Type.DirectionKeys_M_H],
            new TR1TexturePacker(stronghold), stronghold.ObjectTextures);
        Import(level, _tr2KeysType, wall.Models[TR2Type.DirectionKeys_M_H],
            new TR2TexturePacker(wall), wall.ObjectTextures);
        Import(level, _tr3KeysType, jungle.Models[TR3Type.DirectionKeys_M_H],
            new TR3TexturePacker(jungle), jungle.ObjectTextures);
        var keys = wall.Models[TR2Type.DirectionKeys_M_H];
        Import(level, _ps1Type, MatchKeys(floater.Models[TR2Type.DirectionKeys_M_H], keys),
            new TR2TexturePacker(floater), floater.ObjectTextures);

        // Each TR1 level has a palette of its own, which its flat faces index.
        foreach (var (type, source) in new[] { (_saturnType, caves), (_tr1KeysType, stronghold) })
        {
            TRFaceConverter.ConvertFlatFaces(
                level,
                [.. source.Palette.Select(c => c.ToTR1Color())],
                level.Models[type].Meshes,
                texture => texture);
        }
        TRFaceConverter.ConvertFlatFaces(
            level,
            [.. jungle.Palette16.Select(c => c.ToColor())],
            level.Models[_tr3KeysType].Meshes);
        Debug.Assert(level.Models.Values.All(m => !m.Meshes.Any(x => x.ColouredFaces.Any())));
        Debug.Assert(level.Images16.Count == 1);
        return level;
    }

    private static void Import(
        TR3Level level,
        TR3Type type,
        TRModel model,
        TRTexturePacker sourcePacker,
        List<TRObjectTexture> sourceTextures)
    {
        model = model.Clone();
        var regions = sourcePacker.GetMeshRegions(model.Meshes)
            .Values.SelectMany(v => v)
            .ToList();

        var packer = new TR3TexturePacker(level);
        packer.AddRectangles(regions);
        packer.Pack(true);

        level.ObjectTextures.AddRange(regions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));
        model.Meshes
            .SelectMany(m => m.TexturedFaces)
            .Distinct()
            .ToList()
            .ForEach(f =>
            {
                f.Texture = (ushort)level.ObjectTextures.IndexOf(sourceTextures[f.Texture]);
                Debug.Assert(f.Texture < level.ObjectTextures.Count);
            });
        level.Models[type] = model;
    }

    // Turns the PlayStation pad to face the way the keyboard does, stands it as
    // upright as the keyboard, centres it on the keyboard, sizes it to the
    // keyboard's width, and gives it the keyboard's frame. The ring then poses
    // the pad as it poses the keyboard. The pad faces the other way from the
    // keyboard, and its pivot lies far from its middle.
    private static TRModel MatchKeys(TRModel pad, TRModel keys)
    {
        pad = pad.Clone();
        var mesh = pad.Meshes[0];
        var (padMin, padMax) = Extent(mesh.Vertices);
        var (keysMin, keysMax) = Extent(keys.Meshes[0].Vertices);
        var scale = (keysMax.X - keysMin.X) / (padMax.X - padMin.X);

        var lean = _ps1Lean * Math.PI / 180;
        var (cos, sin) = (Math.Cos(lean), Math.Sin(lean));

        // Turns a direction half a circle about Y, then straightens it about X.
        (double X, double Y, double Z) Turn(double x, double y, double z)
            => (-x, y * cos + z * sin, y * sin - z * cos);

        TRVertex Place(TRVertex v)
        {
            var (x, y, z) = Turn(
                v.X - (padMin.X + padMax.X) / 2,
                v.Y - (padMin.Y + padMax.Y) / 2,
                v.Z - (padMin.Z + padMax.Z) / 2);
            return new()
            {
                X = (short)Math.Round(x * scale + (keysMin.X + keysMax.X) / 2),
                Y = (short)Math.Round(y * scale + (keysMin.Y + keysMax.Y) / 2),
                Z = (short)Math.Round(z * scale + (keysMin.Z + keysMax.Z) / 2),
            };
        }

        mesh.Vertices = [.. mesh.Vertices.Select(Place)];
        mesh.Centre = Place(mesh.Centre);
        mesh.CollRadius = (int)Math.Ceiling(mesh.CollRadius * scale);
        mesh.Normals?.ForEach(n =>
        {
            var (x, y, z) = Turn(n.X, n.Y, n.Z);
            n.X = (short)Math.Round(x);
            n.Y = (short)Math.Round(y);
            n.Z = (short)Math.Round(z);
        });

        var frame = keys.Animations[0].Frames[0].Clone();
        pad.Animations[0].Frames[0] = frame;
        frame.Bounds = TRAnimBoundsCalculator.ComputeFrameBounds(pad, frame);
        return pad;
    }

    private static (System.Numerics.Vector3, System.Numerics.Vector3) Extent(List<TRVertex> vertices)
        => (new(vertices.Min(v => v.X), vertices.Min(v => v.Y), vertices.Min(v => v.Z)),
            new(vertices.Max(v => v.X), vertices.Max(v => v.Y), vertices.Max(v => v.Z)));

    private static NamedModels Name(TR3Level baseLevel, TR3Type keysType)
    {
        var models = new NamedModels();
        models.Add(_saturnName, baseLevel.Models[_saturnType]);
        models.Add(_keysName, baseLevel.Models[keysType]);
        models.Add(_ps1Name, baseLevel.Models[_ps1Type]);
        return models;
    }

    private static (TRLevelBase, List<TRSymbol>) CreateTR1Level(TR3Level baseLevel)
    {
        var caves = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        ResetLevel(caves);
        caves.ObjectTextures.AddRange(baseLevel.ObjectTextures);
        return (caves, Name(baseLevel, _tr1KeysType).AddTo(caves));
    }

    private static (TRLevelBase, List<TRSymbol>) CreateTR2Level(TR3Level baseLevel)
    {
        var wall = _control2.Read($"Resources/{TR2LevelNames.GW}");
        ResetLevel(wall);
        wall.ObjectTextures.AddRange(baseLevel.ObjectTextures);
        return (wall, Name(baseLevel, _tr2KeysType).AddTo(wall));
    }

    private static (TRLevelBase, List<TRSymbol>) CreateTR3Level(TR3Level baseLevel)
    {
        var jungle = _control3.Read($"Resources/TR3/{TR3LevelNames.JUNGLE}");
        ResetLevel(jungle);
        jungle.ObjectTextures.AddRange(baseLevel.ObjectTextures);
        return (jungle, Name(baseLevel, _tr3KeysType).AddTo(jungle));
    }

    private static InjectionData CreateData((TRLevelBase, List<TRSymbol>) named, List<TRImage> images)
    {
        var (level, symbols) = named;
        var data = InjectionData.Create(level, InjectionType.General, "controls_models");
        data.Symbols.AddRange(symbols);
        data.Images.AddRange(images.Select(i => new TRTexImage32 { Pixels = i.ToRGBA() }));
        return data;
    }

    private static InjectionData CreateTR4Data(TR3Level baseLevel, List<TRImage> images)
    {
        var level = _control4.Read($"Resources/TR4/{TR4LevelNames.SETH}");
        ResetLevel(level);
        level.ObjectTextures.AddRange(baseLevel.ObjectTextures);
        level.Images.Objects.Images16.AddRange(images.Select(i => new TRTexImage16 { Pixels = i.ToRGB555() }));
        level.Images.Objects.Images32.AddRange(images.Select(i => new TRTexImage32 { Pixels = i.ToRGB32() }));
        var symbols = Name(baseLevel, _tr3KeysType).AddTo(level);

        var data = InjectionData.Create(level, InjectionType.General, "controls_models");
        data.Symbols.AddRange(symbols);
        return data;
    }
}
