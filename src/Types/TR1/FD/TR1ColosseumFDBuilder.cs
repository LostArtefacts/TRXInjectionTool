using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.FD;

public class TR1ColosseumFDBuilder : FDBuilder
{
    private static readonly List<short> _windyRooms
        = new() { 0, 18, 19, 20, 63, 64, 65, 66, 67, 70, 72, 74, 76, 77, 79, 81, 82, 83, 84 };

    public override List<InjectionData> Build()
    {
        TR1Level colosseum = _control1.Read($"Resources/{TR1LevelNames.COLOSSEUM}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.FDFix, "colosseum_fd");
        CreateDefaultTests(data, TR1LevelNames.COLOSSEUM);
        data.FloorEdits = CreateBatTriggerFixes();
        data.FloorEdits.AddRange(AddRoomFlags(_windyRooms, TRRoomFlag.Wind, colosseum.Rooms));

        return new() { data };
    }

    private static List<TRFloorDataEdit> CreateBatTriggerFixes()
    {
        // Replace the duplicated parameter for item 74 with 73 in two triggers.
        FDTrigParamFix fix = new()
        {
            ActionType = FDTrigAction.Object,
            OldParam = 74,
            NewParam = 73,
        };

        return new()
        {
            new()
            {
                RoomIndex = 70,
                X = 7,
                Z = 1,
                Fixes = new() { fix },
            },
            new()
            {
                RoomIndex = 72,
                X = 2,
                Z = 12,
                Fixes = new() { fix },
            },
        };
    }
}
