using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;
using TRXInjectionTool.Types;

namespace TRXInjectionTool.Types.TR3.FD;

public class TR3ScotlandFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.General, "scotland_fd");
        CreateDefaultTests(data, $"TR3/{TR3LevelNames.FLING}");
        data.FloorEdits.AddRange(RemoveFireheadTimers());

        return [data];
    }

    private static IEnumerable<TRFloorDataEdit> RemoveFireheadTimers()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.FLING}");
        for (ushort z = 1; z < 3; z++)
        {
            var trigger = GetTrigger(level, 128, 4, z).Clone() as FDTriggerEntry;
            trigger.Timer = 0;
            yield return MakeTrigger(level, 128, 4, z, trigger);
        }
    }
}
