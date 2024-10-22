using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1MidasTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level midas = _control1.Read($"Resources/{TR1LevelNames.MIDAS}");
        InjectionData data = InjectionData.Create(InjectionType.TextureFix, "midas_textures");

        data.RoomEdits.AddRange(CreateFillers(midas));
        data.RoomEdits.AddRange(CreateRefacings());
        data.RoomEdits.AddRange(CreateRotations());
        data.RoomEdits.AddRange(CreateShifts(midas));

        FixRoom13(midas, data);

        return new() { data };
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR1Level midas)
    {
        return new()
        {
            new()
            {
                RoomIndex = 2,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 2,
                SourceIndex = 49,
                Vertices = new()
                {
                    midas.Rooms[2].Mesh.Rectangles[40].Vertices[0],
                    midas.Rooms[2].Mesh.Rectangles[40].Vertices[3],
                    midas.Rooms[2].Mesh.Rectangles[46].Vertices[2],
                    midas.Rooms[2].Mesh.Rectangles[46].Vertices[1],
                }
            },
            new()
            {
                RoomIndex = 2,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 2,
                SourceIndex = 49,
                Vertices = new()
                {
                    midas.Rooms[2].Mesh.Rectangles[48].Vertices[3],
                    midas.Rooms[2].Mesh.Rectangles[48].Vertices[2],
                    midas.Rooms[2].Mesh.Rectangles[46].Vertices[1],
                    midas.Rooms[2].Mesh.Rectangles[46].Vertices[0],
                }
            },
            new()
            {
                RoomIndex = 53,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = GetSource(midas, TRMeshFaceType.TexturedTriangle, 7).Room,
                SourceIndex = GetSource(midas, TRMeshFaceType.TexturedTriangle, 7).Face,
                Vertices = new()
                {
                    midas.Rooms[53].Mesh.Rectangles[3].Vertices[1],
                    midas.Rooms[53].Mesh.Rectangles[0].Vertices[3],
                    midas.Rooms[53].Mesh.Rectangles[3].Vertices[2],
                }
            }
        };
    }

    private static List<TRRoomTextureReface> CreateRefacings()
    {
        List<TRRoomTextureReface> edits = new()
        {
            new()
            {
                RoomIndex = 45,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 45,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 138,
                TargetIndex = 189
            },
            new()
            {
                RoomIndex = 34,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 34,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 12,
                TargetIndex = 32
            }
        };

        short[] room40Roof = new short[] { 61, 64, 67, 70, 58, 52, 44, 41, 38, 34, 47, 55 };
        foreach (short roof in room40Roof)
        {
            edits.Add(new TRRoomTextureReface
            {
                RoomIndex = 40,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 38,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 0,
                TargetIndex = roof
            });
        }

        return edits;
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(31, TRMeshFaceType.TexturedQuad, 57, 2),
            Rotate(31, TRMeshFaceType.TexturedQuad, 115, 2),
            Rotate(31, TRMeshFaceType.TexturedQuad, 277, 2),
            Rotate(7, TRMeshFaceType.TexturedTriangle, 32, 2),
            Rotate(20, TRMeshFaceType.TexturedTriangle, 0, 2),
        };
    }

    private static List<TRRoomTextureMove> CreateShifts(TR1Level midas)
    {
        return new()
        {
            new()
            {
                RoomIndex = 5,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 55,
                VertexRemap = new()
                {
                    new()
                    {
                        Index = 0,
                        NewVertexIndex = midas.Rooms[5].Mesh.Rectangles[53].Vertices[2],
                    },
                    new()
                    {
                        Index = 1,
                        NewVertexIndex = midas.Rooms[5].Mesh.Rectangles[53].Vertices[1],
                    },
                }
            },
            new()
            {
                RoomIndex = 30,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 171,
                VertexRemap = new()
                {
                    new()
                    {
                        Index = 0,
                        NewVertexIndex = midas.Rooms[30].Mesh.Rectangles[169].Vertices[3],
                    },
                    new()
                    {
                        Index = 1,
                        NewVertexIndex = midas.Rooms[30].Mesh.Rectangles[169].Vertices[2],
                    },
                }
            },
            new()
            {
                RoomIndex = 30,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 167,
                VertexRemap = new()
                {
                    new()
                    {
                        Index = 0,
                        NewVertexIndex = midas.Rooms[30].Mesh.Rectangles[133].Vertices[0],
                    },
                    new()
                    {
                        Index = 1,
                        NewVertexIndex = midas.Rooms[30].Mesh.Rectangles[133].Vertices[3],
                    },
                }
            }
        };
    }

    private static void FixRoom13(TR1Level midas, InjectionData data)
    {
        // The roof in room 13 near the portal into room 9 is a big mess, so these edits
        // are grouped together for easier reference.

        data.RoomEdits.Add(new TRRoomTextureMove
        {
            RoomIndex = 13,
            FaceType = TRMeshFaceType.TexturedQuad,
            TargetIndex = 61,
            VertexRemap = new()
            {
                new()
                {
                    Index = 2,
                    NewVertexIndex = midas.Rooms[13].Mesh.Rectangles[56].Vertices[1]
                },
                new()
                {
                    Index = 3,
                    NewVertexIndex = midas.Rooms[13].Mesh.Rectangles[56].Vertices[0]
                }
            }
        });

        data.RoomEdits.Add(new TRRoomTextureMove
        {
            RoomIndex = 13,
            FaceType = TRMeshFaceType.TexturedQuad,
            TargetIndex = 105,
            VertexRemap = new()
            {
                new()
                {
                    Index = 2,
                    NewVertexIndex = midas.Rooms[13].Mesh.Rectangles[102].Vertices[1]
                },
            }
        });

        for (int i = 0; i < 2; i++)
        {
            data.RoomEdits.Add(new TRRoomVertexMove
            {
                RoomIndex = 13,
                VertexIndex = midas.Rooms[13].Mesh.Rectangles[61].Vertices[i],
                VertexChange = new() { Y = -256 }
            });
            data.RoomEdits.Add(new TRRoomVertexMove
            {
                RoomIndex = 13,
                VertexIndex = midas.Rooms[13].Mesh.Rectangles[65].Vertices[i],
                VertexChange = new() { Y = -256 }
            });
            data.RoomEdits.Add(new TRRoomVertexMove
            {
                RoomIndex = 13,
                VertexIndex = midas.Rooms[13].Mesh.Rectangles[67].Vertices[i],
                VertexChange = new() { Y = -256 }
            });
        }

        data.RoomEdits.Add(new TRRoomVertexMove
        {
            RoomIndex = 13,
            VertexIndex = midas.Rooms[13].Mesh.Rectangles[63].Vertices[3],
            VertexChange = new() { Y = 256 }
        });
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 13,
            FaceType = TRMeshFaceType.TexturedTriangle,
            SourceRoom = 13,
            SourceIndex = 0,
            Vertices = new()
            {
                midas.Rooms[13].Mesh.Rectangles[57].Vertices[1],
                midas.Rooms[13].Mesh.Rectangles[61].Vertices[1],
                midas.Rooms[13].Mesh.Rectangles[63].Vertices[3],
            }
        });

        data.RoomEdits.Add(new TRRoomTextureMove
        {
            RoomIndex = 13,
            FaceType = TRMeshFaceType.TexturedQuad,
            TargetIndex = 63,
            VertexRemap = new()
            {
                new()
                {
                    Index = 0,
                    NewVertexIndex = midas.Rooms[13].Mesh.Rectangles[63].Vertices[3]
                },
                new()
                {
                    Index = 1,
                    NewVertexIndex = midas.Rooms[13].Mesh.Rectangles[63].Vertices[3]
                },
                new()
                {
                    Index = 2,
                    NewVertexIndex = midas.Rooms[13].Mesh.Rectangles[63].Vertices[3]
                },
            }
        });

        data.RoomEdits.Add(new TRRoomVertexMove
        {
            RoomIndex = 13,
            VertexIndex = midas.Rooms[13].Mesh.Triangles[0].Vertices[2],
            VertexChange = new()
            {
                X = -1024,
                Y = 256,
                Z = 2048
            }
        });

        data.RoomEdits.Add(new TRRoomTextureMove
        {
            RoomIndex = 13,
            FaceType = TRMeshFaceType.TexturedTriangle,
            TargetIndex = 0,
            VertexRemap = new()
            {
                new()
                {
                    Index = 0,
                    NewVertexIndex = midas.Rooms[13].Mesh.Rectangles[61].Vertices[1],
                },
                new()
                {
                    Index = 1,
                    NewVertexIndex = midas.Rooms[13].Mesh.Rectangles[68].Vertices[0],
                },
            }
        });

        data.RoomEdits.Add(new TRRoomTextureRotate
        {
            RoomIndex = 13,
            FaceType = TRMeshFaceType.TexturedTriangle,
            TargetIndex = 0,
            Rotations = 2
        });

        data.VisPortalEdits.Add(new TRVisPortalEdit
        {
            BaseRoom = 9,
            LinkRoom = 13,
            PortalIndex = 0,
            VertexChanges = new()
            {
                new() { Y = -256 },
                new() { },
                new() { },
                new() { Y = -256 },
            }
        });

        data.VisPortalEdits.Add(new TRVisPortalEdit
        {
            BaseRoom = 13,
            LinkRoom = 9,
            PortalIndex = 1,
            VertexChanges = new()
            {
                new() { },
                new() { Y = -256 },
                new() { Y = -256 },
                new() { },
            }
        });
    }
}
