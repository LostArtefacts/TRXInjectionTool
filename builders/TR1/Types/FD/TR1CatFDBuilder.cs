using System.Diagnostics;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.FD;

public class TR1CatFDBuilder : FDBuilder
{
    private static readonly List<short> _windyRooms
        = [0, 2, 4, 23, 31, 74, 75, 77, 98, 101, 102, 105];

    public override List<InjectionData> Build()
    {
        var cat = _control1.Read($"Resources/{TR1LevelNames.CAT}");
        var data = InjectionData.Create(TRGameVersion.TR1, InjectionType.FDFix, "cat_fd");
        CreateDefaultTests(data, TR1LevelNames.CAT);
        data.FloorEdits =
        [
            MakeMusicOneShot(14, 1, 1),
            MakeMusicOneShot(14, 1, 2),
            MakeMusicOneShot(98, 3, 5),
            MakeMusicOneShot(100, 3, 2),
            MakeMusicOneShot(100, 3, 3),
            MakeMusicOneShot(100, 3, 4),
            .. AddRoomFlags(_windyRooms, TRRoomFlag.Wind, cat.Rooms),
            .. FixEnemyTriggers(cat),
        ];

        return [data];
    }

    private static IEnumerable<TRFloorDataEdit> FixEnemyTriggers(TR1Level level)
    {
        // If the flip map is activated in room 95 and Lara survives, enemies 193
        // and 194 can't be triggered in the end area.
        var roomA = level.Rooms[103];
        Debug.Assert(roomA.AlternateRoom != -1);
        var roomB = level.Rooms[roomA.AlternateRoom];
        Debug.Assert(roomA.Sectors.Count == roomB.Sectors.Count);

        for (ushort x = 1; x < roomA.NumXSectors - 1; x++)
        {
            for (ushort z = 1; z < roomA.NumXSectors - 1; z++)
            {
                var sector = roomA.GetSector(x, z, TRUnit.Sector);
                if (sector.FDIndex == 0)
                {
                    continue;
                }

                var trigger = level.FloorData[sector.FDIndex]
                    .OfType<FDTriggerEntry>().FirstOrDefault();
                if (trigger == null)
                {
                    continue;
                }

                trigger = trigger.Clone() as FDTriggerEntry;
                trigger.Actions.RemoveAll(a => a.Action != FDTrigAction.Object 
                    || a.Parameter < 193 || a.Parameter > 194);
                if (trigger.Actions.Count == 0)
                {
                    continue;
                }

                yield return MakeTrigger(level, roomA.AlternateRoom, x, z, trigger);
            }
        }
    }
}
