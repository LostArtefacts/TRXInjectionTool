using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.FD;

public class TR2OperaFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level opera = _control2.Read($"Resources/{TR2LevelNames.OPERA}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.FDFix, "opera_fd");
        CreateDefaultTests(data, TR2LevelNames.OPERA);
        data.FloorEdits.Add(FixLooseBoardTrigger());

        // Flood room 136 near the airplane.
        data.FloorEdits.Add(new()
        {
            RoomIndex = 136,
            Fixes = new()
            {
                new FDRoomProperties
                {
                    Flags = opera.Rooms[136].Flags | TRRoomFlag.Water,
                }
            }
        });

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
