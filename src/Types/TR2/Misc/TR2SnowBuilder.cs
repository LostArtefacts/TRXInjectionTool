using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Misc;

public class TR2SnowBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.GW}");
        ResetLevel(level);
        return [GenerateSnowSprite(level, s => level.Sprites[TR2Type.Snowflake_S_H] = s)];
    }
}
