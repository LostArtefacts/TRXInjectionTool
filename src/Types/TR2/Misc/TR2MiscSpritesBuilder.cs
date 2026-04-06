using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Misc;

public class TR2MiscSpritesBuilder : TextureBuilder
{
    public override string ID => "tr2_misc_sprites";

    private static readonly Dictionary<TR3Type, TR2Type> _spriteRemap = new()
    {
        [TR3Type.Snowflake_S_H] = TR2Type.Snowflake_S_H,
        [TR3Type.ShadowSprite_S_H] = TR2Type.Shadow_S_H,
    };

    public override List<InjectionData> Build()
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.GW}");
        var sourceSequence = _control2.Read($"Resources/{TR2LevelNames.ASSAULT}")
            .Sprites[TR2Type.Blood_S_H];
        ResetLevel(level, 1);

        var spriteMap = ImportTR3MiscSprites(level, [.. _spriteRemap.Keys]);
        foreach (var (tr3Type, tr1Type) in _spriteRemap)
        {
            level.Sprites[tr1Type] = spriteMap[tr3Type];
        }
        var spriteTextureBase = level.Sprites.Values.Sum(s => s.Textures.Count);

        level.Sprites[(TR2Type)337] = BuildPinkBlood(
            new TRImage("Resources/TR2/PinkBlood/blood.png"), sourceSequence,
            new TR2TexturePacker(level), spriteTextureBase);

        return [InjectionData.Create(level, InjectionType.General, "misc_sprites")];
    }

    private static TRSpriteSequence BuildPinkBlood(
        TRImage strip, TRSpriteSequence sourceSequence, TR2TexturePacker packer,
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
