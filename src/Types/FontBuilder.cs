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
    protected readonly List<SpriteInfo> _glyphDefs;
    protected readonly Dictionary<string, TRImage> _imageCache;

    public FontBuilder(TRGameVersion gameVersion)
    {
        _resourceDir = string.Format(_resourceDirBase, gameVersion.ToString());
        _glyphDefs = DeserializeFile<List<SpriteInfo>>(Path.Combine(_resourceDir, "glyph_info.json"));
        _glyphDefs.Sort((g1, g2) => g1.mesh_num.CompareTo(g2.mesh_num));
        _imageCache = [];
    }

    public ushort GetAtlas(SpriteInfo glyph)
    {
        var path = Path.Combine(_resourceDir, glyph.filename);
        if (!_imageCache.ContainsKey(glyph.filename))
        {
            _imageCache[glyph.filename] = new(path);
        }

        return (ushort)_imageCache.Keys.ToList().IndexOf(glyph.filename);
    }

    public override List<InjectionData> Build()
    {
        var level = CreateLevel(false);
        var data = InjectionData.Create(level, InjectionType.General, ID);
        data.Images.AddRange(_imageCache.Values.Select(i => new TRTexImage32 { Pixels = i.ToRGBA() }));
        return [ data ];
    }

    private TRLevelBase CreateLevel(bool useLegacyImages)
    {
        var font = new TRSpriteSequence
        {
            Textures = [.. _glyphDefs.Select(glyph => new TRSpriteTexture
            {
                Atlas = GetAtlas(glyph),
                Bounds = new(glyph.x, glyph.y, glyph.w, glyph.h),
                Alignment = new()
                {
                    Left = glyph.l,
                    Top = glyph.t,
                    Right = glyph.r,
                    Bottom = glyph.b,
                },
            })],
        };

        return CreateLevel(font, useLegacyImages);
    }

    protected abstract TRLevelBase CreateLevel(TRSpriteSequence font, bool useLegacyImages);

    public TRLevelBase Publish()
        => CreateLevel(true);

    public abstract string GetPublishedName();
}
