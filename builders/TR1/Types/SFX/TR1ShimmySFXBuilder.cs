using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.SFX;

public class TR1ShimmySFXBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        var caves = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        var fx = caves.SoundEffects[TR1SFX.LaraShimmy2];
        fx.Samples = [.. Directory.GetFiles("Resources/TR1/Lara/Shimmy").Select(File.ReadAllBytes)];
        ResetLevel(caves);
        caves.SoundEffects[TR1SFX.LaraShimmy2] = fx;

        var data = InjectionData.Create(caves, InjectionType.PS1SFX, "shimmy_sfx");
        return [data];
    }
}
