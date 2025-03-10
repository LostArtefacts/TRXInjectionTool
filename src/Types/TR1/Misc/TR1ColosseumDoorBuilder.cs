using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Misc;

public class TR1ColosseumDoorBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.General, "colosseum_door");
        CreateDefaultTests(data, TR1LevelNames.COLOSSEUM);

        // The first frame in OG has the door closed when it should be fully open.
        data.FrameEdits.Add(new()
        {
            ModelID = (uint)TR1Type.Door2,
            AnimIndex = 1,
            Rotation = new() { Y = 768 },
        });

        return new() { data };
    }
}
