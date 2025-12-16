using System.Diagnostics;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.FD;

public class TR2VeniceFDBuilder : FDBuilder
{
    public override string ID => "venice_fd";

    public override List<InjectionData> Build()
    {
        TR2Level level = _control2.Read($"Resources/{TR2LevelNames.VENICE}");

        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.FDFix, ID);
        CreateDefaultTests(data, TR2LevelNames.VENICE);

        data.FloorEdits.Add(FixWindowTrigger(level));
        data.FloorEdits.AddRange(FixMusicTrigger(level));

        return [data];
    }

    private static TRFloorDataEdit FixWindowTrigger(TR2Level level)
    {
        // Remove a redundant breakable window trigger - it's one shot because of music, and so
        // causes issues with save/load.
        var trig = GetTrigger(level, 28, 2, 2);
        trig.Actions.RemoveAll(a => a.Action == FDTrigAction.Object && a.Parameter == 23);
        return MakeTrigger(level, 28, 2, 2, trig);
    }

    private static List<TRFloorDataEdit> FixMusicTrigger(TR2Level level)
    {
        // Extend music trigger in room 19 by two tiles into the water
        var trigger = GetTrigger(level, 19, 4, 5).Clone() as FDTriggerEntry;
        var action = trigger.Actions.FirstOrDefault(a => a.Action == FDTrigAction.PlaySoundtrack);
        Debug.Assert(action != null);
        action.Parameter = TR2MusicTrackBuilder.GetRealTrack(action.Parameter);
        return
        [
            MakeTrigger(level, 11, 4, 4, trigger),
            MakeTrigger(level, 11, 4, 3, trigger),
        ];
    }
}
