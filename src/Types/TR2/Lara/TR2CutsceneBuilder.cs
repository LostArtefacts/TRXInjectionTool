using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Lara;

public class TR2CutsceneBuilder : InjectionBuilder
{
    public override string ID => "tr2_cutscenes";

    public override List<InjectionData> Build()
    {
        return
        [
            CreateCut2Data(),
            CreateCut3Data(),
            CreateCut4Data(),
        ];
    }

    private static InjectionData CreateCut2Data()
    {
        var cut = _control2.Read($"Resources/{TR2LevelNames.OPERA_CUT}");
        var models = new TRDictionary<TR2Type, TRModel>
        {
            [TR2Type.CutsceneActor5] = cut.Models[TR2Type.CutsceneActor5], // Cockpit
            [TR2Type.CutsceneActor7] = cut.Models[TR2Type.CutsceneActor7], // Rail hook
        };
        ResetLevel(cut);
        cut.Models = models;

        foreach (var model in cut.Models.Values)
        {
            model.Animations[0].Commands.Add(new TRFXCommand
            {
                EffectID = (short)TR2FX.ShadowOff,
                FrameNumber = 1,
            });
        }

        return InjectionData.Create(cut, InjectionType.General, "cut2_setup", true);
    }

    private static InjectionData CreateCut3Data()
    {
        var cut = _control2.Read($"Resources/{TR2LevelNames.DA_CUT}");
        var lara = cut.Models[TR2Type.Lara];
        var monk = cut.Models[TR2Type.CutsceneActor4];
        ResetLevel(cut);
        cut.Models[TR2Type.Lara] = lara;
        cut.Models[TR2Type.CutsceneActor4] = monk;

        var endAnim = lara.Animations[^1];
        endAnim.NextFrame = (ushort)endAnim.FrameEnd;

        // Remove mesh swap commands, now handled in Lua
        lara.Animations[0].Commands.Clear();
        lara.Animations[7].Commands.Clear();

        FixMonkDeath(monk);

        return InjectionData.Create(cut, InjectionType.General, "cut3_setup", true);
    }

    private static void FixMonkDeath(TRModel model)
    {
        var level = _control2.Read($"Resources/TR2/Objects/monk_cut3.tr2");
        var deathPoseFrame = level.Models[TR2Type.CutsceneActor4].Animations[0].Frames[0];

        var finalCoreFrame = model.Animations[9].Frames[^1];
        model.Animations[10].Frames[0] = finalCoreFrame;
        
        for (int i = 1; i < model.Animations[10].Frames.Count; i++)
        {
            model.Animations[10].Frames [i] = deathPoseFrame;
        }
        for (int anim = 11; anim < model.Animations.Count; anim++)
        {
            for (int i = 0; i < model.Animations[anim].Frames.Count; i++)
            {
                model.Animations[anim].Frames[i] = deathPoseFrame;
            }
        }
    }

    private static InjectionData CreateCut4Data()
    {
        var cut = _control2.Read($"Resources/{TR2LevelNames.XIAN_CUT}");
        var models = new TRDictionary<TR2Type, TRModel>
        {
            [TR2Type.CutsceneActor5] = cut.Models[TR2Type.CutsceneActor5], // Bartoli
            [TR2Type.CutsceneActor6] = cut.Models[TR2Type.CutsceneActor6], // Goon
            [TR2Type.CutsceneActor8] = cut.Models[TR2Type.CutsceneActor8], // Goon
            [TR2Type.CutsceneActor9] = cut.Models[TR2Type.CutsceneActor9], // Goon
            [TR2Type.CutsceneActor10] = cut.Models[TR2Type.CutsceneActor10], // Goon
        };
        ResetLevel(cut);
        cut.Models = models;

        // Hide shadows when the goons go into the jade
        foreach (var model in cut.Models.Values)
        {
            model.Animations[7].Commands.Add(new TRFXCommand
            {
                EffectID = (short)TR2FX.ShadowOff,
                FrameNumber = 94,
            });
        }

        return InjectionData.Create(cut, InjectionType.General, "cut4_setup", true);
    }
}
