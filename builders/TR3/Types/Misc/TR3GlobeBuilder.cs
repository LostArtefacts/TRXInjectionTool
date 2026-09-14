using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Misc;

public class TR3GlobeBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        var srcLevel = _control3.Read("Resources/TR3/TITLE.TR2");
        CreateModelLevel(srcLevel, TR3Type.Globe_M_H);
        var data = InjectionData.Create(srcLevel, InjectionType.General, "globe_model");
        return [data];
    }
}
