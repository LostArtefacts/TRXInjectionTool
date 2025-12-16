using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.FD;

public class TR2MusicTrackBuilder : InjectionBuilder
{
    public override string ID => "fix_tr2_music_tracks";

    public override List<InjectionData> Build()
    {
        var result = new List<InjectionData>();
        foreach (var (levelName, binName) in _tr2NameMap)
        {
            var level = _control2.Read($"Resources/{levelName}");
            var edits = FixMusicTriggers(level);
            if (!edits.Any())
            {
                continue;
            }

            var data = InjectionData.Create(TRGameVersion.TR2, InjectionType.General, $"{binName}_music_tracks");
            CreateDefaultTests(data, levelName);
            data.FloorEdits.AddRange(edits);
            result.Add(data);
        }

        return result;
    }

    private static IEnumerable<TRFloorDataEdit> FixMusicTriggers(TR2Level level)
    {
        for (short r = 0; r < level.Rooms.Count; r++)
        {
            var room = level.Rooms[r];
            for (ushort x = 1; x < room.NumXSectors - 1; x++)
            {
                for (ushort z = 1; z < room.NumZSectors - 1; z++)
                {
                    var sector = room.GetSector(x, z, TRUnit.Sector);
                    if (sector.FDIndex == 0)
                    {
                        continue;
                    }

                    var fixes = level.FloorData[sector.FDIndex]
                        .OfType<FDTriggerEntry>()
                        .SelectMany(t => t.Actions.Where(a => a.Action == FDTrigAction.PlaySoundtrack))
                        .Select(a => FixTrack(a, r, x, z))
                        .Where(a => a != null);
                    foreach (var fix in fixes)
                    {
                        yield return fix;
                    }
                }
            }
        }
    }

    private static TRFloorDataEdit FixTrack(FDActionItem action, short room, ushort x, ushort z)
    {
        var track = action.Parameter;
        var realTrack = GetRealTrack(track);
        if (track == realTrack)
        {
            return null;
        }

        return new()
        {
            RoomIndex = room,
            X = x,
            Z = z,
            Fixes = new()
            {
                new FDTrigParamFix
                {
                    ActionType = FDTrigAction.PlaySoundtrack,
                    OldParam = track,
                    NewParam = realTrack,
                }
            }
        };
    }

    public static short GetRealTrack(short trackID)
    {
        short[] skippedIDs = { 2, 19, 20, 26, -1 };
        short idx = 0;
        short result = 2;

        for (int i = 2; i < trackID; i++)
        {
            if ((skippedIDs[idx] >= 0) && (i == skippedIDs[idx]))
            {
                idx++;
            }
            else
            {
                result++;
            }
        }
        return result;
    }
}
