using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Cameras;

public class TR1EgyptCameraBuilder : CameraBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level egypt = _control1.Read($"Resources/{TR1LevelNames.EGYPT}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.General, "egypt_cameras");

        // Fix LOS issues with the glide camera in room 20.
        data.CameraEdits.Add(SetCameraPosition(egypt, 4, 76672, -7648, 16384, 20));

        return new() { data };
    }
}
