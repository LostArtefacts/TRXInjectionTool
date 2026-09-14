using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2XianTextureBuilder : TextureBuilder
{
    public override string ID => "xian_textures";

    public override List<InjectionData> Build()
    {
        TR2Level level = _control2.Read($"Resources/{TR2LevelNames.XIAN}");
        InjectionData data = CreateBaseData();

        data.RoomEdits.AddRange(CreateVertexShifts(level));
        data.RoomEdits.AddRange(CreateShifts(level));
        data.RoomEdits.AddRange(CreateFillers(level));
        data.RoomEdits.AddRange(CreateRefacings(level));
        data.RoomEdits.AddRange(CreateRotations());

        FixPassport(level, data);
        FixPushButton(data);

        return new() { data };
    }

    private static List<TRRoomVertexMove> CreateVertexShifts(TR2Level level)
    {
        return new()
        {
            new()
            {
                RoomIndex = 8,
                VertexIndex = level.Rooms[8].Mesh.Rectangles[55].Vertices[0],
                VertexChange = new() { Y = 256 },
            },
            new()
            {
                RoomIndex = 8,
                VertexIndex = level.Rooms[8].Mesh.Rectangles[55].Vertices[1],
                VertexChange = new() { Y = 256 },
            },
            new()
            {
                RoomIndex = 92,
                VertexIndex = level.Rooms[92].Mesh.Rectangles[52].Vertices[0],
                VertexChange = new() { X = 1024, Y = 3072, Z = -1024 },
            },
            new()
            {
                RoomIndex = 92,
                VertexIndex = level.Rooms[92].Mesh.Rectangles[52].Vertices[1],
                VertexChange = new() { X = 2048, Y = 4096, Z = -1024 },
            },
            new()
            {
                RoomIndex = 92,
                VertexIndex = level.Rooms[92].Mesh.Rectangles[269].Vertices[2],
                VertexChange = new() { X = -1024, Y = 3072, Z = 1024 },
            },
            new()
            {
                RoomIndex = 92,
                VertexIndex = level.Rooms[92].Mesh.Rectangles[269].Vertices[3],
                VertexChange = new() { X = -2048, Y = 4096, Z = 1024 },
            },
        };
    }

    private static List<TRRoomTextureMove> CreateShifts(TR2Level level)
    {
        return new()
        {
            new()
            {
                RoomIndex = 8,
                FaceType = TRMeshFaceType.TexturedTriangle,
                TargetIndex = 9,
                VertexRemap = new()
                {
                    new()
                    {
                        Index = 0,
                        NewVertexIndex = level.Rooms[8].Mesh.Rectangles[20].Vertices[1],
                    },
                    new()
                    {
                        Index = 1,
                        NewVertexIndex = level.Rooms[8].Mesh.Rectangles[23].Vertices[2],
                    },
                    new()
                    {
                        Index = 2,
                        NewVertexIndex = level.Rooms[8].Mesh.Rectangles[23].Vertices[1],
                    }
                }
            },
            new()
            {
                RoomIndex = 92,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 52,
                VertexRemap = new()
                {
                    new()
                    {
                        Index = 2,
                        NewVertexIndex = level.Rooms[92].Mesh.Rectangles[73].Vertices[0],
                    },
                    new()
                    {
                        Index = 3,
                        NewVertexIndex = level.Rooms[92].Mesh.Rectangles[73].Vertices[3],
                    }
                }
            },
            new()
            {
                RoomIndex = 92,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 269,
                VertexRemap = new()
                {
                    new()
                    {
                        Index = 0,
                        NewVertexIndex = level.Rooms[92].Mesh.Rectangles[270].Vertices[0],
                    },
                    new()
                    {
                        Index = 1,
                        NewVertexIndex = level.Rooms[92].Mesh.Rectangles[270].Vertices[3],
                    }
                }
            },
        };
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR2Level level)
    {
        return new()
        {
            new()
            {
                RoomIndex = 10,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 12,
                SourceIndex = 101,
                Vertices = new()
                {
                    level.Rooms[10].Mesh.Rectangles[139].Vertices[3],
                    level.Rooms[10].Mesh.Rectangles[139].Vertices[2],
                    level.Rooms[10].Mesh.Rectangles[138].Vertices[0],
                    level.Rooms[10].Mesh.Rectangles[138].Vertices[3],
                }
            },
            new()
            {
                RoomIndex = 10,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 12,
                SourceIndex = 82,
                Vertices = new()
                {
                    level.Rooms[10].Mesh.Rectangles[111].Vertices[3],
                    level.Rooms[10].Mesh.Rectangles[111].Vertices[2],
                    level.Rooms[10].Mesh.Rectangles[110].Vertices[2],
                    level.Rooms[10].Mesh.Rectangles[110].Vertices[1],
                }
            },
            new()
            {
                RoomIndex = 10,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 12,
                SourceIndex = 69,
                Vertices = new()
                {
                    level.Rooms[10].Mesh.Rectangles[87].Vertices[3],
                    level.Rooms[10].Mesh.Rectangles[87].Vertices[2],
                    level.Rooms[10].Mesh.Rectangles[86].Vertices[2],
                    level.Rooms[10].Mesh.Rectangles[86].Vertices[1],
                }
            },
            new()
            {
                RoomIndex = 10,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 12,
                SourceIndex = 58,
                Vertices = new()
                {
                    level.Rooms[10].Mesh.Rectangles[64].Vertices[3],
                    level.Rooms[10].Mesh.Rectangles[64].Vertices[2],
                    level.Rooms[10].Mesh.Rectangles[63].Vertices[2],
                    level.Rooms[10].Mesh.Rectangles[63].Vertices[1],
                }
            },
            new()
            {
                RoomIndex = 10,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 12,
                SourceIndex = 44,
                Vertices = new()
                {
                    level.Rooms[10].Mesh.Rectangles[38].Vertices[3],
                    level.Rooms[10].Mesh.Rectangles[38].Vertices[2],
                    level.Rooms[10].Mesh.Rectangles[37].Vertices[0],
                    level.Rooms[10].Mesh.Rectangles[37].Vertices[3],
                }
            },
            new()
            {
                RoomIndex = 10,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 12,
                SourceIndex = 28,
                Vertices = new()
                {
                    level.Rooms[10].Mesh.Rectangles[1].Vertices[3],
                    level.Rooms[10].Mesh.Rectangles[1].Vertices[2],
                    level.Rooms[10].Mesh.Rectangles[0].Vertices[2],
                    level.Rooms[10].Mesh.Rectangles[0].Vertices[1],
                }
            },

            new()
            {
                RoomIndex = 90,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 90,
                SourceIndex = 252,
                Vertices = new()
                {
                    level.Rooms[90].Mesh.Rectangles[334].Vertices[3],
                    level.Rooms[90].Mesh.Rectangles[334].Vertices[2],
                    level.Rooms[90].Mesh.Rectangles[341].Vertices[2],
                    level.Rooms[90].Mesh.Rectangles[341].Vertices[1],
                }
            },
            new()
            {
                RoomIndex = 90,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 90,
                SourceIndex = 252,
                Vertices = new()
                {
                    level.Rooms[90].Mesh.Rectangles[345].Vertices[3],
                    level.Rooms[90].Mesh.Rectangles[345].Vertices[2],
                    level.Rooms[90].Mesh.Rectangles[346].Vertices[1],
                    level.Rooms[90].Mesh.Rectangles[346].Vertices[0],
                }
            },
            new()
            {
                RoomIndex = 90,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 90,
                SourceIndex = 245,
                Vertices = new()
                {
                    level.Rooms[90].Mesh.Rectangles[287].Vertices[3],
                    level.Rooms[90].Mesh.Rectangles[287].Vertices[2],
                    level.Rooms[90].Mesh.Rectangles[270].Vertices[0],
                    level.Rooms[90].Mesh.Rectangles[314].Vertices[3],
                }
            },
            new()
            {
                RoomIndex = 90,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 90,
                SourceIndex = 252,
                Vertices = new()
                {
                    level.Rooms[90].Mesh.Rectangles[311].Vertices[3],
                    level.Rooms[90].Mesh.Rectangles[311].Vertices[2],
                    level.Rooms[90].Mesh.Rectangles[314].Vertices[3],
                    level.Rooms[90].Mesh.Rectangles[348].Vertices[3],
                }
            },
            new()
            {
                RoomIndex = 90,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 90,
                SourceIndex = 252,
                Vertices = new()
                {
                    level.Rooms[90].Mesh.Rectangles[63].Vertices[1],
                    level.Rooms[90].Mesh.Rectangles[63].Vertices[0],
                    level.Rooms[90].Mesh.Rectangles[65].Vertices[1],
                    level.Rooms[90].Mesh.Rectangles[65].Vertices[0],
                }
            },
            new()
            {
                RoomIndex = 90,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 90,
                SourceIndex = 252,
                Vertices = new()
                {
                    level.Rooms[90].Mesh.Rectangles[115].Vertices[1],
                    level.Rooms[90].Mesh.Rectangles[63].Vertices[1],
                    level.Rooms[90].Mesh.Rectangles[65].Vertices[0],
                    level.Rooms[90].Mesh.Rectangles[115].Vertices[2],
                }
            },
            new()
            {
                RoomIndex = 91,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 91,
                SourceIndex = 211,
                Vertices = new()
                {
                    level.Rooms[91].Mesh.Rectangles[240].Vertices[0],
                    level.Rooms[91].Mesh.Rectangles[240].Vertices[3],
                    level.Rooms[91].Mesh.Rectangles[236].Vertices[1],
                    level.Rooms[91].Mesh.Rectangles[236].Vertices[0],
                }
            },
            new()
            {
                RoomIndex = 91,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 91,
                SourceIndex = 211,
                Vertices = new()
                {
                    level.Rooms[91].Mesh.Rectangles[34].Vertices[3],
                    level.Rooms[91].Mesh.Rectangles[34].Vertices[2],
                    level.Rooms[91].Mesh.Rectangles[32].Vertices[0],
                    level.Rooms[91].Mesh.Rectangles[32].Vertices[3],
                }
            },
        };
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR2Level level)
    {
        return new()
        {
            Reface(level, 23, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1451, 117),
            Reface(level, 92, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1526, 52),
            Reface(level, 92, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1526, 269),
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(17, TRMeshFaceType.TexturedTriangle, 7, 1),
            Rotate(18, TRMeshFaceType.TexturedTriangle, 0, 2),
            Rotate(18, TRMeshFaceType.TexturedTriangle, 2, 1),
            Rotate(18, TRMeshFaceType.TexturedTriangle, 3, 1),
            Rotate(79, TRMeshFaceType.TexturedQuad, 29, 3),
            Rotate(79, TRMeshFaceType.TexturedTriangle, 0, 1),
            Rotate(79, TRMeshFaceType.TexturedTriangle, 1, 1),
            Rotate(79, TRMeshFaceType.TexturedTriangle, 3, 2),
            Rotate(92, TRMeshFaceType.TexturedQuad, 52, 3),
            Rotate(92, TRMeshFaceType.TexturedQuad, 269, 1),
            Rotate(190, TRMeshFaceType.TexturedQuad, 37, 3),
        };
    }

    private InjectionData CreateBaseData()
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.XIAN}");
        FixPlaceholderBridges(level, TR2LevelNames.XIAN, new()
        {
            [TR2Type.SceneryBase + 30] = level.StaticMeshes[TR2Type.SceneryBase + 30],
            [TR2Type.SceneryBase + 31] = level.StaticMeshes[TR2Type.SceneryBase + 31],
            [TR2Type.SceneryBase + 32] = level.StaticMeshes[TR2Type.SceneryBase + 32],
        });
        
        var data = InjectionData.Create(level, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.XIAN);

        {
            level = _control2.Read($"Resources/{TR2LevelNames.XIAN}");
            var mesh = level.Rooms[8].Mesh;
            data.RoomEdits.Add(CreateFace(8, 8, 181, TRMeshFaceType.TexturedQuad, new[]
            {
                mesh.Rectangles[11].Vertices[3], mesh.Rectangles[42].Vertices[0],
                mesh.Rectangles[42].Vertices[3], mesh.Rectangles[8].Vertices[2],                
            }));
            data.RoomEdits.Add(CreateFace(8, 8, 181, TRMeshFaceType.TexturedQuad, new[]
            {
                mesh.Rectangles[181].Vertices[3], mesh.Rectangles[179].Vertices[0],
                mesh.Rectangles[179].Vertices[3], mesh.Rectangles[181].Vertices[0],
            }));

            mesh = level.Rooms[38].Mesh;
            data.RoomEdits.Add(CreateFace(38, 8, 181, TRMeshFaceType.TexturedQuad, new[]
            {
                mesh.Rectangles[38].Vertices[0], mesh.Rectangles[38].Vertices[3],
                mesh.Rectangles[62].Vertices[1], mesh.Rectangles[13].Vertices[2],
            }));
            data.RoomEdits.Add(CreateFace(38, 8, 181, TRMeshFaceType.TexturedQuad, new[]
            {
                mesh.Rectangles[22].Vertices[3], mesh.Rectangles[22].Vertices[2],
                mesh.Rectangles[2].Vertices[3], mesh.Triangles[4].Vertices[1],
            }));
        }

        return data;
    }
}
