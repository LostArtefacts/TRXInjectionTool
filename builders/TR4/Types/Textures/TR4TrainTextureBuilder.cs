using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR4.Textures;

public class TR4TrainTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR4, InjectionType.TextureFix, "train_textures");
        CreateDefaultTests(data, $"TR4/{TR4LevelNames.TRAIN}");
        var level = _control4.Read($"Resources/TR4/{TR4LevelNames.TRAIN}");
        data.RoomEdits.AddRange(FixConnectors(level, 101, 51));
        data.RoomEdits.AddRange(FixConnectors(level, 92, 20));

        return [data];
    }

    // Moves each carriage connector of a flipped room to the position of
    // the connector with the same angle in the base room.
    private static IEnumerable<TRRoomTextureEdit> FixConnectors(TR4Level level, short baseRoom, short flipRoom)
    {
        var connectorID = TR4Type.SceneryBase + 32;
        var baseConnectors = level.Rooms[baseRoom].StaticMeshes.Where(s => s.ID == connectorID).ToList();
        var flipStatics = level.Rooms[flipRoom].StaticMeshes;

        for (int i = 0; i < flipStatics.Count; i++)
        {
            if (flipStatics[i].ID != connectorID)
            {
                continue;
            }

            var connector = baseConnectors.First(s => s.Angle == flipStatics[i].Angle);
            yield return new TRRoomStatic3DEdit
            {
                RoomIndex = flipRoom,
                MeshIndex = i,
                StaticMesh = new TR1RoomStaticMesh
                {
                    X = connector.X,
                    Y = connector.Y,
                    Z = connector.Z,
                    Angle = connector.Angle,
                    Intensity = connector.Colour,
                },
            };
        }
    }
}
