using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.SFX;

public class TR1MinesPushblockSFXBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        var level = _control1.Read($"Resources/{TR1LevelNames.MINES}");
        var models = new TRDictionary<TR1Type, TRModel>
        {
            [TR1Type.PushBlock2] = level.Models[TR1Type.PushBlock2],
            [TR1Type.PushBlock3] = level.Models[TR1Type.PushBlock3],
            [TR1Type.PushBlock4] = level.Models[TR1Type.PushBlock4],
        };

        ResetLevel(level);
        level.Models = models;
        foreach (var model in models.Values)
        {
            model.Animations[1].Commands.Clear();
            model.Animations[2].Commands.Add(new TRSFXCommand
            {
                 FrameNumber = 151,
                 SoundID = (short)TR1SFX.BlockSound,
            });
        }

        var data = InjectionData.Create(level, InjectionType.General, "mines_pushblocks", true);
        CreateDefaultTests(data, TR1LevelNames.MINES);
        data.Animations.Clear();
        data.AnimFrames.Clear();
        data.AnimChanges.Clear();
        data.AnimDispatches.Clear();
        data.Models.Clear();

        data.AnimCmdEdits.AddRange(models.Keys.Select(type => CreateAnimCmdEdit(level, type, 2)));

        return [data];
    }
}
