using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Textures;

public class TR3CavernTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.TextureFix, "cavern_textures");
        FixPushButton(data, TR3LevelNames.WILLIE);
        FixPassport(_control3.Read($"Resources/TR3/{TR3LevelNames.WILLIE}"), data);
        return [data];
    }
}
