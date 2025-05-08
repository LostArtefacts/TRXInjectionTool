using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.FD;

public class TR2GlideCameraBuilder : FDBuilder
{
    private static readonly Dictionary<string, string> _nameMap = new()
    {
        [TR2LevelNames.DA] = "diving",
        [TR2LevelNames.DORIA] = "wreck",
        [TR2LevelNames.MONASTERY] = "barkhang",
        [TR2LevelNames.KINGDOM] = "kingdom",
    };

    public override List<InjectionData> Build()
    {
        List<InjectionData> result = new();
        foreach (string levelName in TR2LevelNames.AsOrderedList)
        {
            TR2Level level = _control2.Read($"Resources/{levelName}");
            var fixes = GenerateFixes(levelName, level);
            if (fixes.Any())
            {
                string mappedName = _nameMap.ContainsKey(levelName)
                    ? _nameMap[levelName]
                    : Path.GetFileNameWithoutExtension(levelName).ToLower();
                InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.General, $"{mappedName}_cameras");
                CreateDefaultTests(data, levelName);
                data.FloorEdits.AddRange(fixes);
                result.Add(data);
            }
        }

        return result;
    }

    private static IEnumerable<TRFloorDataEdit> GenerateFixes(string levelName, TR2Level level)
    {
        for (short r = 0; r < level.Rooms.Count; r++)
        {
            TR2Room room = level.Rooms[r];
            for (ushort x = 1; x < room.NumXSectors - 1; x++)
            {
                for (ushort z = 1; z < room.NumZSectors - 1; z++)
                {
                    var fix = GenerateFix(levelName, level, r, x, z);
                    if (fix != null)
                    {
                        yield return fix;
                    }
                }
            }
        }
    }

    private static TRFloorDataEdit GenerateFix(string levelName, TR2Level level, short room, ushort x, ushort z)
    {
        TRRoomSector sector = level.Rooms[room].GetSector(x, z, TRUnit.Sector);
        if (sector.FDIndex == 0
            || level.FloorData[sector.FDIndex].Find(e => e is FDTriggerEntry) is not FDTriggerEntry trigger
            || trigger.Actions.Find(a => a.Action == FDTrigAction.Camera) is not FDActionItem cameraAction
            || trigger.Actions.Find(a => a.Action == FDTrigAction.LookAtItem) is not null
            || cameraAction.CamAction.MoveTimer <= 1)
        {
            return null;
        }

        FDGlideCameraFix fix = new()
        {
            Timer = cameraAction.CamAction.Timer,
            Glide = cameraAction.CamAction.MoveTimer,
        };
        if (fix.Timer != 0)
        {
            fix.Timer = Math.Max((byte)(cameraAction.CamAction.MoveTimer + 2), cameraAction.CamAction.MoveTimer);
        }

        if (levelName == TR2LevelNames.MONASTERY && room == 26 && x == 11 && z == 2)
        {
            // Prevent a bump in Y pos which is only noticeable with the slower glide.
            fix.Shift.Y = -(TRConsts.Step2 - 16);
        }

        return new()
        {
            RoomIndex = room,
            X = x,
            Z = z,
            Fixes = new() { fix },
        };
    }
}
