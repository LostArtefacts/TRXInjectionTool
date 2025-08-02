using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.FD;

public class TR1SanctuaryFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        var level = _control1.Read($"Resources/{TR1LevelNames.SANCTUARY}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.FDFix, "sanctuary_fd");
        CreateDefaultTests(data, TR1LevelNames.SANCTUARY);
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
            AdjustDoorCamera(level),
        };

        return new() { data };
    }

    private static TRFloorDataEdit AdjustDoorCamera(TR1Level level)
    {
        var trigger = GetTrigger(level, 9, 1, 17);
        trigger.Actions.Find(a => a.CamAction != null)
            .CamAction.Once = false;
        return MakeTrigger(level, 9, 1, 17, trigger);
    }
}
