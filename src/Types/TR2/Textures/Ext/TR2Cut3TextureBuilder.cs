using TRLevelControl.Model;
using TRXInjectionTool.Actions;

namespace TRXInjectionTool.Types.TR2.Textures;

public partial class TR2Cut3TextureBuilder
{
    public static IEnumerable<TRRoomTextureEdit> FixCatwalks(TR2Level level,
        short topRoomIdx = 7, short bottomRoomIdx = 8)
    {
        foreach (var edit in CreateRoom7Edits(level, topRoomIdx)) yield return edit;
        foreach (var edit in CreateRoom8Edits(level, bottomRoomIdx)) yield return edit;
    }

    private static IEnumerable<TRRoomTextureEdit> CreateRoom7Edits(TR2Level level, short roomIdx)
    {
        var room = level.Rooms[roomIdx];

        foreach (var root in new[] { 30, 33, 36, 61, 83, 105, 127, 154, 169, 168, 166, 165, 163 })
        {
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[2]]);
            yield return CreateFace(roomIdx, roomIdx, 31, TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[root].Vertices[root == 33 ? 1 : 3],
                room.Mesh.Rectangles[root].Vertices[2],
                (ushort)(room.Mesh.Vertices.Count - 1),
                root == 30
                    ? room.Mesh.Rectangles[31].Vertices[3]
                    : (ushort)(room.Mesh.Vertices.Count - 2),
            });
        }

        foreach (var root in new[] { 31, 55, 77, 99, 121, 146, 143, 138, 139 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, roomIdx, 31, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }

        foreach (var root in new[] { 27, 33, 57, 53, 75, 79, 97, 101, 123, 119, 151, 148, 144, 141, 136 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, roomIdx, 33, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }
    }

    private static IEnumerable<TRRoomTextureEdit> CreateRoom8Edits(TR2Level level, short roomIdx)
    {
        var room = level.Rooms[roomIdx];

        foreach (var root in new[] { 25, 29, 48, 46, 61, 63, 76, 78, 91, 93, 114, 112, 110, 108, 105 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, roomIdx, 29, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }
    }
}
