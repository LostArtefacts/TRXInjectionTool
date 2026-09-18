using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Textures;

public class TR3TitleTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.TextureFix, "title_textures");

        var level = _control3.Read($"Resources/TR3/TITLE.TR2");
        FixPassport(level, data);

        return [data];
    }
}
