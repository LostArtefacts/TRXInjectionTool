using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Textures;

public class TR3FlingTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.TextureFix, "scotland_textures");

        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.FLING}");
        data.RoomEdits.AddRange(CreateShifts(level));

        return [data];
    }

    private static IEnumerable<TRRoomTextureEdit> CreateShifts(TR3Level level)
    {
        var room = level.Rooms[128];
        var vtx = room.Mesh.Vertices[room.Mesh.Rectangles[30].Vertices[2]];
        vtx.UseCaustics = false;
        vtx.UseWaveMovement = false;
        yield return CreateVertex(128, room, vtx, shift: 0);

        var newIdx = (ushort)(room.Mesh.Vertices.Count - 1);
        yield return CreateQuadShift(128, 30,
        [
            new()
            {
                Index = 2,
                NewVertexIndex = newIdx,
            },
        ]);
        yield return CreateQuadShift(128, 22,
        [
            new()
            {
                Index = 3,
                NewVertexIndex = newIdx,
            },
        ]);
        yield return CreateQuadShift(128, 23,
        [
            new()
            {
                Index = 3,
                NewVertexIndex = newIdx,
            },
        ]);
    }
}
