using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Misc;

public class TR1MiscSpritesBuilder : TextureBuilder
{
    public override string ID => "tr1_misc_sprites";

    private static readonly Dictionary<TR3Type, TR1Type> _spriteRemap = new()
    {
        [TR3Type.Snowflake_S_H] = TR1Type.Snowflake_S_H,
        [TR3Type.ShadowSprite_S_H] = TR1Type.Shadow_S_H,
    };

    public override List<InjectionData> Build()
    {
        var level = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        ResetLevel(level, 1);

        var spriteMap = ImportTR3MiscSprites(level, [.. _spriteRemap.Keys]);
        foreach (var (tr3Type, tr1Type) in _spriteRemap)
        {
            level.Sprites[tr1Type] = spriteMap[tr3Type];
        }
        var spriteTextureBase = level.Sprites.Values.Sum(s => s.Textures.Count);

        level.Sprites[(TR1Type)159] = BuildPinkBlood(
            new TRImage("Resources/TR1/PinkBlood/blood.png"),
            _control1.Read($"Resources/{TR1LevelNames.CAVES}")
                .Sprites[TR1Type.Blood1_S_H],
            new TR1TexturePacker(level), spriteTextureBase);

        return [InjectionData.Create(level, InjectionType.General, "misc_sprites")];
    }

    private static TRSpriteSequence BuildPinkBlood(
        TRImage strip, TRSpriteSequence sourceSequence, TR1TexturePacker packer,
        int spriteTextureBase)
    {
        var frameCount = sourceSequence.Textures.Count;
        var frameWidth = strip.Width / frameCount;
        var sequence = new TRSpriteSequence();
        var regions = new List<TRTextileRegion>();

        for (int i = 0; i < frameCount; i++)
        {
            var sourceTexture = sourceSequence.Textures[i];
            var texture = new TRSpriteTexture
            {
                Alignment = sourceTexture.Alignment,
                Position = sourceTexture.Position,
                Size = sourceTexture.Size,
            };
            sequence.Textures.Add(texture);
            regions.Add(new()
            {
                Bounds = new(0, 0, frameWidth, strip.Height),
                Image = strip.Export(new(i * frameWidth, 0, frameWidth, strip.Height)),
                Segments =
                [
                    new()
                    {
                        Index = spriteTextureBase + i,
                        Texture = texture,
                    },
                ],
            });
        }

        packer.AddRectangles(regions);
        packer.Pack(true);
        return sequence;
    }
}
