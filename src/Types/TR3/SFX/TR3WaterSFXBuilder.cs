using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.SFX;

public class TR3WaterSFXBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.FISHES}");
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.General, "water_sfx");
        var sfx = level.SoundEffects[TR3SFX.VeryLightWater];
        sfx.Mode = TR3SFXMode.Ambient;
        data.SFX.Add(TRSFXData.Create(TR3SFX.VeryLightWater, sfx, true));
        return [data];
    }
}
