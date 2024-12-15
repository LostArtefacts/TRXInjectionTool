using TRLevelControl.Model;
using TRXInjectionTool.Actions;

namespace TRXInjectionTool.Types;

public abstract class ItemBuilder : InjectionBuilder
{
    protected static TRItemEdit SetAngle(TR1Level level, short itemIndex, short angle)
    {
        TR1Entity item = level.Entities[itemIndex].Clone() as TR1Entity;
        item.Angle = angle;

        return new()
        {
            Index = itemIndex,
            Item = item,
        };
    }

    public static TRItemEdit SetAngle(TR2Level level, short itemIndex, short angle)
    {
        // Convert to a TR1Entity
        TR2Entity item = level.Entities[itemIndex];
        return new()
        {
            Index = itemIndex,
            Item = new()
            {
                Angle = angle,
                X = item.X,
                Y = item.Y,
                Z = item.Z,
                Room = item.Room,
            },
        };
    }

    public static TRMeshEdit FixEgyptToppledChair(TR1Type type, TR1Level level)
    {
        TRMesh mesh = level.StaticMeshes[type].Mesh;
        TRMeshEdit edit = new()
        {
            ModelID = (uint)type,
            MeshIndex = 0,
            VertexEdits = new(),
        };

        for (short i = 0; i < mesh.Vertices.Count; i++)
        {
            edit.VertexEdits.Add(new()
            {
                Index = i,
                Change = new()
                {
                    Y = 66,
                }
            });
        }

        return edit;
    }

    public static TRVertexEdit CreateVertexShift(short index, short x = 0, short y = 0, short z = 0)
    {
        return new()
        {
            Index = index,
            Change = new()
            {
                X = x,
                Y = y,
                Z = z,
            }
        };
    }
}
