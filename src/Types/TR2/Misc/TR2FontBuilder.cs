using System.Drawing;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types.TR2.Misc;

public class TR2FontBuilder : InjectionBuilder
{
    private readonly List<GlyphDef> _glyphDefs;
    private readonly Dictionary<string, TRImage> _imageCache;

    public override string ID => "font";

    public TR2FontBuilder()
    {
        _glyphDefs = DeserializeFile<List<GlyphDef>>("Resources/TR2/Font/glyph_info.json");
        _glyphDefs.Sort((g1, g2) => g1.mesh_num.CompareTo(g2.mesh_num));
        _imageCache = new();
    }

    public TRImage GetImage(GlyphDef glyph)
    {
        string path = Path.Combine("Resources/TR2/Font", glyph.filename);
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
        TR2Level wall = _control2.Read($"Resources/{TR2LevelNames.GW}");

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

        ResetLevel(wall, 1);
        TR2TexturePacker packer = new(wall);
        packer.AddRectangles(regions);
        packer.Pack(true);

        GenerateImages8(wall);

        wall.Sprites[TR2Type.FontGraphics_S_H] = font;

        _control2.Write(wall, MakeOutputPath(TRGameVersion.TR2, $"Debug/{ID}.tr2"));

        InjectionData data = InjectionData.Create(wall, InjectionType.General, ID);
        return new() { data };
    }
}
