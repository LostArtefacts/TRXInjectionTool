using System.Drawing;
using TRImageControl;
using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Util;

public static class CrystalUtils
{
    private static readonly Color _blue = Color.FromArgb(188, 220, 220);
    private static readonly Color _purple = Color.FromArgb(64, 64, 252);
    private static readonly Color _green = Color.FromArgb(64, 252, 64);

    public static Dictionary<string, List<TR1Entity>> GetLocations<T>(string path, T type)
    {
        var locations = JsonUtils.DeserializeFile<Dictionary<string, List<Location>>>(path);
        var crystals = new Dictionary<string, List<TR1Entity>>();
        foreach (var (levelName, levelLocs) in locations)
        {
            crystals[levelName] = [.. levelLocs.Select(l => new TR1Entity
            {
                TypeID = (TR1Type)(uint)(object)type,
                X = l.X,
                Y = l.Y,
                Z = l.Z,
                Room = l.Room,
                Intensity = -1,
            })];
        }

        // The location list is managed through trview, so ensure it is reformatted
        // to avoid line bloat. This assumes the given path matches repo structure.
        JsonUtils.Serialize(locations, $"../../{path}");
        return crystals;
    }

    public static IEnumerable<TRFloorDataEdit> ConvertItems(List<TR1Entity> items, Func<short, TRRoomInfo> getRoomInfo)
    {
        return items.Select(item => new TRFloorDataEdit
        {
            RoomIndex = item.Room,
            X = (ushort)((item.X - getRoomInfo(item.Room).X) / TRConsts.Step4),
            Z = (ushort)((item.Z - getRoomInfo(item.Room).Z) / TRConsts.Step4),
            Fixes = [new FDTrigItem
            {
                Item = item,
            }],
        });
    }

    public static InjectionData MakeCrystal(TRGameVersion version, short collectSFXID)
    {
        var caves = new TR1LevelControl().Read($"Resources/{TR1LevelNames.CAVES}");
        var model = caves.Models[TR1Type.SavegameCrystal_P];
        model.Meshes.ForEach(m =>
        {
            m.TexturedRectangles.AddRange(m.ColouredRectangles);
            m.TexturedTriangles.AddRange(m.ColouredTriangles);
            m.ColouredRectangles.Clear();
            m.ColouredTriangles.Clear();
            m.TexturedFaces.ToList().ForEach(f => f.Texture = 0);
        });

        // One mesh per tint, selected at runtime with mesh_bits. Mesh 0 is the
        // PC crystal, 1 the PS1 tint, 2 the heal crystal.
        for (var i = 1; i < 3; i++)
        {
            model.Meshes.Add(model.Meshes[0].Clone());
            model.MeshTrees.Add(new());
            model.Meshes[i].TexturedFaces.ToList().ForEach(f => f.Texture = (ushort)i);

            foreach (var fr in model.Animations[0].Frames)
            {
                fr.Rotations.Add(new());
            }
        }

        TRLevelBase level = version switch
        {
            TRGameVersion.TR1 => new TR1LevelControl().Read($"Resources/{TR1LevelNames.CAVES}"),
            TRGameVersion.TR2 => new TR2LevelControl().Read($"Resources/{TR2LevelNames.GW}"),
            _ => throw new NotSupportedException(),
        };

        if (level is TR1Level level1)
        {
            InjectionBuilder.ResetLevel(level1);
            level1.Models[TR1Type.SavegameCrystal_P] = model;
        }
        else if (level is TR2Level level2)
        {
            InjectionBuilder.ResetLevel(level2);
            level2.Models[TR2Type.SavegameCrystal_P] = model;
        }

        var img = new TRImage(TRConsts.TPageWidth, TRConsts.TPageHeight);
        img.Fill(new(0, 0, 8, 8), _blue);
        img.Fill(new(8, 0, 8, 8), _purple);
        img.Fill(new(16, 0, 8, 8), _green);
        level.ObjectTextures.Add(new TRObjectTexture(0, 0, 8, 8));
        level.ObjectTextures.Add(new TRObjectTexture(8, 0, 8, 8));
        level.ObjectTextures.Add(new TRObjectTexture(16, 0, 8, 8));

        var data = InjectionData.Create(level, InjectionType.General, "crystal");
        data.Images.Add(new() { Pixels = img.ToRGBA() });

        var jungle = new TR3LevelControl().Read($"Resources/TR3/{TR3LevelNames.JUNGLE}");
        var collectSFX = jungle.SoundEffects[TR3SFX.SaveCrystal];
        collectSFX.Volume = 128;
        data.SFX.Add(TRSFXData.Create(collectSFXID, collectSFX));

        return data;
    }

    // TR3's crystal is green in the level data, so the saving and pickup modes
    // cannot simply tint the light. Republish its textures alongside blue
    // copies, and add a second mesh that uses them.
    public static InjectionData MakeTR3Crystal()
    {
        var source = new TR3LevelControl().Read($"Resources/TR3/{TR3LevelNames.JUNGLE}");
        var model = source.Models[TR3Type.SaveCrystal_P];
        var mesh = model.Meshes[0];

        var img = new TRImage(TRConsts.TPageWidth, TRConsts.TPageHeight);
        var greenTextures = new Dictionary<ushort, ushort>();
        var blueTextures = new Dictionary<ushort, ushort>();
        var textures = new List<TRObjectTexture>();
        var packedRects = new Dictionary<Rectangle, Point>();
        var packX = 0;

        foreach (var texIndex in mesh.TexturedFaces.Select(f => f.Texture).Distinct())
        {
            var texture = source.ObjectTextures[texIndex];
            var bounds = GetBounds(texture);
            if (!packedRects.TryGetValue(bounds, out var origin))
            {
                origin = new(packX, 0);
                packedRects[bounds] = origin;
                packX += bounds.Width;

                var segment = new TRImage(source.Images16[texture.Atlas].Pixels).Export(bounds);
                img.Import(segment, origin, false);
                img.Import(segment, origin with { Y = bounds.Height }, false);
                img.Write(new(origin.X, bounds.Height, bounds.Width, bounds.Height), (c, _, _) => HueRotate(c));
            }

            greenTextures[texIndex] = (ushort)textures.Count;
            textures.Add(Remap(texture, bounds, origin));
            blueTextures[texIndex] = (ushort)textures.Count;
            textures.Add(Remap(texture, bounds, origin with { Y = bounds.Height }));
        }

        // Mesh 0 keeps the original green, mesh 1 is the blue copy.
        model.Meshes.Add(mesh.Clone());
        model.MeshTrees.Add(new());
        foreach (var frame in model.Animations[0].Frames)
        {
            frame.Rotations.Add(new());
        }
        foreach (var face in mesh.TexturedFaces)
        {
            face.Texture = greenTextures[face.Texture];
        }
        foreach (var face in model.Meshes[1].TexturedFaces)
        {
            face.Texture = blueTextures[face.Texture];
        }

        var level = new TR3LevelControl().Read($"Resources/TR3/{TR3LevelNames.JUNGLE}");
        InjectionBuilder.ResetLevel(level);
        level.Models[TR3Type.SaveCrystal_P] = model;
        level.ObjectTextures.AddRange(textures);

        var data = InjectionData.Create(level, InjectionType.General, "crystal");
        data.Images.Add(new() { Pixels = img.ToRGBA() });
        return data;
    }

    // Object textures store four vertices; a triangle leaves the last unused.
    private static IEnumerable<TRObjectTextureVert> RealVertices(TRObjectTexture texture)
        => texture.Vertices.Where(v => v.X != 0 || v.Y != 0);

    private static Rectangle GetBounds(TRObjectTexture texture)
    {
        var xs = RealVertices(texture).Select(v => (int)v.X).ToList();
        var ys = RealVertices(texture).Select(v => (int)v.Y).ToList();
        return new(xs.Min(), ys.Min(), xs.Max() - xs.Min() + 1, ys.Max() - ys.Min() + 1);
    }

    private static TRObjectTexture Remap(TRObjectTexture texture, Rectangle bounds, Point origin)
    {
        var copy = texture.Clone();
        copy.Atlas = 0;
        foreach (var vertex in RealVertices(copy).ToList())
        {
            vertex.X = (byte)(origin.X + vertex.X - bounds.X);
            vertex.Y = (byte)(origin.Y + vertex.Y - bounds.Y);
        }
        return copy;
    }

    // The crystal sits around 119 degrees of hue; this lands it near 219.
    private static Color HueRotate(Color color)
    {
        var hue = (color.GetHue() + 100) % 360;
        return FromHSV(hue, color.GetSaturation(), color.GetBrightness(), color.A);
    }

    private static Color FromHSV(float hue, float saturation, float lightness, int alpha)
    {
        var c = (1 - Math.Abs(2 * lightness - 1)) * saturation;
        var x = c * (1 - Math.Abs(hue / 60 % 2 - 1));
        var m = lightness - c / 2;
        var (r, g, b) = hue switch
        {
            < 60 => (c, x, 0f),
            < 120 => (x, c, 0f),
            < 180 => (0f, c, x),
            < 240 => (0f, x, c),
            < 300 => (x, 0f, c),
            _ => (c, 0f, x),
        };
        return Color.FromArgb(
            alpha,
            (int)Math.Round((r + m) * 255),
            (int)Math.Round((g + m) * 255),
            (int)Math.Round((b + m) * 255));
    }
}
