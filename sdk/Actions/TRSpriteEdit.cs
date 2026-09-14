using TRLevelControl;
using TRLevelControl.Model;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Actions;

public class TRSpriteEdit
{
    public int ID { get; set; }
    public TRSpriteAlignment Alignment { get; set; }

    public void Serialize(TRLevelWriter writer, TRGameVersion version)
    {
        writer.Write(ID, TRObjectType.Game, version);
        writer.Write(Alignment.Left);
        writer.Write(Alignment.Top);
        writer.Write(Alignment.Right);        
        writer.Write(Alignment.Bottom);
    }
}
