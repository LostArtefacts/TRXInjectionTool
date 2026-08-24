using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Lara;

public class TR3LaraGunSFXBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.JUNGLE}");
        var edits = FixHolsterSFX(level, true);

        var data = InjectionData.Create(level, InjectionType.General, "lara_rifle_sfx", true);
        data.Animations.Clear();
        data.AnimFrames.Clear();
        data.AnimChanges.Clear();
        data.AnimDispatches.Clear();
        data.Models.Clear();
        data.AnimCmdEdits.AddRange(edits);

        return [data];
    }

    public static List<TRAnimCmdEdit> FixHolsterSFX(TR3Level level, bool resetLevel)
    {
        var animMap = new Dictionary<TR3Type, List<int>>
        {
            [TR3Type.LaraShotgunAnimation_H] = [1],
            [TR3Type.LaraMP5Animation_H] = [1],
            [TR3Type.LaraGrenadeAnimation_H] = [0],
            [TR3Type.LaraHarpoonAnimation_H] = [1, 9, 10],
            [TR3Type.LaraRocketAnimation_H] = [1],
        };

        var models = new TRDictionary<TR3Type, TRModel>();
        foreach (var type in animMap.Keys)
        {
            if (level.Models.TryGetValue(type, out var model))
            {
                models[type] = model;
            }
        }

        var edits = new List<TRAnimCmdEdit>();
        foreach (var (type, model) in models)
        {
            var animIds = animMap[type];
            for (int i = 0; i < animIds.Count; i++)
            {
                var anim = model.Animations[animIds[i]];
                if (i == 0)
                {
                    (anim.Commands[0] as TRSFXCommand).SoundID = (short)TR3SFX.LaraDraw;
                }
                else
                {
                    anim.Commands.Add(new TRSFXCommand
                    {
                        SoundID = (short)TR3SFX.LaraHolster,
                        FrameNumber = 20,
                    });
                }

                edits.Add(CreateAnimCmdEdit(level, type, animIds[i]));
            }

            if (resetLevel)
            {
                for (int i = 0; i < model.Animations.Count; i++)
                {
                    if (!animIds.Contains(i))
                    {
                        model.Animations[i].Commands.Clear();
                    }
                }
            }
        }

        if (resetLevel)
        {
            ResetLevel(level);
            level.Models = models;
        }

        return edits;
    }
}
