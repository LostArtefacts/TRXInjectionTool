using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.FD;

public class TR3TinnosFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.General, "tinnos_fd");
        CreateDefaultTests(data, $"TR3/{TR3LevelNames.TINNOS}");
        data.FloorEdits.AddRange(FixMaskCameras());

        return [data];
    }

    private static IEnumerable<TRFloorDataEdit> FixMaskCameras()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.TINNOS}");
        var room = level.Rooms[67];

        for (ushort x = 2; x <= 4; x++)
        {
            for (ushort z = 2; z <= 4; z++)
            {
                if (x == 3 && z == 3)
                {
                    continue;
                }

                var sector = room.GetSector(x, z, TRUnit.Sector);
                if (sector.FDIndex == 0)
                {
                    continue;
                }

                var trigger = level.FloorData[sector.FDIndex].OfType<FDTriggerEntry>()
                    .Where(t => t.Actions.Any(a => a.Action == FDTrigAction.Camera))
                    .FirstOrDefault();
                if (trigger == null)
                {
                    continue;
                }

                trigger = trigger.Clone() as FDTriggerEntry;
                trigger.TrigType = FDTrigType.Dummy;
                trigger.Actions.RemoveAll(a => a.Action != FDTrigAction.Camera);
                yield return MakeTrigger(level, room.AlternateRoom, x, z, trigger);
            }
        }
    }
}
