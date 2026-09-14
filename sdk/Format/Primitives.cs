namespace TRXInjectionTool.Format;

// Shared format primitives, referenced by many records.

[FormatRecord]
public struct FormatVertex
{
    public short X, Y, Z;
}

[FormatRecord(Doc = "Canonical (TR1/TR2-style) UV encoding, byte order as in classic level files; the TR3 on-disk quirk is normalised by the writer. Each coordinate reads as a u16 little-endian fixed-point value.")]
public struct FormatUV
{
    public byte UCoord, UPixel, VCoord, VPixel;
}

[FormatRecord(Doc = "Full 8-bit channels; converters shift down for games with VGA palettes.")]
public struct FormatColour
{
    public byte Red, Green, Blue;
}

[FormatRecord(Doc = "Canonical room info; never a game's on-disk room header.")]
public struct FormatRoomInfo
{
    public int X, Z, YBottom, YTop;
}

[FormatRecord]
public struct FormatFixed32
{
    public short Whole;
    public ushort Fraction;
}

public enum ObjRefKind
{
    Game = 0,
    Static2D = 1,
    Static3D = 2,
    Symbol = 3,
}

[FormatRecord(Doc = "Ids are absolute; games rebase them in their converters. For SYMBOL, id is an index into this file's symbol table.")]
public struct FormatObjRef
{
    [Doc("ObjRefKind")]
    public int Kind;
    public int Id;
}

[FormatRecord(Doc = "Index into this file's symbol table.")]
public struct FormatSymbolRef
{
    public short Index;
}
