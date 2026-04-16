using TRLevelControl.Model;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types.TR2.Crystals;

public class TR2CrystalBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
        => [.. CreatePlacements(), CreateModel()];

    private static IEnumerable<InjectionData> CreatePlacements()
    {
        var crystals = CrystalUtils.GetLocations("Resources/TR2/crystals.json", TR2Type.SavegameCrystal_P);
        return crystals.Where(kvp => kvp.Value.Count > 0).Select(kvp =>
        {
            var level = _control2.Read($"Resources/{kvp.Key}");
            var data = InjectionData.Create(TRGameVersion.TR2, InjectionType.General, $"{_tr2NameMap[kvp.Key]}_crystals");
            CreateDefaultTests(data, kvp.Key);
            data.FloorEdits.AddRange(CrystalUtils.ConvertItems(kvp.Value, r => level.Rooms[r].Info));
            return data;
        });
    }

    private static InjectionData CreateModel()
        => CrystalUtils.MakeCrystal(TRGameVersion.TR2);
}
