using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2FoolsTextureBuilder : TextureBuilder
{
    public override string ID => "fools_textures";

    public override List<InjectionData> Build()
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.FOOLGOLD}");
        var data = InjectionData.Create(TRGameVersion.TR2, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.FOOLGOLD);

        FixPassport(level, data);

        return new() { data };
    }
}
