using System.Diagnostics;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.FD;

public class TR2LivingFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level living = _control2.Read($"Resources/{TR2LevelNames.LQ}");

        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.FDFix, "living_fd");
        CreateDefaultTests(data, TR2LevelNames.LQ);
        data.FloorEdits.Add(FixFlipmapFlames(living));

        return new() { data };
    }

    private static TRFloorDataEdit FixFlipmapFlames(TR2Level living)
    {
        TR2Room room = living.Rooms[1];
        FDTriggerEntry trigger = living.FloorData[room.GetSector(1, 2, TRUnit.Sector).FDIndex]
            .Find(e => e is FDTriggerEntry) as FDTriggerEntry;
        Debug.Assert(trigger != null);
        Debug.Assert(!trigger.Actions.Any(a => a.Action == FDTrigAction.Object && a.Parameter == 23));

        trigger.Actions.Add(new() { Parameter = 23 });
        return MakeTrigger(living, 1, 1, 2, trigger);
    }
}
