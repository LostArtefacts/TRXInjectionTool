namespace TRXInjectionTool.Format;

// Applicability tests. A file applies only when every test it carries
// passes; a reader that does not know a test's type or version skips it by
// size and treats it as FAILED - an unknown condition cannot be assumed to
// hold.

[FormatRecord]
[Applicability(0, "ITEM_META")]
public sealed class ItemMetaTest
{
    public int Index;
    public FormatObjRef Type;
    public int X;
    public int Y;
    public int Z;
    public short Room;
    public short Angle;
}

[FormatRecord]
[Applicability(1, "ROOM_COUNT")]
public sealed class RoomCountTest
{
    public int Count;
}

[FormatRecord]
[Applicability(2, "ROOM_META")]
public sealed class RoomMetaTest
{
    public int Index;
    public FormatRoomInfo Info;
    public ushort XSize;
    public ushort ZSize;
}

[FormatRecord]
[Applicability(3, "TEXTURE_SAMPLE")]
public sealed class TextureSampleTest
{
    public int TextureIndex;
    public ushort BlendingMode;
    public ushort Atlas;
    [FixedLength(4)]
    public FormatUV[] Vertices;
}

[FormatRecord]
[Applicability(4, "GAME_VERSION", Doc = "Every file carries one; the writer prepends it unconditionally.")]
public sealed class GameVersionTest
{
    [Doc("1 = TR1 ... 5 = TR5")]
    public int Game;
}
