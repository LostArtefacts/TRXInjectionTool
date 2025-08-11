using System.Drawing;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Model;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types;

public abstract class FontBuilder : InjectionBuilder, IPublisher
{
    private static readonly string _resourceDirBase = "Resources/{0}/Font";

    protected readonly string _resourceDir;
    protected readonly List<GlyphDef> _glyphDefs;
    protected readonly Dictionary<string, TRImage> _imageCache;

    public FontBuilder(TRGameVersion gameVersion)
    {
        _resourceDir = string.Format(_resourceDirBase, gameVersion.ToString());
        _glyphDefs = DeserializeFile<List<GlyphDef>>(Path.Combine(_resourceDir, "glyph_info.json"));
        _glyphDefs.Sort((g1, g2) => g1.mesh_num.CompareTo(g2.mesh_num));
        _imageCache = new();
    }

    public TRImage GetImage(GlyphDef glyph)
    {
        string path = Path.Combine(_resourceDir, glyph.filename);
        if (!_imageCache.ContainsKey(path))
        {
            _imageCache[path] = new(path);
        }

        TRImage image = _imageCache[path];
        if (!_imageCache.ContainsKey(glyph.filename))
        {
            _imageCache[glyph.filename] = new(path);
        }

        Rectangle bounds = new(glyph.x, glyph.y, glyph.w, glyph.h);
        return image.Export(bounds);
    }

    public override List<InjectionData> Build()
    {
        var level = CreateLevel();
        var data = InjectionData.Create(level, InjectionType.General, ID);
        return new() { data };
    }

    private TRLevelBase CreateLevel()
    {
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

        return Pack(font, regions);
    }

    protected abstract TRLevelBase Pack(TRSpriteSequence font, List<TRTextileRegion> regions);

    public TRLevelBase Publish()
        => CreateLevel();

    public abstract string GetPublishedName();
}
