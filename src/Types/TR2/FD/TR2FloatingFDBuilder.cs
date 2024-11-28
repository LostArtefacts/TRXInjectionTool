using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.FD;

public class TR2FloatingFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level floating = _control2.Read($"Resources/{TR2LevelNames.FLOATER}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.FDFix, "floating_fd");

        // Rotate and shift the door that leads to room 86, otherwise there is an invisible wall.
        floating.Entities[72].X += TRConsts.Step4;
        data.ItemEdits.Add(ItemBuilder.SetAngle(floating, 72, 16384));

        return new() { data };
    }
}
