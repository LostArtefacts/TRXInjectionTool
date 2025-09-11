using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Misc;

public class TR2WinstonBuilder : InjectionBuilder
{
    public override string ID => "winston_model";

    public override List<InjectionData> Build()
    {
        var winstonLevel = CreateWinstonLevel(TR2LevelNames.ASSAULT);
        _control2.Write(winstonLevel, MakeOutputPath(TRGameVersion.TR2, $"Debug/winston.tr2"));

        var data = InjectionData.Create(winstonLevel, InjectionType.General, ID);
        return [data];
    }
}
