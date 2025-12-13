using System.Diagnostics.CodeAnalysis;

namespace TRXInjectionTool.Util;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public class SpriteInfo
{
    public int mesh_num { get; set; }
    public string filename { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public int w { get; set; }
    public int h { get; set; }
    public short l { get; set; }
    public short t { get; set; }
    public short r { get; set; }
    public short b { get; set; }
}
