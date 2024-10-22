using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1CavesTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level caves = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        InjectionData data = InjectionData.Create(InjectionType.TextureFix, "caves_textures");

        data.RoomEdits.AddRange(CreateFillers(caves));
        data.RoomEdits.AddRange(CreateRefacings(caves));
        data.RoomEdits.AddRange(CreateShifts(caves));

        return new() { data };
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR1Level caves)
    {
        return new()
        {
            new()
            {
                RoomIndex = 1,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 1,
                SourceIndex = 101,
                Vertices = new()
                {
                    caves.Rooms[1].Mesh.Rectangles[102].Vertices[0],
                    caves.Rooms[1].Mesh.Rectangles[73].Vertices[1],
                    caves.Rooms[1].Mesh.Rectangles[73].Vertices[0],
                    caves.Rooms[1].Mesh.Rectangles[75].Vertices[0],
                },
            },

            new()
            {
                RoomIndex = 14,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = GetSource(caves, TRMeshFaceType.TexturedTriangle, 72).Room,
                SourceIndex = GetSource(caves, TRMeshFaceType.TexturedTriangle, 72).Face,
                Vertices = new()
                {
                    caves.Rooms[14].Mesh.Rectangles[108].Vertices[1],
                    caves.Rooms[14].Mesh.Rectangles[107].Vertices[2],
                    caves.Rooms[14].Mesh.Rectangles[107].Vertices[1],
                },
            },

            new()
            {
                RoomIndex = 10,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = GetSource(caves, TRMeshFaceType.TexturedTriangle, 72).Room,
                SourceIndex = GetSource(caves, TRMeshFaceType.TexturedTriangle, 72).Face,
                Vertices = new() { 89, 119, 122 },
            },

            new()
            {
                RoomIndex = 10,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = GetSource(caves, TRMeshFaceType.TexturedTriangle, 72).Room,
                SourceIndex = GetSource(caves, TRMeshFaceType.TexturedTriangle, 72).Face,
                Vertices = new() { 327, 298, 325 },
            },

            new()
            {
                RoomIndex = 30,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = GetSource(caves, TRMeshFaceType.TexturedTriangle, 72).Room,
                SourceIndex = GetSource(caves, TRMeshFaceType.TexturedTriangle, 72).Face,
                Vertices = new()
                {
                    caves.Rooms[30].Mesh.Rectangles[150].Vertices[3],
                    caves.Rooms[30].Mesh.Rectangles[175].Vertices[2],
                    caves.Rooms[30].Mesh.Rectangles[150].Vertices[0],
                },
            },

            new()
            {
                RoomIndex = 30,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = GetSource(caves, TRMeshFaceType.TexturedTriangle, 72).Room,
                SourceIndex = GetSource(caves, TRMeshFaceType.TexturedTriangle, 72).Face,
                Vertices = new()
                {
                    caves.Rooms[30].Mesh.Rectangles[176].Vertices[3],
                    caves.Rooms[30].Mesh.Rectangles[176].Vertices[2],
                    caves.Rooms[30].Mesh.Triangles[21].Vertices[0],
                },
            }
        };
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR1Level caves)
    {
        return new()
        {
            Reface(caves, 6, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 27, 179),
        };
    }

    private static List<TRRoomTextureMove> CreateShifts(TR1Level caves)
    {
        return new()
        {
            new()
            {
                RoomIndex = 14,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 108,
                VertexRemap = new()
                {
                    new()
                    {
                        NewVertexIndex = caves.Rooms[14].Mesh.Rectangles[107].Vertices[2],
                    }
                }
            }
        };
    }
}
