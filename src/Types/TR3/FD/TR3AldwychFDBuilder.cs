using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.FD;

public class TR3AldwychFDBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.General, "aldwych_fd");
        CreateDefaultTests(data, $"TR3/{TR3LevelNames.ALDWYCH}");
        data.FloorEdits.Add(FixDrillAntitrigger());

        return [data];
    }

    private static TRFloorDataEdit FixDrillAntitrigger()
    {
        return new()
        {
            RoomIndex = 116,
            X = 1,
            Z = 5,
            Fixes = [new FDTrigDelete()],
        };
    }
}
