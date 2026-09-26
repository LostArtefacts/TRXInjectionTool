using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types.TR4.Misc;

public class TR4AnimatingBoundsBuilder : InjectionBuilder
{
    private static readonly Dictionary<string, List<TR4Type>> _targets = new()
    {
        [TR4LevelNames.BURIAL] = [TR4Type.Plough],
    };

    public override List<InjectionData> Build()
    {
        var result = new List<InjectionData>();
        foreach (var (levelName, types) in _targets)
        {
            var level = _control4.Read($"Resources/TR4/{levelName}");
            level.SoundEffects.Clear();
            types.ForEach(t => FixBounds(level, t));

            var data = InjectionData.Create(TRGameVersion.TR4, InjectionType.General, $"{_tr4NameMap[levelName]}_animating_bounds");
            result.Add(data);
            CreateDefaultTests(data, $"TR4/{levelName}");
            data.FrameReplacements.AddRange(TRFrameReplacement.CreateFrom(level, types));
        }

        return result;
    }

    private static void FixBounds(TR4Level level, TR4Type type)
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
