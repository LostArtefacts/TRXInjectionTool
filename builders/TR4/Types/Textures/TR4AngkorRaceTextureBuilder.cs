using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR4.Textures;

public class TR4AngkorRaceTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR4, InjectionType.TextureFix, "race_textures");
        CreateDefaultTests(data, $"TR4/{TR4LevelNames.IRIS_RACE}");
        var level = _control4.Read($"Resources/TR4/{TR4LevelNames.IRIS_RACE}");
        data.RoomEdits.AddRange(FixRoom31(level));
        data.RoomEdits.AddRange(FixRoom99_103(level));
        data.RoomEdits.Add(FixRoom4(level));

        return [data];
    }

    private static IEnumerable<TRRoomTextureEdit> FixRoom31(TR4Level level)
    {
        const short roomIdx = 31;
        var room = level.Rooms[roomIdx];
        var vtxPos = new List<ushort>();

        TRRoomVertexCreate MakeVertex(int faceIdx, int vertIdx, short zShift)
        {
            var vtx = room.Mesh.Vertices[room.Mesh.Rectangles[faceIdx].Vertices[vertIdx]];
            vtxPos.Add((ushort)room.Mesh.Vertices.Count);
            var vertex = CreateVertex(roomIdx, room, vtx, shift: 0);
            vertex.Vertex.Vertex.Z += zShift;
            return vertex;
        }

        for (int i = 1; i <= 3; i++)
        {
            yield return MakeVertex(18, 2, (short)(i * TRConsts.Step4));
            yield return MakeVertex(18, 3, (short)(i * TRConsts.Step4));
        }

        // R1
        yield return CreateFace(roomIdx, 26, 0, TRMeshFaceType.TexturedQuad,
        [
            room.Mesh.Rectangles[0].Vertices[2],
            vtxPos[0],
            room.Mesh.Rectangles[18].Vertices[2],
            room.Mesh.Rectangles[0].Vertices[3],
        ]);
        yield return CreateFace(roomIdx, 26, 22, TRMeshFaceType.TexturedQuad,
        [
            vtxPos[0], vtxPos[1],            
            room.Mesh.Rectangles[18].Vertices[3],
            room.Mesh.Rectangles[18].Vertices[2],
        ]);
        yield return CreateFace(roomIdx, 26, 32, TRMeshFaceType.TexturedQuad,
        [
            vtxPos[1],
            room.Mesh.Rectangles[30].Vertices[3],
            room.Mesh.Rectangles[30].Vertices[2],
            room.Mesh.Rectangles[24].Vertices[2],
        ]);
        // R2
        yield return CreateFace(roomIdx, 26, 7, TRMeshFaceType.TexturedQuad,
        [
            room.Mesh.Rectangles[6].Vertices[2],
            vtxPos[2], vtxPos[0],
            room.Mesh.Rectangles[6].Vertices[3],
        ]);
        yield return CreateFace(roomIdx, 26, 26, TRMeshFaceType.TexturedQuad,
        [
            vtxPos[2], vtxPos[3],
            vtxPos[1], vtxPos[0],
        ]);
        yield return CreateFace(roomIdx, 26, 36, TRMeshFaceType.TexturedQuad,
        [
            vtxPos[3],
            room.Mesh.Rectangles[33].Vertices[3],
            room.Mesh.Rectangles[33].Vertices[2],
            vtxPos[1],
        ]);
        // R3
        yield return CreateFace(roomIdx, 26, 11, TRMeshFaceType.TexturedQuad,
        [
            room.Mesh.Rectangles[9].Vertices[2],
            vtxPos[4], vtxPos[2],
            room.Mesh.Rectangles[9].Vertices[3],
        ]);
        yield return CreateFace(roomIdx, 26, 27, TRMeshFaceType.TexturedQuad,
        [
            vtxPos[4], vtxPos[5],
            vtxPos[3], vtxPos[2],
        ]);
        yield return CreateFace(roomIdx, 26, 37, TRMeshFaceType.TexturedQuad,
        [
            vtxPos[5],
            room.Mesh.Rectangles[36].Vertices[3],
            room.Mesh.Rectangles[36].Vertices[2],
            vtxPos[3],
        ]);
        // R4
        yield return CreateFace(roomIdx, 26, 15, TRMeshFaceType.TexturedQuad,
        [
            room.Mesh.Rectangles[12].Vertices[2],
            room.Mesh.Rectangles[15].Vertices[2],
            vtxPos[4],
            room.Mesh.Rectangles[12].Vertices[3],
        ]);
        yield return CreateFace(roomIdx, 26, 28, TRMeshFaceType.TexturedQuad,
        [
            room.Mesh.Rectangles[21].Vertices[3],
            room.Mesh.Rectangles[21].Vertices[2],
            vtxPos[5], vtxPos[4],
        ]);
        yield return CreateFace(roomIdx, 26, 38, TRMeshFaceType.TexturedQuad,
        [
            room.Mesh.Rectangles[27].Vertices[3],
            room.Mesh.Rectangles[27].Vertices[2],
            room.Mesh.Rectangles[39].Vertices[2],
            vtxPos[5],
        ]);

        for (int i = 0; i < 12; i++)
        {
            yield return new TRRoomTextureDoubleSided
            {
                RoomIndex = roomIdx,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = (short)(room.Mesh.Rectangles.Count + i),
                DoubleSided = true,
            };
        }
    }

    private static IEnumerable<TRRoomTextureEdit> FixRoom99_103(TR4Level level)
    {
        var room = level.Rooms[99];
        yield return CreateFace(99, 99, 12, TRMeshFaceType.TexturedTriangle,
        [
            room.Mesh.Rectangles[113].Vertices[2],
            room.Mesh.Triangles[12].Vertices[1],
            room.Mesh.Triangles[12].Vertices[0],
        ]);

        room = level.Rooms[103];
        yield return CreateFace(103, 103, 103, TRMeshFaceType.TexturedQuad,
        [
            room.Mesh.Rectangles[107].Vertices[1],
            room.Mesh.Rectangles[103].Vertices[0],
            room.Mesh.Rectangles[103].Vertices[3],
            room.Mesh.Rectangles[107].Vertices[2],
        ]);
        yield return CreateFace(103, 99, 20, TRMeshFaceType.TexturedQuad,
        [
            room.Mesh.Rectangles[107].Vertices[2],
            room.Mesh.Rectangles[103].Vertices[3],
            room.Mesh.Triangles[35].Vertices[1],
            room.Mesh.Triangles[41].Vertices[0],
        ]);
    }

    private static TRRoomTextureReface FixRoom4(TR4Level level)
    {
        var tex = level.Rooms[4].Mesh.Triangles[50].Texture;
        return Reface(level, 4, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, tex, 52);
    }
}
