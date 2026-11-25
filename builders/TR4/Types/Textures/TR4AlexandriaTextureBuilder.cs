using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR4.Textures;

public class TR4AlexandriaTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR4, InjectionType.TextureFix, "alexandria_textures");
        CreateDefaultTests(data, $"TR4/{TR4LevelNames.ALEXANDRIA}");
        var level = _control4.Read($"Resources/TR4/{TR4LevelNames.ALEXANDRIA}");
        data.RoomEdits.AddRange(FixTrainWheels(level));

        return [data];
    }

    // Raises the train wheels to the height of the carriage, as in Desert
    // Railroad.
    private static IEnumerable<TRRoomStatic3DEdit> FixTrainWheels(TR4Level level)
    {
        const short room = 27;
        var carriage = level.Rooms[room].StaticMeshes[2];
        foreach (var index in new[] { 0, 1 })
        {
            var wheels = level.Rooms[room].StaticMeshes[index];
            yield return new()
            {
                RoomIndex = room,
                MeshIndex = (short)index,
                StaticMesh = new()
                {
                    X = wheels.X,
                    Y = carriage.Y,
                    Z = wheels.Z,
                    Angle = wheels.Angle,
                    Intensity = wheels.Colour,
                },
            };
        }
    }
}
