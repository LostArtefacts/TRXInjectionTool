using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.FD;

public class TR1SanctuaryFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.FDFix, "sanctuary_fd");
        data.FloorEdits = new()
        {
            MakeMusicOneShot(0, 7, 1),
            MakeMusicOneShot(0, 7, 2),
            MakeMusicOneShot(0, 7, 3),
            MakeMusicOneShot(0, 8, 15),
            MakeMusicOneShot(0, 8, 16),
            MakeMusicOneShot(0, 8, 17),
            MakeMusicOneShot(0, 9, 15),
            MakeMusicOneShot(0, 9, 16),
            MakeMusicOneShot(0, 9, 17),
        };

        return new() { data };
    }
}
