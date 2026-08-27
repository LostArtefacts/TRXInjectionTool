using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TRX.Shadow;

public class ShadowSpriteBuilder : InjectionBuilder
{
    public override string ID => "shadow_sprite";

    // TRX-only slot, mapped to O_SHADOW in the TR4 object catalog.
    private const int _tr4SpriteID = 534;

    public override List<InjectionData> Build()
    {
        var level = CreateLevel();
        var data = InjectionData.Create(level, InjectionType.General, ID);
        data.GameVersion = TRGameVersion.TR4;
        data.SpriteSequences[0].SpriteID = _tr4SpriteID;
        return [data];
    }

    private static TR3Level CreateLevel()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.ANTARC}");
        var shadow = level.Sprites[TR3Type.ShadowSprite_S_H];

        var sourcePacker = new TR3TexturePacker(level);
        var regions = sourcePacker.GetSpriteRegions(shadow)
            .Values.SelectMany(r => r).ToList();

        ResetLevel(level, 1);
        var packer = new TR3TexturePacker(level);
        packer.AddRectangles(regions);
        packer.Pack(true);
        level.Sprites[TR3Type.ShadowSprite_S_H] = shadow;

        return level;
    }
}
