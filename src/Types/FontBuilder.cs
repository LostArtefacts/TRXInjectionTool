using TRImageControl;
using TRLevelControl.Model;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types;

public abstract class FontBuilder : InjectionBuilder, IPublisher
{
    private static readonly string _resourceDirBase = "Resources/{0}/Font";

    protected readonly string _resourceDir;
    protected readonly Dictionary<int, List<SpriteInfo>> _glyphDefsByFont;
    protected readonly Dictionary<int, TRSpriteSequence> _fonts;
    protected readonly Dictionary<string, TRImage> _imageCache;
    protected readonly List<string> _atlasOrder;

    public FontBuilder(TRGameVersion gameVersion)
    {
        _resourceDir = string.Format(_resourceDirBase, gameVersion.ToString());
        _imageCache = [];
        _atlasOrder = [];
        _glyphDefsByFont = LoadGlyphDefinitions();
        _fonts = BuildFonts();
    }

    public ushort GetAtlas(SpriteInfo glyph)
    {
        var path = Path.Combine(_resourceDir, glyph.filename);
        if (!_imageCache.ContainsKey(glyph.filename))
        {
            _imageCache[glyph.filename] = new(path);
            _atlasOrder.Add(glyph.filename);
        }

        return (ushort)_atlasOrder.IndexOf(glyph.filename);
    }

    public override List<InjectionData> Build()
    {
        var level = CreateLevel(false);
        var data = InjectionData.Create(level, InjectionType.General, ID);
        data.Images.AddRange(_atlasOrder.Select(name => new TRTexImage32 { Pixels = _imageCache[name].ToRGBA() }));
        return [ data ];
    }

    private TRLevelBase CreateLevel(bool useLegacyImages)
    {
        return CreateLevel(_fonts, useLegacyImages);
    }

    protected abstract TRLevelBase CreateLevel(IReadOnlyDictionary<int, TRSpriteSequence> fonts, bool useLegacyImages);

    public TRLevelBase Publish()
        => CreateLevel(true);

    public abstract string GetPublishedName();

    private Dictionary<int, List<SpriteInfo>> LoadGlyphDefinitions()
    {
        Dictionary<int, List<SpriteInfo>> glyphDefs = [];
        string baseFontFile = Path.Combine(_resourceDir, "glyph_info.json");
        LoadGlyphFile(baseFontFile, 0, glyphDefs);
        if (!glyphDefs.ContainsKey(0))
        {
            throw new FileNotFoundException(baseFontFile);
        }

        if (Directory.Exists(_resourceDir))
        {
            foreach (string glyphFile in Directory.GetFiles(_resourceDir, "glyph_info_font*.json"))
            {
                int? fontIndex = ParseFontIndex(glyphFile);
                if (fontIndex.HasValue)
                {
                    LoadGlyphFile(glyphFile, fontIndex.Value, glyphDefs);
                }
            }
        }

        return glyphDefs;
    }

    private void LoadGlyphFile(string path, int fontIndex, IDictionary<int, List<SpriteInfo>> glyphDefs)
    {
        if (!File.Exists(path))
        {
            return;
        }

        var glyphs = DeserializeFile<List<SpriteInfo>>(path) ?? [];
        glyphs.Sort((g1, g2) => g1.mesh_num.CompareTo(g2.mesh_num));
        glyphDefs[fontIndex] = glyphs;
    }

    private static int? ParseFontIndex(string path)
    {
        string fileName = Path.GetFileNameWithoutExtension(path);
        if (fileName == "glyph_info")
        {
            return 0;
        }

        string suffix = fileName.Replace("glyph_info_font", string.Empty);
        if (int.TryParse(suffix, out int fontIndex))
        {
            return fontIndex;
        }

        return null;
    }

    private Dictionary<int, TRSpriteSequence> BuildFonts()
    {
        Dictionary<int, TRSpriteSequence> fonts = [];

        foreach (var fontEntry in _glyphDefsByFont.OrderBy(f => f.Key))
        {
            var glyphs = fontEntry.Value;
            if (glyphs.Count == 0)
                continue;

            // Determine the full mesh number range
            int minMesh = 0;
            int maxMesh = glyphs.Max(g => g.mesh_num);

            // Create a lookup for quick access
            var glyphLookup = glyphs.ToDictionary(g => g.mesh_num);

            List<TRSpriteTexture> textures = [];

            for (int i = minMesh; i <= maxMesh; i++)
            {
                if (glyphLookup.TryGetValue(i, out var glyph))
                {
                    textures.Add(new TRSpriteTexture
                    {
                        Atlas = GetAtlas(glyph),
                        Bounds = new(glyph.x, glyph.y, glyph.w, glyph.h),
                        Alignment = new()
                        {
                            Left = glyph.l,
                            Top = glyph.t,
                            Right = glyph.r,
                            Bottom = glyph.b
                        }
                    });
                }
                else
                {
                    // Dummy glyph
                    textures.Add(new TRSpriteTexture
                    {
                        Atlas = 0,
                        Bounds = new(0, 0, 1, 1),
                        Alignment = new(),
                    });
                }
            }

            fonts[fontEntry.Key] = new TRSpriteSequence { Textures = [.. textures] };
        }

        return fonts;
    }
}
