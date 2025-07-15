using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2GymTextureBuilder : TextureBuilder
{
    public override string ID => "gym_textures";

    public override List<InjectionData> Build()
    {
        InjectionData data = CreateBaseData();

        TR2Level gym = _control2.Read($"Resources/{TR2LevelNames.ASSAULT}");
        FixLaraTransparency(gym, data);

        return new() { data };
    }

    private InjectionData CreateBaseData()
    {
        TR2Level level = CreateWinstonLevel(TR2LevelNames.ASSAULT);
        // Current injection limitation, do not replace SFX
        level.SoundEffects.Clear();

        FixHomeWindows(level, TR2LevelNames.ASSAULT);

        InjectionData data = InjectionData.Create(level, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.ASSAULT);
        return data;
    }
}
