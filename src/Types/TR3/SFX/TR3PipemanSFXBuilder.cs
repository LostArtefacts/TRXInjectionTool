using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.SFX;

public class TR3PipemanSFXBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.COASTAL}");
        var pipeman = level.Models[TR3Type.TribesmanDart];
        ResetLevel(level);
        level.Models[TR3Type.TribesmanDart] = pipeman;

        var deathAnim = pipeman.Animations[20];
        pipeman.Animations.ForEach(a =>
        {
            if (a != deathAnim)
            {
                a.Commands.Clear();
            }
        });

        deathAnim.Commands.Add(new TRSFXCommand
        {
            FrameNumber = 3,
            SoundID = (short)TR3SFX.BlowpipeNativeDeath,
        });

        var data = InjectionData.Create(level, InjectionType.General, "pipeman_sfx", true);
        data.Animations.Clear();
        data.AnimFrames.Clear();
        data.AnimChanges.Clear();
        data.AnimDispatches.Clear();
        data.Models.Clear();

        data.AnimCmdEdits.Add(new()
        {
            TypeID = (int)TR3Type.TribesmanDart,
            AnimIndex = 20,
            RawCount = data.AnimCommands.Count,
            TotalCount = deathAnim.Commands.Count,
        });

        return [data];
    }
}
