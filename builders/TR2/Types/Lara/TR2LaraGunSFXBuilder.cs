using TRLevelControl.Helpers;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Lara;

public class TR2LaraGunSFXBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.GW}");
        var edits = TR2GunUtils.FixHolsterSFX(level, true);

        var data = InjectionData.Create(level, InjectionType.General, "lara_rifle_sfx", true);
        data.Animations.Clear();
        data.AnimFrames.Clear();
        data.AnimChanges.Clear();
        data.AnimDispatches.Clear();
        data.Models.Clear();
        data.AnimCmdEdits.AddRange(edits);

        return [data];
    }
}
