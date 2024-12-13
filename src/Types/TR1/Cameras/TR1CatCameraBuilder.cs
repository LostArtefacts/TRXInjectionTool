using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Cameras;

public class TR1CatCameraBuilder : CameraBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level cat = _control1.Read($"Resources/{TR1LevelNames.CAT}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.General, "cat_cameras");

        // Move this camera outside to avoid LOS issues.
        data.CameraEdits.Add(SetCameraPosition(cat, 0, 73411, -7448, 49152, 0));

        // Item 74 is an unused camera target, so move this to room 10 and have the camera there
        // look at it instead of the trapdoor, again to avoid LOS issues.
        data.ItemEdits.Add(new()
        {
            Index = 74,
            Item = new()
            {
                X = 71168,
                Y = -5504,
                Z = 61952,
                Room = 10,
            },
        });

        // Update the switch trigger in both flip states.
        foreach (short room in new short[] { 20, 107 })
        {
            data.FloorEdits.Add(new()
            {
                RoomIndex = room,
                X = 7,
                Z = 1,
                Fixes = new()
                {
                    new FDTrigParamFix
                    {
                        ActionType = FDTrigAction.LookAtItem,
                        OldParam = 38,
                        NewParam = 74,
                    }
                }
            });
        }

        return new() { data };
    }
}
