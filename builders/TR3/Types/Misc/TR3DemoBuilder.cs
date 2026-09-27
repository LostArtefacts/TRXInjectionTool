using System.Diagnostics;
using Newtonsoft.Json;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;
using TRXInjectionTool.Types.TR3.Lara;

namespace TRXInjectionTool.Types.TR3.Misc;

// Carries the demos from the PS1 release, taken from the end of
// DEMO1.PSX, DEMO2.PSX and DEMO3.PSX on the disc. The PC release has none.
public class TR3DemoBuilder : InjectionBuilder
{
    private const int _runToSprintLeftAnim = 224;
    private const int _runToSprintRightAnim = 225;
    private const int _extraStepFrame = 23;

    private static readonly List<(string Level, string Name)> _demos =
    [
        (TR3LevelNames.ALDWYCH, "aldwych"),
        (TR3LevelNames.GANGES, "ganges"),
        (TR3LevelNames.NEVADA, "nevada"),
    ];

    public override List<InjectionData> Build()
    {
        var result = new List<InjectionData>();
        foreach (var (level, name) in _demos)
        {
            var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.General, $"{name}_demo");
            CreateDefaultTests(data, $"TR3/{level}");

            var bytes = File.ReadAllBytes($"Resources/TR3/Demos/{name}.bin");
            for (int i = 0; i < bytes.Length; i += sizeof(uint))
            {
                data.DemoData.Add(BitConverter.ToUInt32(bytes, i));
            }

            AddPS1Bounds(data, level, name);
            AddPS1SprintSteps(data);
            result.Add(data);
        }

        return result;
    }

    // Some models have different frame bounds on the PS1, and Lara aims at the
    // middle of these bounds. The demos stay in sync only with the PS1 values,
    // which were read from the console memory while each demo played.
    private static void AddPS1Bounds(InjectionData data, string levelName, string name)
    {
        var path = $"Resources/TR3/Demos/{name}_bounds.json";
        if (!File.Exists(path))
        {
            return;
        }

        var bounds = JsonConvert.DeserializeObject<Dictionary<TR3Type, List<List<short[]>>>>(File.ReadAllText(path));
        var level = _control3.Read($"Resources/TR3/{levelName}");
        foreach (var (type, anims) in bounds)
        {
            var model = level.Models[type];
            for (int i = 0; i < anims.Count; i++)
            {
                var frames = model.Animations[i].Frames;
                for (int j = 0; j < anims[i].Count; j++)
                {
                    var b = anims[i][j];
                    frames[j].Bounds = new()
                    {
                        MinX = b[0],
                        MaxX = b[1],
                        MinY = b[2],
                        MaxY = b[3],
                        MinZ = b[4],
                        MaxZ = b[5],
                    };
                }
            }
        }

        data.FrameReplacements.AddRange(TRFrameReplacement.CreateFrom(level, bounds.Keys));
    }

    // The Lara animations that TRX ships add a third footstep to the starts of
    // the sprint. The PS1 release has only two, and the demos depend on the
    // random numbers that each footstep sound uses. The commands are taken
    // from the same Lara that TRX ships, so that their frame numbers match.
    private static void AddPS1SprintSteps(InjectionData data)
    {
        var level = new TR3LaraAnimBuilder().CreateLevel();
        var lara = level.Models[TR3Type.Lara];

        int[] animIndices = [_runToSprintLeftAnim, _runToSprintRightAnim];
        for (int i = 0; i < lara.Animations.Count; i++)
        {
            var commands = lara.Animations[i].Commands;
            if (animIndices.Contains(i))
            {
                commands.RemoveAll(c => (c is TRSFXCommand s && s.FrameNumber == _extraStepFrame)
                    || (c is TRFootprintCommand f && f.FrameNumber == _extraStepFrame));
            }
            else
            {
                commands.Clear();
            }
        }

        var cmdData = InjectionData.Create(level, InjectionType.General, "sprint_steps", true);
        data.AnimCommands.AddRange(cmdData.AnimCommands);
        foreach (int animIdx in animIndices)
        {
            data.AnimCmdEdits.Add(CreateAnimCmdEdit(level, TR3Type.Lara, animIdx));
        }
        Debug.Assert(data.AnimCmdEdits.Sum(c => c.RawCount) == data.AnimCommands.Count);
    }
}
