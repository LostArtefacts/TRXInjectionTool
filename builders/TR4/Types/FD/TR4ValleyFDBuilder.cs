using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;
using TRXInjectionTool.Types;

namespace TRXInjectionTool.Types.TR4.FD;

public class TR4ValleyFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR4, InjectionType.FDFix, "valley_fd");
        CreateDefaultTests(data, $"TR4/{TR4LevelNames.VALLEY}");
        data.FloorEdits.Add(AddHenchmenTriggers());

        return [data];
    }

    private static TRFloorDataEdit AddHenchmenTriggers()
    {
        // OG hard-coded the spawning of items 7 - 11. TRX moves this to Lua
        // and so an in-level trigger is generated beyond the end-level trigger
        // to allow the stats to include the kills.
        var level = _control4.Read($"Resources/TR4/{TR4LevelNames.VALLEY}");
        return MakeTrigger(level, 9, 1, 1, new()
        {
            Mask = 31,
            Actions = [.. Enumerable.Range(7, 5)
                .Select(i => new FDActionItem { Parameter = (short)i })
            ],
        });
    }
}
