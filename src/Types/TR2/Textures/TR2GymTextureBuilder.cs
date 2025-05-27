using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2GymTextureBuilder : TextureBuilder
{
    public override string ID => "gym_textures";

    public override List<InjectionData> Build()
    {
        TR2Level gym = _control2.Read($"Resources/{TR2LevelNames.ASSAULT}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.ASSAULT);

        FixLaraTransparency(gym, data);

        return new() { data };
    }
}
