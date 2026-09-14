using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.SFX;

public class TR2ScubaSFXBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.RIG}");
        var model = level.Models[TR2Type.ScubaDiver];
        ResetLevel(level);
        level.Models[TR2Type.ScubaDiver] = model;

        var deathAnim = model.Animations[16];
        model.Animations.ForEach(a =>
        {
            if (a != deathAnim)
            {
                a.Commands.Clear();
            }
        });

        var cmd = deathAnim.Commands.Find(c => c is TRSFXCommand);
        if (cmd is TRSFXCommand sfxCmd)
        {
            sfxCmd.FrameNumber = 1;
        }
        else
        {
            throw new ArgumentException("No matching SFX command");
        }

        var data = InjectionData.Create(level, InjectionType.General, "scuba_sfx", true);
        data.Animations.Clear();
        data.AnimFrames.Clear();
        data.AnimChanges.Clear();
        data.AnimDispatches.Clear();
        data.Models.Clear();

        data.AnimCmdEdits.Add(new()
        {
            TypeID = (int)TR2Type.ScubaDiver,
            AnimIndex = 16,
            RawCount = data.AnimCommands.Count,
            TotalCount = deathAnim.Commands.Count,
        });

        return [data];
    }
}
