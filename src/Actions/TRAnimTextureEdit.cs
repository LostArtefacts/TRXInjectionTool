using TRLevelControl;

namespace TRXInjectionTool.Actions;

public class TRAnimTextureEdit
{
    public int Index { get; set; }
    public List<ushort> Textures { get; set; }

    public void Serialize(TRLevelWriter writer)
    {
        writer.Write(Index);
        writer.Write(Textures.Count);
        writer.Write(Textures);
    }
}
