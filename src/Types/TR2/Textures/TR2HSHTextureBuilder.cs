using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2HSHTextureBuilder : TextureBuilder
{
    public override string ID => "house_textures";

    public override List<InjectionData> Build()
    {
        InjectionData data = CreateBaseData();

        TR2Level house = _control2.Read($"Resources/{TR2LevelNames.HOME}");
        FixLaraTransparency(house, data);

        return new() { data };
    }

    private InjectionData CreateBaseData()
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.HOME}");
        var palette = level.Palette.ToList();
        ResetLevel(level, 1);
        level.Palette = palette;

        FixHomeWindows(level, TR2LevelNames.HOME);

        InjectionData data = InjectionData.Create(level, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.HOME);
        return data;
    }
}
