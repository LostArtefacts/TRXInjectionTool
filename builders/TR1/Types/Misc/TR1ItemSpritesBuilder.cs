using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Misc;

public class TR1ItemSpritesBuilder : InjectionBuilder
{
    public override string ID => "tr1_item_sprites";

    // Every level carries the inventory models of these items, but only a few
    // levels carry their pickup sprites.
    private static readonly Dictionary<TR1Type, string> _sources = new()
    {
        [TR1Type.PistolAmmo_S_P] = TR1LevelNames.ASSAULT,
        [TR1Type.ScionPiece1_S_P] = TR1LevelNames.QUALOPEC,
        [TR1Type.ScionPiece2_S_P] = TR1LevelNames.TIHOCAN,
    };

    public override List<InjectionData> Build()
    {
        var sprites = new Dictionary<TR1Type, TRSpriteSequence>();
        var regions = new List<TRTextileRegion>();
        foreach (var (type, levelName) in _sources)
        {
            var source = _control1.Read($"Resources/{levelName}");
            var sequence = source.Sprites[type];
            regions.AddRange(new TR1TexturePacker(source)
                .GetSpriteRegions(sequence)
                .Values.SelectMany(r => r));
            sprites[type] = sequence;
        }

        var level = _control1.Read($"Resources/{TR1LevelNames.ASSAULT}");
        ResetLevel(level, 1);

        var packer = new TR1TexturePacker(level);
        packer.AddRectangles(regions);
        packer.Pack(true);

        foreach (var (type, sequence) in sprites)
        {
            level.Sprites[type] = sequence;
        }

        return [InjectionData.Create(level, InjectionType.General, "item_sprites")];
    }
}
