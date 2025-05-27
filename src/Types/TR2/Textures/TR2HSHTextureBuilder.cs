using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2HSHTextureBuilder : TextureBuilder
{
    public override string ID => "house_textures";

    public override List<InjectionData> Build()
    {
        TR2Level house = _control2.Read($"Resources/{TR2LevelNames.HOME}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.HOME);

        FixLaraTransparency(house, data);

        return new() { data };
    }
}
