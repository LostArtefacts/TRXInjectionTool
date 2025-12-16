using System.Diagnostics;
using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.FD;

public class TR2FloatingFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level floating = _control2.Read($"Resources/{TR2LevelNames.FLOATER}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.FDFix, "floating_fd");
        CreateDefaultTests(data, TR2LevelNames.FLOATER);

        // Rotate and shift the door that leads to room 86, otherwise there is an invisible wall.
        floating.Entities[72].X += TRConsts.Step4;
        data.ItemPosEdits.Add(ItemBuilder.SetAngle(floating, 72, 16384));

        data.FloorEdits.AddRange(FixZiplineReset(floating));
        data.FloorEdits.Add(FixMusicTrigger(floating));
        data.FloorEdits.Add(FixDeathTile(floating));

        return [data];
    }

    private static List<TRFloorDataEdit> FixZiplineReset(TR2Level floating)
    {
        // Reset the zipline if the player goes back towards the level start from the gold secret area.
        FDTriggerEntry resetTrigger = new()
        {
            Mask = 31,
            Actions =
            [
                new() { Parameter = 41 },
            ]
        };

        List<TRFloorDataEdit> edits = [];
        for (ushort z = 3; z < 14; z++)
        {
            edits.Add(MakeTrigger(floating, 41, 2, z, resetTrigger));
        }
        return edits;
    }

    private static TRFloorDataEdit FixMusicTrigger(TR2Level level)
    {
        // Extend music trigger in room 80 by one tile
        var trigger = GetTrigger(level, 80, 1, 2).Clone() as FDTriggerEntry;
        var action = trigger.Actions.FirstOrDefault(a => a.Action == FDTrigAction.PlaySoundtrack);
        Debug.Assert(action != null);
        action.Parameter = TR2MusicTrackBuilder.GetRealTrack(action.Parameter);
        return MakeTrigger(level, 80, 2, 2, trigger);
    }

    private static TRFloorDataEdit FixDeathTile(TR2Level level)
    {
        var sector = level.Rooms[91].GetSector(1, 4, TRUnit.Sector);
        var fd = new FDTrigCreateFix
        {
            Entries = [],
        };
        if (sector.FDIndex != 0)
        {
            fd.Entries.AddRange(level.FloorData[sector.FDIndex]);
        }
        fd.Entries.Add(new FDKillLaraEntry());

        return new()
        {
            RoomIndex = 91,
            X = 1,
            Z = 4,
            Fixes = [fd],
        };
    }
}
