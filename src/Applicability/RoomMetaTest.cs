using TRLevelControl;
using TRLevelControl.Model;

namespace TRXInjectionTool.Applicability;

public class RoomMetaTest : ApplicabilityTest
{
    public override ApplicabilityType Type => ApplicabilityType.RoomMeta;
    public int Index { get; set; }
    public TRRoomInfo Info { get; set; }
    public ushort XSize { get; set; }
    public ushort ZSize { get; set; }

    protected override void SerializeImpl(TRLevelWriter writer, TRGameVersion version)
    {
        writer.Write(Index);
        writer.Write(Info, TRGameVersion.TR1);
        writer.Write(XSize);
        writer.Write(ZSize);
    }
}
