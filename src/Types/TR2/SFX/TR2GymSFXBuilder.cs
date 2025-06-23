using TRDataControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.SFX;

public class TR2GymSFXBuilder : InjectionBuilder
{
    public override string ID => "gym_sfx";

    public override List<InjectionData> Build()
    {
        TR2Level wall = _control2.Read($"Resources/{TR2LevelNames.GW}");
        TR2SFX[] soundIDs = new[]
        {
            TR2SFX.LaraPickup,
        };

        TRModel lara = wall.Models[TR2Type.Lara];
        TRDictionary<TR2SFX, TR2SoundEffect> copiedSounds = new();
        soundIDs.ToList().ForEach(s => copiedSounds[s] = wall.SoundEffects[s]);

        ResetLevel(wall);
        wall.SoundEffects = copiedSounds;

        InjectionData data = InjectionData.Create(wall, InjectionType.General, ID);
        return new() { data };
    }
}
