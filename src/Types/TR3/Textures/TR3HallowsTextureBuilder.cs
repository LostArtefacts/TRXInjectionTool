using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Textures;

public class TR3HallowsTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = CreateBaseData();
        CreateDefaultTests(data, $"TR3/{TR3LevelNames.HALLOWS}");
        return [data];
    }

    private static InjectionData CreateBaseData()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.CITY}");
        CreateModelLevel(level, TR3Type.EyeOfIsis_M_H);
        level.SoundEffects.Clear();

        return InjectionData.Create(level, InjectionType.TextureFix, "stpaul_textures");
    }
}
