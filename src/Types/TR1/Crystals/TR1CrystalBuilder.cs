using TRImageControl;
using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types.TR1.Crystals;

public class TR1CrystalBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
        => [.. CreatePlacements(), CreatePSXModel()];

    private static IEnumerable<InjectionData> CreatePlacements()
    {
        var crystals = CrystalUtils.GetLocations("Resources/TR1/crystals.json", TR1Type.SavegameCrystal_P);
        return crystals.Where(kvp => kvp.Value.Count > 0).Select(kvp =>
        {
            var level = _control1.Read($"Resources/{kvp.Key}");
            var data = InjectionData.Create(TRGameVersion.TR1, InjectionType.General, $"{_tr1NameMap[kvp.Key]}_crystals");
            CreateDefaultTests(data, kvp.Key);
            data.FloorEdits.AddRange(CrystalUtils.ConvertItems(kvp.Value, r => level.Rooms[r].Info));
            return data;
        });
    }

    private static InjectionData CreatePSXModel()
    {
        var level = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        var model = level.Models[TR1Type.SavegameCrystal_P];
        ResetLevel(level);
        level.Models[TR1Type.SavegameCrystal_P] = model;

        var img = new TRImage(TRConsts.TPageWidth, TRConsts.TPageHeight);
        img.Fill(CrystalUtils.Purple);
        level.ObjectTextures.Add(new TRObjectTexture(0, 0, 8, 8));

        model.Meshes.ForEach(m =>
        {
            m.TexturedRectangles.AddRange(m.ColouredRectangles);
            m.TexturedTriangles.AddRange(m.ColouredTriangles);
            m.ColouredRectangles.Clear();
            m.ColouredTriangles.Clear();
            m.TexturedFaces.ToList().ForEach(f => f.Texture = 0);
        });

        var data = InjectionData.Create(level, InjectionType.PSCrystal, "purple_crystal");
        data.Images.Add(new() { Pixels = img.ToRGBA() });
        data.SetMeshOnlyModel((uint)TR1Type.SavegameCrystal_P);
        return data;
    }
}
