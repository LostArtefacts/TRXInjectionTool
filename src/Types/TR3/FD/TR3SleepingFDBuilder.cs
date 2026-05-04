using System.Diagnostics;
using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.FD;

public class TR3SleepingFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.General, "undersea_fd");
        CreateDefaultTests(data, $"TR3/{TR3LevelNames.FISHES}");
        data.FloorEdits.Add(FixSeaweed());
        return [data];
    }

    private static TRFloorDataEdit FixSeaweed()
    {
        // Trigger the seaweed on level start as the fixed camera looks at it.
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.FISHES}");
        var lara = level.Entities.Find(e => e.TypeID == TR3Type.Lara);
        var room = level.Rooms[lara.Room];
        var sector = room.GetSector(lara.X, lara.Z);
        Debug.Assert(sector.FDIndex != 0);
        var trigger = level.FloorData[sector.FDIndex].OfType<FDTriggerEntry>().FirstOrDefault();
        Debug.Assert(trigger != null);

        trigger.Actions.AddRange(level.Entities.Where(e => e.TypeID == TR3Type.Animating4).Select(e =>
        {
            return new FDActionItem
            {
                Action = FDTrigAction.Object,
                Parameter = (short)level.Entities.IndexOf(e),
            };
        }));

        return MakeTrigger(level, lara.Room,
            (ushort)((lara.X - room.Info.X) / TRConsts.Step4),
            (ushort)((lara.Z - room.Info.Z) / TRConsts.Step4), trigger);
    }
}
