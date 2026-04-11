using System.Drawing;
using TRImageControl;
using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types.TR2.Crystals;

public class TR2CrystalBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
        => [.. CreatePlacements(), CreatePSXModel()];

    private static IEnumerable<InjectionData> CreatePlacements()
    {
        var crystals = CrystalUtils.GetLocations("Resources/TR2/crystals.json", TR2Type.SavegameCrystal_P);
        return crystals.Where(kvp => kvp.Value.Count > 0).Select(kvp =>
        {
            var level = _control2.Read($"Resources/{kvp.Key}");
            var data = CreateData(CrystalUtils.Blue, InjectionType.General, $"{_tr2NameMap[kvp.Key]}_crystals");
            CreateDefaultTests(data, kvp.Key);
            data.FloorEdits.AddRange(CrystalUtils.ConvertItems(kvp.Value, r => level.Rooms[r].Info));
            return data;
        });
    }

    private static InjectionData CreatePSXModel()
        => CreateData(CrystalUtils.Purple, InjectionType.PSCrystal, "purple_crystal");

    private static InjectionData CreateData(Color color, InjectionType type, string name)
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.GW}");
        ResetLevel(level, 1);

        var img = new TRImage(TRConsts.TPageWidth, TRConsts.TPageHeight);
        img.Fill(color);
        level.ObjectTextures.Add(new TRObjectTexture(0, 0, 8, 8));

        var caves = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        var model = caves.Models[TR1Type.SavegameCrystal_P];
        model.Meshes.ForEach(m =>
        {
            m.TexturedRectangles.AddRange(m.ColouredRectangles);
            m.TexturedTriangles.AddRange(m.ColouredTriangles);
            m.ColouredRectangles.Clear();
            m.ColouredTriangles.Clear();
            m.TexturedFaces.ToList().ForEach(f => f.Texture = 0);
        });

        level.Models[TR2Type.SavegameCrystal_P] = model;
        level.Images16[0].Pixels = img.ToRGB555();
        level.Images8[0].Pixels = img.ToRGB(level.Palette);

        var data = InjectionData.Create(level, type, name);
        if (type == InjectionType.PSCrystal)
        {
            data.SetMeshOnlyModel((uint)TR2Type.SavegameCrystal_P);
        }
        return data;
    }
}
