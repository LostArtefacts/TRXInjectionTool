using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2TitleTextureBuilder : TextureBuilder
{
    public override string ID => "title_textures";

    public override List<InjectionData> Build()
    {
        var level = _control2.Read($"Resources/title.tr2");
        var data = InjectionData.Create(TRGameVersion.TR2, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, "title.tr2");

        FixPassport(level, data);

        return new() { data };
    }
}
