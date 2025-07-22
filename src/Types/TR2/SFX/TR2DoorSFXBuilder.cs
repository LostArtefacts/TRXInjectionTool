using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.SFX;

public class TR2DoorSFXBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        return new()
        {
            FixGeneralDoor(TR2Type.Door2),
            FixGeneralDoor(TR2Type.Door3),
            FixGeneralDoor(TR2Type.LiftingDoor1),
            FixLQDoors(),
        };
    }

    private static InjectionData FixGeneralDoor(TR2Type type)
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.MONASTERY}");
        var model = level.Models[TR2Type.Door3];
        ResetLevel(level);
        level.Models[type] = model;

        var data = InjectionData.Create(level, InjectionType.General, $"door{(int)type}_sfx", true);
        data.Animations.Clear();
        data.AnimFrames.Clear();
        data.AnimChanges.Clear();
        data.AnimDispatches.Clear();
        data.Models.Clear();

        foreach (int anim in new[] { 1, 3 })
        {
            data.AnimCmdEdits.Add(new()
            {
                TypeID = (int)type,
                AnimIndex = anim,
                RawCount = data.AnimCommands.Count / 2,
                TotalCount = 1,
            });
        }
        return data;
    }

    private static InjectionData FixLQDoors()
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.MONASTERY}");
        TR2SFX[] soundIDs = new[]
        {
            TR2SFX.DoorCreak,
            TR2SFX.Breaking3,
        };

        TRDictionary<TR2SFX, TR2SoundEffect> copiedSounds = new();
        soundIDs.ToList().ForEach(s => copiedSounds[s] = level.SoundEffects[s]);

        ResetLevel(level);
        level.SoundEffects = copiedSounds;

        var lq = _control2.Read($"Resources/{TR2LevelNames.LQ}");
        level.Models[TR2Type.Door2] = lq.Models[TR2Type.Door2];
        level.Models[TR2Type.Door2].Animations[1].Commands.RemoveAt(0);
        level.Models[TR2Type.Door2].Animations[3].Commands.Clear();

        var data = InjectionData.Create(level, InjectionType.General, "living_sfx", true);
        data.Animations.Clear();
        data.AnimFrames.Clear();
        data.AnimChanges.Clear();
        data.AnimDispatches.Clear();
        data.Models.Clear();

        data.AnimCmdEdits.Add(new()
        {
            TypeID = (int)TR2Type.Door2,
            AnimIndex = 1,
            RawCount = data.AnimCommands.Count,
            TotalCount = 1,
        });

        return data;
    }
}
