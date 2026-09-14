using TRLevelControl.Model;
using TRXInjectionTool.Actions;

namespace TRXInjectionTool.Types;

public abstract class CameraBuilder : InjectionBuilder
{
    protected static TRCameraEdit SetCameraPosition(TRCamera camera, short index, int x = -1, int y = -1, int z = -1, short roomNum = -1)
    {
        return new()
        {
            Index = index,
            Camera = new()
            {
                X = x == -1 ? camera.X : x,
                Y = y == -1 ? camera.Y : y,
                Z = z == -1 ? camera.Z : z,
                Room = roomNum == -1 ? camera.Room : roomNum,
                Flag = camera.Flag,
            },
        };
    }

    protected static TRCameraEdit SetCameraPosition(TR1Level level, short index, int x = -1, int y = -1, int z = -1, short roomNum = -1)
    {
        return SetCameraPosition(level.Cameras[index], index, x, y, z, roomNum);
    }

    protected static TRCameraEdit SetCameraPosition(TR2Level level, short index, int x = -1, int y = -1, int z = -1, short roomNum = -1)
    {
        return SetCameraPosition(level.Cameras[index], index, x, y, z, roomNum);
    }
}
