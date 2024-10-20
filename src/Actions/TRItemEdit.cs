using TRLevelControl;
using TRLevelControl.Model;

namespace TRXInjectionTool.Actions;

public class TRItemEdit
{
    public short Index { get; set; }
    public TR1Entity Item { get; set; }

    public void Serialize(TRLevelWriter writer)
    {
        writer.Write(Index);
        writer.Write(Item.Angle);
        writer.Write(Item.X);
        writer.Write(Item.Y);
        writer.Write(Item.Z);
        writer.Write(Item.Room);
    }
}
