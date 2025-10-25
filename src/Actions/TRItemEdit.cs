using TRLevelControl;
using TRLevelControl.Model;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Actions;

public class TRItemPosEdit
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

public class TRItemTypeFlagEdit
{
    public short Index { get; set; }
    public TR1Entity Item { get; set; }

    public void Serialize(TRLevelWriter writer, TRGameVersion version)
    {
        writer.Write(Index);
        writer.Write((int)Item.TypeID, TRObjectType.Game, version);
        writer.Write(Item.Flags);
    }
}
