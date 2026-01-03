using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Misc;

public class TR3SnowBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var level = _control3.Read($"Resources/{TR3LevelNames.JUNGLE}");
        ResetLevel(level);
        return [GenerateSnow(level, s => level.Sprites[TR3Type.Snowflake_S_H] = s)];
    }
}
