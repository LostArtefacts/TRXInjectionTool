using System.Diagnostics;
using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Misc;

public class TR3SwingingFlameBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        var result = new List<InjectionData>();
        foreach (var levelName in new[] { TR3LevelNames.TINNOS, TR3LevelNames.REUNION })
        {
            var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.General, $"{_tr3NameMap[levelName]}_flames");
            result.Add(data);
            var level = _control3.Read($"Resources/TR3/{levelName}");
            UpdateSwingingFlames(level, data);
        }

        return result;
    }

    private static void UpdateSwingingFlames(TR3Level level, InjectionData data)
    {
        // OG uses the approach of timer=1 on pendulums to indicate it should be on fire.
        // Replace this with a similar approach to punks where a flame emitter is placed
        // on the same tile as the item.
        foreach (var item in level.Entities.Where(e => e.TypeID == TR3Type.SwingingThing))
        {
            var room = level.Rooms[item.Room];
            data.FloorEdits.Add(new()
            {
                RoomIndex = item.Room,
                X = (ushort)((item.X - room.Info.X) / TRConsts.Step4),
                Z = (ushort)((item.Z - room.Info.Z) / TRConsts.Step4),
                Fixes = [new FDTrigItem
                {
                    Item = new()
                    {
                        TypeID = (TR1Type)(int)TR3Type.Fire_N,
                        Room = item.Room,
                        X = item.X,
                        Y = item.Y,
                        Z = item.Z,
                    },
                }],
            });
        }

        for (short r = 0; r < level.Rooms.Count; r++)
        {
            var room = level.Rooms[r];
            for (ushort x = 0; x < room.NumXSectors; x++)
            {
                for (ushort z = 0; z < room.NumZSectors; z++)
                {
                    var sector = room.GetSector(x, z, TRUnit.Sector);
                    if (sector.FDIndex == 0)
                    {
                        continue;
                    }

                    var trigger = level.FloorData[sector.FDIndex]
                        .OfType<FDTriggerEntry>()
                        .Where(t => t.TrigType != FDTrigType.AntiTrigger && t.Actions.Any(a => 
                            a.Action == FDTrigAction.Object && level.Entities[a.Parameter].TypeID == TR3Type.SwingingThing))
                        .FirstOrDefault();
                    if (trigger == null)
                    {
                        continue;
                    }

                    Debug.Assert(trigger.Timer == 1);
                    trigger.Timer = 0;
                    data.FloorEdits.Add(FDBuilder.MakeTrigger(level, r, x, z, trigger));
                }
            }
        }
    }
}
