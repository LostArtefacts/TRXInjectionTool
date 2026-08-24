using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.FD;

public class TR3AntarcticaFDBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.FDFix, "antarc_fd");
        CreateDefaultTests(data, $"TR3/{TR3LevelNames.ANTARC}");
        data.FloorEdits.Add(FixRoom20Portal());

        return [data];
    }

    private static TRFloorDataEdit FixRoom20Portal()
    {
        return new TRFloorDataEdit
        {
            RoomIndex = 20,
            X = 6,
            Z = 6,
            Fixes = [new FDPortalOverwrite
            {
                Sky = 19,
            }],
        };
    }
}
