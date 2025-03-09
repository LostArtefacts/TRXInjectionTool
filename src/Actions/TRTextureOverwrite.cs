using TRLevelControl;

namespace TRXInjectionTool.Actions;

public class TRTextureOverwrite
{
    public ushort Page { get; set; }
    public byte X { get; set; }
    public byte Y { get; set; }
    public ushort Width { get; set; }
    public ushort Height { get; set; }
    public uint[] Data { get; set; }

    public void Serialize(TRLevelWriter writer)
    {
        writer.Write(Page);
        writer.Write(X);
        writer.Write(Y);
        writer.Write(Width);
        writer.Write(Height);
        writer.Write(Data);
    }
}
