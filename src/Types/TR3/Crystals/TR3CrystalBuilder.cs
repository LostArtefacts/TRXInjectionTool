using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types.TR3.Crystals;

public class TR3CrystalBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
        => [CrystalUtils.MakeTR3Crystal()];
}
