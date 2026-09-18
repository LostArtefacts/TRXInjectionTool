using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR4.FD;

public class TR4AngkorRaceFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR4, InjectionType.FDFix, "race_fd");
        CreateDefaultTests(data, $"TR4/{TR4LevelNames.IRIS_RACE}");
        var level = _control4.Read($"Resources/TR4/{TR4LevelNames.IRIS_RACE}");
        data.FloorEdits.Add(FixSwitch39(level));
        data.FloorEdits.Add(FixDoor84(level));

        return [data];
    }

    private static TRFloorDataEdit FixSwitch39(TR4Level level)
    {
        var room = level.Rooms[76];
        var sector = TRRoomSectorExt.CloneFrom(room.GetSector(1, 1, TRUnit.Sector));
        sector.Ceiling = TRConsts.NoHeight;
        sector.Floor = TRConsts.NoHeight;
        return new()
        {
            RoomIndex = 76,
            X = 1,
            Z = 1,
            Fixes =
            [
                new FDSectorOverwrite { Sector = sector },
            ],
        };
    }

    private static TRFloorDataEdit FixDoor84(TR4Level level)
    {
        var trigger = GetTrigger(level, 27, 11, 3);
        trigger.Actions
            .First(a => a.Action == FDTrigAction.Object && a.Parameter == 85)
            .Parameter = 84;
        return MakeTrigger(level, 27, 11, 3, trigger);
    }
}
