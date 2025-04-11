using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Misc;

public class TR2GuardianBuilder  : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level palace = _control2.Read($"Resources/{TR2LevelNames.CHICKEN}");
        TRModel guardian = palace.Models[TR2Type.BirdMonster];
        ResetLevel(palace);
        palace.Models[TR2Type.BirdMonster] = guardian;

        TRAnimation deathAnim = guardian.Animations[20];
        guardian.Animations.ForEach(a =>
        {
            if (a != deathAnim)
            {
                a.Commands.Clear();
            }
        });

        deathAnim.Commands.Add(new TRFXCommand
        {
            FrameNumber = deathAnim.FrameEnd,
            EffectID = (short)TR2FX.EndLevel,
        });

        // Compile the commands into flat TR format.
        // We only want the commands, so remove all other model data.
        InjectionData data = InjectionData.Create(palace, InjectionType.General, "guardian_death_commands", true);
        data.Animations.Clear();
        data.AnimFrames.Clear();
        data.AnimChanges.Clear();
        data.AnimDispatches.Clear();
        data.Models.Clear();

        data.AnimCmdEdits.Add(new()
        {
            TypeID = (int)TR2Type.BirdMonster,
            AnimIndex = 20,
            RawCount = data.AnimCommands.Count,
            TotalCount = deathAnim.Commands.Count,
        });

        return new() { data };
    }
}
