using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types.TR2.Textures;

public partial class TR2RigTextureBuilder
{
    private static IEnumerable<TRRoomTextureEdit> FixCatwalks(TR2Level level)
    {
        foreach (var edit in CreateRoom32Edits(level)) yield return edit;
        foreach (var edit in CreateRoom33Edits(level)) yield return edit;
        foreach (var edit in CreateRoom40Edits(level)) yield return edit;
        foreach (var edit in CreateRoom44Edits(level)) yield return edit;
        foreach (var edit in CreateRoom46Edits(level)) yield return edit;
        foreach (var edit in CreateRoom47Edits(level)) yield return edit;
        foreach (var edit in CreateRoom48Edits(level)) yield return edit;
        foreach (var edit in CreateRoom49Edits(level)) yield return edit;
        foreach (var edit in CreateRoom63Edits(level)) yield return edit;
        foreach (var edit in CreateRoom98Edits(level)) yield return edit;
        foreach (var edit in CreateRoom99Edits(level)) yield return edit;
        foreach (var edit in CreateRoom100Edits(level)) yield return edit;
        foreach (var edit in CreateRoom101Edits(level)) yield return edit;
        foreach (var edit in CreateRoom102Edits(level)) yield return edit;
        foreach (var edit in CreateRoom107Edits(level)) yield return edit;
        foreach (var edit in CreateRoom108Edits(level)) yield return edit;
        foreach (var edit in CreateRoom109Edits(level)) yield return edit;
        foreach (var edit in CreateRoom110Edits(level)) yield return edit;
    }

    private static IEnumerable<TRRoomTextureEdit> CreateRoom32Edits(TR2Level level)
    {
        const short roomIdx = 32;
        var room = level.Rooms[roomIdx];

        // Face inversion when viewed from room 33
        foreach (var root in new[] { 4, 12, 24, 35, 47, 59, 72, 14, 26, 49, 61, 75, 70 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 33, 114, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }
    }

    private static IEnumerable<TRRoomTextureEdit> CreateRoom33Edits(TR2Level level)
    {
        const short roomIdx = 33;
        var room = level.Rooms[roomIdx];

        // Group 1: needs vertex creation
        foreach (var root in new[] { 19, 22, 36, 37, 52 })
        {
            var vtx = room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[2]];
            yield return CreateVertex(roomIdx, room, vtx);
            yield return CreateFace(roomIdx, 33, 20, TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[3],
                room.Mesh.Rectangles[root].Vertices[2],
                (ushort)(room.Mesh.Vertices.Count - 1),
                root == 19
                    ? room.Mesh.Rectangles[20].Vertices[3]
                    : (ushort)(room.Mesh.Vertices.Count - 2),
            });
        }

        // Group 2: also needs vertex creation, but gap in floor from above group
        foreach (var root in new[] { 91, 107, 125, 134, 133, 131 })
        {
            var vtx = room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[2]];
            yield return CreateVertex(roomIdx, room, vtx);
            yield return CreateFace(roomIdx, 33, 20, TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[3],
                room.Mesh.Rectangles[root].Vertices[2],
                (ushort)(room.Mesh.Vertices.Count - 1),
                root == 91
                    ? room.Mesh.Rectangles[89].Vertices[3]
                    : (ushort)(room.Mesh.Vertices.Count - 2),
            });
        }

        // Group 3: straightforward face inversion for the sides
        foreach (var root in new[] { 20, 32, 48, 66, 69, 71, 85, 89, 101, 116, 117 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 33, 20, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }

        // Group 4: as above but for the floor
        foreach (var root in new[] { 114, 119, 123, 104, 83, 87, 64, 46, 50, 30, 34, 17 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 33, 114, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }

        // Special case for below the wall beside the pushblock
        yield return CreateFace(roomIdx, 33, 103, TRMeshFaceType.TexturedQuad, new[]
        {
            room.Mesh.Rectangles[106].Vertices[3],
            room.Mesh.Rectangles[106].Vertices[2],
            room.Mesh.Rectangles[103].Vertices[3],
            room.Mesh.Rectangles[103].Vertices[2],
        });
    }

    private static IEnumerable<TRRoomTextureEdit> CreateRoom40Edits(TR2Level level)
    {
        const short roomIdx = 40;
        var room = level.Rooms[roomIdx];

        // Face inversion when viewed from room 101
        foreach (var root in new[] { 75, 63, 61 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 100, 51, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }

        foreach (var root in new[] { 59 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 40, 59, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }
    }

    private static IEnumerable<TRRoomTextureEdit> CreateRoom44Edits(TR2Level level)
    {
        const short roomIdx = 44;
        var room = level.Rooms[roomIdx];

        // Face inversion when viewed from room 101
        foreach (var root in new[] { 68 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 44, 68, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }
    }

    private static IEnumerable<TRRoomTextureEdit> CreateRoom46Edits(TR2Level level)
    {
        const short roomIdx = 46;
        var room = level.Rooms[roomIdx];

        // Face inversion when viewed from room 48
        foreach (var root in new[] { 1, 6, 11, 16, 21, 26, 31, 51, 62, 66, 73 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 46, 1, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }
    }

    private static IEnumerable<TRRoomTextureEdit> CreateRoom47Edits(TR2Level level)
    {
        const short roomIdx = 47;
        var room = level.Rooms[roomIdx];

        // Face inversion when viewed from room 49
        foreach (var root in new[] { 57, 59, 61, 63, 65, 67, 69, 71, 73 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 46, 1, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }
    }

    private static IEnumerable<TRRoomTextureEdit> CreateRoom48Edits(TR2Level level)
    {
        const short roomIdx = 48;
        var room = level.Rooms[roomIdx];

        // Group 1: standalones are awkward - last in this list must be physically connected to first in concat list
        var standalones = new[] { 39, 59, 100, 116, 3 };
        foreach (var root in standalones.Concat(new[] { 7, 11, 15, 19, 24, 29 }))
        {
            if (standalones.Contains(root))
            {
                yield return CreateVertex(roomIdx, room,
                    room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[3]]);
            }
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[2]]);
            yield return CreateFace(roomIdx, 100, 51, TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[3],
                room.Mesh.Rectangles[root].Vertices[2],
                (ushort)(room.Mesh.Vertices.Count - 1),
                (ushort)(room.Mesh.Vertices.Count - 2),
            });
        }

        // Group 2: straightforward face inversion for the sides
        foreach (var root in new[] { 41, 45, 47, 49, 52, 56, 75, 94, 34, 79, 96, 98, 85, 83 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 100, 51, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }

        // Group 3: as above but for the floor
        foreach (var root in new[] { 0, 4, 8, 12, 16, 21, 26, 54, 73, 81, 92 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 100, 15, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }
    }

    private static IEnumerable<TRRoomTextureEdit> CreateRoom49Edits(TR2Level level)
    {
        const short roomIdx = 49;
        var room = level.Rooms[roomIdx];

        // Group 1: vertex creations
        var breakers = new[] { 94, 110, 100, 68 };
        foreach (var root in new[] { 94, 110, 108, 106, 104, 102, 100, 98, 96, 68 })
        {
            if (breakers.Contains(root))
            {
                yield return CreateVertex(roomIdx, room,
                    room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[3]]);
            }
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[2]]);
            yield return CreateFace(roomIdx, 100, 51, TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[3],
                room.Mesh.Rectangles[root].Vertices[2],
                (ushort)(room.Mesh.Vertices.Count - 1),
                (ushort)(room.Mesh.Vertices.Count - 2),
            });

            if (root == 100)
            {
                yield return CreateFace(roomIdx, 100, 51, TRMeshFaceType.TexturedQuad, new[]
                {
                    room.Mesh.Rectangles[75].Vertices[3],
                    room.Mesh.Rectangles[75].Vertices[0],
                    (ushort)(room.Mesh.Vertices.Count - 2),
                    (ushort)(room.Mesh.Vertices.Count - 3),
                });
            }
        }

        // Group 2: straightforward face inversion for the sides
        foreach (var root in new[] { 91, 88, 84, 80, 77, 74, 71 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 100, 51, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }

        // Group 3: as above but for the floor
        foreach (var root in new[] { 66, 69, 72, 75, 78, 82, 86, 89, 92 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 100, 15, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }
    }

    private static IEnumerable<TRRoomTextureEdit> CreateRoom63Edits(TR2Level level)
    {
        const short roomIdx = 63;
        var room = level.Rooms[roomIdx];

        // Face inversion when viewed from room 98
        {
            var face = room.Mesh.Rectangles[56];
            yield return CreateFace(roomIdx, 46, 1, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }
    }

    private static IEnumerable<TRRoomTextureEdit> CreateRoom98Edits(TR2Level level)
    {
        const short roomIdx = 98;
        var room = level.Rooms[roomIdx];

        // Group 1: vertex creations
        {
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[76].Vertices[3]]);
            yield return CreateFace(roomIdx, 100, 51, TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[76].Vertices[3],
                room.Mesh.Rectangles[76].Vertices[2],
                room.Mesh.Rectangles[53].Vertices[2],
                (ushort)(room.Mesh.Vertices.Count - 1),
            });
        }

        // Group 2: straightforward face inversion for the sides
        foreach (var root in new[] { 13, 52, 53 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 100, 51, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }

        // Group 3: as above but for the floor
        {
            var face = room.Mesh.Rectangles[50];
            yield return CreateFace(roomIdx, 100, 15, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }
    }

    private static IEnumerable<TRRoomTextureEdit> CreateRoom99Edits(TR2Level level)
    {
        const short roomIdx = 99;
        var room = level.Rooms[roomIdx];

        // Group 1: vertex creations
        var breakers = new[] { 17, 110, 156 };
        foreach (var root in new[] { 15, 17, 110, 125, 140, 158, 156 })
        {
            if (breakers.Contains(root))
            {
                yield return CreateVertex(roomIdx, room,
                    room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[3]]);
            }
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[2]]);
            yield return CreateFace(roomIdx, 100, 51, TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[3],
                room.Mesh.Rectangles[root].Vertices[2],
                (ushort)(room.Mesh.Vertices.Count - 1),
                root == 15
                    ? room.Mesh.Rectangles[32].Vertices[3]
                    : (ushort)(room.Mesh.Vertices.Count - 2),
            });
        }

        {
            yield return CreateFace(roomIdx, 100, 51, TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[29].Vertices[3],
                room.Mesh.Rectangles[29].Vertices[2],
                room.Mesh.Rectangles[33].Vertices[2],
                room.Mesh.Rectangles[30].Vertices[3],
            });
        }

        // Group 2: straightforward face inversion for the sides
        foreach (var root in new[] { 30, 47, 33, 32, 106, 108, 123, 138 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 100, 51, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }

        // Group 3: as above but for the floor
        foreach (var root in new[] { 13, 27, 104, 121, 136, 154 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 100, 15, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }
    }

    private static IEnumerable<TRRoomTextureEdit> CreateRoom100Edits(TR2Level level)
    {
        const short roomIdx = 100;
        var room = level.Rooms[roomIdx];

        // Group 1: needs vertex creation
        foreach (var root in new[] { 18, 23 })
        {
            var vtx = room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[2]];
            yield return CreateVertex(roomIdx, room, vtx);
            yield return CreateFace(roomIdx, 100, 51, TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[3],
                room.Mesh.Rectangles[root].Vertices[2],
                (ushort)(room.Mesh.Vertices.Count - 1),
                root == 18
                    ? room.Mesh.Rectangles[19].Vertices[3]
                    : (ushort)(room.Mesh.Vertices.Count - 2),
            });
        }

        // Group 2: straightforward face inversion for the sides
        foreach (var root in new[] { 19, 28, 37, 45, 47, 77, 80 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 100, 51, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }

        // Group 3: as above but for the floor
        foreach (var root in new[] { 15, 20 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 100, 15, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }
    }

    private static IEnumerable<TRRoomTextureEdit> CreateRoom101Edits(TR2Level level)
    {
        const short roomIdx = 101;
        var room = level.Rooms[roomIdx];

        // Group 1: needs vertex creation
        foreach (var root in new[] { 25, 3 })
        {
            var vtx = room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[2]];
            yield return CreateVertex(roomIdx, room, vtx);
            yield return CreateFace(roomIdx, 100, 51, TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[3],
                room.Mesh.Rectangles[root].Vertices[2],
                (ushort)(room.Mesh.Vertices.Count - 1),
                root == 25
                    ? room.Mesh.Rectangles[44].Vertices[3]
                    : (ushort)(room.Mesh.Vertices.Count - 2),
            });
        }

        foreach (var root in new[] { 31, 10, 13 })
        {
            if (root == 13)
            {
                yield return CreateVertex(roomIdx, room,
                    room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[3]]);
            }
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[2]]);
            yield return CreateFace(roomIdx, 100, 51, TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[3],
                room.Mesh.Rectangles[root].Vertices[2],
                (ushort)(room.Mesh.Vertices.Count - 1),
                root == 31
                    ? room.Mesh.Rectangles[28].Vertices[3]
                    : (ushort)(room.Mesh.Vertices.Count - 2),
            });
        }

        foreach (var root in new[] { 63, 78, 100, 122, 177 })
        {
            if (root == 177)
            {
                yield return CreateVertex(roomIdx, room,
                    room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[3]]);
            }
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[2]]);
            yield return CreateFace(roomIdx, 100, 51, TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[3],
                room.Mesh.Rectangles[root].Vertices[2],
                (ushort)(room.Mesh.Vertices.Count - 1),
                root == 63
                    ? room.Mesh.Rectangles[60].Vertices[3]
                    : (ushort)(room.Mesh.Vertices.Count - 2),
            });
        }

        // Group 2: straightforward face inversion for the sides
        foreach (var root in new[] { 44, 49, 51, 53, 55, 37, 16, 28, 5, 60, 61, 76, 99, 121, 134, 168, 166, 162, 163, 175, 174 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 100, 51, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }

        // Group 3: as above but for the floor
        foreach (var root in new[] { 0, 22, 26, 29, 32, 34, 8, 11, 58, 74, 96, 118, 164, 160 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 100, 15, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }
    }

    private static IEnumerable<TRRoomTextureEdit> CreateRoom102Edits(TR2Level level)
    {
        const short roomIdx = 102;
        var room = level.Rooms[roomIdx];

        // Group 1: needs vertex creation
        {
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[11].Vertices[2]]);
            yield return CreateFace(roomIdx, 100, 51, TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[11].Vertices[3],
                room.Mesh.Rectangles[11].Vertices[2],
                (ushort)(room.Mesh.Vertices.Count - 1),
                room.Mesh.Rectangles[28].Vertices[3]
            });
        }

        {
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[5].Vertices[2]]);
            yield return CreateFace(roomIdx, 100, 51, TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[5].Vertices[3],
                room.Mesh.Rectangles[5].Vertices[2],
                (ushort)(room.Mesh.Vertices.Count - 1),
                room.Mesh.Rectangles[23].Vertices[3]
            });
        }

        foreach (var root in new[] { 8, 26 })
        {
            if (root == 8)
            {
                yield return CreateVertex(roomIdx, room,
                    room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[3]]);
            }
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[2]]);
            yield return CreateFace(roomIdx, 100, 51, TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[3],
                room.Mesh.Rectangles[root].Vertices[2],
                (ushort)(room.Mesh.Vertices.Count - 1),
                (ushort)(room.Mesh.Vertices.Count - 2),
            });
        }

        {
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[59].Vertices[1]]);
            yield return CreateFace(roomIdx, 100, 51, TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[59].Vertices[2],
                room.Mesh.Rectangles[59].Vertices[1],
                (ushort)(room.Mesh.Vertices.Count - 1),
                room.Mesh.Rectangles[62].Vertices[2],
            });
            yield return CreateFace(roomIdx, 100, 51, TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[56].Vertices[2],
                room.Mesh.Rectangles[56].Vertices[1],
                room.Mesh.Rectangles[58].Vertices[3],
                (ushort)(room.Mesh.Vertices.Count - 1),
            });
        }


        // Group 2: straightforward face inversion for the sides
        foreach (var root in new[] { 45, 23, 40, 13, 28, 54 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 100, 51, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }

        // Group 3: as above but for the floor
        foreach (var root in new[] { 38, 41, 21, 24, 3, 6, 9 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 100, 15, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }
    }

    private static IEnumerable<TRRoomTextureEdit> CreateRoom107Edits(TR2Level level)
    {
        const short roomIdx = 107;
        var room = level.Rooms[roomIdx];

        // Face inversion when viewed from room 99
        foreach (var root in new[] { 16, 34, 92, 107, 122, 144 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 107, 16, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }
    }

    private static IEnumerable<TRRoomTextureEdit> CreateRoom108Edits(TR2Level level)
    {
        const short roomIdx = 108;
        var room = level.Rooms[roomIdx];

        // Face inversion when viewed from room 100
        foreach (var root in new[] { 16, 21 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 108, 16, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }
    }

    private static IEnumerable<TRRoomTextureEdit> CreateRoom109Edits(TR2Level level)
    {
        const short roomIdx = 109;
        var room = level.Rooms[roomIdx];

        // Face inversion when viewed from room 101
        foreach (var root in new[] { 1, 9, 12, 23, 28, 30, 32, 35, 56, 71, 86, 101, 145, 148 })
        {
            var face = room.Mesh.Rectangles[root];
            if (root == 101)
            {
                yield return Rotate(roomIdx, TRMeshFaceType.TexturedQuad, 101, 3);
                face.Rotate(3);
            }
            yield return CreateFace(roomIdx, 108, 16, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }
    }

    private static IEnumerable<TRRoomTextureEdit> CreateRoom110Edits(TR2Level level)
    {
        const short roomIdx = 110;
        var room = level.Rooms[roomIdx];

        // Face inversion when viewed from room 102
        foreach (var root in new[] { 4, 6, 10, 20, 22, 35, 37 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 108, 16, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }

        yield return Reface(level, roomIdx, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1667, 52);
        yield return Reface(level, roomIdx, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1667, 54);
    }
}
