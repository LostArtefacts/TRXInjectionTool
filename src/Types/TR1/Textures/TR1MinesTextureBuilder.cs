using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public partial class TR1MinesTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level mines = _control1.Read($"Resources/{TR1LevelNames.MINES}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.TextureFix, "mines_textures");
        CreateDefaultTests(data, TR1LevelNames.MINES);

        data.RoomEdits.AddRange(CreateFillers(mines));
        data.RoomEdits.AddRange(CreateRefacings(mines));
        data.RoomEdits.AddRange(CreateRotations());
        data.RoomEdits.AddRange(CreateShifts(mines));

        FixCabinRoom(mines, data);
        FixEnemyTextures(data);
        FixPassport(mines, data);

        return [data];
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR1Level mines)
    {
        return
        [
            new()
            {
                RoomIndex = 35,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 35,
                SourceIndex = 2,
                Vertices =
                [
                    mines.Rooms[35].Mesh.Rectangles[4].Vertices[1],
                    mines.Rooms[35].Mesh.Rectangles[0].Vertices[0],
                    mines.Rooms[35].Mesh.Rectangles[0].Vertices[3],
                ]
            },
        ];
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR1Level mines)
    {
        return
        [
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
            Reface(mines, 13, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 3, 29),
            Reface(mines, 24, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1, 140),
            Reface(mines, 47, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 25, 24),
            Reface(mines, 84, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 2, 50),
            Reface(mines, 87, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 157, 10),
            Reface(mines, 88, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 157, 10),
            Reface(mines, 87, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 87, 12),
            Reface(mines, 88, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 87, 12),
        ];
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return
        [
            Rotate(23, TRMeshFaceType.TexturedTriangle, 8, 2),
            Rotate(23, TRMeshFaceType.TexturedTriangle, 6, 2),
            Rotate(24, TRMeshFaceType.TexturedTriangle, 0, 1),
            Rotate(29, TRMeshFaceType.TexturedQuad, 140, 3),
            Rotate(30, TRMeshFaceType.TexturedQuad, 134, 3),
            Rotate(31, TRMeshFaceType.TexturedQuad, 178, 3),
            Rotate(31, TRMeshFaceType.TexturedTriangle, 2, 2),
            Rotate(47, TRMeshFaceType.TexturedQuad, 24, 3),
            Rotate(47, TRMeshFaceType.TexturedTriangle, 0, 1),
            Rotate(87, TRMeshFaceType.TexturedTriangle, 10, 2),
            Rotate(87, TRMeshFaceType.TexturedTriangle, 13, 2),
            Rotate(88, TRMeshFaceType.TexturedTriangle, 10, 2),
            Rotate(88, TRMeshFaceType.TexturedTriangle, 13, 2),
            Rotate(92, TRMeshFaceType.TexturedTriangle, 1, 2),
            Rotate(93, TRMeshFaceType.TexturedTriangle, 0, 2),
            Rotate(97, TRMeshFaceType.TexturedTriangle, 0, 2),
            Rotate(98, TRMeshFaceType.TexturedTriangle, 1, 2),
        ];
    }

    private static List<TRRoomTextureMove> CreateShifts(TR1Level mines)
    {
        return
        [
            new()
            {
                RoomIndex = 55,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 8,
                VertexRemap =
                [
                    new()
                    {
                        NewVertexIndex = mines.Rooms[55].Mesh.Rectangles[9].Vertices[3],
                    },
                    new()
                    {
                        Index = 1,
                        NewVertexIndex = mines.Rooms[55].Mesh.Rectangles[9].Vertices[2],
                    },
                ]
            }
        ];
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
            short[] meshIndices = [1, 3, 4, 7,];
            foreach (short meshIndex in meshIndices)
            {
                data.MeshEdits.Add(new()
                {
                    ModelID = (uint)TR1Type.SkateboardKid,
                    MeshIndex = meshIndex,
                    FaceEdits =
                    [
                        new()
                        {
                            FaceType = TRMeshFaceType.ColouredQuad,
                            MeshIndex = tone1,
                            TargetFaceIndices = [0],
                        }
                   ],
                });
            }
        }

        {
            // Cowboy
            short[] meshIndices = [4, 7,];
            foreach (short meshIndex in meshIndices)
            {
                data.MeshEdits.Add(new()
                {
                    ModelID = (uint)TR1Type.Cowboy,
                    MeshIndex = meshIndex,
                    FaceEdits =
                    [
                        new()
                        {
                            FaceType = TRMeshFaceType.ColouredQuad,
                            MeshIndex = tone1,
                            TargetFaceIndices = [0],
                        }
                    ],
                });
            }
        }

        {
            // Baldy
            data.MeshEdits.AddRange(
            [
                new()
                {
                    ModelID = (uint)TR1Type.Kold,
                    MeshIndex = 1,
                    FaceEdits =
                    [
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
                    ],
                },

                new()
                {
                    ModelID = (uint)TR1Type.Kold,
                    MeshIndex = 2,
                    FaceEdits =
                    [
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
                    ],
                },

                new()
                {
                    ModelID = (uint)TR1Type.Kold,
                    MeshIndex = 5,
                    FaceEdits =
                    [
                        new()
                        {
                            FaceType = TRMeshFaceType.ColouredQuad,
                            MeshIndex = tone2,
                            TargetFaceIndices = GetRange(0, 6),
                        },
                    ],
                },

                new()
                {
                    ModelID = (uint)TR1Type.Kold,
                    MeshIndex = 8,
                    FaceEdits =
                    [
                        new()
                        {
                            FaceType = TRMeshFaceType.ColouredQuad,
                            MeshIndex = tone2,
                            TargetFaceIndices = GetRange(0, 6),
                        },
                    ],
                },
            ]);
        }
    }
}
