using System.Drawing;
using TRImageControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Misc;

public class TR3FishSpritesBuilder : TextureBuilder, IPublisher
{
    private static readonly Dictionary<TR3Type, List<Rectangle>> _fishMap = new()
    {
        [TR3Type.PirahnaGfx] =
        [
            new(0, 0, 33, 33),
            new(33, 0, 33, 33),
        ],
        [TR3Type.TropicalFishGfx] =
        [
            new(66, 0, 30, 30),
            new(96, 0, 32, 32),
        ],
    };

    public override List<InjectionData> Build()
    {
        var level = CreateLevel();
        var data = InjectionData.Create(level, InjectionType.General, "fish_sprites");
        return [data];
    }

    private static TR3Level CreateLevel()
    {
        var level = _control3.Read($"Resources/{TR3LevelNames.JUNGLE}");
        var alignment = new Dictionary<TR3Type, TRSpriteAlignment>
        {
            [TR3Type.PirahnaGfx] = level.Sprites[TR3Type.MiscSprites_S_H].Textures[10].Alignment,
            [TR3Type.TropicalFishGfx] = level.Sprites[TR3Type.MiscSprites_S_H].Textures[11].Alignment,
        };

        ResetLevel(level, 1);

        var image = new TRImage("Resources/TR3/Misc/fish.png");
        var tile = new TRImage();
        var pos = new Point(1, 1);
        foreach (var (type, segments) in _fishMap)
        {
            var sequence = new TRSpriteSequence();
            level.Sprites[type] = sequence;

            foreach (var segment in segments)
            {
                var fish = image.Export(segment);
                tile.Import(fish, pos);
                sequence.Textures.Add(new()
                {
                    Bounds = new(pos.X, pos.Y, fish.Width, fish.Height),
                    Alignment = alignment[type].Clone(),
                });
                pos.X += fish.Width + 1;
            }
        }

        level.Images16[0].Pixels = tile.ToRGB555();
        GenerateImages8(level, [.. level.Palette.Select(c => c.ToTR1Color())]);
        return level;
    }

    public string GetPublishedName()
        => "fish.tr2";

    public TRLevelBase Publish()
        => CreateLevel();
}
