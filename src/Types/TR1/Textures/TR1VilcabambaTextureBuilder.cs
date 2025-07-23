using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1VilcabambaTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level vilcabamba = _control1.Read($"Resources/{TR1LevelNames.VILCABAMBA}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.TextureFix, "vilcabamba_textures");
        CreateDefaultTests(data, TR1LevelNames.VILCABAMBA);

        data.RoomEdits.AddRange(CreateFillers(vilcabamba));
        data.RoomEdits.AddRange(CreateRefacings());
        data.RoomEdits.AddRange(CreateRotations());

        FixBatTransparency(vilcabamba, data);
        FixWolfTransparency(vilcabamba, data);
        FixPassport(vilcabamba, data);

        return new() { data };
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR1Level vilcabamba)
    {
        return new()
        {
            new()
            {
                RoomIndex = 15,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 15,
                SourceIndex = 22,
                Vertices = new()
                {
                    vilcabamba.Rooms[15].Mesh.Rectangles[163].Vertices[1],
                    vilcabamba.Rooms[15].Mesh.Rectangles[163].Vertices[0],
                    vilcabamba.Rooms[15].Mesh.Rectangles[166].Vertices[0],
                },
            },
            new()
            {
                RoomIndex = 21,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 5,
                Vertices = new()
                {
                    vilcabamba.Rooms[21].Mesh.Rectangles[4].Vertices[3],
                    vilcabamba.Rooms[21].Mesh.Rectangles[4].Vertices[2],
                    vilcabamba.Rooms[21].Mesh.Rectangles[5].Vertices[1],
                    vilcabamba.Rooms[21].Mesh.Rectangles[5].Vertices[0],
                },
            },
        };
    }

    private static List<TRRoomTextureReface> CreateRefacings()
    {
        return new()
        {
            new()
            {
                RoomIndex = 26,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 26,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 425,
                TargetIndex = 403,
            },
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(15, TRMeshFaceType.TexturedQuad, 191, 3),
            Rotate(26, TRMeshFaceType.TexturedTriangle, 0, 1),
            Rotate(73, TRMeshFaceType.TexturedQuad, 3, 3),
        };
    }
}
