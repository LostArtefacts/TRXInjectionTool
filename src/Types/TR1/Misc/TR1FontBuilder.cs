using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Misc;

public class TR1FontBuilder : InjectionBuilder
{
    private readonly List<GlyphDef> _glyphDefs;
    private readonly Dictionary<string, TRImage> _imageCache;

    public override string ID => "font";

    public TR1FontBuilder()
    {
        _glyphDefs = DeserializeFile<List<GlyphDef>>("Resources/TR1/Font/glyph_info.json");
        _glyphDefs.Sort((g1, g2) => g1.mesh_num.CompareTo(g2.mesh_num));
        _imageCache = new();
    }

    public TRImage GetImage(GlyphDef glyph)
    {
        string path = Path.Combine("Resources/TR1/Font", glyph.filename);
        if (!_imageCache.ContainsKey(path))
        {
            _imageCache[path] = new(path);
        }

        TRImage image = _imageCache[path];
        if (!_imageCache.ContainsKey(glyph.filename))
        {
            _imageCache[glyph.filename] = new(glyph.filename);
        }

        Rectangle bounds = new(glyph.x, glyph.y, glyph.w, glyph.h);
        return image.Export(bounds);
    }

    public override List<InjectionData> Build()
    {
        TR1Level caves = _control1.Read($"Resources/{TR1LevelNames.CAVES}");

        TRSpriteSequence font = new();
        List<TRTextileRegion> regions = new();

        foreach (GlyphDef glyph in _glyphDefs)
        {
            TRSpriteTexture texture = new()
            {
                Bounds = new(glyph.x, glyph.y, glyph.w, glyph.h),
                Alignment = new()
                {
                    Left = glyph.l,
                    Top = glyph.t,
                    Right = glyph.r,
                    Bottom = glyph.b,
                },
            };

            font.Textures.Add(texture);

            regions.Add(new()
            {
                Bounds = texture.Bounds,
                Image = GetImage(glyph),
                Segments = new()
                {
                    new()
                    {
                        Index = glyph.mesh_num,
                        Texture = texture,
                    },
                },
            });
        }

        ResetLevel(caves, 1);
        TR1TexturePacker packer = new(caves);
        packer.AddRectangles(regions);
        packer.Pack(true);

        caves.Sprites[TR1Type.FontGraphics_S_H] = font;

        _control1.Write(caves, MakeOutputPath(TRGameVersion.TR1, $"Debug/{ID}.phd"));

        InjectionData data = InjectionData.Create(caves, InjectionType.General, ID);

        return new() { data };
    }
}

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public class GlyphDef
{
    public int mesh_num { get; set; }
    public string filename { get; set; }
    public int x {  get; set; }
    public int y { get; set; }
    public int w { get; set; }
    public int h { get; set; }
    public short l { get; set; }
    public short t { get; set; }
    public short r { get; set; }
    public short b { get; set; }
}
