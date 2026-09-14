using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Textures;

public class TR3ReunionTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.TextureFix, "reunion_textures");

        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.REUNION}");
        FixPassport(level, data);

        return [data];
    }
}
