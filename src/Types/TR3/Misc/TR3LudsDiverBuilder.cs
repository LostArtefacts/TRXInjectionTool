using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Misc;

public class TR3LudsDiverBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.LUDS}");
        var diver = level.Models[TR3Type.DamagingAnimating1];
        ResetLevel(level);
        level.Models[TR3Type.DamagingAnimating1] = diver;

        // Anim 1 links to an invalid next frame, which causes the diver to
        // revert to a default stance/position after "dying". Add a dummy
        // animation that hides the diver.
        diver.Animations.Add(new()
        {
            NextAnimation = 2,
            Speed = new(),
            Accel = new(),
            FrameRate = 1,
            StateID = 1,
            Frames = [new()
            {
                Bounds = new(),
                OffsetY = 4096,
                Rotations = [.. diver.Meshes.Select(m => new TRAnimFrameRotation())],
            }],
        });
        diver.Animations[1].NextAnimation = 2;
        diver.Animations[1].NextFrame = 0;

        var data = InjectionData.Create(level, InjectionType.General, "luds_diver_animation", true);
        return [data];
    }
}
