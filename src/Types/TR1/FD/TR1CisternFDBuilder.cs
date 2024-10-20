using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.FD;

public class TR1CisternFDBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        InjectionData data = InjectionData.Create(InjectionType.FDFix, "cistern_fd");
        data.FloorEdits = new()
        {
            MakeKeyTrigger(),
        };

        return new() { data };
    }

    private static TRFloorDataEdit MakeKeyTrigger()
    {
        // Add a pickup trigger in room 56 to avoid softlock in the unflipped state.
        return new()
        {
            RoomIndex = 56,
            X = 1,
            Z = 4,
            Fixes = new()
            {
                new FDTrigCreateFix
                {
                    Entries = new()
                    {
                        new FDTriggerEntry
                        {
                            TrigType = FDTrigType.Pickup,
                            Mask = 31,
                            Actions = new()
                            {
                                new() { Parameter = 67 },
                                new() { Parameter = 104 },
                                new() { Parameter = 65 },
                            },
                        },
                    },
                },
            },
        };
    }
}
