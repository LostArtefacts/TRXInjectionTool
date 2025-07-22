using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.SFX;

public class TR1DoorSFXBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        return new()
        {
            FixVilcabambaDoor(),
            FixMinesTrapdoor(),
            FixAtlantisTrapdoors(),
        };
    }

    private static InjectionData FixVilcabambaDoor()
    {
        var level = _control1.Read($"Resources/{TR1LevelNames.VILCABAMBA}");
        var model = level.Models[TR1Type.Door3];
        ResetLevel(level);
        level.Models[TR1Type.Door3] = model;
        model.Animations[1].Commands.Add(new TRSFXCommand
        {
            FrameNumber = 1,
            SoundID = (short)TR1SFX.TrapdoorClose,
        });
        model.Animations[2].Commands.Add(new TRSFXCommand
        {
            FrameNumber = 1,
            SoundID = (short)TR1SFX.TrapdoorClose,
        });

        var data = InjectionData.Create(level, InjectionType.General, "vilcabamba_door_sfx", true);
        CreateDefaultTests(data, TR1LevelNames.VILCABAMBA);

        data.Animations.Clear();
        data.AnimFrames.Clear();
        data.AnimChanges.Clear();
        data.AnimDispatches.Clear();
        data.Models.Clear();

        foreach (int anim in new[] { 1, 2 })
        {
            data.AnimCmdEdits.Add(new()
            {
                TypeID = (int)(TR1Type.Door3),
                AnimIndex = anim,
                RawCount = data.AnimCommands.Count / 2,
                TotalCount = 1,
            });
        }

        return data;
    }

    private static InjectionData FixMinesTrapdoor()
    {
        var data = GetTrapdoorData("mines_door_sfx");
        foreach (int anim in new[] { 1, 3 })
        {
            data.AnimCmdEdits.Add(new()
            {
                TypeID = (int)TR1Type.Trapdoor2,
                AnimIndex = anim,
                RawCount = data.AnimCommands.Count / 2,
                TotalCount = 1,
            });
        }

        return data;
    }

    private static InjectionData FixAtlantisTrapdoors()
    {
        var data = GetTrapdoorData("atlantis_door_sfx");
        data.AnimCommands.AddRange(data.AnimCommands); // Two types

        foreach (var type in new[] { TR1Type.Trapdoor1, TR1Type.Trapdoor2 })
        {
            foreach (int anim in new[] { 1, 3 })
            {
                data.AnimCmdEdits.Add(new()
                {
                    TypeID = (int)type,
                    AnimIndex = anim,
                    RawCount = data.AnimCommands.Count / 4,
                    TotalCount = 1,
                });
            }
        }

        return data;
    }

    private static InjectionData GetTrapdoorData(string binName)
    {
        var level = _control1.Read($"Resources/{TR1LevelNames.VILCABAMBA}");
        var model = level.Models[TR1Type.Trapdoor1];
        var sfx1 = level.SoundEffects[TR1SFX.TrapdoorOpen];
        var sfx2 = level.SoundEffects[TR1SFX.TrapdoorClose];
        ResetLevel(level);
        level.Models[TR1Type.Trapdoor2] = model;
        level.SoundEffects[TR1SFX.TrapdoorOpen] = sfx1;
        level.SoundEffects[TR1SFX.TrapdoorClose] = sfx2;

        var data = InjectionData.Create(level, InjectionType.General, binName, true);
        data.Animations.Clear();
        data.AnimFrames.Clear();
        data.AnimChanges.Clear();
        data.AnimDispatches.Clear();
        data.Models.Clear();

        return data;
    }
}
