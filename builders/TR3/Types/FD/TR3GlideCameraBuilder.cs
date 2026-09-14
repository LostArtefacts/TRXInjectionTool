using System.Diagnostics;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.FD;

public class TR3GlideCameraBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.General, "tinnos_cameras");
        CreateDefaultTests(data, $"TR3/{TR3LevelNames.TINNOS}");
        data.FloorEdits.Add(FixTinnosCamera());
        return [data];
    }

    private static TRFloorDataEdit FixTinnosCamera()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.TINNOS}");
        var trigger = GetTrigger(level, 44, 14, 5);
        var camAction = trigger?.Actions.FirstOrDefault(a => a.Action == FDTrigAction.Camera);
        Debug.Assert(camAction != null);

        var fix = new FDGlideCameraFix
        {
            Timer = camAction.CamAction.Timer,
            Glide = camAction.CamAction.MoveTimer,
            Shift = new() { Y = -1280 },
        };

        return new()
        {
            RoomIndex = 44,
            X = 14,
            Z = 5,
            Fixes = [fix],
        };
    }
}
