using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Misc;

public class TR3MiscSpritesBuilder : TextureBuilder, IPublisher
{
    public override string ID => "tr3_misc_sprites";

    private static readonly List<TR3Type> _spriteRemap =
    [
        TR3Type.Snowflake_S_H,
        TR3Type.ShadowSprite_S_H,
    ];

    public override List<InjectionData> Build()
    {
        var level = CreateLevel();
        return [InjectionData.Create(level, InjectionType.General, "misc_sprites")];
    }

    private static TR3Level CreateLevel()
    {
        var level = _control3.Read($"Resources/{TR3LevelNames.JUNGLE}");
        ResetLevel(level, 1);

        var spriteMap = ImportTR3MiscSprites(level, _spriteRemap);
        foreach (var type in _spriteRemap)
        {
            level.Sprites[type] = spriteMap[type];
        }

        return level;
    }

    public string GetPublishedName()
        => "misc_sprites.tr2";

    public TRLevelBase Publish()
        => CreateLevel();
}
