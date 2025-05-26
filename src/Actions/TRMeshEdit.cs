using TRLevelControl;
using TRLevelControl.Model;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Actions;

public class TRMeshEdit
{
    public uint ModelID { get; set; }
    public short MeshIndex { get; set; }
    public TRVertex Centre { get; set; } = new();
    public int CollRadius { get; set; }
    public List<TRFaceTextureEdit> FaceEdits { get; set; } = new();
    public List<TRVertexEdit> VertexEdits { get; set; } = new();

    public void Serialize(TRLevelWriter writer, TRGameVersion version)
    {
        int sceneryBase = version == TRGameVersion.TR1 ? (int)TR1Type.SceneryBase : (int)TR2Type.SceneryBase;
        TRObjectType type = ModelID >= sceneryBase ? TRObjectType.Static3D : TRObjectType.Game;
        writer.Write((int)ModelID, type, version);
        writer.Write(MeshIndex);
        writer.Write(Centre);
        writer.Write(CollRadius);

        writer.Write((uint)FaceEdits.Count);
        FaceEdits.ForEach(f => f.Serialize(writer, version));

        writer.Write((uint)VertexEdits.Count);
        VertexEdits.ForEach(v => v.Serialize(writer));
    }
}
