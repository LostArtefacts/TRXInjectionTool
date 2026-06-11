using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Textures;

public class TR3NevadaTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.TextureFix, "nevada_textures");
        CreateDefaultTests(data, $"TR3/{TR3LevelNames.NEVADA}");
        data.RoomEdits.AddRange(FixWaterfallVertices());
        return [data];
    }

    private static IEnumerable<TRRoomTextureEdit> FixWaterfallVertices()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.NEVADA}");
        var room = level.Rooms[31];

        TRRoomVertxFlagChange StopMovement(int face, int vertex)
        {
            var idx = room.Mesh.Rectangles[face].Vertices[vertex];
            return new TRRoomVertxFlagChange
            {
                RoomIndex = 31,
                VertexIndex = idx,
                Flags = (ushort)(room.Mesh.Vertices[idx].Attributes | 0x8000),
            };
        }

        yield return StopMovement(2, 0);
        yield return StopMovement(2, 1);
        yield return StopMovement(7, 1);
        yield return StopMovement(13, 0);
        yield return StopMovement(13, 1);
    }
}
