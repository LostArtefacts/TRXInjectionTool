using System.Diagnostics;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.FD;

public class TR1TihocanFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level tihocan = _control1.Read($"Resources/{TR1LevelNames.TIHOCAN}");
        InjectionData data = InjectionData.Create(InjectionType.FDFix, "tihocan_fd");
        data.FloorEdits = new()
        {
            MakeRatTrigger(tihocan),
        };

        return new() { data };
    }

    private static TRFloorDataEdit MakeRatTrigger(TR1Level tihocan)
    {
        // Extend the rat trigger in room 62.
        FDTriggerEntry trigger = GetTrigger(tihocan, 47, 2, 7);
        Debug.Assert(trigger != null);

        return MakeTrigger(tihocan, 62, 2, 7, trigger.Clone() as FDTriggerEntry);
    }
}
