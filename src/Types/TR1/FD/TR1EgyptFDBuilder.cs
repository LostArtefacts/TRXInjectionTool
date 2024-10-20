using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.FD;

public class TR1EgyptFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        InjectionData data = InjectionData.Create(InjectionType.FDFix, "egypt_fd");
        data.FloorEdits = new()
        {
            MakeMusicOneShot(0, 3, 3),
            MakeMusicOneShot(0, 4, 3),
            MakeMusicOneShot(0, 5, 3),
            MakeMusicOneShot(19, 10, 7),
            MakeMusicOneShot(22, 6, 4),
            MakeMusicOneShot(22, 6, 5),
            MakeMusicOneShot(22, 7, 4),
            MakeMusicOneShot(22, 7, 5),
            MakeMusicOneShot(61, 5, 1),
            MakeMusicOneShot(93, 2, 2),
        };

        return new() { data };
    }
}
