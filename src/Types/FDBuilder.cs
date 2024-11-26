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
}
