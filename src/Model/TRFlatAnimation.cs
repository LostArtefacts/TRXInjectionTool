using TRLevelControl;
using TRLevelControl.Model;
using TRLevelReader.Model;

namespace TRXInjectionTool.Model;

public class TRFlatAnimation
{
    public uint FrameOffset { get; set; }
    public byte FrameRate { get; set; }
    public byte FrameSize { get; set; }
    public ushort StateID { get; set; }
    public FixedFloat32 Speed { get; set; }
    public FixedFloat32 Accel { get; set; }
    public FixedFloat32 LateralSpeed { get; set; }
    public FixedFloat32 LateralAccel { get; set; }
    public ushort FrameStart { get; set; }
    public ushort FrameEnd { get; set; }
    public ushort NextAnimation { get; set; }
    public ushort NextFrame { get; set; }
    public ushort NumStateChanges { get; set; }
    public ushort StateChangeOffset { get; set; }
    public ushort NumAnimCommands { get; set; }
    public ushort AnimCommand { get; set; }

    public void Serialize(TRLevelWriter writer, TRGameVersion version)
    {
        writer.Write(FrameOffset);
        writer.Write(FrameRate);
        writer.Write(FrameSize);
        writer.Write(StateID);
        writer.Write(Speed.Serialize());
        writer.Write(Accel.Serialize());
        if (version >= TRGameVersion.TR4)
        {
            writer.Write(LateralSpeed.Serialize());
            writer.Write(LateralAccel.Serialize());
        }
        writer.Write(FrameStart);
        writer.Write(FrameEnd);
        writer.Write(NextAnimation);
        writer.Write(NextFrame);
        writer.Write(NumStateChanges);
        writer.Write(StateChangeOffset);
        writer.Write(NumAnimCommands);
        writer.Write(AnimCommand);
    }
}
