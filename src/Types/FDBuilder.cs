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
        TRRoomSector sector = level.Rooms[room].GetSector(x, z, TRUnit.Sector);
        if (sector.FDIndex != 0)
        {
            return level.FloorData[sector.FDIndex].FirstOrDefault(e => e is FDTriggerEntry) as FDTriggerEntry;
        }

        return null;
    }

    protected static TRFloorDataEdit RemoveTrigger(TR1Level level, short room, ushort x, ushort z)
    {
        FDTrigCreateFix fd = MakeTrigFix(level, room, x, z);

        return new()
        {
            RoomIndex = room,
            X = x,
            Z = z,
            Fixes = new() { fd },
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

    protected static FDTrigCreateFix MakeTrigFix(TR1Level level, short room, ushort x, ushort z)
    {
        FDTrigCreateFix fd = new()
        {
            Entries = new(),
        };

        // Ensure we capture all FD excluding trigger entries.
        TRRoomSector sector = level.Rooms[room].GetSector(x, z, TRUnit.Sector);
        if (sector.FDIndex != 0)
        {
            fd.Entries.AddRange(level.FloorData[sector.FDIndex].Where(e => e is not FDTriggerEntry));
        }

        return fd;
    }
}
