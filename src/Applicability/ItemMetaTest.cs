using TRLevelControl;
using TRLevelControl.Model;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Applicability;

public class ItemMetaTest : ApplicabilityTest
{
    public override ApplicabilityType Type => ApplicabilityType.ItemMeta;
    public int Index { get; set; }
    public int TypeID { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public short Room { get; set; }
    public short Angle { get; set; }

    protected override void SerializeImpl(TRLevelWriter writer, TRGameVersion version)
    {
        writer.Write(Index);
        writer.Write(TypeID, TRObjectType.Game, version);
        writer.Write(X);
        writer.Write(Y);
        writer.Write(Z);
        writer.Write(Room);
        writer.Write(Angle);
    }
}
