using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.FD;

public class TR1CatFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.FDFix, "cat_fd");
        CreateDefaultTests(data, TR1LevelNames.CAT);
        data.FloorEdits = new()
        {
            MakeMusicOneShot(14, 1, 1),
            MakeMusicOneShot(14, 1, 2),
            MakeMusicOneShot(98, 3, 5),
            MakeMusicOneShot(100, 3, 2),
            MakeMusicOneShot(100, 3, 3),
            MakeMusicOneShot(100, 3, 4),
        };

        return new() { data };
    }
}
