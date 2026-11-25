using TRLevelControl;
using TRLevelControl.Model;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Actions;

public class TRMeshEdit
{
    public uint ModelID { get; set; }
    public TRObjectType? EnforcedType { get; set; }
    public short MeshIndex { get; set; }
    public TRVertex Centre { get; set; } = new();
    public int CollRadius { get; set; }
    public List<TRFaceTextureEdit> FaceEdits { get; set; } = [];
    public List<TRFaceEffectsEdit> FaceEffects { get; set; } = [];
    public List<TRVertexEdit> VertexEdits { get; set; } = [];
    public List<TRMeshFaceRotation> FaceRotations { get; set; } = [];
    public List<TRMeshFaceAddition> FaceAdditions { get; set; } = [];

    public void Serialize(TRLevelWriter writer, TRGameVersion version)
    {
        TRObjectType type;
        if (EnforcedType.HasValue)
        {
            type = EnforcedType.Value;
        }
        else
        {
            int sceneryBase = version.GetSceneryBase();
            type = ModelID >= sceneryBase ? TRObjectType.Static3D : TRObjectType.Game;
        }
            
        writer.Write((int)ModelID, type, version);
        writer.Write(MeshIndex);
        writer.Write(Centre);
        writer.Write(CollRadius);

        writer.Write((uint)FaceEdits.Count);
        FaceEdits.ForEach(f => f.Serialize(writer, version));

        writer.Write((uint)VertexEdits.Count);
        VertexEdits.ForEach(v => v.Serialize(writer));

        writer.Write((uint)FaceEffects.Count);
        FaceEffects.ForEach(f => f.Serialize(writer));
    }
}

public class TRMeshFaceRotation
{
    public TRMeshFaceType FaceType { get; set; }
    public short FaceIndex { get; set; }
    public byte Rotations { get; set; }
}

public class TRMeshFaceAddition
{
    public TRMeshFaceType FaceType { get; set; }
    public ushort Texture { get; set; }
    public bool DoubleSided { get; set; }
    public ushort Effects { get; set; }
    public List<ushort> Vertices { get; set; } = [];
}
