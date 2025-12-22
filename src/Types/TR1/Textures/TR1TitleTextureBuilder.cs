using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1TitleTextureBuilder : TextureBuilder
{
    public override string ID => "tr1_title_textures";

    public override List<InjectionData> Build()
    {
        var level = _control1.Read($"Resources/TITLE.PHD");
        var data = InjectionData.Create(TRGameVersion.TR1, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, "TITLE.PHD");

        FixPassport(level, data);

        return new() { data };
    }
}
