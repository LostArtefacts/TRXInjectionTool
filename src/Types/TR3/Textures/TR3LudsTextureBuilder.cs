using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Textures;

public class TR3LudsTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.TextureFix, "luds_textures");
        TR3AldwychTextureBuilder.FixStaircaseMesh(data, 49);
        data.RoomEdits.AddRange(FixRoom77());
        return [data];
    }

    private static IEnumerable<TRRoomTextureEdit> FixRoom77()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.LUDS}");

        const short roomIdx = 77;
        var room = level.Rooms[roomIdx];
        var vtxPos = new List<ushort>();

        TRRoomVertexCreate MakeVertex(int faceIdx, int vertIdx)
        {
            var vtx = room.Mesh.Vertices[room.Mesh.Rectangles[faceIdx].Vertices[vertIdx]];
            vtxPos.Add((ushort)room.Mesh.Vertices.Count);
            var vertex = CreateVertex(roomIdx, room, vtx, shift: 0);
            vertex.Vertex.Vertex.Z += TRConsts.Step4;
            return vertex;
        }

        {
            yield return MakeVertex(37, 1);
            yield return CreateFace(roomIdx, 73, 1, TRMeshFaceType.TexturedQuad,
            [
                vtxPos[0],
                room.Mesh.Rectangles[37].Vertices[1],
                room.Mesh.Rectangles[37].Vertices[0],
                room.Mesh.Rectangles[94].Vertices[0],
            ]);
            yield return CreateFace(roomIdx, 73, 1, TRMeshFaceType.TexturedQuad,
            [
                room.Mesh.Rectangles[44].Vertices[0],
                vtxPos[0],
                room.Mesh.Rectangles[97].Vertices[1],
                room.Mesh.Rectangles[44].Vertices[1],
            ]);
        }
        {
            yield return MakeVertex(48, 1);
            yield return CreateFace(roomIdx, 73, 1, TRMeshFaceType.TexturedQuad,
            [
                vtxPos[1],
                room.Mesh.Rectangles[48].Vertices[1],
                room.Mesh.Rectangles[48].Vertices[0],
                room.Mesh.Rectangles[100].Vertices[0],
            ]);
            yield return CreateFace(roomIdx, 73, 1, TRMeshFaceType.TexturedQuad,
            [
                room.Mesh.Rectangles[55].Vertices[0],
                vtxPos[1],
                room.Mesh.Rectangles[103].Vertices[1],
                room.Mesh.Rectangles[55].Vertices[1],
            ]);
        }
        {
            yield return MakeVertex(59, 1);
            yield return CreateFace(roomIdx, 73, 1, TRMeshFaceType.TexturedQuad,
            [
                vtxPos[2],
                room.Mesh.Rectangles[59].Vertices[1],
                room.Mesh.Rectangles[59].Vertices[0],
                room.Mesh.Rectangles[105].Vertices[0],
            ]);
            yield return CreateFace(roomIdx, 73, 1, TRMeshFaceType.TexturedQuad,
            [
                room.Mesh.Rectangles[66].Vertices[0],
                vtxPos[2],
                room.Mesh.Rectangles[106].Vertices[1],
                room.Mesh.Rectangles[66].Vertices[1],
            ]);
        }

        {
            yield return Reface(level, roomIdx, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 2105, 69);
            yield return Rotate(roomIdx, TRMeshFaceType.TexturedQuad, 69, 1);
            yield return CreateFace(roomIdx, 73, 1, TRMeshFaceType.TexturedQuad,
            [
                room.Mesh.Rectangles[78].Vertices[0],
                room.Mesh.Rectangles[69].Vertices[1],
                room.Mesh.Rectangles[69].Vertices[0],
                room.Mesh.Rectangles[78].Vertices[1],
            ]);
        }

        {
            yield return Reface(level, roomIdx, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 2105, 81);
            yield return Reface(level, roomIdx, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 2105, 86);
            yield return Rotate(roomIdx, TRMeshFaceType.TexturedQuad, 81, 3);
            yield return Rotate(roomIdx, TRMeshFaceType.TexturedQuad, 86, 1);
        }
    }
}
