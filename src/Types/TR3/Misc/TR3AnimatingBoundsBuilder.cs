using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types.TR3.Misc;

public class TR3AnimatingBoundsBuilder : InjectionBuilder
{
    private static readonly Dictionary<string, List<TR3Type>> _targets = new()
    {
        [TR3LevelNames.COASTAL] = [TR3Type.Animating1],
        [TR3LevelNames.HSC] = [TR3Type.Animating4],
        [TR3LevelNames.HALLOWS] = [TR3Type.Animating1],
        [TR3LevelNames.FISHES] = [TR3Type.Animating4],
    };

    public override List<InjectionData> Build()
    {
        var result = new List<InjectionData>();
        foreach (var (levelName, types) in _targets)
        {
            var level = _control3.Read($"Resources/TR3/{levelName}");
            types.ForEach(t => FixBounds(level, t));

            var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.General, $"{_tr3NameMap[levelName]}_animating_bounds");
            result.Add(data);
            CreateDefaultTests(data, $"TR3/{levelName}");
            data.FrameReplacements.AddRange(TRFrameReplacement.CreateFrom(level, types));
        }

        return result;
    }

    private static void FixBounds(TR3Level level, TR3Type type)
    {
        if (!level.Models.TryGetValue(type, out TRModel model))
        {
            return;
        }

        foreach (var frame in model.Animations.SelectMany(a => a.Frames))
        {
            frame.Bounds = TRAnimBoundsCalculator.ComputeFrameBounds(model, frame);
        }
    }
}
