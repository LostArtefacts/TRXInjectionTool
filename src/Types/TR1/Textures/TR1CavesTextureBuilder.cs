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
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.TextureFix, "caves_textures");
        CreateDefaultTests(data, TR1LevelNames.CAVES);

        data.RoomEdits.AddRange(CreateFillers(caves));
        data.RoomEdits.AddRange(CreateRefacings(caves));
        data.RoomEdits.AddRange(CreateShifts(caves));
        data.RoomEdits.AddRange(CreateRotations());

        FixBatTransparency(caves, data);
        FixWolfTransparency(caves, data);
        FixPassport(caves, data);

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
        return
        [
            Reface(caves, 0, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 38, 13),
            Reface(caves, 0, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 30, 21),
            Reface(caves, 1, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 49, 21),
            Reface(caves, 1, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 30, 33),
            Reface(caves, 1, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 7, 227),
            Reface(caves, 1, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 5, 228),
            Reface(caves, 1, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 14, 33),
            Reface(caves, 1, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 14, 34),
            Reface(caves, 1, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 187, 35),
            Reface(caves, 1, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 14, 37),
            Reface(caves, 1, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 14, 39),
            Reface(caves, 1, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 51, 29),
            Reface(caves, 1, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 172, 154),
            Reface(caves, 6, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 29, 1),
            Reface(caves, 6, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 29, 3),
            Reface(caves, 6, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 84, 13),
            Reface(caves, 6, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 160, 19),
            Reface(caves, 6, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 27, 179),
            Reface(caves, 14, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 0, 231),
            Reface(caves, 22, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedQuad, 43, 0),
            Reface(caves, 24, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 33, 0),
            Reface(caves, 24, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 138, 1),
            Reface(caves, 30, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 209, 10),
            Reface(caves, 32, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 54, 13),
            Reface(caves, 32, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 107, 216),
            Reface(caves, 32, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 114, 212),
            Reface(caves, 32, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 203, 112),
            Reface(caves, 32, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 153, 116),
            Reface(caves, 32, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 159, 3),
        ];
    }

    private static List<TRRoomTextureEdit> CreateShifts(TR1Level caves)
    {
        var mesh32 = caves.Rooms[32].Mesh;
        return
        [
            new TRRoomTextureMove
            {
                RoomIndex = 14,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 108,
                VertexRemap =
                [
                    new()
                    {
                        NewVertexIndex = caves.Rooms[14].Mesh.Rectangles[107].Vertices[2],
                    }
                ]
            },
            new TRRoomVertexCreate
            {
                RoomIndex = 32,
                Vertex = new()
                {
                    Lighting = mesh32.Vertices[mesh32.Rectangles[211].Vertices[0]].Lighting,
                    Vertex = new()
                    {
                        X = 4438,
                        Y = 3584,
                        Z = 14336,
                    }
                },
            },
            new TRRoomTextureMove
            {
                RoomIndex = 32,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 216,
                VertexRemap =
                [
                    new()
                    {
                        NewVertexIndex = (ushort)mesh32.Vertices.Count,
                    },
                    new()
                    {
                        Index = 1,
                        NewVertexIndex = mesh32.Rectangles[211].Vertices[3]
                    },
                    new()
                    {
                        Index = 2,
                        NewVertexIndex = mesh32.Rectangles[210].Vertices[2]
                    },
                    new()
                    {
                        Index = 3,
                        NewVertexIndex = mesh32.Rectangles[215].Vertices[1]
                    }
                ]
            },
            new TRRoomTextureMove
            {
                RoomIndex = 32,
                FaceType = TRMeshFaceType.TexturedTriangle,
                TargetIndex = 13,
                VertexRemap =
                [
                    new()
                    {
                        NewVertexIndex = mesh32.Rectangles[211].Vertices[3]
                    },
                    new()
                    {
                        Index = 1,
                        NewVertexIndex = mesh32.Rectangles[216].Vertices[0]
                    },
                    new()
                    {
                        Index = 2,
                        NewVertexIndex = mesh32.Rectangles[216].Vertices[1]
                    },
                ]
            }
        ];
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return
        [
            Rotate(0, TRMeshFaceType.TexturedTriangle, 3, 2),
            Rotate(0, TRMeshFaceType.TexturedTriangle, 21, 2),
            Rotate(0, TRMeshFaceType.TexturedTriangle, 18, 2),
            Rotate(0, TRMeshFaceType.TexturedTriangle, 19, 2),
            Rotate(0, TRMeshFaceType.TexturedQuad, 257, 1),
            Rotate(1, TRMeshFaceType.TexturedTriangle, 11, 2),
            Rotate(1, TRMeshFaceType.TexturedTriangle, 15, 2),
            Rotate(1, TRMeshFaceType.TexturedTriangle, 20, 2),
            Rotate(1, TRMeshFaceType.TexturedTriangle, 25, 2),
            Rotate(1, TRMeshFaceType.TexturedTriangle, 27, 2),
            Rotate(1, TRMeshFaceType.TexturedTriangle, 21, 2),
            Rotate(1, TRMeshFaceType.TexturedTriangle, 32, 2),
            Rotate(1, TRMeshFaceType.TexturedTriangle, 33, 2),
            Rotate(2, TRMeshFaceType.TexturedTriangle, 13, 2),
            Rotate(2, TRMeshFaceType.TexturedTriangle, 19, 2),
            Rotate(4, TRMeshFaceType.TexturedTriangle, 7, 2),
            Rotate(6, TRMeshFaceType.TexturedTriangle, 0, 2),
            Rotate(6, TRMeshFaceType.TexturedTriangle, 1, 2),
            Rotate(6, TRMeshFaceType.TexturedTriangle, 3, 2),
            Rotate(6, TRMeshFaceType.TexturedTriangle, 13, 1),
            Rotate(6, TRMeshFaceType.TexturedTriangle, 19, 1),
            Rotate(22, TRMeshFaceType.TexturedTriangle, 0, 2),
            Rotate(24, TRMeshFaceType.TexturedTriangle, 0, 1),
            Rotate(24, TRMeshFaceType.TexturedTriangle, 1, 1),
            Rotate(24, TRMeshFaceType.TexturedTriangle, 2, 2),
            Rotate(24, TRMeshFaceType.TexturedTriangle, 6, 2),
            Rotate(30, TRMeshFaceType.TexturedTriangle, 13, 1),
            Rotate(30, TRMeshFaceType.TexturedTriangle, 16, 2),
            Rotate(30, TRMeshFaceType.TexturedTriangle, 10, 2),
            Rotate(30, TRMeshFaceType.TexturedTriangle, 11, 2),
            Rotate(32, TRMeshFaceType.TexturedTriangle, 0, 2),
            Rotate(32, TRMeshFaceType.TexturedTriangle, 1, 2),
        ];
    }
}
