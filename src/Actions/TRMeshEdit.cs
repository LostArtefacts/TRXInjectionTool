using TRLevelControl;
using TRLevelControl.Model;

namespace TRXInjectionTool.Actions;

public class TRMeshEdit
{
    public uint ModelID { get; set; }
    public short MeshIndex { get; set; }
    public TRVertex Centre { get; set; } = new();
    public int CollRadius { get; set; }
    public List<TRFaceTextureEdit> FaceEdits { get; set; } = new();
    public List<TRVertexEdit> VertexEdits { get; set; } = new();

    public void Serialize(TRLevelWriter writer)
    {
        writer.Write(ModelID);
        writer.Write(MeshIndex);
        writer.Write(Centre);
        writer.Write(CollRadius);

        writer.Write((uint)FaceEdits.Count);
        FaceEdits.ForEach(f => f.Serialize(writer));

        writer.Write((uint)VertexEdits.Count);
        VertexEdits.ForEach(v => v.Serialize(writer));
    }
}
