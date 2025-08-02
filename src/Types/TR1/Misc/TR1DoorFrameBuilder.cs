using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Misc;

public class TR1DoorFrameBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        return new[] { TR1Type.Door2, TR1Type.Door3, TR1Type.Door4 }
            .Select(type =>
            {
                var data = InjectionData.Create(TRGameVersion.TR1, InjectionType.General, $"door{(int)type}_frames");
                // The first frame in OG has the door closed when it should be fully open.
                data.FrameEdits.Add(new()
                {
                    ModelID = (uint)type,
                    AnimIndex = 1,
                    Rotation = new() { Y = 768 },
                });
                return data;
            }).ToList();
    }
}
