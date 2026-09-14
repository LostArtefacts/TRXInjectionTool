using TRLevelControl;
using TRLevelControl.Model;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types.TR3.Crystals;

public class TR3CrystalBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
        => [.. CreatePlacements(), CrystalUtils.MakeTR3Crystal()];

    private static IEnumerable<InjectionData> CreatePlacements()
    {
        var crystals = CrystalUtils.GetLocations("Resources/TR3/crystals.json", TR3Type.SaveCrystal_P);
        return crystals.Where(kvp => kvp.Value.Count > 0).Select(kvp =>
        {
            var level = _control3.Read($"Resources/TR3/{kvp.Key}");
            var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.General, $"{_tr3NameMap[kvp.Key]}_crystals");
            CreateDefaultTests(data, $"TR3/{kvp.Key}");

            // TR3 requires crystals to be triggered or otherwise active by default.
            kvp.Value.ForEach(i => i.Flags = TRConsts.FullMask << 9);
            data.FloorEdits.AddRange(CrystalUtils.ConvertItems(kvp.Value, r => level.Rooms[r].Info));
            return data;
        });
    }
}
