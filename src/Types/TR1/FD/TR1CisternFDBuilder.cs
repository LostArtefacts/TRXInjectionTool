using System.Diagnostics;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.FD;

public class TR1CisternFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level cistern = _control1.Read($"Resources/{TR1LevelNames.CISTERN}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.FDFix, "cistern_fd");
        CreateDefaultTests(data, TR1LevelNames.CISTERN);
        data.FloorEdits = new()
        {
            MakeKeyTrigger(cistern),
        };

        return new() { data };
    }

    private static TRFloorDataEdit MakeKeyTrigger(TR1Level cistern)
    {
        // Add a pickup trigger in room 56 to avoid softlock in the unflipped state.
        FDTriggerEntry trigger = GetTrigger(cistern, 72, 1, 4);
        Debug.Assert(trigger != null);

        return MakeTrigger(cistern, 56, 1, 4, trigger.Clone() as FDTriggerEntry);
    }
}
