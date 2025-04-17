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
    {
        return GetTrigger(level.Rooms[room].GetSector(x, z, TRUnit.Sector), level.FloorData);
    }

    protected static FDTriggerEntry GetTrigger(TR2Level level, short room, ushort x, ushort z)
    {
        return GetTrigger(level.Rooms[room].GetSector(x, z, TRUnit.Sector), level.FloorData);
    }

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

    protected static TRFloorDataEdit MakeTrigger(TR1Level level, short room, ushort x, ushort z, FDTriggerEntry trigger)
    {
        FDTrigCreateFix fd = MakeTrigFix(level, room, x, z);
        fd.Entries.Add(trigger);

        return new()
        {
            RoomIndex = room,
            X = x,
            Z = z,
            Fixes = new() { fd },
        };
    }

    protected static TRFloorDataEdit MakeTrigger(TR2Level level, short room, ushort x, ushort z, FDTriggerEntry trigger)
    {
        FDTrigCreateFix fd = MakeTrigFix(level, room, x, z);
        fd.Entries.Add(trigger);

        return new()
        {
            RoomIndex = room,
            X = x,
            Z = z,
            Fixes = new() { fd },
        };
    }

    protected static FDTrigCreateFix MakeTrigFix(TR1Level level, short room, ushort x, ushort z)
    {
        return MakeTrigFix(level.Rooms[room].GetSector(x, z, TRUnit.Sector), level.FloorData);
    }

    protected static FDTrigCreateFix MakeTrigFix(TR2Level level, short room, ushort x, ushort z)
    {
        return MakeTrigFix(level.Rooms[room].GetSector(x, z, TRUnit.Sector), level.FloorData);
    }

    protected static FDTrigCreateFix MakeTrigFix(TRRoomSector sector, FDControl floorData)
    {
        FDTrigCreateFix fd = new()
        {
            Entries = new(),
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

    protected static TRVisPortalEdit DeletePortal<R>(List<R> rooms, int baseRoomIndex, int portalIndex)
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
}
