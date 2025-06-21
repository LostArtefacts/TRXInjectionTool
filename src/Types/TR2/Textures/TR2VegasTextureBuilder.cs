using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2VegasTextureBuilder : TextureBuilder
{
    public override string ID => "vegas_textures";

    public override List<InjectionData> Build()
    {
        InjectionData data = CreateBaseData();
        return new() { data };
    }

    private InjectionData CreateBaseData()
    {
        TR2Level level = CreateWinstonLevel(TR2LevelNames.VEGAS);
        // Current injection limitation, do not replace SFX
        level.SoundEffects.Clear();

        InjectionData data = InjectionData.Create(level, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.VEGAS);
        return data;
    }
}
