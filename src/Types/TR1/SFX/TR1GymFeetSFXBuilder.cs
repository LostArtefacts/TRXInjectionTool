using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.SFX;

public class TR1GymFeetSFXBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        // Barefoot SFX are handled by outfits, so reset TR1 gym to use the regular sound.
        var level = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        var data = InjectionData.Create(TRGameVersion.TR1, InjectionType.General, "lara_feet_sfx");
        data.SFX.Add(TRSFXData.Create(TR1SFX.LaraFeet, level.SoundEffects[TR1SFX.LaraFeet]));
        return [data];
    }
}
