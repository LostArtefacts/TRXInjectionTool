using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1PyramidTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level pyramid = _control1.Read($"Resources/{TR1LevelNames.PYRAMID}");
        InjectionData data = InjectionData.Create(InjectionType.TextureFix, "pyramid_textures");

        data.RoomEdits.AddRange(CreateFillers(pyramid));
        data.RoomEdits.AddRange(CreateRefacings());
        data.RoomEdits.AddRange(CreateRotations());

        FixRoom25(pyramid, data);

        return new() { data };
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR1Level pyramid)
    {
        return new()
        {
            new()
            {
                RoomIndex = 66,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 66,
                SourceIndex = 20,
                Vertices = new()
                {
                    pyramid.Rooms[66].Mesh.Rectangles[14].Vertices[1],
                    pyramid.Rooms[66].Mesh.Rectangles[3].Vertices[0],
                    pyramid.Rooms[66].Mesh.Rectangles[3].Vertices[3],
                    pyramid.Rooms[66].Mesh.Rectangles[14].Vertices[2],
                }
            },
            new()
            {
                RoomIndex = 21,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 22,
                Vertices = new()
                {
                    pyramid.Rooms[21].Mesh.Rectangles[24].Vertices[3],
                    pyramid.Rooms[21].Mesh.Rectangles[22].Vertices[0],
                    pyramid.Rooms[21].Mesh.Rectangles[22].Vertices[3],
                    pyramid.Rooms[21].Mesh.Rectangles[58].Vertices[1],
                }
            },
            new()
            {
                RoomIndex = 21,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 18,
                Vertices = new()
                {
                    pyramid.Rooms[21].Mesh.Rectangles[22].Vertices[0],
                    pyramid.Rooms[21].Mesh.Rectangles[24].Vertices[3],
                    pyramid.Rooms[21].Mesh.Rectangles[16].Vertices[0],
                    pyramid.Rooms[21].Mesh.Rectangles[16].Vertices[3],
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
                RoomIndex = 2,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 2,
                SourceIndex = 89,
                TargetIndex = 65,
            },
            new()
            {
                RoomIndex = 5,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 5,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 71,
                TargetIndex = 100
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
                RoomIndex = 66,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 66,
                SourceIndex = 104,
                TargetIndex = 92,
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
                TargetIndex = 5
            },
            new()
            {
                RoomIndex = 52,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 51,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 1,
                TargetIndex = 8
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
                TargetIndex = 48
            },
            new()
            {
                RoomIndex = 54,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 54,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 239,
                TargetIndex = 255
            },
            new()
            {
                RoomIndex = 54,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 54,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 204,
                TargetIndex = 228
            }
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(50, TRMeshFaceType.TexturedQuad, 43, 1),
            Rotate(65, TRMeshFaceType.TexturedQuad, 43, 1),
            Rotate(5, TRMeshFaceType.TexturedQuad, 113, 3),
            Rotate(5, TRMeshFaceType.TexturedQuad, 100, 1),
            Rotate(31, TRMeshFaceType.TexturedQuad, 70, 2),
            Rotate(49, TRMeshFaceType.TexturedTriangle, 10, 1),
            Rotate(50, TRMeshFaceType.TexturedTriangle, 6, 2),
            Rotate(54, TRMeshFaceType.TexturedQuad, 255, 1),
            Rotate(54, TRMeshFaceType.TexturedQuad, 228, 3),
        };
    }

    private static void FixRoom25(TR1Level pyramid, InjectionData data)
    {
        TR1RoomVertex copy = pyramid.Rooms[25].Mesh.Vertices[pyramid.Rooms[25].Mesh.Rectangles[91].Vertices[0]];
        data.RoomEdits.Add(new TRRoomVertexCreate
        {
            RoomIndex = 25,
            Vertex = new()
            {
                Lighting = copy.Lighting,
                Vertex = new()
                {
                    X = copy.Vertex.X,
                    Y = (short)(copy.Vertex.Y - 512),
                    Z = copy.Vertex.Z
                }
            }
        });

        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 25,
            FaceType = TRMeshFaceType.TexturedTriangle,
            SourceRoom = GetSource(pyramid, TRMeshFaceType.TexturedTriangle, 60).Room,
            SourceIndex = GetSource(pyramid, TRMeshFaceType.TexturedTriangle, 60).Face,
            Vertices = new()
            {
                pyramid.Rooms[25].Mesh.Rectangles[105].Vertices[3],
                (ushort)pyramid.Rooms[25].Mesh.Vertices.Count,
                pyramid.Rooms[25].Mesh.Rectangles[105].Vertices[0],
            }
        });

        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 25,
            FaceType = TRMeshFaceType.TexturedTriangle,
            SourceRoom = GetSource(pyramid, TRMeshFaceType.TexturedTriangle, 60).Room,
            SourceIndex = GetSource(pyramid, TRMeshFaceType.TexturedTriangle, 60).Face,
            Vertices = new()
            {
                pyramid.Rooms[25].Mesh.Rectangles[91].Vertices[0],
                pyramid.Rooms[25].Mesh.Rectangles[105].Vertices[0],
                (ushort)pyramid.Rooms[25].Mesh.Vertices.Count,
            }
        });
    }
}
