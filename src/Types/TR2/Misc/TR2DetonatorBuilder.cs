using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Misc;

public class TR2DetonatorBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.BARTOLI}");
        var detonator = level.Models[TR2Type.DetonatorBox];
        ResetLevel(level);
        level.Models[TR2Type.DetonatorBox] = detonator;

        var anim = detonator.Animations[0];
        anim.Commands.Add(new TRFXCommand
        {
            FrameNumber = 76,
            EffectID = (short)TR2FX.DynamicLightsOn,
        });
        anim.Commands.Add(new TRFXCommand
        {
            FrameNumber = 100,
            EffectID = (short)TR2FX.DynamicLightsOff,
        });

        var data = InjectionData.Create(level, InjectionType.General, "detonator_lights", true);
        data.Animations.Clear();
        data.AnimFrames.Clear();
        data.AnimChanges.Clear();
        data.AnimDispatches.Clear();
        data.Models.Clear();
        
        data.AnimCmdEdits.Add(CreateAnimCmdEdit(level, TR2Type.DetonatorBox, 0));
        return [data];
    }
}
