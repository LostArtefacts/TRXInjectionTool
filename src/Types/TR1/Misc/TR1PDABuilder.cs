using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Misc;

public class TR1PDABuilder : InjectionBuilder, IPublisher
{
    public override string ID => "tr1_pda";

    public override List<InjectionData> Build()
    {
        var pdaLevel = CreatePDALevel();
        var data = InjectionData.Create(pdaLevel, InjectionType.General, "pda_model");

        return new() { data };
    }

    public TRLevelBase Publish()
        => CreatePDALevel();

    public string GetPublishedName()
        => "pda.phd";
}
