using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1AtlantisTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level atlantis = _control1.Read($"Resources/{TR1LevelNames.ATLANTIS}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.TextureFix, "atlantis_textures");

        data.RoomEdits.AddRange(CreateFillers(atlantis));
        data.RoomEdits.AddRange(CreateRefacings());
        data.RoomEdits.AddRange(CreateRotations());
        data.RoomEdits.AddRange(CreateShifts(atlantis));

        return new() { data };
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR1Level atlantis)
    {
        return new()
        {
            new()
            {
                RoomIndex = 87,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 87,
                SourceIndex = 4,
                Vertices = new()
                {
                    atlantis.Rooms[87].Mesh.Rectangles[20].Vertices[1],
                    atlantis.Rooms[87].Mesh.Rectangles[12].Vertices[0],
                    atlantis.Rooms[87].Mesh.Rectangles[12].Vertices[3],
                    atlantis.Rooms[87].Mesh.Rectangles[20].Vertices[2],
                }
            },
            new()
            {
                RoomIndex = 87,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = GetSource(atlantis, TRMeshFaceType.TexturedQuad, 82).Room,
                SourceIndex = GetSource(atlantis, TRMeshFaceType.TexturedQuad, 82).Face,
                Vertices = new()
                {
                    atlantis.Rooms[87].Mesh.Rectangles[15].Vertices[0],
                    atlantis.Rooms[87].Mesh.Rectangles[15].Vertices[1],
                    atlantis.Rooms[87].Mesh.Rectangles[20].Vertices[2],
                    atlantis.Rooms[87].Mesh.Rectangles[12].Vertices[3],
                }
            },
            new()
            {
                RoomIndex = 58,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 87,
                SourceIndex = 4,
                Vertices = new()
                {
                    atlantis.Rooms[58].Mesh.Rectangles[20].Vertices[1],
                    atlantis.Rooms[58].Mesh.Rectangles[12].Vertices[0],
                    atlantis.Rooms[58].Mesh.Rectangles[12].Vertices[3],
                    atlantis.Rooms[58].Mesh.Rectangles[20].Vertices[2],
                }
            },
            new()
            {
                RoomIndex = 58,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = GetSource(atlantis, TRMeshFaceType.TexturedQuad, 82).Room,
                SourceIndex = GetSource(atlantis, TRMeshFaceType.TexturedQuad, 82).Face,
                Vertices = new()
                {
                    atlantis.Rooms[58].Mesh.Rectangles[15].Vertices[0],
                    atlantis.Rooms[58].Mesh.Rectangles[15].Vertices[1],
                    atlantis.Rooms[58].Mesh.Rectangles[20].Vertices[2],
                    atlantis.Rooms[58].Mesh.Rectangles[12].Vertices[3],
                }
            }
        };
    }

    private static List<TRRoomTextureReface> CreateRefacings()
    {
        return new()
        {
            new()
            {
                RoomIndex = 85,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 12,
                SourceIndex = 20,
                TargetIndex = 19,
            },
            new()
            {
                RoomIndex = 5,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 5,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 71,
                TargetIndex = 102
            },
            new()
            {
                RoomIndex = 18,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 18,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 106,
                TargetIndex = 61
            },
            new()
            {
                RoomIndex = 43,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 18,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 106,
                TargetIndex = 62
            },
            new()
            {
                RoomIndex = 78,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 78,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 18,
                TargetIndex = 24
            },
            new()
            {
                RoomIndex = 5,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 5,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 25,
                TargetIndex = 28
            },
            new()
            {
                RoomIndex = 5,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 5,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 22,
                TargetIndex = 25
            },
            new()
            {
                RoomIndex = 5,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 5,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 19,
                TargetIndex = 22
            },
            new()
            {
                RoomIndex = 5,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 5,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 16,
                TargetIndex = 19
            },
            new()
            {
                RoomIndex = 5,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 5,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 13,
                TargetIndex = 16
            },
            new()
            {
                RoomIndex = 5,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 5,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 10,
                TargetIndex = 13
            },
            new()
            {
                RoomIndex = 5,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 5,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 7,
                TargetIndex = 10
            },
            new()
            {
                RoomIndex = 5,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 5,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 2,
                TargetIndex = 7
            },
            new()
            {
                RoomIndex = 5,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 5,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 4,
                TargetIndex = 2
            },
            new()
            {
                RoomIndex = 52,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 51,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 8,
                TargetIndex = 1
            },
            new()
            {
                RoomIndex = 52,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 51,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 5,
                TargetIndex = 4
            },
            new()
            {
                RoomIndex = 52,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 51,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 1,
                TargetIndex = 6
            },
            new()
            {
                RoomIndex = 36,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 36,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 18,
                TargetIndex = 24
            },
            new()
            {
                RoomIndex = 53,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 53,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 64,
                TargetIndex = 47
            },
            new()
            {
                RoomIndex = 54,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 54,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 232,
                TargetIndex = 248
            },
            new()
            {
                RoomIndex = 54,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 54,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 201,
                TargetIndex = 222
            }
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(5, TRMeshFaceType.TexturedQuad, 115, 3),
            Rotate(5, TRMeshFaceType.TexturedQuad, 102, 1),
            Rotate(78, TRMeshFaceType.TexturedQuad, 24, 3),
            Rotate(58, TRMeshFaceType.TexturedQuad, 30, 2),
            Rotate(87, TRMeshFaceType.TexturedQuad, 30, 2),
            Rotate(50, TRMeshFaceType.TexturedQuad, 43, 1),
            Rotate(49, TRMeshFaceType.TexturedTriangle, 10, 1),
            Rotate(54, TRMeshFaceType.TexturedQuad, 248, 1),
            Rotate(54, TRMeshFaceType.TexturedQuad, 222, 3),
            Rotate(50, TRMeshFaceType.TexturedTriangle, 7, 2),
            Rotate(13, TRMeshFaceType.TexturedTriangle, 2, 2),
        };
    }

    private static List<TRRoomTextureMove> CreateShifts(TR1Level atlantis)
    {
        return new()
        {
            new()
            {
                RoomIndex = 27,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 14,
                VertexRemap = new()
                {
                    new()
                    {
                        NewVertexIndex = atlantis.Rooms[27].Mesh.Rectangles[13].Vertices[2]
                    },
                    new()
                    {
                        Index = 1,
                        NewVertexIndex = atlantis.Rooms[27].Mesh.Rectangles[13].Vertices[1]
                    }
                }
            },
            new()
            {
                RoomIndex = 87,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 15,
                VertexRemap = new()
                {
                    new()
                    {
                        NewVertexIndex = atlantis.Rooms[87].Mesh.Rectangles[12].Vertices[0]
                    },
                    new()
                    {
                        Index = 1,
                        NewVertexIndex = atlantis.Rooms[87].Mesh.Rectangles[18].Vertices[0]
                    }
                }
            },
            new()
            {
                RoomIndex = 58,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 15,
                VertexRemap = new()
                {
                    new()
                    {
                        NewVertexIndex = atlantis.Rooms[58].Mesh.Rectangles[12].Vertices[0]
                    },
                    new()
                    {
                        Index = 1,
                        NewVertexIndex = atlantis.Rooms[58].Mesh.Rectangles[18].Vertices[0]
                    }
                }
            }
        };
    }
}
