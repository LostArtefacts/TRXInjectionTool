using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Textures;

public class TR3GangesTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.TextureFix, "ganges_textures");
        FixPushButton(data, TR3LevelNames.GANGES);
        FixPassport(_control3.Read($"Resources/TR3/{TR3LevelNames.GANGES}"), data);
        return [data];
    }
}
