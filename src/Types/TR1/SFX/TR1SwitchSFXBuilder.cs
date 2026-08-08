using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.SFX;

public class TR1SwitchSFXBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        return new()
        {
            FixSwitch(TR1LevelNames.MINES, TR1Type.WallSwitch, "wall_switch_sfx"),
            FixSwitch(TR1LevelNames.ATLANTIS, TR1Type.UnderwaterSwitch, "uw_switch_sfx"),
        };
    }

    private static InjectionData FixSwitch(string levelName, TR1Type type, string binName)
    {
        var level = _control1.Read($"Resources/{levelName}");
        var model = level.Models[type];
        ResetLevel(level);
        level.Models[type] = model;

        var anims = new[] { 2, 3 };
        foreach (int anim in anims)
        {
            model.Animations[anim].Commands.RemoveAll(c => c is TRSFXCommand);
        }

        var data = InjectionData.Create(level, InjectionType.General, binName, true);
        data.Animations.Clear();
        data.AnimFrames.Clear();
        data.AnimChanges.Clear();
        data.AnimDispatches.Clear();
        data.Models.Clear();

        foreach (int anim in anims)
        {
            data.AnimCmdEdits.Add(new()
            {
                TypeID = (int)type,
                AnimIndex = anim,
                RawCount = data.AnimCommands.Count / anims.Length,
                TotalCount = 1,
            });
        }

        return data;
    }
}
