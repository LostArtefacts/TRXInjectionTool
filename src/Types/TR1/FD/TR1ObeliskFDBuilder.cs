using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.FD;

public class TR1ObeliskFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        InjectionData data = InjectionData.Create(InjectionType.FDFix, "obelisk_fd");
        data.FloorEdits = new()
        {
            MakeMusicOneShot(12, 3, 12),
            MakeMusicOneShot(32, 2, 8),
            MakeMusicOneShot(52, 7, 5),
            CreateFlipSwitchTrigger(),
        };

        return new() { data };
    }

    private static TRFloorDataEdit CreateFlipSwitchTrigger()
    {
        // Make the trigger in room 66 a switch type, otherwise Lara need only stand on it to
        // activate the flipmap.
        return new()
        {
            RoomIndex = 66,
            X = 7,
            Z = 5,
            Fixes = new()
            {
                new FDTrigCreateFix
                {
                    Entries = new()
                    {
                        new FDTriggerEntry
                        {
                            TrigType = FDTrigType.Switch,
                            Mask = 31,
                            SwitchOrKeyRef = 34,
                            Actions = new()
                            {
                                new() { Parameter = 60 },
                                new() { Action = FDTrigAction.FlipOff },
                            },
                        },
                    },
                },
            },
        };
    }
}
