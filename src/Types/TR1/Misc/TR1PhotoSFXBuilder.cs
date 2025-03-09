using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Misc;

public class TR1PhotoSFXBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level caves = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        TR1SoundEffect fx = caves.SoundEffects[TR1SFX.MenuChoose];
        ResetLevel(caves);

        caves.SoundEffects[TR1SFX.MenuChoose] = fx;

        InjectionData data = InjectionData.Create(caves, InjectionType.General, "photo");
        return new() { data };
    }
}
