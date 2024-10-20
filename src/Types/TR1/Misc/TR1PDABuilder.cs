using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Misc;

public class TR1PDABuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level caves = _control1.Read($@"Resources\{TR1LevelNames.CAVES}");
        CreateModelLevel(caves, TR1Type.Map_M_U);

        InjectionData data = InjectionData.Create(caves, InjectionType.General, "pda_model");
        return new() { data };
    }
}
