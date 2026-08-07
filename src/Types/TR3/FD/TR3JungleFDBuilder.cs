using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.FD;

public class TR3JungleFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.FDFix, "jungle_fd");
        CreateDefaultTests(data, $"TR3/{TR3LevelNames.JUNGLE}");
        data.FloorEdits.AddRange(FixFireTriggers());

        return [data];
    }

    private static IEnumerable<TRFloorDataEdit> FixFireTriggers()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.JUNGLE}");
        var pred = new Predicate<FDActionItem>(
            a => a.Action == FDTrigAction.Object && a.Parameter == 104);

        // Emitter 104 is triggered twice on several sectors instead of 103/104.
        foreach (var roomIdx in new short[] { 0, 81 })
        {
            var room = level.Rooms[roomIdx];
            for (ushort x = 1; x < room.NumXSectors - 1; x++)
            {
                for (ushort z = 1; z <= room.NumZSectors - 1; z++)
                {
                    var sector = room.GetSector(x, z, TRUnit.Sector);
                    if (sector.FDIndex == 0)
                    {
                        continue;
                    }

                    var trigger = level.FloorData[sector.FDIndex].OfType<FDTriggerEntry>()
                        .FirstOrDefault(t => t.Actions.Count(pred.Invoke) > 1);
                    if (trigger == null)
                    {
                        continue;
                    }

                    trigger = trigger.Clone() as FDTriggerEntry;
                    trigger.Actions.RemoveAll(pred);
                    trigger.Actions.AddRange(new[] { 103, 104 }
                        .Select(p => new FDActionItem
                        {
                            Action = FDTrigAction.Object,
                            Parameter = (short)p,
                        }));

                    yield return MakeTrigger(level, roomIdx, x, z, trigger);
                }
            }
        }
    }
}
