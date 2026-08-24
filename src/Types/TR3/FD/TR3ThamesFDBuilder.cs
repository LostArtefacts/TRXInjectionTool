using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.FD;

public class TR3ThamesFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.FDFix, "thames_fd");
        CreateDefaultTests(data, $"TR3/{TR3LevelNames.THAMES}");

        data.ItemFlagEdits.AddRange(FixGuardPatrols());
        data.FloorEdits.Add(FixCrystalTrigger());

        return [data];
    }

    private static IEnumerable<TRItemTypeFlagEdit> FixGuardPatrols()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.THAMES}");
        foreach (var idx in new short[] { 84, 214 })
        {
            yield return ItemBuilder.SetType(level, idx, TR3Type.AIPatrol1_N);
        }
    }

    private static TRFloorDataEdit FixCrystalTrigger()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.THAMES}");
        var trigger = GetTrigger(level, 159, 1, 6);
        return MakeTrigger(level, 181, 1, 1, trigger.Clone() as FDTriggerEntry);
    }
}
