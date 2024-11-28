using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.FD;

public class TR2IcePalaceFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level palace = _control2.Read($"Resources/{TR2LevelNames.CHICKEN}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.FDFix, "palace_fd");

        // Rotate and shift the door that leads to the Jade secret, otherwise there is an invisible wall.
        // Although this is an item shift, it's included in FD as it's closest to that in terms of config setting.
        palace.Entities[143].X += TRConsts.Step4;
        data.ItemEdits.Add(ItemBuilder.SetAngle(palace, 143, 16384));

        // Duplicate the gong hammer pickup trigger into the adjacent tile.
        FDTriggerEntry hammerTrigger = GetTrigger(palace, 29, 4, 6);
        data.FloorEdits.Add(MakeTrigger(palace, 29, 3, 6, hammerTrigger));

        return new() { data };
    }
}
