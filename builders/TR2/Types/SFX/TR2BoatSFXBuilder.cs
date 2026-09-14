using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.SFX;

public class TR2BoatSFXBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.VENICE}");
        var sfx = level.SoundEffects[TR2SFX.BodySlump];
        ResetLevel(level);
        level.SoundEffects[TR2SFX.BodySlump] = sfx;

        var data = InjectionData.Create(level, InjectionType.General, "boat_sfx");
        return [data];
    }
}
