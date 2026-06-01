using System.Drawing;
using TRImageControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Misc;

public class TR3BatSpritesBuilder : InjectionBuilder, IPublisher
{
    private static readonly Dictionary<TR3Type, List<Rectangle>> _batMap = new()
    {
        [TR3Type.BatGfx] =
        [
            new(0, 0, 32, 32),
            new(32, 0, 32, 32),
        ],
    };

    public override List<InjectionData> Build()
    {
        var level = CreateLevel();
        var data = InjectionData.Create(level, InjectionType.General, "bat_sprites");
        return [data];
    }

    private static TR3Level CreateLevel()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.MADUBU}");
        var alignment = level.Sprites[TR3Type.MiscSprites_S_H].Textures[12].Alignment;
        ResetLevel(level, 1);

        var image = new TRImage("Resources/TR3/Misc/bat.png");
        var tile = new TRImage();
        var pos = new Point(0, 0);
        foreach (var (type, segments) in _batMap)
        {
            var sequence = new TRSpriteSequence();
            level.Sprites[type] = sequence;

            foreach (var segment in segments)
            {
                var bat = image.Export(segment);
                tile.Import(bat, pos);
                sequence.Textures.Add(new()
                {
                    Bounds = new(pos.X, pos.Y, bat.Width, bat.Height),
                    Alignment = alignment.Clone(),
                });
                pos.X += bat.Width;
            }
        }

        level.Images16[0].Pixels = tile.ToRGB555();
        GenerateImages8(level, [.. level.Palette.Select(c => c.ToTR1Color())]);
        return level;
    }

    public string GetPublishedName()
        => "bat.tr2";

    public TRLevelBase Publish()
        => CreateLevel();
}
