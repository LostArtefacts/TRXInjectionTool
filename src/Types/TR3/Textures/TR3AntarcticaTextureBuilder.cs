using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Textures;

public class TR3AntarcticaTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = CreateBaseData();
        FixPushButton(data, TR3LevelNames.ANTARC);
        return [data];
    }

    private static InjectionData CreateBaseData()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.ANTARC}");
        var button = level.Models[TR3Type.PushButtonSwitch];
        button.Meshes[0].TexturedRectangles.Add(new()
        {
            Type = TRFaceType.Rectangle,
            Texture = button.Meshes[1].TexturedRectangles.First().Texture,
            Vertices = [3, 7, 4, 0],
        });
        
        CreateModelLevel(level, TR3Type.PushButtonSwitch);
        level.Models[TR3Type.PushButtonSwitch].Animations.Clear();

        var data = InjectionData.Create(level, InjectionType.TextureFix, "antarc_textures");
        CreateDefaultTests(data, $"TR3/{TR3LevelNames.ANTARC}");
        data.SetMeshOnlyModel((uint)TR3Type.PushButtonSwitch);
        return data;
    }
}
