using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.FD;

public class TR2ColdWarFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level coldWar = _control2.Read($"Resources/{TR2LevelNames.COLDWAR}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.FDFix, "coldwar_fd");
        CreateDefaultTests(data, TR2LevelNames.COLDWAR);
        data.FloorEdits.Add(FixCollapsibleTileTrigger(coldWar));

        return new() { data };
    }

    private static TRFloorDataEdit FixCollapsibleTileTrigger(TR2Level level)
    {
        // Restore the missing trigger for tile 141 in the flipped room.
        FDTriggerEntry trigger = GetTrigger(level, 80, 11, 10);
        return MakeTrigger(level, 82, 11, 10, trigger);
    }
}
