using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Misc;

public class TR3ValveWheelBuilder : InjectionBuilder
{
    private static readonly List<string> _targets =
    [
        TR3LevelNames.COASTAL,
        TR3LevelNames.ANTARC,
        TR3LevelNames.FISHES,
    ];

    private const int _fullTurn = 1024;
    private const int _originalSweep = 882;

    public override List<InjectionData> Build()
    {
        return [.. _targets.Select(levelName =>
        {
            var level = _control3.Read($"Resources/TR3/{levelName}");
            var wheel = level.Models[TR3Type.ValveWheelOrPulley];
            level.Models = new()
            {
                [TR3Type.ValveWheelOrPulley] = wheel,
            };

            wheel.Animations.ForEach(ExpandTurn);

            var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.General, $"{_tr3NameMap[levelName]}_wheel_frames");
            CreateDefaultTests(data, $"TR3/{levelName}");
            data.FrameReplacements.AddRange(TRFrameReplacement.CreateFrom(level));
            return data;
        })];
    }

    private static void ExpandTurn(TRAnimation anim)
    {
        // The turning animations sweep 310 degrees but chain into an idle frame
        // at zero, so the wheel jumps back once Lara lets go. Stretching each
        // sweep into a whole revolution lands it on the idle pose instead.
        foreach (var frame in anim.Frames)
        {
            var rotation = frame.Rotations[0];
            var angle = TRAngleUtils.ToGame(rotation.Z);
            if (angle == 0)
            {
                continue;
            }

            var turned = (int)Math.Round((angle - _fullTurn) * (double)_fullTurn / _originalSweep);
            rotation.Z = TRAngleUtils.FromGame((short)((turned % _fullTurn + _fullTurn) % _fullTurn));
        }
    }
}
