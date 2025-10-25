using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.FD;

public class TR2FloatingFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level floating = _control2.Read($"Resources/{TR2LevelNames.FLOATER}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.FDFix, "floating_fd");
        CreateDefaultTests(data, TR2LevelNames.FLOATER);

        // Rotate and shift the door that leads to room 86, otherwise there is an invisible wall.
        floating.Entities[72].X += TRConsts.Step4;
        data.ItemPosEdits.Add(ItemBuilder.SetAngle(floating, 72, 16384));

        data.FloorEdits.AddRange(FixZiplineReset(floating));

        return new() { data };
    }

    private static List<TRFloorDataEdit> FixZiplineReset(TR2Level floating)
    {
        // Reset the zipline if the player goes back towards the level start from the gold secret area.
        FDTriggerEntry resetTrigger = new()
        {
            Mask = 31,
            Actions = new()
            {
                new() { Parameter = 41 },
            }
        };

        List<TRFloorDataEdit> edits = new();
        for (ushort z = 3; z < 14; z++)
        {
            edits.Add(MakeTrigger(floating, 41, 2, z, resetTrigger));
        }
        return edits;
    }
}
