using TRLevelControl;
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
        CreateDefaultTests(data, TR2LevelNames.COT);
        data.FloorEdits.AddRange(FixMaskRoomFlipmap(catacombs));
        data.FloorEdits.AddRange(FixStartMusicTriggers(catacombs));
        FixYetiRoom(catacombs, data);

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

    private static List<TRFloorDataEdit> FixStartMusicTriggers(TR2Level catacombs)
    {
        // Avoid triggering dramatic music when shimmying to the stone secret, or using
        // the ladder to reach the bottom.
        return new()
        {
            ConvertTrigger(catacombs, 6, 2, 1, FDTrigType.Pad),
            ConvertTrigger(catacombs, 6, 2, 2, FDTrigType.Pad),
            ConvertTrigger(catacombs, 6, 3, 3, FDTrigType.Pad),
            ConvertTrigger(catacombs, 6, 3, 4, FDTrigType.Pad),
            ConvertTrigger(catacombs, 74, 2, 1, FDTrigType.Pad),
            ConvertTrigger(catacombs, 74, 2, 2, FDTrigType.Pad),
        };
    }

    private static void FixYetiRoom(TR2Level level, InjectionData data)
    {
        data.FloorEdits.AddRange(new[] { 0, 11 }.Select(z =>
        {
            var sector = TRRoomSectorExt.CloneFrom(level.Rooms[41].GetSector(7, z, TRUnit.Sector));
            sector.FDIndex = 0;
            sector.Ceiling = TRConsts.NoHeight;
            sector.Floor = TRConsts.NoHeight;
            return new TRFloorDataEdit
            {
                RoomIndex = 41,
                X = 7,
                Z = (ushort)z,
                Fixes = new()
                {
                    new FDSectorOverwrite { Sector = sector },
                    new FDPortalOverwrite(),
                }
            };
        }));

        var mesh = level.Rooms[41].Mesh;
        data.RoomEdits.Add(TextureBuilder.CreateFace(41, 50, 96, TRMeshFaceType.TexturedQuad, new[]
        {
            mesh.Rectangles[115].Vertices[1], mesh.Rectangles[135].Vertices[0],
            mesh.Rectangles[134].Vertices[3], mesh.Rectangles[115].Vertices[2],
        }));
        data.RoomEdits.Add(TextureBuilder.CreateFace(41, 50, 96, TRMeshFaceType.TexturedQuad, new[]
        {
            mesh.Rectangles[117].Vertices[1], mesh.Rectangles[96].Vertices[0],
            mesh.Rectangles[96].Vertices[3], mesh.Rectangles[116].Vertices[2],
        }));
    }
}
