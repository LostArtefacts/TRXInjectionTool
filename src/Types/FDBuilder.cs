using TRLevelControl.Model;
using TRXInjectionTool.Actions;

namespace TRXInjectionTool.Types;

public abstract class FDBuilder : InjectionBuilder
{
    protected static TRFloorDataEdit MakeMusicOneShot(short room, ushort x, ushort z)
    {
        return new()
        {
            RoomIndex = room,
            X = x,
            Z = z,
            Fixes = new()
            {
                new FDMusicOneShot()
            },
        };
    }

    protected static FDTriggerEntry GetTrigger(TR1Level level, short room, ushort x, ushort z)
        => GetTrigger(level.Rooms[room].GetSector(x, z, TRUnit.Sector), level.FloorData);

    protected static FDTriggerEntry GetTrigger(TR2Level level, short room, ushort x, ushort z)
        => GetTrigger(level.Rooms[room].GetSector(x, z, TRUnit.Sector), level.FloorData);

    protected static FDTriggerEntry GetTrigger(TR3Level level, short room, ushort x, ushort z)
        => GetTrigger(level.Rooms[room].GetSector(x, z, TRUnit.Sector), level.FloorData);

    protected static FDTriggerEntry GetTrigger(TRRoomSector sector, FDControl floorData)
    {
        return sector.FDIndex == 0
            ? null
            : floorData[sector.FDIndex].FirstOrDefault(e => e is FDTriggerEntry) as FDTriggerEntry;
    }

    protected static TRFloorDataEdit RemoveTrigger(TR1Level level, short room, ushort x, ushort z)
    {
        return new()
        {
            RoomIndex = room,
            X = x,
            Z = z,
            Fixes = new() { MakeTrigFix(level, room, x, z) },
        };
    }

    protected static TRFloorDataEdit RemoveTrigger(TR2Level level, short room, ushort x, ushort z)
    {
        return new()
        {
            RoomIndex = room,
            X = x,
            Z = z,
            Fixes = new() { MakeTrigFix(level, room, x, z) },
        };
    }

    public static TRFloorDataEdit MakeTrigger(TRLevelBase level, short room, ushort x, ushort z, FDTriggerEntry trigger)
    {
        FDTrigCreateFix fd;
        if (level is TR1Level level1)
        {
            fd = MakeTrigFix(level1, room, x, z);
        }
        else if (level is TR2Level level2)
        {
            fd = MakeTrigFix(level2, room, x, z);
        }
        else if (level is TR3Level level3)
        {
            fd = MakeTrigFix(level3, room, x, z);
        }
        else
        {
            throw new ArgumentException("TR1-3 only supported");
        }
        fd.Entries.Add(trigger);

        return new()
        {
            RoomIndex = room,
            X = x,
            Z = z,
            Fixes = [fd],
        };
    }

    protected static FDTrigCreateFix MakeTrigFix(TR1Level level, short room, ushort x, ushort z)
        => MakeTrigFix(level.Rooms[room].GetSector(x, z, TRUnit.Sector), level.FloorData);

    protected static FDTrigCreateFix MakeTrigFix(TR2Level level, short room, ushort x, ushort z)
        => MakeTrigFix(level.Rooms[room].GetSector(x, z, TRUnit.Sector), level.FloorData);

    protected static FDTrigCreateFix MakeTrigFix(TR3Level level, short room, ushort x, ushort z)
        => MakeTrigFix(level.Rooms[room].GetSector(x, z, TRUnit.Sector), level.FloorData);

    protected static FDTrigCreateFix MakeTrigFix(TRRoomSector sector, FDControl floorData)
    {
        var fd = new FDTrigCreateFix
        {
            Entries = [],
        };

        // Ensure we capture all FD excluding trigger entries.
        if (sector.FDIndex != 0)
        {
            fd.Entries.AddRange(floorData[sector.FDIndex].Where(e => e is not FDTriggerEntry));
        }

        return fd;
    }

    protected static TRFloorDataEdit ConvertTrigger(TR1Level level, short room, ushort x, ushort z, FDTrigType newType)
    {
        return ConvertTrigger(GetTrigger(level, room, x, z), room, x, z, newType);
    }

    protected static TRFloorDataEdit ConvertTrigger(TR2Level level, short room, ushort x, ushort z, FDTrigType newType)
    {
        return ConvertTrigger(GetTrigger(level, room, x, z), room, x, z, newType);
    }

    protected static TRFloorDataEdit ConvertTrigger(FDTriggerEntry existingTrigger, short room, ushort x, ushort z, FDTrigType newType)
    {
        if (existingTrigger == null)
        {
            throw new Exception("A base trigger must exist for a type conversion.");
        }

        return new()
        {
            RoomIndex = room,
            X = x,
            Z = z,
            Fixes = new()
            {
                new FDTrigTypeFix() { NewType = newType }
            }
        };
    }

    public static TRVisPortalEdit DeletePortal<R>(List<R> rooms, int baseRoomIndex, int portalIndex)
        where R : TRRoom
    {
        // "Nullify" the vertices - the game will then disable the portal.
        var portal = rooms[baseRoomIndex].Portals[portalIndex];
        return new()
        {
            BaseRoom = (short)baseRoomIndex,
            LinkRoom = (short)portal.AdjoiningRoom,
            PortalIndex = (ushort)portalIndex,
            VertexChanges = rooms[baseRoomIndex].Portals[portalIndex].Vertices.Select(v =>
            {
                return new TRVertex
                {
                    X = (short)-v.X,
                    Y = (short)-v.Y,
                    Z = (short)-v.Z,
                };
            }).ToList(),
        };
    }

    protected static TRFloorDataEdit MakeSlant
        (TR1Level level, short room, ushort x, ushort z, sbyte xSlant, sbyte zSlant, FDSlantType type = FDSlantType.Floor)
    {
        TRRoomSector sector = level.Rooms[room].GetSector(x, z, TRUnit.Sector);
        return new()
        {
            RoomIndex = room,
            X = x,
            Z = z,
            Fixes = new()
            {
                MakeSlantFix(sector, level.FloorData, xSlant, zSlant, type),
            },
        };
    }

    protected static TRFloorDataEdit MakeSlant
        (TR2Level level, short room, ushort x, ushort z, sbyte xSlant, sbyte zSlant, FDSlantType type = FDSlantType.Floor)
    {
        TRRoomSector sector = level.Rooms[room].GetSector(x, z, TRUnit.Sector);
        return new()
        {
            RoomIndex = room,
            X = x,
            Z = z,
            Fixes = new()
            {
                MakeSlantFix(sector, level.FloorData, xSlant, zSlant, type),
            },
        };
    }

    protected static FDTrigCreateFix MakeSlantFix
        (TRRoomSector sector, FDControl floorData, sbyte xSlant, sbyte zSlant, FDSlantType type = FDSlantType.Floor)
    {
        FDTrigCreateFix fd = new()
        {
            Entries = new(),
        };

        if (sector.FDIndex != 0)
        {
            fd.Entries.AddRange(floorData[sector.FDIndex]);
            var currentSlant = fd.Entries.Find(e => e is FDSlantEntry slant && slant.Type == type);
            if (currentSlant != null)
            {
                fd.Entries.Remove(currentSlant);
            }
        }

        fd.Entries.Add(new FDSlantEntry
        {
            Type = type,
            XSlant = xSlant,
            ZSlant = zSlant
        });

        return fd;
    }

    public static IEnumerable<TRFloorDataEdit> AddRoomFlags<R>(List<short> roomIndices, TRRoomFlag flag, List<R> rooms)
        where R : TRRoom
        => AmendRoomFlags(roomIndices, flag, rooms, true);

    public static IEnumerable<TRFloorDataEdit> RemoveRoomFlags<R>(List<short> roomIndices, TRRoomFlag flag, List<R> rooms)
        where R : TRRoom
        => AmendRoomFlags(roomIndices, flag, rooms, false);

    private static IEnumerable<TRFloorDataEdit> AmendRoomFlags<R>(List<short> roomIndices, TRRoomFlag flag, List<R> rooms, bool add)
        where R : TRRoom
    {
        return roomIndices
            .Concat(roomIndices.Where(r => rooms[r].AlternateRoom != -1).Select(r => rooms[r].AlternateRoom))
            .Distinct()
            .Select(r => new TRFloorDataEdit
            {
                RoomIndex = r,
                Fixes = [new FDRoomProperties
                {
                    Flags = add ? (rooms[r].Flags | flag) : (rooms[r].Flags & ~flag),
                }]
            });
    }
}
