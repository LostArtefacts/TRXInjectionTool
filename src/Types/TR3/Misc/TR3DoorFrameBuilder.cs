using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Misc;

public class TR3DoorFrameBuilder : InjectionBuilder
{
    private readonly Dictionary<string, TR3Type> _targets = new()
    {
        [TR3LevelNames.GANGES] = TR3Type.Door1,
        [TR3LevelNames.NEVADA] = TR3Type.Door2,
        [TR3LevelNames.ANTARC] = TR3Type.Door4,
        [TR3LevelNames.WILLIE] = TR3Type.Door1,
        [TR3LevelNames.CLIFF] = TR3Type.Door2,
    };

    public override List<InjectionData> Build()
    {
        return [.. _targets.Select(kvp =>
        {
            var level = _control3.Read($"Resources/TR3/{kvp.Key}");
            var doorType = kvp.Value;
            level.Models = new()
            {
                [doorType] = level.Models[doorType],
            };

            level.Models[doorType].Animations[1].Frames[0].Rotations[0].Y = 768;

            var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.General, $"{_tr3NameMap[kvp.Key]}_door{(int)doorType}_frames");
            CreateDefaultTests(data, $"TR3/{kvp.Key}");
            data.FrameReplacements.AddRange(TRFrameReplacement.CreateFrom(level));
            return data;
        })];
    }
}
