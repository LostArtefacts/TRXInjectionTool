using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Objects;

public class TR2InvBGBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.GW}");
        ResetLevel(level);

        var dummy = new TRModel();
        dummy.Meshes.Add(new() { Normals = [] });
        dummy.Animations.Add(new()
        {
            FrameRate = 1,
            Accel = new(),
            Speed = new(),
        });
        dummy.Animations[0].Frames.Add(new()
        {
            Bounds = new(),
            Rotations = [new()],
        });
        level.Models[TR2Type.MenuBackground_H] = dummy;
        var data = InjectionData.Create(level, InjectionType.General, "inv_background");
        return [data];
    }
}
