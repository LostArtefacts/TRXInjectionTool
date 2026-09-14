using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Misc;

public class TR1EnemySFXBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        return new()
        {
            FixDeathSFX(TR1LevelNames.KHAMOON, "panther_sfx", TR1Type.Panther, TR1SFX.LionDeath, 5),
            FixDeathSFX(TR1LevelNames.MINES, "skate_kid_sfx", TR1Type.SkateboardKid, TR1SFX.SkateKidDeath, 13),
        };
    }

    private static InjectionData FixDeathSFX(string levelName, string binName, TR1Type type, TR1SFX sfxID, int animIndex)
    {
        TR1Level level = _control1.Read($"Resources/{levelName}");
        TRModel model = level.Models[type];
        ResetLevel(level);
        level.Models[type] = model;

        TRAnimation deathAnim = model.Animations[animIndex];
        model.Animations.ForEach(a =>
        {
            if (a != deathAnim)
            {
                a.Commands.Clear();
            }
        });

        var cmd = deathAnim.Commands.Find(c => c is TRSFXCommand s && s.SoundID == (short)sfxID);
        if (cmd is TRSFXCommand sfxCmd)
        {
            sfxCmd.FrameNumber = 1;
        }
        else
        {
            throw new ArgumentException("No matching SFX command");
        }

        InjectionData data = InjectionData.Create(level, InjectionType.General, binName, true);
        data.Animations.Clear();
        data.AnimFrames.Clear();
        data.AnimChanges.Clear();
        data.AnimDispatches.Clear();
        data.Models.Clear();

        data.AnimCmdEdits.Add(new()
        {
            TypeID = (int)type,
            AnimIndex = animIndex,
            RawCount = data.AnimCommands.Count,
            TotalCount = deathAnim.Commands.Count,
        });

        return data;
    }
}
