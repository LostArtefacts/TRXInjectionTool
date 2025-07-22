using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.SFX;

public class TR2GymSFXBuilder : InjectionBuilder
{
    public override string ID => "gym_sfx";

    public override List<InjectionData> Build()
    {
        TR2Level baseLevel = _control2.Read($"Resources/{TR2LevelNames.MONASTERY}");
        TR2SFX[] soundIDs = new[]
        {
            TR2SFX.LaraPickup, TR2SFX.DoorCreak, TR2SFX.Breaking3,
        };

        TRDictionary<TR2SFX, TR2SoundEffect> copiedSounds = new();
        soundIDs.ToList().ForEach(s => copiedSounds[s] = baseLevel.SoundEffects[s]);

        var door = baseLevel.Models[TR2Type.Door3];
        ResetLevel(baseLevel);
        baseLevel.SoundEffects = copiedSounds;
        baseLevel.Models[TR2Type.Door1] = door;

        var data = InjectionData.Create(baseLevel, InjectionType.General, ID, true);
        data.Animations.Clear();
        data.AnimFrames.Clear();
        data.AnimChanges.Clear();
        data.AnimDispatches.Clear();
        data.Models.Clear();

        foreach (int anim in new[] { 1, 3 })
        {
            data.AnimCmdEdits.Add(new()
            {
                TypeID = (int)TR2Type.Door1,
                AnimIndex = anim,
                RawCount = data.AnimCommands.Count / 2,
                TotalCount = 1,
            });
        }

        return new() { data };
    }
}
