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
    // TRX slots for the crystal's inventory model. TR3 has one of its own.
    private const int _tr1OptionID = 328;
    private const int _tr2OptionID = 372;

    // How much bigger than the reference menu model the crystal is drawn. The
    // TR1 and TR2 crystal is the slimmer of the two, so it takes more.
    private const double _optionScale = 1.6;
    private const double _optionScale12 = 1.92;

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
            var reference = level1.Models[TR1Type.LargeMed_M_H];
            InjectionBuilder.ResetLevel(level1);
            level1.Models[TR1Type.SavegameCrystal_P] = model;
            level1.Models[(TR1Type)_tr1OptionID] =
                MakeOptionModel(model, reference, _optionScale12);
        }
        else if (level is TR2Level level2)
        {
            var reference = level2.Models[TR2Type.LargeMed_M_H];
            InjectionBuilder.ResetLevel(level2);
            level2.Models[TR2Type.SavegameCrystal_P] = model;
            level2.Models[(TR2Type)_tr2OptionID] =
                MakeOptionModel(model, reference, _optionScale12);
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
        level.Models[TR3Type.SaveCrystal_M_H] =
            MakeOptionModel(model, source.Models[TR3Type.LargeMed_M_H], _optionScale);
        level.ObjectTextures.AddRange(textures);

        var data = InjectionData.Create(level, InjectionType.General, "crystal");
        data.Images.Add(new() { Pixels = img.ToRGBA() });
        return data;
    }

    // The crystal is authored as a floor pickup: it is several times the size
    // of any menu model and its frame offset lifts it well above the origin.
    // Recentre it on the ring's vertical axis and scale it to the reference
    // model's box, which keeps it spinning about its own middle.
    private static TRModel MakeOptionModel(TRModel model, TRModel reference, double factor)
    {
        var refBox = GetDrawnBounds(reference);
        var box = GetDrawnBounds(model);
        var scale = factor
            * Math.Min(
                Math.Min(refBox.Width / box.Width, refBox.Height / box.Height),
                refBox.Depth / box.Depth);
        var target = new Point3(0, refBox.CentreY, 0);

        // The vertices are relative to the frame offset, which the fitted
        // model no longer carries.
        var offset = model.Animations[0].Frames[0];
        var option = model.Clone();
        foreach (var mesh in option.Meshes)
        {
            mesh.CollRadius = (int)Math.Round(mesh.CollRadius * scale);
            mesh.Centre = Fit(mesh.Centre, offset, box, scale, target);
            mesh.Vertices.ForEach(v =>
            {
                var fitted = Fit(v, offset, box, scale, target);
                v.X = fitted.X;
                v.Y = fitted.Y;
                v.Z = fitted.Z;
            });
        }

        // The offset is now part of the vertices, and the model no longer
        // moves, so every frame shares the same bounds.
        foreach (var frame in option.Animations.SelectMany(a => a.Frames))
        {
            frame.OffsetX = frame.OffsetY = frame.OffsetZ = 0;
            frame.Bounds = new()
            {
                MinX = (short)Math.Round(target.X - box.Width * scale / 2),
                MaxX = (short)Math.Round(target.X + box.Width * scale / 2),
                MinY = (short)Math.Round(target.Y - box.Height * scale / 2),
                MaxY = (short)Math.Round(target.Y + box.Height * scale / 2),
                MinZ = (short)Math.Round(target.Z - box.Depth * scale / 2),
                MaxZ = (short)Math.Round(target.Z + box.Depth * scale / 2),
            };
        }

        return option;
    }

    private record Point3(double X, double Y, double Z);

    private record DrawnBounds(double MinX, double MaxX, double MinY, double MaxY, double MinZ, double MaxZ)
    {
        public double Width => MaxX - MinX;
        public double Height => MaxY - MinY;
        public double Depth => MaxZ - MinZ;
        public double CentreX => (MinX + MaxX) / 2;
        public double CentreY => (MinY + MaxY) / 2;
        public double CentreZ => (MinZ + MaxZ) / 2;
    }

    // Where the model actually lands when drawn: its vertices shifted by the
    // frame offset.
    private static DrawnBounds GetDrawnBounds(TRModel model)
    {
        var frame = model.Animations[0].Frames[0];
        var vertices = model.Meshes.SelectMany(m => m.Vertices).ToList();
        return new(
            vertices.Min(v => v.X) + frame.OffsetX, vertices.Max(v => v.X) + frame.OffsetX,
            vertices.Min(v => v.Y) + frame.OffsetY, vertices.Max(v => v.Y) + frame.OffsetY,
            vertices.Min(v => v.Z) + frame.OffsetZ, vertices.Max(v => v.Z) + frame.OffsetZ);
    }

    private static TRVertex Fit(
        TRVertex vertex, TRAnimFrame offset, DrawnBounds box, double scale, Point3 target)
    {
        return new()
        {
            X = Fit(vertex.X + offset.OffsetX, box.CentreX, scale, target.X),
            Y = Fit(vertex.Y + offset.OffsetY, box.CentreY, scale, target.Y),
            Z = Fit(vertex.Z + offset.OffsetZ, box.CentreZ, scale, target.Z),
        };
    }

    private static short Fit(double value, double centre, double scale, double target)
        => (short)Math.Round((value - centre) * scale + target);

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
