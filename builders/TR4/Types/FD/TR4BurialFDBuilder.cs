using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR4.FD;

public class TR4BurialFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR4, InjectionType.FDFix, "burial_fd");
        CreateDefaultTests(data, $"TR4/{TR4LevelNames.BURIAL}");
        data.FloorEdits.Add(FixFlameEmitterTrigger());

        return [data];
    }

    private static TRFloorDataEdit FixFlameEmitterTrigger()
    {
        var level = _control4.Read($"Resources/TR4/{TR4LevelNames.BURIAL}");
        var trigger = GetTrigger(level, 110, 1, 13).Clone() as FDTriggerEntry;
        return MakeTrigger(level, 110, 1, 14, trigger);
    }
}
