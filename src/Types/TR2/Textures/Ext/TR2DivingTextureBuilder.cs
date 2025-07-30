using TRLevelControl.Model;
using TRXInjectionTool.Actions;

namespace TRXInjectionTool.Types.TR2.Textures;

public partial class TR2DivingTextureBuilder
{
    private static IEnumerable<TRRoomTextureEdit> FixCatwalks(TR2Level level)
    {
        foreach (var edit in CreateRoom52Edits(level)) yield return edit;
        foreach (var edit in CreateRoom54Edits(level)) yield return edit;
        foreach (var edit in TR2Cut3TextureBuilder.FixCatwalks(level, 76, 77)) yield return edit;
    }

    private static IEnumerable<TRRoomTextureEdit> CreateRoom52Edits(TR2Level level)
    {
        const short roomIdx = 52;
        var room = level.Rooms[roomIdx];

        foreach (var root in new[] { 10, 28, 40, 51, 61, 71, 84, 94, 104, 114, 124, 134 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 52, 10, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }
    }

    private static IEnumerable<TRRoomTextureEdit> CreateRoom54Edits(TR2Level level)
    {
        const short roomIdx = 54;
        var room = level.Rooms[roomIdx];

        {
            yield return CreateFace(roomIdx, 54, 7, TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[6].Vertices[2],
                room.Mesh.Rectangles[6].Vertices[1],
                room.Mesh.Rectangles[9].Vertices[2],
                room.Mesh.Rectangles[7].Vertices[3],
            });
            yield return CreateFace(roomIdx, 54, 7, TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[99].Vertices[3],
                room.Mesh.Rectangles[99].Vertices[0],
                room.Mesh.Rectangles[100].Vertices[2],
                room.Mesh.Rectangles[101].Vertices[3],
            });
            yield return CreateFace(roomIdx, 54, 7, TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[51].Vertices[3],
                room.Mesh.Rectangles[51].Vertices[2],
                room.Mesh.Rectangles[36].Vertices[2],
                room.Mesh.Rectangles[76].Vertices[3],
            });
        }

        foreach (var root in new[] { 7, 16, 21, 26, 31, 36, 76, 85, 90, 95, 100, 9, 17, 22, 27, 32, 37, 60, 78, 86, 91, 96, 101 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 54, 7, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }

        foreach (var root in new[] { 5, 15, 20, 25, 30, 35, 55, 74, 84, 89, 94, 99 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, 54, 5, TRMeshFaceType.TexturedQuad, new[]
            {
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            });
        }

        {
            // Fix stretching
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[44].Vertices[1]]);
            yield return CreateVertex(roomIdx, room,
                room.Mesh.Vertices[room.Mesh.Rectangles[44].Vertices[0]]);

            yield return CreateQuadShift(roomIdx, 68, new()
            {
                new(0, (ushort)(room.Mesh.Vertices.Count - 2)),
                new(1, room.Mesh.Rectangles[70].Vertices[0]),
            });
            yield return CreateQuadShift(roomIdx, 44, new()
            {
                new(0, (ushort)(room.Mesh.Vertices.Count - 1)),
                new(1, (ushort)(room.Mesh.Vertices.Count - 2)),
            });
            yield return CreateQuadShift(roomIdx, 42, new()
            {
                new(0, room.Mesh.Rectangles[48].Vertices[1]),
                new(1, (ushort)(room.Mesh.Vertices.Count - 1)),
            });

            yield return CreateFace(roomIdx, 54, 7, TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[68].Vertices[0], room.Mesh.Rectangles[68].Vertices[1],
                room.Mesh.Rectangles[70].Vertices[0], (ushort)(room.Mesh.Vertices.Count - 2),
            });
            yield return CreateFace(roomIdx, 54, 7, TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[44].Vertices[0], room.Mesh.Rectangles[44].Vertices[1],
                (ushort)(room.Mesh.Vertices.Count - 2), (ushort)(room.Mesh.Vertices.Count - 1),
            });
            yield return CreateFace(roomIdx, 54, 7, TRMeshFaceType.TexturedQuad, new[]
            {
                room.Mesh.Rectangles[42].Vertices[0], room.Mesh.Rectangles[42].Vertices[1],
                (ushort)(room.Mesh.Vertices.Count - 1), room.Mesh.Rectangles[48].Vertices[1],
            });
        }
    }
}
