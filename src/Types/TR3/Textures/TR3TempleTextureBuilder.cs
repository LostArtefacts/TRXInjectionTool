using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Textures;

public class TR3TempleTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.TextureFix, "temple_textures");

        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.RUINS}");
        FixPassport(level, data);

        return [data];
    }
}
