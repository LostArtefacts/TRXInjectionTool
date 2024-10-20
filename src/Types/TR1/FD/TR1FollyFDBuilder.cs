using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.FD;

public class TR1FollyFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        InjectionData data = InjectionData.Create(InjectionType.FDFix, "folly_fd");
        data.FloorEdits = new()
        {
            MakeMusicOneShot(18, 1, 6),
            CreateKeyMusic(),
            ShiftBoulderRoom(),
        };
        data.FloorEdits.AddRange(ShiftNeptuneMusic());

        return new() { data };
    }

    private static TRFloorDataEdit CreateKeyMusic()
    {
        // Play track 15 after the keys are all used.
        return new()
        {
            RoomIndex = 54,
            X = 1,
            Z = 3,
            Fixes = new()
            {
                new FDTrigCreateFix
                {
                    Entries = new()
                    {
                        new FDTriggerEntry
                        {
                            TrigType = FDTrigType.HeavyTrigger,
                            Mask = 31,
                            OneShot = true,
                            Actions = new()
                            {
                                new()
                                {
                                    Action = FDTrigAction.PlaySoundtrack,
                                    Parameter = 15
                                },
                            },
                        },
                    },
                },
            },
        };
    }

    private static TRFloorDataEdit ShiftBoulderRoom()
    {
        // Shift the room that holds the boulder that opens the exit door
        // so the player can't hear it rolling.
        return new()
        {
            RoomIndex = 54,
            Fixes = new()
            {
                new FDRoomShift
                {
                    YShift = -11520,
                },
            },
        };
    }

    private static List<TRFloorDataEdit> ShiftNeptuneMusic()
    {
        // Shift Neptune music trigger into the room itself. Added to two sectors in case the corner is clipped.
        List<TRFloorDataEdit> edits = new()
        {
            new()
            {
                RoomIndex = 4,
                X = 4,
                Z = 1,
                Fixes = new()
                {
                    new FDTrigCreateFix
                    {
                        Entries = new()
                        {
                            new FDSlantEntry
                            {
                                Type = FDSlantType.Ceiling,
                                ZSlant = 1
                            },
                        },
                    },
                },
            },
        };


        FDTrigCreateFix music = new()
        {
            Entries = new()
            {
                new FDTriggerEntry
                {
                    TrigType = FDTrigType.Trigger,
                    Mask = 31,
                    OneShot = true,
                    Actions = new()
                    {
                        new()
                        {
                            Action = FDTrigAction.FlipOn,
                            Parameter = 1
                        },
                        new()
                        {
                            Action = FDTrigAction.PlaySoundtrack,
                            Parameter = 3
                        },
                    },
                },
            },
        };

        for (ushort x = 3; x < 5; x++)
        {
            edits.Add(new()
            {
                RoomIndex = 4,
                X = x,
                Z = 3,
                Fixes = new() { music },
            });
        }

        return edits;
    }
}
