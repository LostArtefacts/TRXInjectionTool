using System.Drawing;
using TRImageControl;
using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Misc;

public class TR2DaggerSpriteBuilder : InjectionBuilder
{
    public override string ID => "dagger_sprite";

    public override List<InjectionData> Build()
    {
        TR2Level lair = _control2.Read($"Resources/{TR2LevelNames.LAIR}");
        List<Color> basePalette = new(lair.Palette.Select(c => c.ToTR1Color()));
        ResetLevel(lair, 1);

        TRImage sprite = new("Resources/TR2/dagger.png");
        TRSpriteTexture texture = new()
        {
            Alignment = new()
            {
                Left = (short)-(sprite.Width + 64),
                Top = (short)-(sprite.Height * 2),
                Right = (short)(sprite.Width - 64),
            },
            Position = new(),
            Size = sprite.Size,
        };

        TRImage tile = new(TRConsts.TPageWidth, TRConsts.TPageHeight);
        tile.Import(sprite, new(0, 0));
        lair.Images16[0].Pixels = tile.ToRGB555();
        GenerateImages8(lair, basePalette);

        TRSpriteSequence sequence = new();
        sequence.Textures.Add(texture);
        lair.Sprites[TR2Type.Puzzle2_S_P] = sequence;

        InjectionData data = InjectionData.Create(lair, InjectionType.General, ID);
        return new() { data };
    }
}
