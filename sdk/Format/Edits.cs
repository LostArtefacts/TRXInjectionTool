namespace TRXInjectionTool.Format;

// Edit records carried in the DATA_EDITS chunk.

[FormatRecord]
[Block(Container.DataEdits, 18, "ITEM_POS_EDITS")]
public sealed class ItemPosEdit
{
    public short Index;
    public short Angle;
    public int X;
    public int Y;
    public int Z;
    public short Room;
}

[FormatRecord]
[Block(Container.DataEdits, 33, "ITEM_FLAG_EDITS")]
public sealed class ItemFlagEdit
{
    public short Index;
    public FormatObjRef Type;
    public ushort Flags;
}

[FormatRecord]
[Block(Container.DataEdits, 37, "ITEM_NAME_EDITS")]
public sealed class ItemNameEdit
{
    public short Index;
    public string Name;
}

[FormatRecord]
[Block(Container.DataEdits, 26, "OBJECT_3D_EDITS")]
public sealed class Object3DEdit
{
    public int TypeID;
    public byte Collidable;
    public byte Visible;
    [FixedLength(6)]
    [Doc("minX, maxX, minY, maxY, minZ, maxZ")]
    public short[] CollBox;
    [FixedLength(6)]
    public short[] VisBox;
}

[FormatRecord]
[Block(Container.DataEdits, 20, "TEXTURE_EDITS")]
public sealed class TextureOverwrite
{
    public ushort Page;
    public byte X;
    public byte Y;
    public ushort Width;
    public ushort Height;
    [ImpliedLength("Width * Height")]
    public uint[] Pixels;
}

[FormatRecord]
[Block(Container.DataEdits, 35, "ANIM_TEXTURES")]
public sealed class AnimTextureEdit
{
    public int Index;
    [LengthPrefix(typeof(int))]
    public ushort[] Textures;
}

[FormatRecord]
[Block(Container.DataEdits, 42, "ANIM_TEXTURE_ADDS")]
public sealed class AnimTextureAdd
{
    [LengthPrefix(typeof(int))]
    public ushort[] Textures;
}

[FormatRecord]
[Block(Container.DataEdits, 27, "ANIM_CMD_EDITS")]
public sealed class AnimCmdEdit
{
    public FormatObjRef TypeID;
    public int AnimIndex;
    public int RawCount;
    public int TotalCount;
}

[FormatRecord]
[Block(Container.DataEdits, 34, "ANIM_EDITS")]
public sealed class AnimEdit
{
    public FormatObjRef ModelID;
    public int AnimIndex;
    public FormatFixed32 Speed;
}

[FormatRecord]
[Block(Container.DataEdits, 31, "OBJ_TYPE_EDITS")]
public sealed class ObjTypeEdit
{
    public FormatObjRef BaseType;
    public FormatObjRef TargetType;
}

[FormatRecord]
[Block(Container.DataEdits, 36, "OBJ_LINK_EDITS")]
public sealed class ObjLinkEdit
{
    public FormatObjRef BaseType;
    public FormatObjRef SourceType;
}

[FormatRecord]
[Block(Container.DataEdits, 28, "SPRITE_EDITS")]
public sealed class SpriteEdit
{
    public FormatObjRef ID;
    public short Left;
    public short Top;
    public short Right;
    public short Bottom;
}

[FormatRecord]
[Block(Container.DataEdits, 25, "FRAME_EDITS")]
public sealed class FrameEdit
{
    public FormatObjRef ModelID;
    public int AnimIndex;
    [Doc("((y & 0x3F) << 10) | (z & 0x3FF)")]
    public short PackedYZ;
    [Doc("(x << 4) | ((y & 0xFC0) >> 6)")]
    public short PackedXY;
}

[FormatRecord]
public sealed class FrameReplacementAnim
{
    public int AnimID;
    [LengthPrefix(typeof(int))]
    public ushort[] Frames;
}

[FormatRecord]
[Block(Container.DataEdits, 32, "FRAME_REPLACE")]
public sealed class FrameReplacement
{
    public FormatObjRef ModelID;
    [LengthPrefix(typeof(int))]
    public FrameReplacementAnim[] Anims;
}

[FormatRecord]
public sealed class FaceEdit
{
    public FormatObjRef ModelID;
    [Doc("< 0 = palette entry")]
    public short MeshIndex;
    [Doc("FACE_TYPE")]
    public uint FaceType;
    public short FaceIndex;
    [LengthPrefix(typeof(uint))]
    public short[] TargetFaceIndices;
}

[FormatRecord]
public sealed class VertexEdit
{
    public short Index;
    public FormatVertex Change;
}

[FormatRecord]
public sealed class FaceEffectEdit
{
    [Doc("FACE_TYPE")]
    public uint FaceType;
    public short FaceIndex;
    public ushort Effects;
    public byte Reflective;
}

[FormatRecord]
[Block(Container.DataEdits, 19, "MESH_EDITS")]
public sealed class MeshEdit
{
    public FormatObjRef ModelID;
    public short MeshIndex;
    public FormatVertex Centre;
    public int CollRadius;
    [LengthPrefix(typeof(uint))]
    public FaceEdit[] FaceEdits;
    [LengthPrefix(typeof(uint))]
    public VertexEdit[] VertexEdits;
    [LengthPrefix(typeof(uint))]
    public FaceEffectEdit[] FaceEffects;
}

[FormatRecord]
[Block(Container.DataEdits, 23, "VIS_PORTAL_EDITS")]
public sealed class VisPortalEdit
{
    public short BaseRoom;
    public short LinkRoom;
    public ushort PortalIndex;
    [FixedLength(4)]
    public FormatVertex[] VertexChanges;
}

[FormatRecord]
[Block(Container.DataEdits, 24, "CAMERA_EDITS")]
public sealed class CameraEdit
{
    public short Index;
    public int X;
    public int Y;
    public int Z;
    public short Room;
    public ushort Flag;
}

[FormatRecord(Doc = "Derived allocation counts so the engine can size arrays in its pre-pass.")]
[Block(Container.DataEdits, 21, "ROOM_EDIT_META")]
public sealed class RoomEditMeta
{
    public short RoomIndex;
    public short NumVertices;
    public short NumQuads;
    public short NumTriangles;
    public short NumSprites;
    public short NumStatic3Ds;
    public short NumSectors;
}

// --- Room mesh edits ---

[Union(typeof(uint))]
[Block(Container.DataEdits, 22, "ROOM_EDITS")]
public abstract class RoomEdit
{
    public short RoomIndex;
    [Doc("FACE_TYPE")]
    public uint FaceType;
}

[Case(0)]
public sealed class RoomReface : RoomEdit
{
    public short TargetIndex;
    public short SourceRoom;
    [Doc("FACE_TYPE")]
    public uint SourceFaceType;
    public short SourceIndex;
}

[FormatRecord]
public sealed class VertexRemap
{
    public short Index;
    public ushort NewVertexIndex;
}

[Case(1)]
public sealed class RoomMoveFace : RoomEdit
{
    public short TargetIndex;
    [LengthPrefix(typeof(uint))]
    public VertexRemap[] Remaps;
}

[Case(2)]
public sealed class RoomMoveVertex : RoomEdit
{
    public ushort VertexIndex;
    public short DX;
    public short DY;
    public short DZ;
    public short ShadeChange;
}

[Case(3)]
public sealed class RoomRotateFace : RoomEdit
{
    public short TargetIndex;
    public byte Rotations;
}

[Case(4)]
public sealed class RoomAddFace : RoomEdit
{
    public short SourceRoom;
    public short SourceIndex;
    [ImpliedLength("4 for quads, 3 for triangles, by FaceType")]
    public ushort[] Vertices;
}

[Case(5)]
public sealed class RoomAddVertex : RoomEdit
{
    public short X;
    public short Y;
    public short Z;
    public short Lighting;
}

[Case(6)]
public sealed class RoomAddSprite : RoomEdit
{
    public int ID;
    public ushort Vertex;
    public ushort Frame;
}

[Case(7)]
public sealed class RoomAddStatic3D : RoomEdit
{
    public int X;
    public int Y;
    public int Z;
    public short Angle;
    public ushort Intensity;
    public ushort ID;
}

[Case(8)]
public sealed class RoomEditStatic3D : RoomEdit
{
    public int MeshIndex;
    public int X;
    public int Y;
    public int Z;
    public short Angle;
    public ushort Intensity;
}

[Case(9)]
public sealed class RoomSetVertexFlags : RoomEdit
{
    public ushort VertexIndex;
    public ushort Flags;
}

[Case(10)]
public sealed class RoomSetDoubleSided : RoomEdit
{
    public short TargetIndex;
    public byte DoubleSided;
}

// --- Floor data edits ---

[Union(typeof(uint))]
public abstract class FdFix
{
}

[Case(0)]
public sealed class FdTrigParam : FdFix
{
    public byte ActionType;
    public short OldParam;
    public short NewParam;
}

[Case(1)]
public sealed class FdMusicOneShot : FdFix
{
}

[Case(2)]
public sealed class FdInsert : FdFix
{
    [LengthPrefix(typeof(uint))]
    public ushort[] Data;
}

[Case(3)]
public sealed class FdRoomShift : FdFix
{
    public uint XShift;
    public uint ZShift;
    public int YShift;
}

[Case(4)]
public sealed class FdTrigItem : FdFix
{
    public FormatObjRef Type;
    public short Room;
    public int X;
    public int Y;
    public int Z;
    public short Angle;
    public short Intensity;
    public ushort Flags;
    public string Name;
}

[Case(5)]
public sealed class FdRoomProperties : FdFix
{
    public ushort Flags;
    public byte Reverb;
    [Doc("255 = unchanged")]
    public byte FlipGroup;
}

[Case(6)]
public sealed class FdTrigType : FdFix
{
    public byte NewType;
}

[Case(7)]
public sealed class FdSectorOverwrite : FdFix
{
    public ushort FDIndex;
    public ushort BoxIndex;
    [Doc("-1 = none")]
    public short RoomBelow;
    public short Floor;
    public short RoomAbove;
    public short Ceiling;
}

[Case(8)]
public sealed class FdGlideCamera : FdFix
{
    public byte Timer;
    public byte Glide;
    public FormatVertex Shift;
}

[Case(9)]
public sealed class FdZoneFix : FdFix
{
    [ImpliedLength("target game's ground-zone count")]
    public ushort[] FlipOffGround;
    public ushort FlipOffFly;
    [ImpliedLength("same count")]
    public ushort[] FlipOnGround;
    public ushort FlipOnFly;
}

[Case(10)]
public sealed class FdPortalOverwrite : FdFix
{
    [Doc("-1 = unchanged")]
    public short Wall;
    public short Sky;
    public short Pit;
}

[Case(11)]
public sealed class FdClimbInsert : FdFix
{
    [Doc("bit0 +Z, bit1 +X, bit2 -Z, bit3 -X")]
    public int Direction;
}

[Case(12)]
public sealed class FdTrigDelete : FdFix
{
}

[Case(13)]
public sealed class FdTriangulation : FdFix
{
    [Doc("bit0 floor present, bit1 ceiling present")]
    public int Type;
    [ImpliedLength("floor words then ceiling words, by Type bits")]
    public ushort[] Data;
}

[Case(14)]
public sealed class FdMineCart : FdFix
{
    [Doc("0 none, 1 left, 2 right, 3 stop")]
    public int Type;
}

[Case(15)]
public sealed class FdMaterial : FdFix
{
    public byte Material;
}

[Case(16)]
public sealed class FdSectorExtension : FdFix
{
    public ushort AdditionalXSectors;
    public ushort AdditionalZSectors;
}

[Case(17)]
public sealed class FdNamedTrigParam : FdFix
{
    public byte ActionType;
    public short OldParam;
    [Doc("in place of a raw param")]
    public FormatSymbolRef Symbol;
}

[FormatRecord]
[Block(Container.DataEdits, 17, "FLOOR_EDITS")]
public sealed class FloorDataEdit
{
    public short RoomIndex;
    public ushort X;
    public ushort Z;
    [LengthPrefix(typeof(uint))]
    public FdFix[] Fixes;
}

// --- Property edits ---

[Union(typeof(int))]
public abstract class PropertyValue
{
}

[Case(0)]
public sealed class PropInt : PropertyValue
{
    public int Value;
}

[Case(1)]
public sealed class PropFloat : PropertyValue
{
    public float Value;
}

[Case(2)]
public sealed class PropDouble : PropertyValue
{
    public double Value;
}

[Case(3)]
public sealed class PropBool : PropertyValue
{
    [Doc("1/0")]
    public int Value;
}

[Case(4)]
public sealed class PropXYZ : PropertyValue
{
    public int X;
    public int Y;
    public int Z;
}

[Case(5)]
public sealed class PropRGB : PropertyValue
{
    public FormatColour Value;
}

[FormatRecord]
public sealed class Property
{
    public string Name;
    public PropertyValue Value;
}

[Union(typeof(int))]
[Block(Container.DataEdits, 39, "PROPERTY_EDITS")]
public abstract class PropertyEdit
{
}

[Case(0)]
public sealed class ObjectPropertyEdit : PropertyEdit
{
    public FormatObjRef ObjectId;
    [LengthPrefix(typeof(int))]
    public Property[] Properties;
}

[Case(1)]
public sealed class ItemPropertyEdit : PropertyEdit
{
    public int ItemIndex;
    [LengthPrefix(typeof(int))]
    public Property[] Properties;
}
