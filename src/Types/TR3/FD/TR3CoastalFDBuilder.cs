using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.FD;

public class TR3CoastalFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.General, "coastal_fd");
        CreateDefaultTests(data, $"TR3/{TR3LevelNames.COASTAL}");
        data.FloorEdits.AddRange(FixFishTriggers());

        return [data];
    }

    private static IEnumerable<TRFloorDataEdit> FixFishTriggers()
    {
        // Fish no longer require split timer triggers. Activate each directly under Lara
        // so they don't spawn in later in-sight.
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.COASTAL}");
        var fish = level.Entities.Where(e => e.TypeID == TR3Type.Fish)
            .Select(e => (short)level.Entities.IndexOf(e))
            .ToList();

        var lara = level.Entities.Find(e => e.TypeID == TR3Type.Lara);
        var sector = level.GetRoomSector(lara.X, lara.Y, lara.Z, lara.Room);
        var baseTrigger = level.FloorData[sector.FDIndex].OfType<FDTriggerEntry>().First();
        baseTrigger.Actions.AddRange(fish.Where(i => !baseTrigger.Actions.Any(a => a.Parameter == i))
            .Select(i => new FDActionItem { Parameter = i }));
        yield return MakeTrigger(level, lara.Room, 2, 1, baseTrigger);

        var room = level.Rooms[lara.Room];
        for (ushort x = 1; x < room.NumXSectors - 2; x++)
        {
            for (ushort z = 1; z < room.NumZSectors - 2; z++)
            {
                sector = room.GetSector(x, z, TRUnit.Sector);
                if (sector.FDIndex == 0)
                {
                    continue;
                }

                var trigger = level.FloorData[sector.FDIndex].OfType<FDTriggerEntry>()
                    .FirstOrDefault(t => t != baseTrigger && t.Actions.Any(a => fish.Contains(a.Parameter)));
                if (trigger == null)
                {
                    continue;
                }

                trigger.Actions.RemoveAll(a => fish.Contains(a.Parameter));
                yield return MakeTrigger(level, lara.Room, x, z, trigger);
            }
        }
    }
}
