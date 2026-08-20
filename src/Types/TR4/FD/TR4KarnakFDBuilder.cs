using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR4.FD;

public class TR4KarnakFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR4, InjectionType.FDFix, "karnak_fd");
        CreateDefaultTests(data, $"TR4/{TR4LevelNames.KARNAK}");
        FixCutsceneGeometry(data);

        return [data];
    }

    private static void FixCutsceneGeometry(InjectionData data)
    {
        var level = _control4.Read($"Resources/TR4/{TR4LevelNames.KARNAK}");
        var room0 = level.Rooms[0];
        var room3 = level.Rooms[3];

        var baseSector = room3.GetSector(9, 1, TRUnit.Sector);
        var wallSector = room3.GetSector(9, 0,  TRUnit.Sector);
        var xDiff = room0.NumXSectors - room3.NumXSectors;

        data.FloorEdits.Add(new()
        {
            RoomIndex = 3,
            Fixes = [new FDRoomExtension
            {
                RoomIndex = 3,
                AdditionalXSectors = (ushort)xDiff,
                SizeChange = (short)(xDiff * room3.NumZSectors),
            }],            
        });

        // Extend room 3 in +X to sit fully atop room 0
        for (var x = room3.NumXSectors - 1; x < room0.NumXSectors; x++)
        {
            for (var z = 0; z < room3.NumZSectors; z++)
            {
                var isInteriorZ = z > 0 && z < room3.NumZSectors - 1;
                var isBoundaryX = x == room0.NumXSectors - 1;
                var sector = x == room3.NumXSectors - 1
                    ? (isInteriorZ ? baseSector : null)
                    : (isBoundaryX || !isInteriorZ ? wallSector : baseSector);

                if (sector == null)
                    continue;

                data.FloorEdits.Add(new()
                {
                    RoomIndex = 3,
                    X = (ushort)x,
                    Z = (ushort)z,
                    Fixes = [new FDSectorOverwrite
                    {
                        Sector = TRRoomSectorExt.CloneFrom(sector),
                    }],
                });
            }
        }

        // Add sky portals from room 0 to 3
        for (var x = room3.NumXSectors - 1; x < room0.NumXSectors - 1; x++)
        {
            for (var z = 1; z < room0.NumZSectors - 1; z++)
            {
                data.FloorEdits.Add(new()
                {
                    RoomIndex = 0,
                    X = (ushort)x,
                    Z = (ushort)z,
                    Fixes = [new FDPortalOverwrite { Sky = 3 }],
                });
            }
        }

        // Extend visibility portals in both rooms
        var oldXExtent = (short)((room3.NumXSectors - 1) * TRConsts.Step4);
        var xChange = (short)(xDiff * TRConsts.Step4);
        foreach (var roomNum in new[] { 0, 3 })
        {
            var room = level.Rooms[roomNum];
            for (var j = 0; j < room.Portals.Count; j++)
            {
                var portal = room.Portals[j];
                if (portal.AdjoiningRoom != 0 && portal.AdjoiningRoom != 3)
                    continue;

                data.VisPortalEdits.Add(new()
                {
                    BaseRoom = (short)roomNum,
                    LinkRoom = (short)portal.AdjoiningRoom,
                    PortalIndex = (ushort)j,
                    VertexChanges = [.. portal.Vertices
                        .Select(vertex => new TRVertex
                        {
                            X = vertex.X == oldXExtent ? xChange : (short)0,
                        })],
                });
            }
        }
    }
}
