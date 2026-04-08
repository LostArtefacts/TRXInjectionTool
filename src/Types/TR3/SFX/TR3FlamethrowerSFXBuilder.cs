using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.SFX;

public class TR3FlamethrowerSFXBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.ANTARC}");
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.General, "flamethrower_sfx");
        data.SFX.Add(TRSFXData.Create(TR3SFX.FlameThrowerLoop, level.SoundEffects[TR3SFX.FlameThrowerLoop]));
        return [data];
    }
}
