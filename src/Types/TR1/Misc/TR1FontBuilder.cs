using System.Drawing;
using TRImageControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;

namespace TRXInjectionTool.Types.TR1.Misc;

public class TR1FontBuilder : FontBuilder
{
    public override string ID => "tr1_font";

    public TR1FontBuilder()
        : base(TRGameVersion.TR1) { }

    protected override TRLevelBase CreateLevel(TRSpriteSequence font, bool useLegacyImages)
    {
        var level = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        ResetLevel(level);

        if (useLegacyImages)
        {
            var palette = GeneratePalette();
            level.Images8 = [.. _imageCache.Values.Select(i => new TRTexImage8
            {
                Pixels = i.ToRGB(palette),
            })];
            level.Palette = palette;
        }

        level.Sprites[TR1Type.FontGraphics_S_H] = font;
        return level;
    }

    public override string GetPublishedName()
        => "font.phd";

    private List<TRColour> GeneratePalette()
    {
        var palette = new List<Color>
        {
            Color.Transparent,
        };
        _imageCache.Values.ToList().ForEach(img =>
        {
            img.Read((c, x, y) =>
            {
                if (c.A != 0 && !palette.Contains(c))
                {
                    palette.Add(c);
                }
            });
        });

        while (palette.Count < 256)
        {
            palette.Add(Color.Black);
        }

        return palette.Select(c => c.ToTRColour()).ToList();
    }
}
