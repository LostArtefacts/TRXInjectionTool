using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Misc;

public class TR1SnowBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var level = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        ResetLevel(level);
        return [GenerateSnowSprite(level, s => level.Sprites[TR1Type.Snowflake_S_H] = s)];
    }
}
