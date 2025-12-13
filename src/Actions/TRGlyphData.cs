using TRLevelControl;
using TRLevelControl.Model;
using System.Text;

public class TRGlyphData
{
    public string Text { get; set; }
    public short Role { get; set; }
    public short MeshIdx { get; set; }
    public short Width { get; set; }
    public short CombineMeshIdx { get; set; }
    public short CombineOffsetX { get; set; }
    public short CombineOffsetY { get; set; }

    public void Serialize(TRLevelWriter writer, TRGameVersion version)
    {
        var bytes = Encoding.UTF8.GetBytes(Text.TrimEnd('\0'));
        writer.Write((byte)bytes.Length);
        writer.Write(bytes);
        writer.Write(Role);
        writer.Write(MeshIdx);
        writer.Write(Width);
        writer.Write(CombineMeshIdx);
        writer.Write(CombineOffsetX);
        writer.Write(CombineOffsetY);
    }
}
