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
        InjectionData data = CreateBaseData();
        CreateDefaultTests(data, TR1LevelNames.ATLANTIS);

        data.RoomEdits.AddRange(CreateFillers(atlantis));
        data.RoomEdits.AddRange(CreateRefacings(atlantis));
        data.RoomEdits.AddRange(CreateRotations());
        data.RoomEdits.AddRange(CreateShifts(atlantis));
        FixPlatformDoorArea(data, atlantis);
        FixPassport(atlantis, data);

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

    private static List<TRRoomTextureReface> CreateRefacings(TR1Level atlantis)
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
            },
            Reface(atlantis, 88, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 8, 10),
            Reface(atlantis, 90, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 8, 6),
            Reface(atlantis, 62, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 8, 7),
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

    private static void FixPlatformDoorArea(InjectionData data, TR1Level atlantis)
    {
        var mesh = atlantis.Rooms[36].Mesh;
        data.RoomEdits.AddRange(new[] { 0, 1 }.Select(i =>
        {
            return new TRRoomVertexMove
            {
                RoomIndex = 36,
                VertexIndex = mesh.Rectangles[40].Vertices[i],
                VertexChange = new() { Y = 256 },
            };
        }));
        data.RoomEdits.AddRange(new[] { 0, 1 }.Select(i =>
        {
            return new TRRoomVertexCreate
            {
                RoomIndex = 36,
                Vertex = mesh.Vertices[mesh.Rectangles[40].Vertices[i]],
            };
        }));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            FaceType = TRMeshFaceType.TexturedQuad,
            RoomIndex = 36,
            SourceRoom = 36,
            SourceIndex = 45,
            Vertices = new()
            {
                (ushort)mesh.Vertices.Count,
                (ushort)(mesh.Vertices.Count + 1),
                mesh.Rectangles[40].Vertices[1],
                mesh.Rectangles[40].Vertices[0],
            },
        });
        data.RoomEdits.Add(Reface(atlantis, 36, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 12, 40));
        data.RoomEdits.Add(Reface(atlantis, 36, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 134, 39));

        data.RoomEdits.Add(new TRRoomTextureMove
        {
            FaceType = TRMeshFaceType.TexturedQuad,
            RoomIndex = 36,
            TargetIndex = 45,
            VertexRemap = new()
            {
                new()
                {
                    Index = 1,
                    NewVertexIndex = (ushort)mesh.Vertices.Count,
                }
            },
        });
        data.RoomEdits.Add(new TRRoomTextureMove
        {
            FaceType = TRMeshFaceType.TexturedQuad,
            RoomIndex = 36,
            TargetIndex = 34,
            VertexRemap = new()
            {
                new()
                {
                    Index = 0,
                    NewVertexIndex = (ushort)(mesh.Vertices.Count + 1),
                }
            },
        });
    }

    private static InjectionData CreateBaseData()
    {
        TR1Level baseLevel = CreateAtlantisContinuityLevel(TR1Type.SceneryBase + 17);
        InjectionData data = InjectionData.Create(baseLevel, InjectionType.TextureFix, "atlantis_textures");
        data.RoomEdits.Add(new TRRoomStatic3DCreate
        {
            ID = 17,
            RoomIndex = 50,
            StaticMesh = new()
            {
                X = 52736,
                Y = -20404,
                Z = 45568,
                Intensity = 6000,
            }
        });

        data.RoomEdits.Add(new TRRoomStatic3DCreate
        {
            ID = 18,
            RoomIndex = 48,
            StaticMesh = new()
            {
                X = 65024 - 512,
                Y = -15616,
                Z = 45568 - 512,
                Angle = 16384,
                Intensity = 4096,
            }
        });

        data.ItemEdits.Add(new()
        {
            Index = 69,
            Item = new()
            {
                Angle = -16384,
                X = 64000 + 64,
                Y = -15616,
                Z = 45568,
                Room = 48,
            },
        });

        return data;
    }
}
