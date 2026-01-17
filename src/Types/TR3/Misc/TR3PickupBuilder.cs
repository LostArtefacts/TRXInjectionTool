using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Misc;

public class TR3PickupBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        return
        [
            FixOraDagger(),
        ];
    }

    private static InjectionData FixOraDagger()
    {
        // Bounding box is inaccurate, meaning it becomes embedded in the floor.
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.PUNA}");
        level.Models = new()
        {
            [TR3Type.OraDagger_P] = level.Models[TR3Type.OraDagger_P],
            [TR3Type.OraDagger_M_H] = level.Models[TR3Type.OraDagger_M_H],
        };

        foreach (var frame in level.Models.Values.SelectMany(m => m.Animations.SelectMany(a => a.Frames)))
        {
            frame.Bounds.MaxY += 75;
        }

        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.General, "ora_dagger");
        data.FrameReplacements.AddRange(TRFrameReplacement.CreateFrom(level));
        return data;
    }
}
