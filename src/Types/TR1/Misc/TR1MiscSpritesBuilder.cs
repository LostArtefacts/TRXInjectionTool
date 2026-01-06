using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Misc;

public class TR1MiscSpritesBuilder : TextureBuilder
{
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

        return [InjectionData.Create(level, InjectionType.General, "misc_sprites")];
    }   
}
