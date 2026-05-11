using TRLevelControl.Helpers;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Misc;

public class TR3WinstonBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        var winstonLevel = CreateTR3WinstonLevel(TR3LevelNames.ASSAULT);
        var data = InjectionData.Create(winstonLevel, InjectionType.General, "winston_model");
        return [data];
    }
}
