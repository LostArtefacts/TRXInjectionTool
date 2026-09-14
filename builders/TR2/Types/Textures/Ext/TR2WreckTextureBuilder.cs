using TRLevelControl.Model;
using TRXInjectionTool.Actions;

namespace TRXInjectionTool.Types.TR2.Textures;

public partial class TR2WreckTextureBuilder
{
    private static IEnumerable<TRRoomTextureEdit> FixCatwalks(TR2Level level)
    {
        foreach (var edit in CreateRoom71Edits(level)) yield return edit;
        foreach (var edit in CreateDoubleSidedEdits(level, 1)) yield return edit;
        foreach (var edit in CreateDoubleSidedEdits(level, 71)) yield return edit;
        foreach (var edit in CreateDoubleSidedEdits(level, 72)) yield return edit;
    }

    private static IEnumerable<TRRoomTextureEdit> CreateRoom71Edits(TR2Level level)
    {
        const short roomIdx = 71;
        var room = level.Rooms[roomIdx];
        var source = GetSource(level, TRMeshFaceType.TexturedQuad, 1866);

        SimulateLighting(room, level.Rooms[72]);

        {
            var vtx = room.Mesh.Vertices[room.Mesh.Rectangles[41].Vertices[2]];
            yield return CreateVertex(roomIdx, room, vtx);
            vtx = room.Mesh.Vertices[room.Mesh.Rectangles[41].Vertices[3]];
            yield return CreateVertex(roomIdx, room, vtx);
            yield return CreateFace(roomIdx, source.Room, (ushort)source.Face, TRMeshFaceType.TexturedQuad,
            [
                room.Mesh.Rectangles[41].Vertices[2],
                room.Mesh.Rectangles[41].Vertices[3],
                (ushort)(room.Mesh.Vertices.Count - 1),
                (ushort)(room.Mesh.Vertices.Count - 2),
            ]);
        }

        foreach (var root in new[] { 60, 75, 72, 69, 66, 63, 46, 31, 5, 2, 10, 15, 18, 23, 26 })
        {
            var vtx = room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[2]];
            yield return CreateVertex(roomIdx, room, vtx);
            yield return CreateFace(roomIdx, source.Room, (ushort)source.Face, TRMeshFaceType.TexturedQuad,
            [
                room.Mesh.Rectangles[root].Vertices[3],
                room.Mesh.Rectangles[root].Vertices[2],
                (ushort)(room.Mesh.Vertices.Count - 1),
                (ushort)(room.Mesh.Vertices.Count - 2),
            ]);
        }

        foreach (var root in new[] { 35, 36, 40, 53 })
        {
            var face = room.Mesh.Rectangles[root];
            yield return CreateFace(roomIdx, roomIdx, 35, TRMeshFaceType.TexturedQuad,
            [
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            ]);
        }
    }

    private static IEnumerable<TRRoomTextureEdit> CreateDoubleSidedEdits(TR2Level level, short roomIdx)
    {
        var room = level.Rooms[roomIdx];
        foreach (var face in room.Mesh.Rectangles.Where(f => f.Texture == 1723))
        {
            yield return CreateFace(roomIdx, roomIdx, (ushort)room.Mesh.Rectangles.IndexOf(face), TRMeshFaceType.TexturedQuad,
            [
                face.Vertices[1], face.Vertices[0], face.Vertices[3], face.Vertices[2]
            ]);
        }
    }

    private static void SimulateLighting(TR2Room topRoom, TR2Room bottomRoom)
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
