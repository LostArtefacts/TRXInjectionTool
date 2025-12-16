using TRImageControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;

namespace TRXInjectionTool.Types.TR3.Misc;

public class TR3FontBuilder : FontBuilder
{
    public override string ID => "tr3_font";

    public TR3FontBuilder()
        : base(TRGameVersion.TR3) { }

    protected override TRLevelBase CreateLevel(TRSpriteSequence font, bool useLegacyImages)
    {
        var level = _control3.Read($"Resources/{TR3LevelNames.JUNGLE}");
        var palette = level.Palette.Select(c => c.ToTR1Color()).ToList();
        ResetLevel(level);

        if (useLegacyImages)
        {
            level.Images16 = [.. _imageCache.Values.Select(i => new TRTexImage16
            {
                Pixels = i.ToRGB555(),
            })];
            GenerateImages8(level, palette);
        }

        level.Sprites[TR3Type.FontGraphics_S_H] = font;
        return level;
    }

    public override string GetPublishedName()
        => "font.tr2";
}
