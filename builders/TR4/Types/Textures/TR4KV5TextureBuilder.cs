using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR4.Textures;

public class TR4KV5TextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR4, InjectionType.TextureFix, "kv5_textures");
        CreateDefaultTests(data, $"TR4/{TR4LevelNames.KV5}");
        var level = _control4.Read($"Resources/TR4/{TR4LevelNames.KV5}");
        data.RoomEdits.AddRange(CreateRefacings(level));
        data.RoomEdits.AddRange(CreateRotations());
        data.RoomEdits.AddRange(CreateFillers(level));
        data.RoomEdits.AddRange(FixRoom69(level));

        return [data];
    }

    private static IEnumerable<TRRoomTextureReface> CreateRefacings(TR4Level level)
    {
        foreach (var face in new[] { 1, 3, 5, 8, 10, 11 })
        {
            yield return Reface(level, 4, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1623, (short)face);
        }

        yield return Reface(level, 50, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1810, 23);
        yield return Reface(level, 65, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1812, 3);

        yield return Reface(level, 37, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1723, 79);
        yield return Reface(level, 37, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1760, 82);
    }

    private static IEnumerable<TRRoomTextureRotate> CreateRotations()
    {
        var rand = new Random(20261020);
        foreach (var face in new[] { 1, 3, 5, 8, 10, 11 })
        {
            var rot = rand.Next(0, 4);
            if (rot != 0)
            {
                yield return Rotate(4, TRMeshFaceType.TexturedQuad, (short)face, (byte)rot);
            }
        }
    }

    private static IEnumerable<TRRoomTextureEdit> CreateFillers(TR4Level level)
    {
        yield return CreateFace(2, 2, 8, TRMeshFaceType.TexturedTriangle,
        [
            level.Rooms[2].Mesh.Rectangles[19].Vertices[1],
            level.Rooms[2].Mesh.Rectangles[19].Vertices[0],
            level.Rooms[2].Mesh.Rectangles[40].Vertices[3],
        ]);

        var vtx = level.Rooms[77].Mesh.Vertices[level.Rooms[77].Mesh.Rectangles[53].Vertices[0]];
        yield return CreateVertex(77, level.Rooms[77], vtx, shift: -512);
        yield return CreateFace(77, 13, 36, TRMeshFaceType.TexturedTriangle,
        [
            (ushort)(level.Rooms[77].Mesh.Vertices.Count - 1),
            level.Rooms[77].Mesh.Rectangles[54].Vertices[3],
            level.Rooms[77].Mesh.Rectangles[53].Vertices[0],
        ]);
    }

    private static IEnumerable<TRRoomTextureEdit> FixRoom69(TR4Level level)
    {
        const short roomIdx = 69;
        var room = level.Rooms[roomIdx];
        var vtxPos = new List<ushort>();

        TRRoomVertexCreate MakeVertex(int faceIdx, int vertIdx, short yShift, short zShift)
        {
            var vtx = room.Mesh.Vertices[room.Mesh.Rectangles[faceIdx].Vertices[vertIdx]];
            vtxPos.Add((ushort)room.Mesh.Vertices.Count);
            var vertex = CreateVertex(roomIdx, room, vtx, shift: yShift);
            vertex.Vertex.Vertex.Z += zShift;
            return vertex;
        }

        yield return MakeVertex(5, 2, -TRConsts.Step4, 0);

        for (int i = 1; i <= 6; i++)
        {
            var z = (short)(i * TRConsts.Step4);
            yield return MakeVertex(5, 2, -TRConsts.Step4, z);
            if (i < 6)
            {
                yield return MakeVertex(5, 2, 0, z);
            }
        }

        List<ushort[]> quadVerts =
        [
            [vtxPos[0], vtxPos[1], vtxPos[2], room.Mesh.Rectangles[5].Vertices[2]],
            [vtxPos[1], vtxPos[3], vtxPos[4], vtxPos[2]],
            [vtxPos[3], vtxPos[5], vtxPos[6], vtxPos[4]],
            [vtxPos[5], vtxPos[7], vtxPos[8], vtxPos[6]],
            [vtxPos[7], vtxPos[9], vtxPos[10], vtxPos[8]],
            [vtxPos[9], vtxPos[11], room.Mesh.Rectangles[6].Vertices[3], vtxPos[10]],
        ];

        var source = GetSource(level, TRMeshFaceType.TexturedQuad, 1865);
        foreach (var verts in quadVerts)
        {
            yield return CreateFace(roomIdx, source.Room, (ushort)source.Face, TRMeshFaceType.TexturedQuad, verts);
        }
    }
}
