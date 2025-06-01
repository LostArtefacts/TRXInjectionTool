using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;

namespace TRXInjectionTool.Types.TR1.Misc;

public class TR1FontBuilder : FontBuilder
{
    public TR1FontBuilder()
        : base(TRGameVersion.TR1) { }

    protected override TRLevelBase Pack(TRSpriteSequence font, List<TRTextileRegion> regions)
    {
        TR1Level caves = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        ResetLevel(caves, 1);
        TR1TexturePacker packer = new(caves);
        packer.AddRectangles(regions);
        packer.Pack(true);

        caves.Sprites[TR1Type.FontGraphics_S_H] = font;

        _control1.Write(caves, MakeOutputPath(TRGameVersion.TR1, $"Debug/{ID}.phd"));
        return caves;
    }
}
