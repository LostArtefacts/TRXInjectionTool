using TRLevelControl;

namespace TRXInjectionTool.Applicability;

public class RoomCountTest : ApplicabilityTest
{
    public override ApplicabilityType Type => ApplicabilityType.RoomCount;
    public int RoomCount { get; set; }

    protected override void SerializeImpl(TRLevelWriter writer)
    {
        writer.Write(RoomCount);
    }
}
