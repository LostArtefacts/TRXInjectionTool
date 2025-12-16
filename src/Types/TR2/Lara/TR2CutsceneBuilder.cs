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
            CreateCut3Data(),
        ];
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
}
