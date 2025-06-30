using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Misc;

public class TR1PDABuilder : InjectionBuilder
{
    public override string ID => "tr1-pda";

    public override List<InjectionData> Build()
    {
        var pdaLevel = CreatePDALevel();
        ExportLevelZip(pdaLevel, ID);

        var data = InjectionData.Create(pdaLevel, InjectionType.General, "pda_model");
        return new() { data };
    }
}
