using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.FD;

public class TR1AtlantisFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        InjectionData data = InjectionData.Create(InjectionType.FDFix, "atlantis_fd");
        data.FloorEdits = new()
        {
            MakeMusicOneShot(59, 1, 1),
        };

        return new() { data };
    }
}
