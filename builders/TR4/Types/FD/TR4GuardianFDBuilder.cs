using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR4.FD;

public class TR4GuardianFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR4, InjectionType.FDFix, "guardian_fd");
        CreateDefaultTests(data, $"TR4/{TR4LevelNames.GUARDIAN}");
        var level = _control4.Read($"Resources/TR4/{TR4LevelNames.GUARDIAN}");
        foreach (short roomIdx in new short[] { 62, 70, 71, 72, 73 })
        {
            data.FloorEdits.Add(FixSwitchRoom(level, roomIdx));
        }

        return [data];
    }

    // Turns the sector behind each switch into a wall, so that the camera
    // stops at the switch.
    private static TRFloorDataEdit FixSwitchRoom(TR4Level level, short roomIdx)
    {
        var room = level.Rooms[roomIdx];
        var sector = TRRoomSectorExt.CloneFrom(room.GetSector(1, 1, TRUnit.Sector));
        sector.Ceiling = TRConsts.NoHeight;
        sector.Floor = TRConsts.NoHeight;
        return new()
        {
            RoomIndex = roomIdx,
            X = 1,
            Z = 1,
            Fixes =
            [
                new FDSectorOverwrite { Sector = sector },
            ],
        };
    }
}
