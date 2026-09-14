using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.FD;

public class TR3GymFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.FDFix, "gym_fd");
        CreateDefaultTests(data, $"TR3/{TR3LevelNames.ASSAULT}");

        data.FloorEdits.AddRange(FixZipTriggers());

        return [data];
    }

    private static IEnumerable<TRFloorDataEdit> FixZipTriggers()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.ASSAULT}");
        var trigger = new FDTriggerEntry
        {
            Mask = 31,
            Actions =
            [
                new() { Parameter = (short)level.Entities
                    .FindIndex(e => e.TypeID == TR3Type.ZiplineHandle) },
            ],
        };
        for (ushort z = 1; z < 4; z++)
        {
            yield return MakeTrigger(level, 42, 3, z, trigger);
        }
    }
}
