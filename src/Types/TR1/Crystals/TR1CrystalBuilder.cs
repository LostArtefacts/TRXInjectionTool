using TRLevelControl.Model;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types.TR1.Crystals;

public class TR1CrystalBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
        => [.. CreatePlacements(), CreateModel()];

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

    private static InjectionData CreateModel()
        => CrystalUtils.MakeCrystal(TRGameVersion.TR1);
}
