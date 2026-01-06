using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Misc;

public class TR2MiscSpritesBuilder : TextureBuilder
{
    private static readonly Dictionary<TR3Type, TR2Type> _spriteRemap = new()
    {
        [TR3Type.Snowflake_S_H] = TR2Type.Snowflake_S_H,
        [TR3Type.ShadowSprite_S_H] = TR2Type.Shadow_S_H,
    };

    public override List<InjectionData> Build()
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.GW}");
        ResetLevel(level, 1);

        var spriteMap = ImportTR3MiscSprites(level, [.. _spriteRemap.Keys]);
        foreach (var (tr3Type, tr1Type) in _spriteRemap)
        {
            level.Sprites[tr1Type] = spriteMap[tr3Type];
        }

        return [InjectionData.Create(level, InjectionType.General, "misc_sprites")];
    }
}
