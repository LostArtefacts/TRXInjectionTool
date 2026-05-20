using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.FD;

public class TR3AldwychFDBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.General, "aldwych_fd");
        CreateDefaultTests(data, $"TR3/{TR3LevelNames.ALDWYCH}");
        data.FloorEdits.Add(FixDrillAntitrigger());
        data.ItemPosEdits.Add(FixPunkFire());

        return [data];
    }

    private static TRFloorDataEdit FixDrillAntitrigger()
    {
        return new()
        {
            RoomIndex = 116,
            X = 1,
            Z = 5,
            Fixes = [new FDTrigDelete()],
        };
    }

    private static TRItemPosEdit FixPunkFire()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.ALDWYCH}");
        var fire = level.Entities[200];
        var punk = level.Entities[190];
        fire.X = punk.X;
        fire.Y = punk.Y;
        fire.Z = punk.Z;
        return ItemBuilder.MoveToRoom(level, 200, punk.Room);
    }
}
