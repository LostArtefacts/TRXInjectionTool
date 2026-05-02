using System.Diagnostics;
using TRImageControl;
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
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.TextureFix, "midas_textures");
        CreateDefaultTests(data, TR1LevelNames.MIDAS);

        data.RoomEdits.AddRange(CreateFillers(midas));
        data.RoomEdits.AddRange(CreateRefacings());
        data.RoomEdits.AddRange(CreateRotations());
        data.RoomEdits.AddRange(CreateShifts(midas));

        FixArches(midas, data);
        FixRoom13(midas, data);
        FixPassport(midas, data);

        return [data];
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR1Level midas)
    {
        return
        [
            new()
            {
                RoomIndex = 2,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 2,
                SourceIndex = 49,
                Vertices =
                [
                    midas.Rooms[2].Mesh.Rectangles[40].Vertices[0],
                    midas.Rooms[2].Mesh.Rectangles[40].Vertices[3],
                    midas.Rooms[2].Mesh.Rectangles[46].Vertices[2],
                    midas.Rooms[2].Mesh.Rectangles[46].Vertices[1],
                ]
            },
            new()
            {
                RoomIndex = 2,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 2,
                SourceIndex = 49,
                Vertices =
                [
                    midas.Rooms[2].Mesh.Rectangles[48].Vertices[3],
                    midas.Rooms[2].Mesh.Rectangles[48].Vertices[2],
                    midas.Rooms[2].Mesh.Rectangles[46].Vertices[1],
                    midas.Rooms[2].Mesh.Rectangles[46].Vertices[0],
                ]
            },
            new()
            {
                RoomIndex = 53,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = GetSource(midas, TRMeshFaceType.TexturedTriangle, 7).Room,
                SourceIndex = GetSource(midas, TRMeshFaceType.TexturedTriangle, 7).Face,
                Vertices =
                [
                    midas.Rooms[53].Mesh.Rectangles[3].Vertices[1],
                    midas.Rooms[53].Mesh.Rectangles[0].Vertices[3],
                    midas.Rooms[53].Mesh.Rectangles[3].Vertices[2],
                ]
            }
        ];
    }

    private static List<TRRoomTextureReface> CreateRefacings()
    {
        List<TRRoomTextureReface> edits =
        [
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
        ];

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
        return
        [
            Rotate(31, TRMeshFaceType.TexturedQuad, 57, 2),
            Rotate(31, TRMeshFaceType.TexturedQuad, 115, 2),
            Rotate(31, TRMeshFaceType.TexturedQuad, 277, 2),
            Rotate(28, TRMeshFaceType.TexturedQuad, 22, 2),
            Rotate(7, TRMeshFaceType.TexturedTriangle, 32, 2),
            Rotate(20, TRMeshFaceType.TexturedTriangle, 0, 2),
        ];
    }

    private static List<TRRoomTextureMove> CreateShifts(TR1Level midas)
    {
        return
        [
            new()
            {
                RoomIndex = 5,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 55,
                VertexRemap =
                [
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
                ]
            },
            new()
            {
                RoomIndex = 30,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 171,
                VertexRemap =
                [
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
                ]
            },
            new()
            {
                RoomIndex = 30,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 167,
                VertexRemap =
                [
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
                ]
            }
        ];
    }

    private static void FixArches(TR1Level level, InjectionData data)
    {
        var mesh = level.Rooms[69].StaticMeshes.First(s => s.ID == TR1Type.SceneryBase + 15);
        mesh.Z += 21;
        data.RoomEdits.Add(new TRRoomStatic3DEdit
        {
            RoomIndex = 69,
            MeshIndex = level.Rooms[69].StaticMeshes.IndexOf(mesh),
            StaticMesh = mesh,
        });

        foreach (int id in new[] { 14, 15 })
        {
            data.MeshEdits.Add(new()
            {
                EnforcedType = TRObjectType.Static3D,
                ModelID = (uint)(TR1Type.SceneryBase) + (uint)id,
                VertexEdits = [.. new[] { 0, 3, 4, 7 }.Select(i => new TRVertexEdit
                {
                    Index = (short)i,
                    Change = new() { X = (short)(id == 14 ? -20 : -29) },
                })],
            });
        }

        foreach (var id in new[] { 784, 786, 788, 798, 818, 832 })
        {
            var info = level.ObjectTextures[id];
            var tile = new TRImage(level.Images8[info.Atlas].Pixels, level.Palette);
            var img = tile.Export(info.Bounds);
            if (!img.Pixels.Any(p => p == 0))
            {
                continue;
            }

            img.Write((c, x, y) =>
            {
                if (c.A == 0)
                {
                    c = img.GetPixel(x, y + 1);
                    Debug.Assert(c.A != 0);
                }
                return c;
            });
            data.TextureOverwrites.Add(new()
            {
                Page = info.Atlas,
                X = (byte)info.Position.X,
                Y = (byte)info.Position.Y,
                Width = (ushort)img.Width,
                Height = (ushort)img.Height,
                Data = img.ToRGBA(),
            });
        }
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
            VertexRemap =
            [
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
            ]
        });

        data.RoomEdits.Add(new TRRoomTextureMove
        {
            RoomIndex = 13,
            FaceType = TRMeshFaceType.TexturedQuad,
            TargetIndex = 105,
            VertexRemap =
            [
                new()
                {
                    Index = 2,
                    NewVertexIndex = midas.Rooms[13].Mesh.Rectangles[102].Vertices[1]
                },
            ]
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
            Vertices =
            [
                midas.Rooms[13].Mesh.Rectangles[57].Vertices[1],
                midas.Rooms[13].Mesh.Rectangles[61].Vertices[1],
                midas.Rooms[13].Mesh.Rectangles[63].Vertices[3],
            ]
        });

        data.RoomEdits.Add(new TRRoomTextureMove
        {
            RoomIndex = 13,
            FaceType = TRMeshFaceType.TexturedQuad,
            TargetIndex = 63,
            VertexRemap =
            [
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
            ]
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
            VertexRemap =
            [
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
            ]
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
            VertexChanges =
            [
                new() { Y = -256 },
                new() { },
                new() { },
                new() { Y = -256 },
            ]
        });

        data.VisPortalEdits.Add(new TRVisPortalEdit
        {
            BaseRoom = 13,
            LinkRoom = 9,
            PortalIndex = 1,
            VertexChanges =
            [
                new() { },
                new() { Y = -256 },
                new() { Y = -256 },
                new() { },
            ]
        });
    }
}
