using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR4.Misc;

public class TR4FontBuilder : FontBuilder
{
    public override string ID => "tr4_font";

    public TR4FontBuilder()
        : base(TRGameVersion.TR4) { }

    protected override TRLevelBase CreateLevel(
        IReadOnlyDictionary<int, TRSpriteSequence> fonts, bool useLegacyImages)
    {
        var level = _control4.Read($"Resources/TR4/{TR4LevelNames.ANGKOR}");
        ResetLevel(level);

        if (useLegacyImages)
        {
            level.Images.Objects.Images32 = [.. _atlasOrder.Select(name => new TRTexImage32
            {
                Pixels = _imageCache[name].ToRGBA(),
            })];
            level.Images.Objects.Images16 = [.. _atlasOrder.Select(name => new TRTexImage16
            {
                Pixels = _imageCache[name].ToRGB555(),
            })];
        }

        level.Sprites[(TR4Type)500] = fonts[0];
        level.Sprites[(TR4Type)501] = fonts[1];

        return level;
    }

    public override string GetPublishedName()
        => "font.tr4";
}
