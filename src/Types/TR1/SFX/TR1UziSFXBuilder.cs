using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.SFX;

public class TR1UziSFXBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level caves = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        TR1SoundEffect fx = caves.SoundEffects[TR1SFX.LaraUziFire];
        fx.Samples = new()
        {
            File.ReadAllBytes("Resources/TR1/Uzis/0.wav"),
        };
        ResetLevel(caves);

        caves.SoundEffects[TR1SFX.LaraUziFire] = fx;

        InjectionData data = InjectionData.Create(caves, InjectionType.PS1SFX, "uzi_sfx");
        return new() { data };
    }
}
