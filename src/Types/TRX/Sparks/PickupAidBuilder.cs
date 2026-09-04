using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TRX.Sparks;

public class PickupAidBuilder : InjectionBuilder
{
    public override string ID => "pickup_aid";

    private static readonly Dictionary<TRGameVersion, int> _gameMap = new()
    {
        [TRGameVersion.TR1] = (int)TR1Type.Unused02,
        [TRGameVersion.TR2] = (int)TR2Type.PickupAid,
        [TRGameVersion.TR3] = 428,
        [TRGameVersion.TR4] = 535,
    };

    public override List<InjectionData> Build()
    {
        var level = CreateTwinkle();
        return [.. _gameMap.Select(kvp =>
        {
            var data = InjectionData.Create(level, InjectionType.General, ID);
            data.SpriteSequences[0].SpriteID = kvp.Value;
            data.GameVersion = kvp.Key;
            return data;
        })];
    }

    private static TR2Level CreateTwinkle()
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.FLOATER}");
        var twinkle = level.Sprites[TR2Type.XianGuardSparkles_S_H];

        var packer = new TR2TexturePacker(level);
        var regions = packer.GetSpriteRegions(twinkle)
            .Values.SelectMany(r => r);
        ResetLevel(level, 1);

        packer = new(level);
        packer.AddRectangles(regions);
        packer.Pack(true);

        foreach (var texture in twinkle.Textures)
        {
            texture.Alignment.Left -= 40;
            texture.Alignment.Right += 40;
            texture.Alignment.Top -= 40;
        }

        level.Sprites[TR2Type.XianGuardSparkles_S_H] = twinkle;
        return level;
    }
}
