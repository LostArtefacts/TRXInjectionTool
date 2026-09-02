using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.FD;

public class TR3CompoundFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.FDFix, "compound_fd");
        CreateDefaultTests(data, $"TR3/{TR3LevelNames.HSC}");
        data.FloorEdits.Add(FixLadder93());

        return [data];
    }

    private static TRFloorDataEdit FixLadder93()
    {
        return new()
        {
            RoomIndex = 93,
            X = 5,
            Z = 2,
            Fixes = [new FDPortalOverwrite
            {
                Wall = 87,
            }],
        };
    }
}
