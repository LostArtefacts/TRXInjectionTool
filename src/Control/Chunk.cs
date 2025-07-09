using TRLevelControl;

namespace TRXInjectionTool.Control;

public class Chunk
{
    public ChunkType Type { get; set; }
    public int BlockCount { get; set; }
    public byte[] Data { get; set; }

    public void Serialize(TRLevelWriter writer)
    {
        writer.Write((int)Type);
        writer.Write(BlockCount);
        writer.Write(Data.Length);
        writer.Write(Data);
    }
}

public enum ChunkType
{
    TextureData   = 0,
    TextureInfo   = 1,
    MeshData      = 2,
    AnimationData = 3,
    ObjectData    = 4,
    SFXData       = 5,
    DataEdits     = 6,
    CameraData    = 7,
}

public enum BlockType
{
    Palette         = 0,
    Images          = 1,
    ObjectTextures  = 2,
    SpriteTextures  = 3,
    SpriteSequences = 4,
    ObjectMeshes    = 5,
    MeshPointers    = 6,
    AnimChanges     = 7,
    AnimDispatches  = 8,
    AnimCommands    = 9,
    MeshTrees       = 10,
    AnimFrames      = 11,
    Animations      = 12,
    Objects         = 13,
    SampleInfos     = 14,
    SampleIndices   = 15,
    SampleData      = 16,
    FloorEdits      = 17,
    ItemEdits       = 18,
    MeshEdits       = 19,
    TextureEdits    = 20,
    RoomEditSummary = 21,
    RoomEdits       = 22,
    VisPortalEdits  = 23,
    CameraEdits     = 24,
    FrameEdits      = 25,
    StaticEdits     = 26,
    AnimCmdEdits    = 27,
    SpriteEdit      = 28,
    StaticObjects   = 29,
    CinematicFrames = 30,
    ObjectTypeEdits = 31,
}
