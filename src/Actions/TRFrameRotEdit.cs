using TRLevelControl;
using TRLevelControl.Model;

namespace TRXInjectionTool.Actions;

public class TRFrameRotEdit
{
    // Temporary measure until further frame re-handling engine side i.e. to target
    // specific frame and rot indices.

    public uint ModelID { get; set; }
    public int AnimIndex { get; set; }
    public TRVertex Rotation { get; set; }

    public void Serialize(TRLevelWriter writer)
    {
        writer.Write(ModelID);
        writer.Write(AnimIndex);        
        writer.Write(PackYZRotation(Rotation.Y, Rotation.Z));
        writer.Write(PackXYRotation(Rotation.X, Rotation.Y));
    }

    private static short PackXYRotation(int x, int y)
    {
        return (short)((x << 4) | ((y & 0x0FC0) >> 6));
    }

    private static short PackYZRotation(int y, int z)
    {
        return (short)(((y & 0x003F) << 10) | (z & 0x03FF));
    }
}
