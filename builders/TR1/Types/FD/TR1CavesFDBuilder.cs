using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.FD;

public class TR1CavesFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.FDFix, "caves_fd");
        CreateDefaultTests(data, TR1LevelNames.CAVES);

        // Dart stairway, room 34
        for (ushort x = 1; x < 5; x++)
        {
            data.FloorEdits.Add(MakeMusicOneShot(34, x, 12));
        }

        data.FloorEdits.Add(FixCollapsibleTileTrigger());

        return [data];
    }

    private static TRFloorDataEdit FixCollapsibleTileTrigger()
    {
        // Remove the timer from the trigger under collapsible tile 50
        // otherwise it will reset its position.
        var level = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        var trig = GetTrigger(level, 32, 2, 12);
        trig.Timer = 0;
        return MakeTrigger(level, 32, 2, 12, trig);
    }
}
