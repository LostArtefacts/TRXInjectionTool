using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.FD;

public class TR1TihocanFDBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        InjectionData data = InjectionData.Create(InjectionType.FDFix, "tihocan_fd");
        data.FloorEdits = new()
        {
            MakeRatTrigger(),
        };

        return new() { data };
    }

    private static TRFloorDataEdit MakeRatTrigger()
    {
        // Extend the rat trigger in room 62.
        return new()
        {
            RoomIndex = 62,
            X = 2,
            Z = 7,
            Fixes = new()
            {
                new FDTrigCreateFix
                {
                    Entries = new()
                    {
                        new FDTriggerEntry
                        {
                            Mask = 31,
                            Actions = new()
                            {
                                new() { Parameter = 34 },
                            },
                        },
                    },
                },
            },
        };
    }
}
