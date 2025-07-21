using TRLevelControl.Helpers;
using TRLevelControl.Model;
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

        var trig = GetTrigger(level, 28, 2, 2);
        trig.Actions.RemoveAll(a => a.Action == FDTrigAction.Object && a.Parameter == 23);
        data.FloorEdits.Add(MakeTrigger(level, 28, 2, 2, trig));

        return new() { data };
    }
}
