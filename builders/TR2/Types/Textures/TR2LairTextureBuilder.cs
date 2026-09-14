using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2LairTextureBuilder : TextureBuilder
{
    public override string ID => "lair_textures";

    public override List<InjectionData> Build()
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.LAIR}");
        var data = InjectionData.Create(TRGameVersion.TR2, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.LAIR);

        FixPassport(level, data);

        return new() { data };
    }
}
