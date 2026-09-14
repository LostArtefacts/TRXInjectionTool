using TRLevelControl;

namespace TRXInjectionTool.Actions;

// A range of textures a face cycles through, brought in by the file rather
// than named among the level's own. The textures are the file's, and the
// engine moves them into the level's space as it reads them.
public class TRAnimTextureAdd
{
    public List<ushort> Textures { get; set; } = [];

    public void Serialize(TRLevelWriter writer)
    {
        writer.Write(Textures.Count);
        writer.Write(Textures);
    }
}
