using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.FD;

public class TR2XianFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level xian = _control2.Read($"Resources/{TR2LevelNames.XIAN}");

        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.FDFix, "xian_fd");
        data.FloorEdits.AddRange(FixDeathTiles(xian));

        return new() { data };
    }

    private static List<TRFloorDataEdit> FixDeathTiles(TR2Level xian)
    {
        List<TRFloorDataEdit> edits = new();

        TR2Room chamber = xian.Rooms[91];
        for (ushort x = 1; x < chamber.NumXSectors - 1; x++)
        {
            for (ushort z = 1; z < chamber.NumZSectors - 1; z++)
            {
                TRRoomSector sector = chamber.Sectors[x * chamber.NumZSectors + z];
                if (sector.IsWall
                    || (sector.FDIndex != 0 && xian.FloorData[sector.FDIndex].Any(e => e is FDKillLaraEntry)))
                {
                    continue;
                }

                FDTrigCreateFix fd = MakeTrigFix(sector, xian.FloorData);
                fd.Entries.Add(new FDKillLaraEntry());

                edits.Add(new()
                {
                    RoomIndex = 91,
                    X = x,
                    Z = z,
                    Fixes = new() { fd },
                });
            }
        }

        return edits;
    }
}
