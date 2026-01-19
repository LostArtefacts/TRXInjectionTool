using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types.TR3.Misc;

public class TR3HallowsAnimatingBoundsBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        TR3Level level = _control3.Read($"Resources/TR3/{TR3LevelNames.HALLOWS}");

        // Animating1 does not have proper animation bounds, which results in it
        // getting culled too soon.
        const TR3Type targetType = TR3Type.Animating1;

        if (level.Models.TryGetValue(targetType, out TRModel model))
        {
            foreach (var frame in model.Animations.SelectMany(a => a.Frames))
            {
                frame.Bounds = TRAnimBoundsCalculator.ComputeFrameBounds(model, frame);
            }
        }

        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.General, "stpaul_animating_bounds");
        CreateDefaultTests(data, $"TR3/{TR3LevelNames.HALLOWS}");
        data.FrameReplacements.AddRange(TRFrameReplacement.CreateFrom(level, [targetType]));
        return [data];
    }
}
