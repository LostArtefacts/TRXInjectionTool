using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR4.Textures;

public class TR4SemerkhetTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR4, InjectionType.TextureFix, "semerkhet_textures");
        CreateDefaultTests(data, $"TR4/{TR4LevelNames.SEMERKHET}");
        var level = _control4.Read($"Resources/TR4/{TR4LevelNames.SEMERKHET}");
        data.RoomEdits.AddRange(FixRoom94(level));
        data.RoomEdits.AddRange(FixRoom126());

        return [data];
    }

    // Shrinks the stretched wall face to the frieze above the door in room
    // 112. The door covers the wall below, and a face there flickers with it.
    private static IEnumerable<TRRoomTextureEdit> FixRoom94(TR4Level level)
    {
        const short roomIdx = 94;
        var room = level.Rooms[roomIdx];
        var newVertex = (ushort)room.Mesh.Vertices.Count;
        yield return CreateVertex(roomIdx, room, room.Mesh.Vertices[89], room.Mesh.Vertices[88].Lighting);
        yield return CreateQuadShift(roomIdx, 59,
        [
            new() { Index = 2, NewVertexIndex = 88 },
            new() { Index = 3, NewVertexIndex = newVertex },
        ]);
    }

    // Closes the recess that holds the receptacle.
    private static IEnumerable<TRRoomTextureEdit> FixRoom126()
    {
        const short roomIdx = 126;
        foreach (short face in new short[] { 120, 125, 152 })
        {
            yield return DoubleSided(roomIdx, TRMeshFaceType.TexturedQuad, face, true);
        }

        yield return CreateFace(roomIdx, roomIdx, 124, TRMeshFaceType.TexturedQuad, [155, 122, 150, 149]);
        yield return CreateFace(roomIdx, roomIdx, 123, TRMeshFaceType.TexturedQuad, [147, 117, 119, 153]);
    }
}
