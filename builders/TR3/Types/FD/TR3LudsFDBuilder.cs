using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.FD;

public class TR3LudsFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.FDFix, "luds_fd");
        CreateDefaultTests(data, $"TR3/{TR3LevelNames.LUDS}");

        data.ItemFlagEdits.AddRange(FixGuardPatrols());        

        return [data];
    }

    private static IEnumerable<TRItemTypeFlagEdit> FixGuardPatrols()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.LUDS}");
        yield return ItemBuilder.SetType(level, 19, TR3Type.AIPatrol1_N);
    }
}
