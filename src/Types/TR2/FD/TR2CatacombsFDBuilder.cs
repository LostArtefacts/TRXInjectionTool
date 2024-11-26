using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.FD;

public class TR2CatacombsFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level catacombs = _control2.Read($"Resources/{TR2LevelNames.COT}");

        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.FDFix, "catacombs_fd");
        data.FloorEdits.AddRange(FixMaskRoomFlipmap(catacombs));

        return new() { data };
    }

    private static List<TRFloorDataEdit> FixMaskRoomFlipmap(TR2Level catacombs)
    {
        List<TRFloorDataEdit> edits = new();

        // The collapsible tile triggers and the ladder entry are missing in flipped room 116.
        TR2Room room = catacombs.Rooms[98];
        for (ushort x = 12; x < 15; x++)
        {
            TRRoomSector sector = room.GetSector(x, 8, TRUnit.Sector);
            edits.Add(new()
            {
                RoomIndex = 116,
                X = x,
                Z = 8,
                Fixes = new()
                {
                    new FDTrigCreateFix()
                    {
                        Entries = new(catacombs.FloorData[sector.FDIndex].Select(e => e.Clone())),
                    }
                }
            });
        }

        // Similarly, the snow leopard triggers are missing.
        FDTriggerEntry catTrigger = GetTrigger(catacombs, 98, 10, 1);
        for (ushort z = 1; z < 9; z++)
        {
            edits.Add(MakeTrigger(catacombs, 116, 10, z, catTrigger.Clone() as FDTriggerEntry));
        }

        // And the goon triggers.
        FDTriggerEntry goonTrigger = GetTrigger(catacombs, 98, 1, 5);
        for (ushort z = 5; z < 8; z++)
        {
            edits.Add(MakeTrigger(catacombs, 116, 1, z, goonTrigger.Clone() as FDTriggerEntry));
        }

        return edits;
    }
}
