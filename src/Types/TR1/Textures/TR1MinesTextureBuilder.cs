using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1MinesTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level mines = _control1.Read($"Resources/{TR1LevelNames.MINES}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.TextureFix, "mines_textures");
        CreateDefaultTests(data, TR1LevelNames.MINES);

        data.RoomEdits.AddRange(CreateFillers(mines));
        data.RoomEdits.AddRange(CreateRefacings());
        data.RoomEdits.AddRange(CreateRotations());
        data.RoomEdits.AddRange(CreateShifts(mines));

        FixEnemyTextures(data);

        return new() { data };
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR1Level mines)
    {
        return new()
        {
            new()
            {
                RoomIndex = 35,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 35,
                SourceIndex = 2,
                Vertices = new()
                {
                    mines.Rooms[35].Mesh.Rectangles[4].Vertices[1],
                    mines.Rooms[35].Mesh.Rectangles[0].Vertices[0],
                    mines.Rooms[35].Mesh.Rectangles[0].Vertices[3],
                }
            },
        };
    }

    private static List<TRRoomTextureReface> CreateRefacings()
    {
        return new()
        {
            new()
            {
                RoomIndex = 69,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 69,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 172,
                TargetIndex = 174
            },
            new()
            {
                RoomIndex = 23,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 23,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 99,
                TargetIndex = 89
            },
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(23, TRMeshFaceType.TexturedTriangle, 8, 2),
            Rotate(23, TRMeshFaceType.TexturedTriangle, 6, 2),
            Rotate(24, TRMeshFaceType.TexturedTriangle, 0, 1),
        };
    }

    private static List<TRRoomTextureMove> CreateShifts(TR1Level mines)
    {
        return new()
        {
            new()
            {
                RoomIndex = 55,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 8,
                VertexRemap = new()
                {
                    new()
                    {
                        NewVertexIndex = mines.Rooms[55].Mesh.Rectangles[9].Vertices[3],
                    },
                    new()
                    {
                        Index = 1,
                        NewVertexIndex = mines.Rooms[55].Mesh.Rectangles[9].Vertices[2],
                    },
                }
            }
        };
    }

    private static void FixEnemyTextures(InjectionData data)
    {
        data.Palette.Add(new());
        // Skin tone for skatebaord kid and cowboy
        data.Palette.Add(new()
        {
            Red = 204,
            Green = 132,
            Blue = 88,
        });
        // Skin tone for baldy
        data.Palette.Add(new()
        {
            Red = 112,
            Green = 72,
            Blue = 16,
        });

        const short tone1 = -1;
        const short tone2 = -2;

        {
            // Skateboard kid
            short[] meshIndices = new short[] { 1, 3, 4, 7, };
            foreach (short meshIndex in meshIndices)
            {
                data.MeshEdits.Add(new()
                {
                    ModelID = (uint)TR1Type.SkateboardKid,
                    MeshIndex = meshIndex,
                    FaceEdits = new()
                    {
                        new()
                        {
                            FaceType = TRMeshFaceType.ColouredQuad,
                            MeshIndex = tone1,
                            TargetFaceIndices = new() { 0 },
                        }
                   },
                });
            }
        }

        {
            // Cowboy
            short[] meshIndices = new short[] { 4, 7, };
            foreach (short meshIndex in meshIndices)
            {
                data.MeshEdits.Add(new()
                {
                    ModelID = (uint)TR1Type.Cowboy,
                    MeshIndex = meshIndex,
                    FaceEdits = new()
                    {
                        new()
                        {
                            FaceType = TRMeshFaceType.ColouredQuad,
                            MeshIndex = tone1,
                            TargetFaceIndices = new() { 0 },
                        }
                    },
                });
            }
        }

        {
            // Baldy
            data.MeshEdits.AddRange(new List<TRMeshEdit>
            {
                new()
                {
                    ModelID = (uint)TR1Type.Kold,
                    MeshIndex = 1,
                    FaceEdits = new()
                    {
                        new()
                        {
                            FaceType = TRMeshFaceType.ColouredQuad,
                            MeshIndex = tone2,
                            TargetFaceIndices = GetRange(10, 3),
                        },
                        new()
                        {
                            FaceType = TRMeshFaceType.ColouredTriangle,
                            MeshIndex = tone2,
                            TargetFaceIndices = GetRange(4, 4),
                        },
                    },
                },

                new()
                {
                    ModelID = (uint)TR1Type.Kold,
                    MeshIndex = 2,
                    FaceEdits = new()
                    {
                        new()
                        {
                            FaceType = TRMeshFaceType.ColouredQuad,
                            MeshIndex = tone2,
                            TargetFaceIndices = GetRange(0, 8),
                        },
                        new()
                        {
                            FaceType = TRMeshFaceType.ColouredTriangle,
                            MeshIndex = tone2,
                            TargetFaceIndices = GetRange(0, 15),
                        },
                    },
                },

                new()
                {
                    ModelID = (uint)TR1Type.Kold,
                    MeshIndex = 5,
                    FaceEdits = new()
                    {
                        new()
                        {
                            FaceType = TRMeshFaceType.ColouredQuad,
                            MeshIndex = tone2,
                            TargetFaceIndices = GetRange(0, 6),
                        },
                    },
                },

                new()
                {
                    ModelID = (uint)TR1Type.Kold,
                    MeshIndex = 8,
                    FaceEdits = new()
                    {
                        new()
                        {
                            FaceType = TRMeshFaceType.ColouredQuad,
                            MeshIndex = tone2,
                            TargetFaceIndices = GetRange(0, 6),
                        },
                    },
                },
            });
        }
    }
}
