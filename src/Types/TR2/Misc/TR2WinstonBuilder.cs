using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Misc;

public class TR2WinstonBuilder : InjectionBuilder
{
    private static readonly Dictionary<string, string> _sourceMap = new()
    {
        [TR2LevelNames.ASSAULT] = "winston_model",
        [TR2LevelNames.VEGAS] = "winston_model_gm",
    };

public override string ID => "tr2-winston";

    public override List<InjectionData> Build()
    {
        var result = new List<InjectionData>();
        foreach (var (level, binName) in _sourceMap)
        {
            var winstonLevel = CreateWinstonLevel(level);
            _control2.Write(winstonLevel, MakeOutputPath(TRGameVersion.TR2, $"Debug/winston_{level}"));
            result.Add(InjectionData.Create(winstonLevel, InjectionType.General, binName));
        }

        return result;
    }
}
