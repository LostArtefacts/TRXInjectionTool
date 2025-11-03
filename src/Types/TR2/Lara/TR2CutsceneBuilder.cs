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
        ResetLevel(cut);
        cut.Models[TR2Type.Lara] = lara;

        var endAnim = lara.Animations[^1];
        endAnim.NextFrame = (ushort)endAnim.FrameEnd;

        return InjectionData.Create(cut, InjectionType.General, "cut3_setup", true);
    }
}
