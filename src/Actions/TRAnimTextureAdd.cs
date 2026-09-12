using TRLevelControl;

namespace TRXInjectionTool.Actions;

public class TRAnimTextureAdd
{
    public List<ushort> Textures { get; set; } = [];

    public void Serialize(TRLevelWriter writer)
    {
        writer.Write(Textures.Count);
        writer.Write(Textures);
    }
}
