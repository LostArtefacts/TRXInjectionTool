using System.Drawing;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Misc;

public class TR2ItemSpritesBuilder : InjectionBuilder
{
    public override string ID => "tr2_item_sprites";

    // These levels carry the inventory models of the weapons, but no weapon
    // lies in them, so they carry none of the pickup sprites.
    private static readonly Dictionary<string, string> _targets = new()
    {
        [TR2LevelNames.FLOATER] = "floating_item_sprites",
        [TR2LevelNames.LAIR] = "lair_item_sprites",
    };

    private static readonly List<TR2Type> _types =
    [
        TR2Type.Pistols_S_P,
        TR2Type.Shotgun_S_P,
        TR2Type.Automags_S_P,
        TR2Type.Uzi_S_P,
        TR2Type.Harpoon_S_P,
        TR2Type.M16_S_P,
        TR2Type.GrenadeLauncher_S_P,
    ];

    public override List<InjectionData> Build()
    {
        return [.. _targets.Select(t =>
        {
            var data = InjectionData.Create(CreateLevel(), InjectionType.General, t.Value);
            CreateDefaultTests(data, t.Key);
            return data;
        })];
    }

    private static TR2Level CreateLevel()
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.GW}");
        var sprites = _types.ToDictionary(t => t, t => level.Sprites[t]);
        var regions = new TR2TexturePacker(level)
            .GetSpriteRegions(sprites.Values)
            .Values.SelectMany(r => r)
            .ToList();

        List<Color> basePalette = [.. level.Palette.Select(c => c.ToTR1Color())];
        ResetLevel(level, 1);

        var packer = new TR2TexturePacker(level);
        packer.AddRectangles(regions);
        packer.Pack(true);

        foreach (var (type, sequence) in sprites)
        {
            level.Sprites[type] = sequence;
        }

        GenerateImages8(level, basePalette);
        return level;
    }
}
