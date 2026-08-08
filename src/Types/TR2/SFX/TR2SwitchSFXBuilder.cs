using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.SFX;

public class TR2SwitchSFXBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.FATHOMS}");
        var model = level.Models[TR2Type.WallSwitch];
        ResetLevel(level);
        level.Models[TR2Type.WallSwitch] = model;

        var anims = new[] { 2, 3 };
        foreach (int anim in anims)
        {
            model.Animations[anim].Commands.RemoveAll(c => c is TRSFXCommand);
        }

        var data = InjectionData.Create(level, InjectionType.General, "wall_switch_sfx", true);
        data.Animations.Clear();
        data.AnimFrames.Clear();
        data.AnimChanges.Clear();
        data.AnimDispatches.Clear();
        data.Models.Clear();

        foreach (int anim in anims)
        {
            data.AnimCmdEdits.Add(new()
            {
                TypeID = (int)TR2Type.WallSwitch,
                AnimIndex = anim,
                RawCount = data.AnimCommands.Count / anims.Length,
                TotalCount = 1,
            });
        }

        return new() { data };
    }
}
