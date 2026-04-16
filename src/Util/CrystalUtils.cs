using System.Diagnostics;
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

    public static InjectionData MakeCrystal(TRGameVersion version)
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

        model.Meshes.Add(model.Meshes[0].Clone());
        model.MeshTrees.Add(new());
        model.Meshes[1].TexturedFaces.ToList().ForEach(f => f.Texture = 1);
        
        foreach (var fr in model.Animations[0].Frames)
        {
            fr.Rotations.Add(new());
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
        level.ObjectTextures.Add(new TRObjectTexture(0, 0, 8, 8));
        level.ObjectTextures.Add(new TRObjectTexture(8, 0, 8, 8));

        var data = InjectionData.Create(level, InjectionType.General, "crystal");
        data.Images.Add(new() { Pixels = img.ToRGBA() });
        return data;
    }
}
