using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types.TR3.Textures;

public class TR3TinnosTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = CreateBaseData();
        return [data];
    }

    private static InjectionData CreateBaseData()
    {
        var tinnos = _control3.Read($"Resources/TR3/{TR3LevelNames.TINNOS}");
        var thames = _control3.Read($"Resources/TR3/{TR3LevelNames.THAMES}");
        var button = tinnos.Models[TR3Type.PushButtonSwitch];
        ResetLevel(tinnos);

        tinnos.Models[TR3Type.PushButtonSwitch] = button;
        button.Animations = thames.Models[TR3Type.PushButtonSwitch].Animations;
        foreach (var frame in button.Animations.SelectMany(a => a.Frames))
        {
            frame.Bounds = TRAnimBoundsCalculator.ComputeFrameBounds(button, frame);
        }

        button.Meshes.Clear();
        button.MeshTrees.Clear();

        var data = InjectionData.Create(tinnos, InjectionType.TextureFix, "tinnos_textures");
        CreateDefaultTests(data, $"TR3/{TR3LevelNames.TINNOS}");
        return data;
    }
}
