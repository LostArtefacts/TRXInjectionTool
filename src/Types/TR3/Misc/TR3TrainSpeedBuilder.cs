using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Misc;

public class TR3TrainSpeedBuilder : InjectionBuilder
{
    private static readonly List<Setup> _setups =
    [
        new(TR3LevelNames.FISHES, "undersea", 200),
        new(TR3LevelNames.MADHOUSE, "zoo", 100),
    ];

    public override List<InjectionData> Build()
    {
        var result = new List<InjectionData>();
        foreach (var setup in _setups)
        {
            var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.General, $"{setup.Name}_train");
            CreateDefaultTests(data, $"TR3/{setup.Level}");
            result.Add(data);

            var level = _control3.Read($"Resources/TR3/{setup.Level}");
            var anim = level.Models[TR3Type.SubwayTrain].Animations[0];
            anim.Speed.Whole = setup.Speed;
            data.AnimEdits.Add(new()
            {
                ModelID = (uint)TR3Type.SubwayTrain,
                Anim = anim,
            });
        }

        return result;
    }

    private class Setup(string level, string name, short speed)
    {
        public string Level { get; set; } = level;
        public string Name { get; set; } = name;
        public short Speed { get; set; } = speed;
    }
}
