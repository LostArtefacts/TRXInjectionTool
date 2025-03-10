using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1ValleyTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level valley = _control1.Read($"Resources/{TR1LevelNames.VALLEY}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.TextureFix, "valley_textures");
        CreateDefaultTests(data, TR1LevelNames.VALLEY);

        data.RoomEdits.AddRange(CreateFillers(valley));
        data.RoomEdits.AddRange(CreateRefacings(valley));
        data.RoomEdits.AddRange(CreateRotations());
        data.RoomEdits.AddRange(CreateShifts(valley));
        data.RoomEdits.AddRange(CreateVertexShifts(valley));

        TR1CommonTextureBuilder.FixWolfTransparency(valley, data);

        return new() { data };
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR1Level valley)
    {
        return new()
        {
            new()
            {
                RoomIndex = 25,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 25,
                SourceIndex = 89,
                Vertices = new()
                {
                    valley.Rooms[25].Mesh.Rectangles[138].Vertices[3],
                    valley.Rooms[25].Mesh.Rectangles[138].Vertices[2],
                    valley.Rooms[25].Mesh.Triangles[89].Vertices[0],
                }
            },
            new()
            {
                RoomIndex = 25,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 25,
                SourceIndex = 89,
                Vertices = new()
                {
                    valley.Rooms[25].Mesh.Rectangles[125].Vertices[0],
                    valley.Rooms[25].Mesh.Rectangles[125].Vertices[3],
                    valley.Rooms[25].Mesh.Triangles[90].Vertices[0],
                }
            },
            new()
            {
                RoomIndex = 90,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 23,
                SourceIndex = 49,
                Vertices = new()
                {
                    valley.Rooms[90].Mesh.Rectangles[50].Vertices[1],
                    valley.Rooms[90].Mesh.Rectangles[50].Vertices[0],
                    valley.Rooms[90].Mesh.Rectangles[69].Vertices[1],
                    valley.Rooms[90].Mesh.Rectangles[69].Vertices[0],
                }
            },
            new()
            {
                RoomIndex = 26,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 23,
                SourceIndex = 49,
                Vertices = new()
                {
                    valley.Rooms[26].Mesh.Rectangles[1].Vertices[2],
                    valley.Rooms[26].Mesh.Rectangles[1].Vertices[1],
                    valley.Rooms[26].Mesh.Rectangles[0].Vertices[1],
                    valley.Rooms[26].Mesh.Rectangles[0].Vertices[0],
                }
            },
            new()
            {
                RoomIndex = 9,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 9,
                SourceIndex = 0,
                Vertices = new()
                {
                    valley.Rooms[9].Mesh.Rectangles[50].Vertices[3],
                    valley.Rooms[9].Mesh.Rectangles[50].Vertices[2],
                    valley.Rooms[9].Mesh.Rectangles[49].Vertices[0],
                }
            },
            new()
            {
                RoomIndex = 6,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 6,
                SourceIndex = 17,
                Vertices = new()
                {
                    valley.Rooms[6].Mesh.Rectangles[34].Vertices[0],
                    valley.Rooms[6].Mesh.Rectangles[34].Vertices[3],
                    valley.Rooms[6].Mesh.Rectangles[37].Vertices[1],
                }
            },
            new()
            {
                RoomIndex = 6,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 6,
                SourceIndex = 17,
                Vertices = new()
                {
                    valley.Rooms[6].Mesh.Rectangles[37].Vertices[2],
                    valley.Rooms[6].Mesh.Rectangles[37].Vertices[1],
                    valley.Rooms[6].Mesh.Rectangles[34].Vertices[3],
                }
            }
        };
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR1Level valley)
    {
        return new()
        {
            new()
            {
                RoomIndex = 6,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = GetSource(valley, TRMeshFaceType.TexturedTriangle, 69).Room,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = GetSource(valley, TRMeshFaceType.TexturedTriangle, 69).Face,
                TargetIndex = 0,
            },
            new()
            {
                RoomIndex = 9,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 9,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 35,
                TargetIndex = 4
            },
            new()
            {
                RoomIndex = 9,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 9,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 35,
                TargetIndex = 5
            },
            new()
            {
                RoomIndex = 9,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 9,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 38,
                TargetIndex = 9
            },
            new()
            {
                RoomIndex = 9,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 9,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 38,
                TargetIndex = 10
            },
            new()
            {
                RoomIndex = 9,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 9,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 79,
                TargetIndex = 77
            },
            new()
            {
                RoomIndex = 9,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 9,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 64,
                TargetIndex = 79
            },
            new()
            {
                RoomIndex = 35,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 35,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 53,
                TargetIndex = 51
            },
            new()
            {
                RoomIndex = 35,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 35,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 53,
                TargetIndex = 56
            },
            new()
            {
                RoomIndex = 35,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 35,
                SourceFaceType = TRMeshFaceType.TexturedTriangle,
                SourceIndex = 17,
                TargetIndex = 26
            },
            new()
            {
                RoomIndex = 35,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 35,
                SourceFaceType = TRMeshFaceType.TexturedTriangle,
                SourceIndex = 17,
                TargetIndex = 28
            },
            new()
            {
                RoomIndex = 16,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 16,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 99,
                TargetIndex = 89
            },
            new()
            {
                RoomIndex = 34,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 6,
                SourceFaceType = TRMeshFaceType.TexturedTriangle,
                SourceIndex = 4,
                TargetIndex = 6
            },
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(9, TRMeshFaceType.TexturedTriangle, 4, 2),
            Rotate(9, TRMeshFaceType.TexturedTriangle, 5, 1),
            Rotate(9, TRMeshFaceType.TexturedTriangle, 9, 1),
            Rotate(9, TRMeshFaceType.TexturedTriangle, 10, 1),
            Rotate(35, TRMeshFaceType.TexturedQuad, 51, 2),
            Rotate(35, TRMeshFaceType.TexturedQuad, 56, 2),
            Rotate(35, TRMeshFaceType.TexturedQuad, 28, 1),
            Rotate(16, TRMeshFaceType.TexturedQuad, 89, 1),
            Rotate(27, TRMeshFaceType.TexturedTriangle, 26, 2),
            Rotate(27, TRMeshFaceType.TexturedTriangle, 27, 2),
        };
    }

    private static List<TRRoomTextureMove> CreateShifts(TR1Level valley)
    {
        return new()
        {
            new()
            {
                RoomIndex = 27,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 135,
                VertexRemap = new()
                {
                    new()
                    {
                        NewVertexIndex = valley.Rooms[27].Mesh.Rectangles[134].Vertices[0]
                    },
                    new()
                    {
                        Index = 3,
                        NewVertexIndex = valley.Rooms[27].Mesh.Rectangles[123].Vertices[0]
                    }
                }
            },
            new()
            {
                RoomIndex = 51,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 165,
                VertexRemap = new()
                {
                    new()
                    {
                        NewVertexIndex = valley.Rooms[51].Mesh.Rectangles[151].Vertices[0]
                    },
                    new()
                    {
                        Index = 1,
                        NewVertexIndex = valley.Rooms[51].Mesh.Rectangles[151].Vertices[3]
                    }
                }
            }
        };
    }

    private static List<TRRoomVertexMove> CreateVertexShifts(TR1Level valley)
    {
        return new()
        {
            new()
            {
                RoomIndex = 63,
                VertexIndex = valley.Rooms[63].Mesh.Rectangles[145].Vertices[2],
                VertexChange = new() { Y = -768 }
            },
            new()
            {
                RoomIndex = 63,
                VertexIndex = valley.Rooms[63].Mesh.Rectangles[145].Vertices[3],
                VertexChange = new() { Y = -768 }
            },
        };
    }
}
