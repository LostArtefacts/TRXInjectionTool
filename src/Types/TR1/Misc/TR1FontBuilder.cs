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

    public TR1FontBuilder()
    {
        _glyphDefs = DeserializeFile<List<GlyphDef>>("Resources/TR1/Font/glyph_info.json");
        _glyphDefs.Sort((g1, g2) => g1.mesh_num.CompareTo(g2.mesh_num));
        _imageCache = new();
    }

    public TRImage GetImage(GlyphDef glyph)
    {
        if (!_imageCache.ContainsKey(glyph.filename))
        {
            _imageCache[glyph.filename] = new(glyph.filename);
        }

        TRImage image = _imageCache[glyph.filename];
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
                    Top = (short)(-glyph.h),
                    Right = (short)(glyph.w),
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

        _control1.Write(caves, "Output/font.phd");

        InjectionData data = InjectionData.Create(caves, InjectionType.General, "font");
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
}
