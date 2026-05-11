using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Textures;

public class TR3GymTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = CreateBaseData();
        FixPushButton(data, TR3LevelNames.ASSAULT);
        return [data];
    }

    private static InjectionData CreateBaseData()
    {
        var level = CreateTR3WinstonLevel(TR3LevelNames.ASSAULT);
        level.SoundEffects.Clear();

        var data = InjectionData.Create(level, InjectionType.TextureFix, "gym_textures");
        CreateDefaultTests(data, $"TR3/{TR3LevelNames.ASSAULT}");

        data.SetMeshOnlyModel((uint)TR3Type.Winston);

        return data;
    }
}
