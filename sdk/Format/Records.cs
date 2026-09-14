namespace TRXInjectionTool.Format;

// Canonical data records. One layout per record for every game; converters
// in the engine drop fields the target game has no use for.

public enum SymbolContext
{
    Objects = 0,
    Music = 1,
    Samples = 2,
    LaraStates = 3,
    LaraAnims = 4,
    ItemActions = 5,
}

[FormatRecord]
[Block(Container.Symbols, 40, "SYMBOLS", Doc = "The symbols chunk is always written first when present; later records may reference the table.")]
public sealed class Symbol
{
    [Doc("SymbolContext")]
    public int Context;
    [Doc("the local slot this name stands for; records meaning the name write this slot with a symbol marker")]
    public int Slot;
    [Doc("ASCII, max 255 bytes")]
    public string Name;
    [Doc("reserved, 0")]
    public int Flags;
}

[FormatRecord]
[Block(Container.TextureData, 0, "PALETTE")]
public sealed class PaletteEntry
{
    public FormatColour Colour;
}

[FormatRecord(Doc = "All 32-bit pages' pixels, then all 8-bit pages' pixels; elementCount is the page count.")]
[Block(Container.TextureData, 1, "TEXTURE_PAGES")]
public sealed class TexturePages
{
    [ImpliedLength("elementCount * 256 * 256 pixels of u32, then as many of u8")]
    public byte[] Pixels;
}

[FormatRecord(Doc = "TR4 superset; the trailing metadata is always computed from source UVs and ignored by converters for games without it.")]
[Block(Container.TextureInfo, 2, "OBJECT_TEXTURES")]
public sealed class ObjectTexture
{
    public ushort Attribute;
    public ushort TileAndFlag;
    [Doc("0 when the source had none")]
    public ushort NewFlags;
    [FixedLength(4)]
    [Doc("unused 4th vertex zeroed")]
    public FormatUV[] Vertices;
    public uint OriginalU;
    public uint OriginalV;
    public uint WidthMinusOne;
    public uint HeightMinusOne;
}

[FormatRecord]
[Block(Container.TextureInfo, 3, "SPRITE_TEXTURES")]
public sealed class SpriteTexture
{
    public ushort Atlas;
    public byte X;
    public byte Y;
    public ushort Width;
    public ushort Height;
    public short Left;
    public short Top;
    public short Right;
    public short Bottom;
}

[FormatRecord(Doc = "For slot-referenced sequences length is negative (the classic convention) and startIndex is 0; for symbol-named sequences length is the positive sprite count and startIndex locates the first sprite texture this file brings.")]
[Block(Container.TextureInfo, 4, "SPRITE_SEQUENCES")]
public sealed class SpriteSequence
{
    public FormatObjRef SpriteID;
    public short Length;
    public ushort StartIndex;
}

[FormatRecord]
[Block(Container.MeshData, 6, "MESH_POINTERS")]
public sealed class MeshPointers
{
    [ImpliedLength("elementCount entries")]
    public uint[] Pointers;
}

[FormatRecord(Doc = "One face encoding for all games; records are packed, no trailing alignment. elementCount counts meshes (TRXJ counted 16-bit words).")]
[Block(Container.MeshData, 5, "OBJECT_MESHES")]
public sealed class Mesh
{
    public FormatVertex Centre;
    public int CollRadius;
    [LengthPrefix(typeof(short))]
    public FormatVertex[] Vertices;
    [Doc("> 0: that many normals follow; < 0: that many lights follow")]
    public short NormalCount;
    [ImpliedLength("NormalCount when positive")]
    public FormatVertex[] Normals;
    [ImpliedLength("-NormalCount when negative")]
    public short[] Lights;
    [LengthPrefix(typeof(ushort))]
    public Face4[] TexturedQuads;
    [LengthPrefix(typeof(ushort))]
    public Face3[] TexturedTriangles;
    [LengthPrefix(typeof(ushort))]
    [Doc("empty for sources without coloured faces")]
    public Face4[] ColouredQuads;
    [LengthPrefix(typeof(ushort))]
    public Face3[] ColouredTriangles;
}

[FormatRecord]
public sealed class Face4
{
    [FixedLength(4)]
    public ushort[] Vertices;
    public ushort Texture;
    [Doc("0 where the source game has none")]
    public ushort Effects;
}

[FormatRecord]
public sealed class Face3
{
    [FixedLength(3)]
    public ushort[] Vertices;
    public ushort Texture;
    public ushort Effects;
}

[FormatRecord]
[Block(Container.AnimationData, 7, "ANIM_CHANGES")]
public sealed class AnimChange
{
    public ushort StateID;
    public ushort NumAnimDispatches;
    public ushort AnimDispatch;
}

[FormatRecord]
[Block(Container.AnimationData, 8, "ANIM_RANGES")]
public sealed class AnimRange
{
    public short Low;
    public short High;
    public short NextAnimation;
    public short NextFrame;
}

[FormatRecord]
[Block(Container.AnimationData, 9, "ANIM_COMMANDS")]
public sealed class AnimCommand
{
    [Doc("raw command word")]
    public short Value;
}

[FormatRecord]
[Block(Container.AnimationData, 10, "ANIM_BONES")]
public sealed class AnimBone
{
    public uint Flags;
    public int OffsetX;
    public int OffsetY;
    public int OffsetZ;
}

[FormatRecord]
public sealed class FrameRotation
{
    [Doc("16-bit angle units")]
    public short X;
    public short Y;
    public short Z;
}

[FormatRecord(Doc = "One canonical layout for every game; the engine packs frames into its native form at load.")]
[Block(Container.AnimationData, 11, "ANIM_FRAMES")]
public sealed class AnimFrame
{
    public short MinX;
    public short MaxX;
    public short MinY;
    public short MaxY;
    public short MinZ;
    public short MaxZ;
    public short OffsetX;
    public short OffsetY;
    public short OffsetZ;
    [LengthPrefix(typeof(ushort))]
    [Doc("one rotation per mesh")]
    public FrameRotation[] Rotations;
}

[FormatRecord(Doc = "TR4 superset; converters drop fields the target game lacks.")]
[Block(Container.AnimationData, 12, "ANIMS")]
public sealed class Animation
{
    [Doc("ordinal of the animation's first frame in this file's ANIM_FRAMES")]
    public uint FrameOffset;
    public byte FrameRate;
    public byte FrameSize;
    public ushort StateID;
    public FormatFixed32 Speed;
    public FormatFixed32 Accel;
    [Doc("0 for games without lateral motion")]
    public FormatFixed32 LateralSpeed;
    [Doc("0 likewise")]
    public FormatFixed32 LateralAccel;
    public ushort FrameStart;
    public ushort FrameEnd;
    public ushort NextAnimation;
    public ushort NextFrame;
    public ushort NumStateChanges;
    public ushort StateChangeOffset;
    public ushort NumAnimCommands;
    public ushort AnimCommand;
}

[FormatRecord]
[Block(Container.ObjectData, 13, "OBJECTS")]
public sealed class Model
{
    public FormatObjRef ID;
    public ushort NumMeshes;
    public ushort StartingMesh;
    public uint MeshTree;
    [Doc("frame ordinal in this file's ANIM_FRAMES; 0xFFFFFFFF = mesh-only model")]
    public uint FrameOffset;
    public ushort Animation;
}

[FormatRecord]
[Block(Container.ObjectData, 29, "STATIC_OBJECTS")]
public sealed class StaticObject
{
    public uint ID;
    [Doc("mesh pointer index")]
    public ushort Mesh;
    [FixedLength(6)]
    [Doc("minX, maxX, minY, maxY, minZ, maxZ")]
    public short[] VisBox;
    [FixedLength(6)]
    public short[] CollBox;
    public ushort Flags;
}

[Union(typeof(byte), Doc = "How a sample's audio data is carried.")]
public abstract class SfxSample
{
}

[Case(0)]
public sealed class SfxSampleInline : SfxSample
{
    [LengthPrefix(typeof(uint))]
    public byte[] WavData;
}

[Case(1)]
public sealed class SfxSampleRef : SfxSample
{
    [Doc("index into the game's main[_gold].sfx")]
    public uint Index;
}

[FormatRecord(Doc = "Converters apply per-game volume scaling and drop pitch/range for games without them.")]
[Block(Container.SfxData, 14, "SAMPLE_INFOS")]
[Block(Container.SfxData, 41, "NAMED_SAMPLE_INFOS", Doc = "id is a symbol-table index")]
public sealed class Sfx
{
    [Doc("SAMPLE_INFOS: sound id; NAMED_SAMPLE_INFOS: symbol index")]
    public short ID;
    [Doc("canonical 0-0x7FFF scale")]
    public ushort Volume;
    public ushort Chance;
    [Doc("0 where the source game has none")]
    public byte Pitch;
    [Doc("0 likewise")]
    public byte Range;
    public ushort Characteristics;
    [LengthPrefix(typeof(byte))]
    public SfxSample[] Samples;
}

[FormatRecord]
[Block(Container.CameraData, 30, "CINEMATIC_FRAMES")]
public sealed class CinematicFrame
{
    public short TargetX;
    public short TargetY;
    public short TargetZ;
    public short PosZ;
    public short PosY;
    public short PosX;
    public short FOV;
    public short Roll;
}

[FormatRecord]
[Block(Container.CameraData, 38, "FLYBY_CAMERAS")]
public sealed class FlybyCamera
{
    public int X;
    public int Y;
    public int Z;
    public int DX;
    public int DY;
    public int DZ;
    public byte Sequence;
    public byte Index;
    public ushort FOV;
    public short Roll;
    public ushort Timer;
    public ushort Speed;
    public ushort Flags;
    public uint RoomID;
}
