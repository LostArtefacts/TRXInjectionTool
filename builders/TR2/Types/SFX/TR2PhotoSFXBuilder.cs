using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.SFX;

public class TR2PhotoSFXBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level wall = _control2.Read($"Resources/{TR2LevelNames.GW}");
        TR2SoundEffect fx = wall.SoundEffects[TR2SFX.MenuLaraHome];
        ResetLevel(wall);

        wall.SoundEffects[TR2SFX.MenuLaraHome] = fx;

        InjectionData data = InjectionData.Create(wall, InjectionType.General, "photo");
        return new() { data };
    }
}
