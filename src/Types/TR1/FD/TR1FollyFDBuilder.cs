using System.Diagnostics;
using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.FD;

public class TR1FollyFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level folly = _control1.Read($"Resources/{TR1LevelNames.FOLLY}");
        InjectionData data = InjectionData.Create(InjectionType.FDFix, "folly_fd");
        data.FloorEdits = new()
        {
            MakeMusicOneShot(18, 1, 6),
            CreateKeyMusic(folly),
            ShiftBoulderRoom(),
        };
        data.FloorEdits.AddRange(ShiftNeptuneMusic(folly));

        return new() { data };
    }

    private static TRFloorDataEdit CreateKeyMusic(TR1Level folly)
    {
        // Play track 15 after the keys are all used.
        return MakeTrigger(folly, 54, 1, 3, new()
        {
            TrigType = FDTrigType.HeavyTrigger,
            Mask = TRConsts.FullMask,
            OneShot = true,
            Actions = new()
            {
                new()
                {
                    Action = FDTrigAction.PlaySoundtrack,
                    Parameter = 15
                },
            },
        });
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

    private static List<TRFloorDataEdit> ShiftNeptuneMusic(TR1Level folly)
    {
        // Shift Neptune music trigger into the room itself. Added to two sectors in case the corner is clipped.
        FDTriggerEntry musicTrig = GetTrigger(folly, 4, 4, 1);
        Debug.Assert(musicTrig != null);

        List<TRFloorDataEdit> edits = new()
        {
            RemoveTrigger(folly, 4, 4, 1),
        };

        for (ushort x = 3; x < 5; x++)
        {
            FDTriggerEntry trig = GetTrigger(folly, 4, x, 3);
            trig.Actions.AddRange(musicTrig.Actions);
            trig.OneShot = true;
            edits.Add(MakeTrigger(folly, 4, x, 3, trig));
        }

        return edits;
    }
}
