using TRLevelControl;
using TRLevelControl.Model;

namespace TRXInjectionTool.Actions;

public class TRVertexEdit
{
    public short Index { get; set; }
    public TRVertex Change { get; set; }

    public void Serialize(TRLevelWriter writer)
    {
        writer.Write(Index);
        writer.Write(Change);
    }
}
