using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR4.FD;

public class TR4AngkorFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR4, InjectionType.FDFix, "angkor_fd");
        CreateDefaultTests(data, $"TR4/{TR4LevelNames.ANGKOR}");
        FixRoom60Portal(data);

        return [data];
    }

    private static void FixRoom60Portal(InjectionData data)
    {
        data.VisPortalEdits.Add(new()
        {
            BaseRoom = 60,
            LinkRoom = 64,
            PortalIndex = 2,
            VertexChanges =
            [
                new() { Z = -TRConsts.Step4 },
                new(),
                new(),
                new() { Z = -TRConsts.Step4 },
            ],
        });
        data.VisPortalEdits.Add(new()
        {
            BaseRoom = 64,
            LinkRoom = 60,
            PortalIndex = 2,
            VertexChanges =
            [
                new() { Z = -TRConsts.Step4 },
                new() { Z = -TRConsts.Step4 },
                new(),
                new(),
            ],
        });

        data.FloorEdits.Add(new()
        {
            RoomIndex = 64,
            X = 1,
            Z = 2,
            Fixes = [new FDPortalOverwrite { Sky = -1, Pit = 69 }],
        });
    }
}
