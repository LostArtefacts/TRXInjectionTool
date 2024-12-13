using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Cameras;

public class TR1MinesCameraBuilder : CameraBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level mines = _control1.Read($"Resources/{TR1LevelNames.MINES}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.General, "mines_cameras");

        // Prevent seeing the flipmap boat while the actual boat is moving into position.
        data.CameraEdits.Add(SetCameraPosition(mines, 7, 53760, -7296, 28160, 3));

        return new() { data };
    }
}
