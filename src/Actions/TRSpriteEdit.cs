using TRLevelControl;
using TRLevelControl.Model;

namespace TRXInjectionTool.Actions;

public class TRSpriteEdit
{
    public int ID { get; set; }
    public TRSpriteAlignment Alignment { get; set; }

    public void Serialize(TRLevelWriter writer)
    {
        writer.Write(ID);
        writer.Write(Alignment.Left);
        writer.Write(Alignment.Top);
        writer.Write(Alignment.Right);        
        writer.Write(Alignment.Bottom);
    }
}
