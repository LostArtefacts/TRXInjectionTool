using TRLevelControl;
using TRLevelControl.Model;

namespace TRXInjectionTool.Actions;

public abstract class TRRoomTextureEdit
{
    public enum TRRoomTextureFixType
    {
        Reface,
        MoveFace,
        MoveVertex,
        RotateFace,
        AddFace,
        AddVertex,
    }

    public abstract TRRoomTextureFixType FixType { get; }
    public short RoomIndex { get; set; }
    public TRMeshFaceType FaceType { get; set; }
    public abstract uint AdditionalMeshLength { get; }

    public void Serialize(TRLevelWriter writer)
    {
        writer.Write((uint)FixType);
        writer.Write(RoomIndex);
        writer.Write((uint)FaceType);
        SerializeImpl(writer);
    }

    protected abstract void SerializeImpl(TRLevelWriter writer);
}

public class TRRoomTextureReface : TRRoomTextureEdit
{
    // Take the texture on level.Rooms[RoomIndex].{Rectangles|Triangles}[SourceIndex}
    // and apply it to level.Rooms[RoomIndex].{Rectangles|Triangles}[TargetIndex}
    public override TRRoomTextureFixType FixType => TRRoomTextureFixType.Reface;

    public override uint AdditionalMeshLength => 0;

    public short SourceRoom { get; set; }
    public TRMeshFaceType SourceFaceType { get; set; }
    public short SourceIndex { get; set; }
    public short TargetIndex { get; set; }

    protected override void SerializeImpl(TRLevelWriter writer)
    {
        writer.Write(TargetIndex);
        writer.Write(SourceRoom);
        writer.Write((uint)SourceFaceType);
        writer.Write(SourceIndex);
    }
}

public class TRRoomTextureMove : TRRoomTextureEdit
{
    public override TRRoomTextureFixType FixType => TRRoomTextureFixType.MoveFace;
    public override uint AdditionalMeshLength => 0;
    public short TargetIndex { get; set; }
    public List<TRRoomVertexRemap> VertexRemap { get; set; }

    protected override void SerializeImpl(TRLevelWriter writer)
    {
        writer.Write(TargetIndex);
        writer.Write((uint)VertexRemap.Count);
        VertexRemap.ForEach(v => v.Serialize(writer));
    }
}

public class TRRoomVertexMove : TRRoomTextureEdit
{
    public override TRRoomTextureFixType FixType => TRRoomTextureFixType.MoveVertex;
    public override uint AdditionalMeshLength => 0;
    public ushort VertexIndex { get; set; }
    public TRVertex VertexChange { get; set; }
    public short ShadeChange { get; set; }

    protected override void SerializeImpl(TRLevelWriter writer)
    {
        writer.Write(VertexIndex);
        writer.Write(VertexChange.X);
        writer.Write(VertexChange.Y);
        writer.Write(VertexChange.Z);
        writer.Write(ShadeChange);
    }
}

public class TRRoomVertexRemap
{
    public short Index { get; set; }
    public ushort NewVertexIndex { get; set; }

    public void Serialize(TRLevelWriter writer)
    {
        writer.Write(Index);
        writer.Write(NewVertexIndex);
    }
}

public class TRRoomTextureCreate : TRRoomTextureEdit
{
    public override TRRoomTextureFixType FixType => TRRoomTextureFixType.AddFace;
    public override uint AdditionalMeshLength => (uint)(Vertices.Count + 1);
    public short SourceRoom { get; set; }
    public short SourceIndex { get; set; }
    public List<ushort> Vertices { get; set; }

    protected override void SerializeImpl(TRLevelWriter writer)
    {
        writer.Write(SourceRoom);
        writer.Write(SourceIndex);
        writer.Write(Vertices);
    }
}

public class TRRoomVertexCreate : TRRoomTextureEdit
{
    public override TRRoomTextureFixType FixType => TRRoomTextureFixType.AddVertex;
    public override uint AdditionalMeshLength => 4u;
    public TR1RoomVertex Vertex { get; set; }

    protected override void SerializeImpl(TRLevelWriter writer)
    {
        writer.Write(Vertex.Vertex.X);
        writer.Write(Vertex.Vertex.Y);
        writer.Write(Vertex.Vertex.Z);
        writer.Write(Vertex.Lighting);
    }
}

public class TRRoomTextureRotate : TRRoomTextureEdit
{
    public override TRRoomTextureFixType FixType => TRRoomTextureFixType.RotateFace;
    public override uint AdditionalMeshLength => 0;
    public short TargetIndex { get; set; }
    public byte Rotations { get; set; }

    protected override void SerializeImpl(TRLevelWriter writer)
    {
        writer.Write(TargetIndex);
        writer.Write(Rotations);
    }
}
