using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public partial class TR1StrongholdTextureBuilder
{
    private static readonly (int x, int z)[] _outerShifts =
    {
        (0, 1), (-1, 0), (0, -1), (1, 0),
    };

    private static readonly (int x, int z)[][] _innerShifts = 
    {
        new[] { (-1, 0), (0, 1) },
        new[] { (0, -1), (-1, 0) },
        new[] { (1, 0), (0, -1) },
        new[] { (0, 1), (1, 0) },
    };

    private static void FixMiscRooms(TR1Level level, InjectionData data)
    {
        data.RoomEdits.AddRange(FixRoom5(level, false));
        data.RoomEdits.AddRange(FixRoom5(level, true));
        data.RoomEdits.AddRange(FixRoom6(level));
        data.RoomEdits.AddRange(FixRoom13(level));
        data.RoomEdits.AddRange(FixRoom18(level));
        data.VisPortalEdits.AddRange(FixRoom16());        
    }

    private static IEnumerable<TRRoomTextureEdit> FixRoom5(TR1Level level, bool flip)
    {        
        const short shade = 6576;
        var roomIdx = (short)(flip ? 74 : 5);
        var room = level.Rooms[roomIdx];

        foreach (var edit in FixOuterHub(level, roomIdx, room.Mesh.Rectangles[flip ? 761 : 764].Vertices[2], shade))
        {
            yield return edit;
        }

        foreach (var edit in FixInnerHub(level, roomIdx, room.Mesh.Rectangles[flip ? 666 : 669].Vertices[1], shade))
        {
            yield return edit;
        }

        var mesh = room.Mesh;
        var floors = new ushort[] { 720, 723 };
        var floor = 0;
        foreach (var face in mesh.Rectangles.Where(f =>
            f.Vertices.All(v => mesh.Vertices[v].Vertex.Y == room.Info.YBottom - 256)))
        {
            yield return CreateFace(roomIdx, 5, floors[floor++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }
    }

    private static IEnumerable<TRRoomTextureEdit> FixRoom6(TR1Level level)
    {
        const short roomIdx = 6;
        var room = level.Rooms[roomIdx];
        var mesh = room.Mesh;
        var floors = new ushort[] { 720, 723 };
        var floor = 0;
        foreach (var face in mesh.Rectangles.Where(f =>
            f.Vertices.All(v => mesh.Vertices[v].Vertex.Y == room.Info.YTop)))
        {
            yield return CreateFace(roomIdx, 5, floors[floor++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }
    }

    private static IEnumerable<TRRoomTextureEdit> FixOuterHub
        (TR1Level level, short roomIdx, ushort startVtx, short shade)
    {
        var room = level.Rooms[roomIdx];
        var mesh = room.Mesh;
        var vtx1 = startVtx;

        yield return CreateVertex(roomIdx, room, mesh.Vertices[vtx1], shade);

        for (int i = 0; i < _outerShifts.Length; i++)
        {
            var (outerX, outerZ) = _outerShifts[i];
            for (int j = 0; j < 9; j++)
            {
                var vtx2 = FindVertex(room, vtx1, outerX, outerZ);
                yield return CreateVertex(roomIdx, room, room.Mesh.Vertices[vtx2], shade);
                yield return CreateFace(roomIdx, 6, 399, TRMeshFaceType.TexturedQuad, new[]
                {
                    (ushort)(room.Mesh.Vertices.Count - 2),
                    (ushort)(room.Mesh.Vertices.Count - 1),
                    vtx2, vtx1,
                });
                vtx1 = vtx2;
            }
        }
    }

    private static IEnumerable<TRRoomTextureEdit> FixInnerHub
        (TR1Level level, short roomIdx, ushort startVtx, short shade)
    {
        var room = level.Rooms[roomIdx];
        var mesh = room.Mesh;
        var vtx1 = startVtx;
        var vtx2 = vtx1;

        var sources = new[] { 225, 225, 225, 225, 242, 242, 225 }
            .Select(t => GetSource(level, TRMeshFaceType.TexturedQuad, (ushort)t))
            .ToArray();
        var sourceIdx = 0;
        var source = sources[sourceIdx++];

        IEnumerable<TRRoomTextureEdit> MakeFace()
        {
            yield return CreateVertex(roomIdx, room, mesh.Vertices[vtx2], shade);
            yield return CreateFace(roomIdx, source.Room, (ushort)source.Face, TRMeshFaceType.TexturedQuad, new[]
            {
                vtx1, vtx2,
                (ushort)(room.Mesh.Vertices.Count - 1),
                (ushort)(room.Mesh.Vertices.Count - 2),
            });
            vtx1 = vtx2;
            source = sources[sourceIdx++ % sources.Length];
        }
        
        yield return CreateVertex(roomIdx, room, mesh.Vertices[vtx1], shade);

        for (int i = 0; i < _outerShifts.Length; i++)
        {
            var (outerX, outerZ) = _outerShifts[i];
            for (int j = 0; j < 3; j++)
            {
                vtx2 = FindVertex(room, vtx1, outerX, outerZ);
                foreach (var edit in MakeFace()) yield return edit;
            }

            for (int j = 0; j < 4; j++)
            {
                var (innerX, innerZ) = _innerShifts[i][j % 2];
                vtx2 = FindVertex(room, vtx1, innerX, innerZ);
                foreach (var edit in MakeFace()) yield return edit;
            }
        }
    }

    private static IEnumerable<TRRoomTextureEdit> FixRoom13(TR1Level level)
    {
        var room = level.Rooms[13];
        var mesh = room.Mesh;

        yield return CreateVertex(13, room, mesh.Vertices[mesh.Rectangles[15].Vertices[0]], -1, 1024);
        yield return CreateVertex(13, room, mesh.Vertices[mesh.Rectangles[18].Vertices[0]], -1, 1024);
        yield return CreateFace(13, 13, 15, TRMeshFaceType.TexturedQuad, new[]
        {
            mesh.Rectangles[15].Vertices[0],
            mesh.Rectangles[18].Vertices[0],
            (ushort)(mesh.Vertices.Count - 1),
            (ushort)(mesh.Vertices.Count - 2),
        });

        yield return CreateVertex(13, room, mesh.Vertices[mesh.Rectangles[246].Vertices[3]]);
        yield return CreateFace(13, 13, 161, TRMeshFaceType.TexturedQuad, new[]
        {
            mesh.Rectangles[216].Vertices[2],
            (ushort)(mesh.Vertices.Count -1),
            mesh.Rectangles[246].Vertices[3],
            mesh.Rectangles[246].Vertices[2],
        });
        yield return CreateFace(13, 13, 33, TRMeshFaceType.TexturedTriangle, new[]
        {
            (ushort)(mesh.Vertices.Count -1),
            mesh.Rectangles[289].Vertices[2],
            mesh.Rectangles[289].Vertices[1],
        });
    }

    private static IEnumerable<TRVisPortalEdit> FixRoom16()
    {
        yield return CreatePortalFix(16, 18, 2, new()
        {
            new() { Y = -1024 },
            new(),
            new(),
            new() { Y = -1024 },
        });
        yield return CreatePortalFix(18, 16, 0, new()
        {
            new(),
            new() { Y = -1024 },
            new() { Y = -1024 },
            new(),
        });
    }

    private static IEnumerable<TRRoomTextureEdit> FixRoom18(TR1Level level)
    {
        var mesh = level.Rooms[18].Mesh;
        foreach (var (face, vtx, shift) in new List<(short Face, short Vtx, short Shift)>
        {
            (7, 2, -2048), (7, 3, -2048),
            (5, 2, -2560), (5, 3, -2560),
            (3, 1, -2048), (3, 2, -2048),
            (1, 2, -512),  (1, 3, -512),
        })
        {
            yield return new TRRoomVertexMove
            {
                RoomIndex = 18,
                VertexIndex = mesh.Rectangles[face].Vertices[vtx],
                VertexChange = new() { Y = shift },
            };
        }

        yield return CreateFace(18, 18, 1, TRMeshFaceType.TexturedQuad, new[]
        {
            mesh.Rectangles[1].Vertices[3], mesh.Rectangles[1].Vertices[2],
            mesh.Rectangles[0].Vertices[1], mesh.Rectangles[0].Vertices[0],
        });
        yield return CreateFace(18, 18, 35, TRMeshFaceType.TexturedTriangle, new[]
        {
            mesh.Rectangles[7].Vertices[1], mesh.Rectangles[6].Vertices[1],
            mesh.Rectangles[7].Vertices[2],
        });
        yield return CreateFace(18, 18, 32, TRMeshFaceType.TexturedQuad, new[]
        {
            mesh.Rectangles[5].Vertices[2], mesh.Rectangles[5].Vertices[1],
            mesh.Rectangles[4].Vertices[2], mesh.Rectangles[4].Vertices[1],
        });
        yield return CreateFace(18, 18, 28, TRMeshFaceType.TexturedQuad, new[]
        {
            mesh.Rectangles[3].Vertices[1], mesh.Rectangles[3].Vertices[0],
            mesh.Rectangles[2].Vertices[2], mesh.Rectangles[2].Vertices[1],
        });
        yield return CreateFace(18, 18, 25, TRMeshFaceType.TexturedQuad, new[]
        {
            mesh.Rectangles[1].Vertices[2], mesh.Rectangles[1].Vertices[1],
            mesh.Rectangles[0].Vertices[2], mesh.Rectangles[0].Vertices[1],
        });

        foreach (var face in new[] { 25, 28, 32, 36 })
        {
            yield return new TRRoomTextureReface
            {
                RoomIndex = 18,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 18,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 20,
                TargetIndex = (short)face,
            };
        }

        foreach (var vtx in mesh.Vertices.Where(v => v.Vertex.Y == level.Rooms[18].Info.YBottom))
        {
            yield return new TRRoomVertexMove
            {
                RoomIndex = 18,
                VertexIndex = (ushort)mesh.Vertices.IndexOf(vtx),
                ShadeChange = (short)(8192 - vtx.Lighting),
            };
        }

        foreach (var face in mesh.Rectangles.Where(r => r.Texture == mesh.Rectangles[37].Texture))
        {
            yield return CreateFace(18, 18, 37, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }

        foreach (var (left, top, right, slantLeft, slantRight) in
            new List<(short Left, short Top, short Right, short SlantLeft, short SlantRight)>
        {
            (190, 188, 230, 186, 226),
            (298, 296, 338, 294, 334),
            (407, 405, 448, 403, 444),
        })
        {
            yield return CreateFace(18, 18, 37, TRMeshFaceType.TexturedQuad, new[]
            {
                mesh.Rectangles[top].Vertices[2], mesh.Rectangles[top].Vertices[3],
                mesh.Rectangles[right].Vertices[2], mesh.Rectangles[left].Vertices[3],
            });
            yield return CreateFace(18, 18, 188, TRMeshFaceType.TexturedQuad, new[]
            {
                mesh.Rectangles[right].Vertices[3], mesh.Rectangles[left].Vertices[2],
                mesh.Rectangles[left].Vertices[3], mesh.Rectangles[right].Vertices[2],
            });
            yield return CreateFace(18, 18, 188, TRMeshFaceType.TexturedQuad, new[]
            {
                mesh.Rectangles[slantLeft].Vertices[3], mesh.Rectangles[slantRight].Vertices[2],
                mesh.Rectangles[slantRight].Vertices[3], mesh.Rectangles[slantLeft].Vertices[2],
            });

            yield return CreateFace(18, 18, 188, TRMeshFaceType.TexturedQuad, new[]
            {
                mesh.Rectangles[top].Vertices[1], mesh.Rectangles[top].Vertices[0],
                mesh.Rectangles[top].Vertices[3], mesh.Rectangles[top].Vertices[2]
            });
            yield return CreateFace(18, 18, 188, TRMeshFaceType.TexturedQuad, new[]
            {
                mesh.Rectangles[slantRight].Vertices[1], mesh.Rectangles[slantLeft].Vertices[0],
                mesh.Rectangles[slantLeft].Vertices[1], mesh.Rectangles[slantRight].Vertices[0]
            });
        }

        foreach (var (left, top, right, slantLeft, slantRight) in
            new List<(short Left, short Top, short Right, short SlantLeft, short SlantRight)>
        {
            (412, 365, 367, 417, 372),
            (15, 257, 14, 307, 263),
            (9, 149, 8, 199, 155),
        })
        {
            if (left == 412)
            {
                yield return CreateFace(18, 18, 37, TRMeshFaceType.TexturedQuad, new[]
                {
                    mesh.Rectangles[top].Vertices[0], mesh.Rectangles[top].Vertices[1],
                    mesh.Rectangles[right].Vertices[2], mesh.Rectangles[left].Vertices[3],
                });
                yield return CreateFace(18, 18, 188, TRMeshFaceType.TexturedQuad, new[]
                {
                    mesh.Rectangles[right].Vertices[3], mesh.Rectangles[left].Vertices[2],
                    mesh.Rectangles[left].Vertices[3], mesh.Rectangles[right].Vertices[2],
                });
            }
            else
            {
                yield return CreateFace(18, 18, 188, TRMeshFaceType.TexturedQuad, new[]
                {
                    mesh.Rectangles[top].Vertices[0], mesh.Rectangles[top].Vertices[1],
                    mesh.Triangles[right].Vertices[2], mesh.Triangles[left].Vertices[2],
                });
                yield return CreateFace(18, 18, 15, TRMeshFaceType.TexturedTriangle, new[]
                {
                    mesh.Triangles[left].Vertices[1], mesh.Triangles[left].Vertices[0], mesh.Triangles[left].Vertices[2]
                });
                yield return CreateFace(18, 18, 15, TRMeshFaceType.TexturedTriangle, new[]
                {
                    mesh.Triangles[right].Vertices[2], mesh.Triangles[right].Vertices[1], mesh.Triangles[right].Vertices[0]
                });
            }
            yield return CreateFace(18, 18, 188, TRMeshFaceType.TexturedQuad, new[]
            {
                mesh.Rectangles[slantLeft].Vertices[3], mesh.Rectangles[slantRight].Vertices[2],
                mesh.Rectangles[slantRight].Vertices[3], mesh.Rectangles[slantLeft].Vertices[2],
            });
            yield return CreateFace(18, 18, 188, TRMeshFaceType.TexturedQuad, new[]
            {
                mesh.Rectangles[top].Vertices[1], mesh.Rectangles[top].Vertices[0],
                mesh.Rectangles[top].Vertices[3], mesh.Rectangles[top].Vertices[2]
            });
            yield return CreateFace(18, 18, 188, TRMeshFaceType.TexturedQuad, new[]
            {
                mesh.Rectangles[slantRight].Vertices[1], mesh.Rectangles[slantLeft].Vertices[0],
                mesh.Rectangles[slantLeft].Vertices[1], mesh.Rectangles[slantRight].Vertices[0]
            });
        }
    }

    private static ushort FindVertex(TR1Room room, int cur, int x, int z)
    {
        var vtx = room.Mesh.Vertices[cur].Vertex;
        var tar = vtx.Clone();
        tar.X += (short)(x * 1024);
        tar.Z += (short)(z * 1024);
        for (ushort i = 0; i < room.Mesh.Vertices.Count; i++)
        {
            var test = room.Mesh.Vertices[i].Vertex;
            if (test.X == tar.X && test.Y == tar.Y && test.Z == tar.Z)
            {
                return i;
            }
        }
        throw new Exception();
    }
}
