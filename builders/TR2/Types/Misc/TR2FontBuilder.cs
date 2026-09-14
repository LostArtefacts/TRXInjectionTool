using TRImageControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;

namespace TRXInjectionTool.Types.TR2.Misc;

public class TR2FontBuilder : FontBuilder
{
    public override string ID => "tr2_font";

    public TR2FontBuilder()
        : base(TRGameVersion.TR2) { }

    protected override TRLevelBase CreateLevel(IReadOnlyDictionary<int, TRSpriteSequence> fonts, bool useLegacyImages)
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.GW}");
        var palette = level.Palette.Select(c => c.ToTR1Color()).ToList();
        ResetLevel(level);

        if (useLegacyImages)
        {
            level.Images16 = [.. _atlasOrder.Select(name => new TRTexImage16
            {
                Pixels = _imageCache[name].ToRGB555(),
            })];
            GenerateImages8(level, palette);
        }

        level.Sprites[TR2Type.FontGraphics_S_H] = fonts[0];
        level.Sprites[TR2Type.FontGraphicsSmall_S_H] = fonts[1];

        return level;
    }

    public override string GetPublishedName()
        => "font.tr2";
}
