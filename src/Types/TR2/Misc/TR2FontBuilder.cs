using System.Drawing;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;

namespace TRXInjectionTool.Types.TR2.Misc;

public class TR2FontBuilder : FontBuilder
{
    public override string ID => "tr2-font";

    public TR2FontBuilder()
        : base(TRGameVersion.TR2) { }

    protected override TRLevelBase Pack(TRSpriteSequence font, List<TRTextileRegion> regions)
    {
        TR2Level wall = _control2.Read($"Resources/{TR2LevelNames.GW}");
        List<Color> basePalette = new(wall.Palette.Select(c => c.ToTR1Color()));
        ResetLevel(wall, 1);
        TR2TexturePacker packer = new(wall);
        packer.AddRectangles(regions);
        packer.Pack(true);

        GenerateImages8(wall, basePalette);

        wall.Sprites[TR2Type.FontGraphics_S_H] = font;
        return wall;
    }

    public override string GetPublishedName()
        => "font.tr2";
}
