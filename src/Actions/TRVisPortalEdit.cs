using System.Diagnostics;
using TRLevelControl;
using TRLevelControl.Model;

namespace TRXInjectionTool.Actions;

public class TRVisPortalEdit
{
    public short BaseRoom { get; set; }
    public short LinkRoom { get; set; }
    public ushort PortalIndex { get; set; }
    public List<TRVertex> VertexChanges { get; set; }

    public void Serialize(TRLevelWriter writer)
    {
        Debug.Assert(VertexChanges.Count == 4);
        writer.Write(BaseRoom);
        writer.Write(LinkRoom);
        writer.Write(PortalIndex);
        writer.Write(VertexChanges);
    }
}
