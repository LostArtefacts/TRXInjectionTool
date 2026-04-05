using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Textures;

public class TR3Area51TextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.AREA51}");
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.TextureFix, "area51_textures");
        CreateDefaultTests(data, $"TR3/{TR3LevelNames.AREA51}");

        FixSlidingDoor(data);
        data.TextureOverwrites.Add(TR3HSCTextureBuilder.FixGrating(level, 2023));

        return [data];
    }

    private static void FixSlidingDoor(InjectionData data)
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.AREA51}");
        var door = level.Models[TR3Type.Door7];
        level.Models = new()
        {
            [TR3Type.Door7] = level.Models[TR3Type.Door7],
        };

        foreach (var frame in door.Animations.SelectMany(a => a.Frames))
        {
            frame.OffsetY += 30;
            frame.Bounds.MinY += 30;
            frame.Bounds.MaxY += 30;
        }
        data.FrameReplacements.AddRange(TRFrameReplacement.CreateFrom(level));
    }
}
