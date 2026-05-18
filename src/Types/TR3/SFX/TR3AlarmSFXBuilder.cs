using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.SFX;

public class TR3AlarmSFXBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.ALDWYCH}");
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.General, "alarm_sfx");
        data.SFX.Add(TRSFXData.Create(TR3SFX.Alarm1, level.SoundEffects[TR3SFX.Alarm1]));
        return [data];
    }
}
