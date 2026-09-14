using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.SFX;

public class TR3HissSFXBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.RXTECH}");
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.General, "hiss_sfx");
        var sfx = level.SoundEffects[TR3SFX.LoopForGasHiss];
        sfx.Mode = TR3SFXMode.Ambient;
        sfx.Volume /= 2;
        data.SFX.Add(TRSFXData.Create(TR3SFX.LoopForGasHiss, sfx));
        return [data];
    }
}
