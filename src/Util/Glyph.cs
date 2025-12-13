using System.Diagnostics.CodeAnalysis;

namespace TRXInjectionTool.Util;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public class SpriteImage
{
    public string filename { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public int w { get; set; }
    public int h { get; set; }
    public short l { get; set; }
    public short t { get; set; }
    public short r { get; set; }
    public short b { get; set; }
    public int mesh_num { get; set; }
}

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public class GlyphEntry
{
    public string text { get; set; }
    public short width { get; set; }
    public short role { get; set; }
    public int mesh_num { get; set; }
    public Combine combine { get; set; } = new();

    public class Combine {
        public short mesh_num { get; set; }
        public short offset_x { get; set; }
        public short offset_y { get; set; }
    };
}
