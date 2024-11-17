using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.FD;

public class TR2OperaFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.FDFix, "opera_fd");
        data.FloorEdits.Add(FixLooseBoardTrigger());

        return new() { data };
    }

    private static TRFloorDataEdit FixLooseBoardTrigger()
    {
        // Replace the duplicated parameter for item 204 with 203 in room 44.
        FDTrigParamFix fix = new()
        {
            ActionType = FDTrigAction.Object,
            OldParam = 204,
            NewParam = 203,
        };

        return new()
        {
            RoomIndex = 44,
            X = 8,
            Z = 4,
            Fixes = new() { fix },
        };
    }
}
