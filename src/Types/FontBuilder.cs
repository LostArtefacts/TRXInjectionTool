using System.Drawing;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Model;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types;

public abstract class FontBuilder : InjectionBuilder
{
    private static readonly string _resourceDirBase = "Resources/{0}/Font";

    protected readonly string _resourceDir;
    protected readonly List<SpriteImage> _glyphSprites;
    protected readonly List<GlyphEntry> _glyphEntries;
    protected readonly Dictionary<string, TRImage> _imageCache;

    public FontBuilder(TRGameVersion gameVersion)
    {
        _resourceDir = string.Format(_resourceDirBase, gameVersion.ToString());
        _glyphSprites = DeserializeFile<List<SpriteImage>>(Path.Combine(_resourceDir, "glyph_sprites.json"));
        _glyphSprites.Sort((g1, g2) => g1.mesh_num.CompareTo(g2.mesh_num));
        _glyphEntries = DeserializeFile<List<GlyphEntry>>(Path.Combine(_resourceDir, "glyph_entries.json"));
        _imageCache = new();
    }

    public TRImage GetImage(SpriteImage glyph)
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
        foreach (GlyphEntry glyph in _glyphEntries) {
            data.Glyphs.Add(new()
            {
                Text = glyph.text,
                Role = glyph.role,
                MeshIdx = (short)glyph.mesh_num,
                Width = (short)glyph.width,
                CombineMeshIdx = glyph.combine.mesh_num,
                CombineOffsetX = glyph.combine.offset_x,
                CombineOffsetY = glyph.combine.offset_y,
            });
        }
        return new() { data };
    }

    private TRLevelBase CreateLevel()
    {
        TRSpriteSequence font = new();
        List<TRTextileRegion> regions = new();

        foreach (SpriteImage glyph in _glyphSprites)
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
}
