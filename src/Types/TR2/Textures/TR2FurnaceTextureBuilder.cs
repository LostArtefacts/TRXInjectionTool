using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2FurnaceTextureBuilder : TextureBuilder
{
    public override string ID => "furnace_textures";

    public override List<InjectionData> Build()
    {
        TR2Level furnace = _control2.Read($"Resources/{TR2LevelNames.FURNACE}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.FURNACE);

        FixWolfTransparency(furnace, data);

        return new() { data };
    }
}
