using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types.TR1.Textures;

public partial class TR1CisternTextureBuilder : TextureBuilder
{
    public static IEnumerable<TRRoomTextureEdit> FixCatwalks(TR1Level level)
    {
        foreach (var edit in CreateRoom9WallEdits(level)) yield return edit;
        foreach (var edit in CreateDoubleSidedEdits(level, 9)) yield return edit;
        foreach (var edit in CreateDoubleSidedEdits(level, 12)) yield return edit;
    }

    private static IEnumerable<TRRoomTextureEdit> CreateRoom9WallEdits(TR1Level level)
    {
        const short roomIdx = 9;
        var room = level.Rooms[roomIdx];
        SimulateLighting(room, level.Rooms[12]);

        foreach (var root in new[] { 20, 2, 1, 5, 7, 8, 6, 22, 35 })
        {
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[root == 6 ? 3 : 2]]);
            yield return CreateFace(roomIdx, roomIdx, 33, TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[root == 6 ? 2 : 3],
                room.Mesh.Rectangles[root].Vertices[root == 6 ? 3 : 2],
                (ushort)(room.Mesh.Vertices.Count - 1),
                root == 20
                    ? room.Mesh.Rectangles[32].Vertices[3]
                    : (ushort)(room.Mesh.Vertices.Count - 2),
            });
        }

        foreach (var root in new[] { 75, 76, 74, 64, 50 })
        {
            if (root == 75)
            {
                yield return CreateVertex(roomIdx, room,
                    room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[0]]);
                yield return CreateVertex(roomIdx, room,
                    room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[1]]);
                yield return CreateFace(roomIdx, roomIdx, 33, TRMeshFaceType.TexturedQuad, new[]
                {
                    room.Mesh.Rectangles[root].Vertices[0],
                    room.Mesh.Rectangles[root].Vertices[1],
                    (ushort)(room.Mesh.Vertices.Count - 1),
                    (ushort)(room.Mesh.Vertices.Count - 2),
                });
            }
            else
            {
                yield return CreateVertex(roomIdx, room,
                    room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[2]]);
                yield return CreateFace(roomIdx, roomIdx, 33, TRMeshFaceType.TexturedQuad, new[]
                {
                    room.Mesh.Rectangles[root].Vertices[3],
                    room.Mesh.Rectangles[root].Vertices[2],
                    root == 50
                        ? room.Mesh.Rectangles[49].Vertices[2]
                        : (ushort)(room.Mesh.Vertices.Count - 1),
                    (ushort)(room.Mesh.Vertices.Count - 2),
                });
            }   
        }

        var rotated = new[] { 56, 45, 28, 18, 30, 47, 62, 73 };
        foreach (var root in new[] { 56, 69, 59, 45, 28, 13, 12, 17, 18, 30, 47, 62, 73, 82, 83, 88, 87, 86 })
        {
            bool rot = rotated.Contains(root);
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[rot ? 1 : 2]]);
            yield return CreateFace(roomIdx, roomIdx, 33, TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[rot ? 2 : 3],
                room.Mesh.Rectangles[root].Vertices[rot ? 1 : 2],
                (ushort)(room.Mesh.Vertices.Count - 1),
                root == 56
                    ? room.Mesh.Rectangles[54].Vertices[3]
                    : (ushort)(room.Mesh.Vertices.Count - 2),
            });
        }

        yield return CreateFace(roomIdx, roomIdx, 33, TRMeshFaceType.TexturedQuad, new[]
        {
            room.Mesh.Rectangles[52].Vertices[3],
            room.Mesh.Rectangles[52].Vertices[2],
            room.Mesh.Rectangles[54].Vertices[2],
            room.Mesh.Rectangles[49].Vertices[3],
        });

        foreach (var root in new[] { 32, 49, 54 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, roomIdx, 33, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }
    }

    private static IEnumerable<TRRoomTextureEdit> CreateDoubleSidedEdits(TR1Level level, short roomIdx)
    {
        var room = level.Rooms[roomIdx];
        // Match up previous texture fixes
        if (roomIdx == 9)
        {
            room.Mesh.Rectangles[21].Rotate(1);
            room.Mesh.Rectangles[19].Rotate(3);
            room.Mesh.Rectangles[19].Texture = 48;
            room.Mesh.Rectangles[48].Texture = 48;
            room.Mesh.Rectangles[53].Texture = 49;
            room.Mesh.Rectangles[67].Texture = 53;
        }
        else if (roomIdx == 12)
        {
            room.Mesh.Rectangles[32].Rotate(2);
            room.Mesh.Rectangles[60].Rotate(2);
            room.Mesh.Rectangles[29].Texture = 34;
            room.Mesh.Rectangles[32].Texture = 49;
            room.Mesh.Rectangles[60].Texture = 49;
        }

        var texIds = new[] { 48, 49, 53 };
        var flipFace = texIds[0];
        
        foreach (var face in room.Mesh.Rectangles.Where(f => texIds.Contains(f.Texture)))
        {
            face.Rotate(face.Texture == flipFace ? 1 : 0);
            yield return CreateFace(roomIdx, roomIdx, (ushort)room.Mesh.Rectangles.IndexOf(face), TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }
    }

    private static void SimulateLighting(TR1Room topRoom, TR1Room bottomRoom)
    {
        topRoom.Mesh.Vertices.Where(v => v.Vertex.Y == topRoom.Info.YBottom - 256)
            .ToList().ForEach(v =>
            {
                short x = (short)(topRoom.Info.X + v.Vertex.X - bottomRoom.Info.X);
                short z = (short)(topRoom.Info.Z + v.Vertex.Z - bottomRoom.Info.Z);
                var match = bottomRoom.Mesh.Vertices.Find(v2 => v2.Vertex.X == x && v2.Vertex.Z == z && v2.Vertex.Y == topRoom.Info.YBottom);
                if (match != null)
                {
                    v.Lighting = match.Lighting;
                }
            });
    }
}
