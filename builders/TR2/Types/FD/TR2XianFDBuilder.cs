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
        CreateDefaultTests(data, TR2LevelNames.XIAN);
        data.FloorEdits.AddRange(FixDeathTiles(xian));
        data.FloorEdits.AddRange(FixChamberKeySoftlock(xian));
        data.StaticMeshEdits.AddRange(FixBridgeCollision(xian));

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

    private static List<TRFloorDataEdit> FixChamberKeySoftlock(TR2Level xian)
    {
        // Reset the spike walls in the main chamber in case the player misses the key.
        FDTriggerEntry antiTrigger = new()
        {
            TrigType = FDTrigType.AntiTrigger,
            Mask = 31,
            Actions = new()
            {
                new() { Parameter = 141 },
                new() { Parameter = 142 },
                new() { Parameter = 143 },
            }
        };

        // Use a pickup trigger to mark the walls as one shot.
        FDTriggerEntry pickupTrigger = new()
        {
            TrigType = FDTrigType.Pickup,
            Mask = 31,
            OneShot = true,
            Actions = new()
            {
                new() { Parameter = 205 },
                new() { Parameter = 141 },
                new() { Parameter = 142 },
                new() { Parameter = 143 },
                new() { Parameter = 148 },
            }
        };

        List<TRFloorDataEdit> edits = new()
        {
            // The ones beside the keyhole
            MakeTrigger(xian, 90, 10, 9, antiTrigger),
            MakeTrigger(xian, 90, 11, 9, antiTrigger),
            MakeTrigger(xian, 92, 9, 9, antiTrigger),

            MakeTrigger(xian, 91, 9, 8, antiTrigger),
            MakeTrigger(xian, 91, 10, 8, antiTrigger),
            MakeTrigger(xian, 91, 11, 8, antiTrigger),

            MakeTrigger(xian, 91, 8, 5, antiTrigger),
            MakeTrigger(xian, 91, 8, 6, antiTrigger),
            MakeTrigger(xian, 91, 8, 7, antiTrigger),

            // The one after the ladder
            MakeTrigger(xian, 92, 1, 9, new()
            {
                TrigType = FDTrigType.AntiTrigger,
                Mask = 31,
                Actions = new()
                {
                    new() { Parameter = 148 },
                }
            }),

            // When the key is picked up, the spike walls will not reset.
            MakeTrigger(xian, 92, 9, 10, pickupTrigger),
            MakeTrigger(xian, 92, 9, 11, pickupTrigger),
        };

        return edits;
    }

    private static IEnumerable<TRStaticMeshEdit> FixBridgeCollision(TR2Level level)
    {
        return new[] { 31, 32 }
            .Select(id => new TRStaticMeshEdit
            {
                TypeID = id,
                Mesh = new()
                {
                    CollisionBox = new(),
                    VisibilityBox = level.StaticMeshes[(TR2Type)((int)TR2Type.SceneryBase + id)].VisibilityBox,
                    NonCollidable = true,
                    Visible = true,
                }
            });
    }
}
