using System.Diagnostics;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.FD;

public class TR1ObeliskFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level obelisk = _control1.Read($"Resources/{TR1LevelNames.OBELISK}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.FDFix, "obelisk_fd");
        data.FloorEdits = new()
        {
            MakeMusicOneShot(12, 3, 12),
            MakeMusicOneShot(32, 2, 8),
            MakeMusicOneShot(52, 7, 5),
            CreateFlipSwitchTrigger(obelisk),
        };

        return new() { data };
    }

    private static TRFloorDataEdit CreateFlipSwitchTrigger(TR1Level obelisk)
    {
        // Make the trigger in room 66 a switch type, otherwise Lara need only stand on it to
        // activate the flipmap.
        FDTriggerEntry trigger = GetTrigger(obelisk, 66, 7, 5);
        Debug.Assert(trigger != null);

        trigger.TrigType = FDTrigType.Switch;
        trigger.SwitchOrKeyRef = 34;

        return MakeTrigger(obelisk, 66, 7, 5, trigger);
    }
}
