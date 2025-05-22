using System.Diagnostics;
using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.FD;

public class TR2IcePalaceFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level palace = _control2.Read($"Resources/{TR2LevelNames.CHICKEN}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.FDFix, "palace_fd");
        CreateDefaultTests(data, TR2LevelNames.CHICKEN);

        data.FloorEdits.AddRange(FixSpringboardTrigger(palace));

        // Rotate and shift the door that leads to the Jade secret, otherwise there is an invisible wall.
        // Although this is an item shift, it's included in FD as it's closest to that in terms of config setting.
        palace.Entities[143].X += TRConsts.Step4;
        data.ItemEdits.Add(ItemBuilder.SetAngle(palace, 143, 16384));

        // Duplicate the gong hammer pickup trigger into the adjacent tile.
        FDTriggerEntry hammerTrigger = GetTrigger(palace, 29, 4, 6);
        data.FloorEdits.Add(MakeTrigger(palace, 29, 3, 6, hammerTrigger));

        data.FloorEdits.Add(FixZoning(palace));

        return new() { data };
    }

    private static List<TRFloorDataEdit> FixSpringboardTrigger(TR2Level palace)
    {
        FDTriggerEntry springTrig = GetTrigger(palace, 104, 3, 2);
        Debug.Assert(springTrig != null);

        return new()
        {
            RemoveTrigger(palace, 104, 3, 2),
            MakeTrigger(palace, 104, 2, 2, springTrig),
        };
    }

    private static TRFloorDataEdit FixZoning(TR2Level palace)
    {
        // Box 377 is in room 48, which isn't part of the flipmap, but it also spills
        // into room 45 which has flip room 110. The original editor appears to have not
        // recognised this and so the box's flip zone infers there is no zone link.
        TRRoomSector sector = palace.Rooms[48].GetSector(5, 4, TRUnit.Sector);
        TRZoneGroup zone = palace.Boxes[sector.BoxIndex].Zone;
        zone.FlipOnZone = zone.FlipOffZone.Clone();
        return new()
        {
            RoomIndex = 48,
            X = 5,
            Z = 4,
            Fixes = new()
            {
                new FDZoneFix
                {
                    ZoneOverwrite = zone,
                },
            },
        };
    }
}
