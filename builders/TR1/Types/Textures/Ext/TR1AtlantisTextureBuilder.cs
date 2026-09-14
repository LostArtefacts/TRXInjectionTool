using TRLevelControl.Model;
using TRXInjectionTool.Actions;

namespace TRXInjectionTool.Types.TR1.Textures;

public partial class TR1AtlantisTextureBuilder
{
    public static IEnumerable<TRRoomTextureEdit> FixCatwalks(TR1Level level)
    {
        foreach (var edit in CreateRoom7Edits(level, false)) yield return edit;
        foreach (var edit in CreateRoom9Edits(level, false)) yield return edit;
        foreach (var edit in CreateRoom7Edits(level, true)) yield return edit;
        foreach (var edit in CreateRoom9Edits(level, true)) yield return edit;

        foreach (var edit in CreateRoom13Edits(level)) yield return edit;
        foreach (var edit in CreateRoom14Edits(level)) yield return edit;
    }

    private static IEnumerable<TRRoomTextureEdit> CreateRoom7Edits(TR1Level level, bool flip)
    {
        var roomIdx = (short)(flip ? 96 : 7);
        var room = level.Rooms[roomIdx];

        var sides = new ushort[] { 128, 131 };
        var side = 0;

        // Group 1: go around the sides and fill in, cycling the above textures
        {
            var root = 83;
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[3]]);
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[2]]);
            yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[3],
                room.Mesh.Rectangles[root].Vertices[2],
                (ushort)(room.Mesh.Vertices.Count - 1),
                (ushort)(room.Mesh.Vertices.Count - 2),
            });

            root = 23;
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[1]]);
            yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[2],
                room.Mesh.Rectangles[root].Vertices[1],
                (ushort)(room.Mesh.Vertices.Count - 1),
                (ushort)(room.Mesh.Vertices.Count - 2),
            });

            root = 28;
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[0]]);
            yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[1],
                room.Mesh.Rectangles[root].Vertices[0],
                (ushort)(room.Mesh.Vertices.Count - 1),
                (ushort)(room.Mesh.Vertices.Count - 2),
            });

            root = 32;
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[0]]);
            yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[1],
                room.Mesh.Rectangles[root].Vertices[0],
                (ushort)(room.Mesh.Vertices.Count - 1),
                (ushort)(room.Mesh.Vertices.Count - 2),
            });

            root = 90;
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[3]]);
            yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[0],
                room.Mesh.Rectangles[root].Vertices[3],
                (ushort)(room.Mesh.Vertices.Count - 1),
                (ushort)(room.Mesh.Vertices.Count - 2),
            });

            root = 95;
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[3]]);
            yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[0],
                room.Mesh.Rectangles[root].Vertices[3],
                (ushort)(room.Mesh.Vertices.Count - 1),
                room.Mesh.Rectangles[158].Vertices[3],
            });

            root = 46;
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[0]]);
            yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[1],
                room.Mesh.Rectangles[root].Vertices[0],
                (ushort)(room.Mesh.Vertices.Count - 1),
                (ushort)(room.Mesh.Vertices.Count - 2),
            });

            root = 50;
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[0]]);
            yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[1],
                room.Mesh.Rectangles[root].Vertices[0],
                (ushort)(room.Mesh.Vertices.Count - 1),
                (ushort)(room.Mesh.Vertices.Count - 2),
            });

            root = 53;
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[0]]);
            yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[1],
                room.Mesh.Rectangles[root].Vertices[0],
                (ushort)(room.Mesh.Vertices.Count - 1),
                (ushort)(room.Mesh.Vertices.Count - 2),
            });

            root = 103;
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[0]]);
            yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[1],
                room.Mesh.Rectangles[root].Vertices[0],
                (ushort)(room.Mesh.Vertices.Count - 1),
                (ushort)(room.Mesh.Vertices.Count - 2),
            });

            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[3]]);
            yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[0],
                room.Mesh.Rectangles[root].Vertices[3],
                (ushort)(room.Mesh.Vertices.Count - 1),
                (ushort)(room.Mesh.Vertices.Count - 2),
            });

            foreach (var root2 in new[] { 168, 212, 252, 292, 340 })
            {
                yield return CreateVertex(roomIdx, room,
                    room.Mesh.Vertices[room.Mesh.Rectangles[root2].Vertices[0]]);
                yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
                {
                    room.Mesh.Rectangles[root2].Vertices[1],
                    room.Mesh.Rectangles[root2].Vertices[0],
                    (ushort)(room.Mesh.Vertices.Count - 1),
                    (ushort)(room.Mesh.Vertices.Count - 2),
                });
            }

            root = 396;
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[2]]);
            yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[3],
                room.Mesh.Rectangles[root].Vertices[2],
                (ushort)(room.Mesh.Vertices.Count - 1),
                (ushort)(room.Mesh.Vertices.Count - 2),
            });

            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[1]]);
            yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[2],
                room.Mesh.Rectangles[root].Vertices[1],
                (ushort)(room.Mesh.Vertices.Count - 1),
                (ushort)(room.Mesh.Vertices.Count - 2),
            });

            foreach (var root2 in new[] { 437, 435 })
            {
                yield return CreateVertex(roomIdx, room,
                    room.Mesh.Vertices[room.Mesh.Rectangles[root2].Vertices[3]]);
                yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
                {
                    room.Mesh.Rectangles[root2].Vertices[0],
                    room.Mesh.Rectangles[root2].Vertices[3],
                    (ushort)(room.Mesh.Vertices.Count - 1),
                    (ushort)(room.Mesh.Vertices.Count - 2),
                });
            }

            root = 433;
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[0]]);
            yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[1],
                room.Mesh.Rectangles[root].Vertices[0],
                (ushort)(room.Mesh.Vertices.Count - 1),
                (ushort)(room.Mesh.Vertices.Count - 2),
            });

            root = 383;
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[3]]);
            yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[0],
                room.Mesh.Rectangles[root].Vertices[3],
                (ushort)(room.Mesh.Vertices.Count - 1),
                (ushort)(room.Mesh.Vertices.Count - 2),
            });

            root = 376;
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[1]]);
            yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[2],
                room.Mesh.Rectangles[root].Vertices[1],
                (ushort)(room.Mesh.Vertices.Count - 1),
                room.Mesh.Rectangles[378].Vertices[2],
            });

            foreach (var root2 in new[] { 424, 422, 419 })
            {
                yield return CreateVertex(roomIdx, room,
                    room.Mesh.Vertices[room.Mesh.Rectangles[root2].Vertices[0]]);
                yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
                {
                    room.Mesh.Rectangles[root2].Vertices[1],
                    room.Mesh.Rectangles[root2].Vertices[0],
                    (ushort)(room.Mesh.Vertices.Count - 1),
                    (ushort)(room.Mesh.Vertices.Count - 2),
                });
            }

            root = 417;
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[2]]);
            yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[3],
                room.Mesh.Rectangles[root].Vertices[2],
                (ushort)(room.Mesh.Vertices.Count - 1),
                (ushort)(room.Mesh.Vertices.Count - 2),
            });

            root = 415;
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[1]]);
            yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[2],
                room.Mesh.Rectangles[root].Vertices[1],
                (ushort)(room.Mesh.Vertices.Count - 1),
                (ushort)(room.Mesh.Vertices.Count - 2),
            });

            foreach (var root2 in new[] { 413, 411 })
            {
                yield return CreateVertex(roomIdx, room,
                    room.Mesh.Vertices[room.Mesh.Rectangles[root2].Vertices[0]]);
                yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
                {
                    room.Mesh.Rectangles[root2].Vertices[1],
                    room.Mesh.Rectangles[root2].Vertices[0],
                    (ushort)(room.Mesh.Vertices.Count - 1),
                    (ushort)(room.Mesh.Vertices.Count - 2),
                });
            }

            root = 350;
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[3]]);
            yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[0],
                room.Mesh.Rectangles[root].Vertices[3],
                (ushort)(room.Mesh.Vertices.Count - 1),
                (ushort)(room.Mesh.Vertices.Count - 2),
            });

            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[2]]);
            yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[3],
                room.Mesh.Rectangles[root].Vertices[2],
                (ushort)(room.Mesh.Vertices.Count - 1),
                (ushort)(room.Mesh.Vertices.Count - 2),
            });

            root = 348;
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[2]]);
            yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[3],
                room.Mesh.Rectangles[root].Vertices[2],
                (ushort)(room.Mesh.Vertices.Count - 1),
                (ushort)(room.Mesh.Vertices.Count - 2),
            });

            foreach (var root2 in new[] { 296, 256, 216, 172, 110 })
            {
                yield return CreateVertex(roomIdx, room,
                    room.Mesh.Vertices[room.Mesh.Rectangles[root2].Vertices[2]]);
                yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
                {
                    room.Mesh.Rectangles[root2].Vertices[3],
                    room.Mesh.Rectangles[root2].Vertices[2],
                    (ushort)(room.Mesh.Vertices.Count - 1),
                    (ushort)(room.Mesh.Vertices.Count - 2),
                });
            }

            foreach (var root2 in new[] { 70, 73 })
            {
                yield return CreateVertex(roomIdx, room,
                    room.Mesh.Vertices[room.Mesh.Rectangles[root2].Vertices[0]]);
                yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
                {
                    room.Mesh.Rectangles[root2].Vertices[1],
                    room.Mesh.Rectangles[root2].Vertices[0],
                    (ushort)(room.Mesh.Vertices.Count - 1),
                    (ushort)(room.Mesh.Vertices.Count - 2),
                });
            }

            root = 73;
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[3]]);
            yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[0],
                room.Mesh.Rectangles[root].Vertices[3],
                (ushort)(room.Mesh.Vertices.Count - 1),
                (ushort)(room.Mesh.Vertices.Count - 2),
            });

            foreach (var root2 in new[] { 10, 14, 18 })
            {
                yield return CreateVertex(roomIdx, room,
                    room.Mesh.Vertices[room.Mesh.Rectangles[root2].Vertices[0]]);
                yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
                {
                    room.Mesh.Rectangles[root2].Vertices[1],
                    room.Mesh.Rectangles[root2].Vertices[0],
                    (ushort)(room.Mesh.Vertices.Count - 1),
                    (ushort)(room.Mesh.Vertices.Count - 2),
                });
            }
        }

        // Group 2: straightforward face inversion for the outsides
        foreach (var root in new[] {
            131, 136, 189, 193, 235, 275, 315, 316, 366, 362, 358, 309, 307, 266, 226, 183, 182,
            126, 124, 128, 147, 158, 203, 206, 246, 286, 331, 332, 324, 322, 277, 237, 196, 195,
        })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }

        // Group 3: as above but for the floor
        var floors = new ushort[] { 300, 260 };
        var floor = 0;
        foreach (var root in new[] {
            305, 302, 300, 262, 260, 222, 220, 178, 176, 114, 116, 120, 75, 77, 79, 81, 84, 86, 88,
            141, 138, 133, 191, 233, 273, 318, 320, 313, 373, 370, 367, 364, 360, 356, 353, 97, 99,
            101, 155, 159, 162, 165, 204, 207, 209, 244, 247, 249, 284, 287, 289, 329, 333, 335, 337,
            393, 390, 387,
        })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 7, floors[floor++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }
    }

    private static IEnumerable<TRRoomTextureEdit> CreateRoom9Edits(TR1Level level, bool flip)
    {
        var roomIdx = (short)(flip ? 95 : 9);
        var room = level.Rooms[roomIdx];

        var floors = new ushort[] { 300, 260 };
        var floor = 0;
        foreach (var root in new[] {
            280, 278, 276, 274, 272, 270, 268, 234, 232, 230, 199, 201, 168, 166, 136, 138, 102, 104,
            106, 62, 64, 66, 68, 71, 73, 75, 115, 113, 111, 145, 176, 208, 239, 241, 243, 288, 290, 292,
            248, 250, 252, 254, 215, 217, 219, 185, 187, 189, 152, 154, 156, 120, 122, 124, 126, 84, 86, 88,
        })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 7, floors[floor++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }
    }

    private static IEnumerable<TRRoomTextureEdit> CreateRoom13Edits(TR1Level level)
    {
        const short roomIdx = 13;
        var room = level.Rooms[roomIdx];

        var sides = new ushort[] { 128, 131 };
        int side = 0;

        // Group 1: go around the sides and fill in, cycling the above textures
        foreach (var root in new[] { 27, 3 })
        {
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[2]]);
            yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[3],
                room.Mesh.Rectangles[root].Vertices[2],
                (ushort)(room.Mesh.Vertices.Count - 1),
                root == 27
                    ? room.Mesh.Rectangles[51].Vertices[2]
                    : (ushort)(room.Mesh.Vertices.Count - 2),
            });
        }

        {
            yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[0].Vertices[1],
                room.Mesh.Rectangles[0].Vertices[2],
                room.Mesh.Rectangles[6].Vertices[3],
                (ushort)(room.Mesh.Vertices.Count - 1),
            });

            yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[46].Vertices[2],
                room.Mesh.Rectangles[46].Vertices[3],
                room.Mesh.Rectangles[65].Vertices[2],
                room.Mesh.Rectangles[48].Vertices[3],
            });
        }

        // Group 2: straightforward face inversion for the outsides
        foreach (var root in new[] { 48, 45, 41, 42, 61, 63, 65, 50, 54, 56, 36, 34, 31, 8 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 7, sides[side++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }

        // Group 3: as above but for the floor
        var floors = new ushort[] { 300, 260 };
        var floor = 0;
        foreach (var root in new[] { 0, 25, 29, 32, 39, 43, 46 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 7, floors[floor++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }
    }

    private static IEnumerable<TRRoomTextureEdit> CreateRoom14Edits(TR1Level level)
    {
        const short roomIdx = 14;
        var room = level.Rooms[roomIdx];

        var floors = new ushort[] { 300, 260 };
        var floor = 0;
        foreach (var root in new[] { 1, 43, 48, 50, 55, 57, 59 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 7, floors[floor++ % 2], TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }
    }
}
