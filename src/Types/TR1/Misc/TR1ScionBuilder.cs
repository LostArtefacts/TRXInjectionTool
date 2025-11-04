using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Misc;

public class TR1ScionBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.General, "scion_collision");
        data.MeshEdits.Add(new()
        {
            ModelID = (uint)TR1Type.ScionPiece3_S_P,
            MeshIndex = 0,
            CollRadius = TRConsts.Step1 - 63,
        });

        var level = _control1.Read($"Resources/{TR1LevelNames.PYRAMID}");
        level.Models = new()
        {
            [TR1Type.ScionPiece3_S_P] = level.Models[TR1Type.ScionPiece3_S_P],
        };
        foreach (var frame in level.Models[TR1Type.ScionPiece3_S_P].Animations.SelectMany(a => a.Frames))
        {
            frame.Bounds.MinY -= 100;
            frame.Bounds.MaxY += 100;
            frame.Bounds.MinX -= 30;
            frame.Bounds.MaxX += 30;
            frame.Bounds.MinZ -= 30;
            frame.Bounds.MaxX += 30;
        }

        data.FrameReplacements.AddRange(TRFrameReplacement.CreateFrom(level));

        return new() { data };
    }
}
