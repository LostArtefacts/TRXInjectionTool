using TRLevelControl.Model;
using TRXInjectionTool.Actions;

namespace TRXInjectionTool.Types;

public abstract class ItemBuilder : InjectionBuilder
{
    protected static TRItemPosEdit CreateItemPosEdit(TRLevelBase level, short itemIndex, Action<ITRLocatable> modAction)
    {
        var item = level switch
        {
            TR1Level l => (ITRLocatable)l.Entities[itemIndex].Clone(),
            TR2Level l => (ITRLocatable)l.Entities[itemIndex].Clone(),
            TR3Level l => (ITRLocatable)l.Entities[itemIndex].Clone(),
            _ => throw new InvalidOperationException("TR1-3 items supported only.")
        };

        modAction(item);

        return new()
        {
            Index = itemIndex,
            Item = new()
            {
                Angle = item.Angle,
                X = item.X,
                Y = item.Y,
                Z = item.Z,
                Room = item.Room,
            },
        };
    }

    public static TRItemPosEdit SetAngle(TRLevelBase level, short itemIndex, short angle)
        => CreateItemPosEdit(level, itemIndex, i => i.Angle = angle);

    public static TRItemPosEdit MoveToRoom(TRLevelBase level, short itemIndex, short room)
        => CreateItemPosEdit(level, itemIndex, i => i.Room = room);

    public static TRMeshEdit FixEgyptToppledChair(TR1Type type, TR1Level level)
    {
        var mesh = level.StaticMeshes[type].Mesh;
        var max = mesh.Vertices.Max(v => v.Y);
        var min = mesh.Vertices.Min(v => v.Y);
        return new()
        {
            ModelID = (uint)type,
            VertexEdits = Enumerable.Range(0, mesh.Vertices.Count).Select(i =>
            {
                var y = mesh.Vertices[i].Y;
                var edit = new TRVertexEdit
                {
                    Index = (short)i,
                    Change = new()
                    {
                        Y = (short)((y == min || y == max) ? (-max - 1) : (-max))
                    }
                };

                if (i == 6)
                {
                    edit.Change.Z = 1;
                }
                else if (i == 7)
                {
                    edit.Change.Z = -3;
                }

                return edit;
            }).ToList(),
        };
    }

    public static TRStaticMeshEdit FixEgyptPillar(TR1Level level)
    {
        var pillar = level.StaticMeshes[TR1Type.SceneryBase + 30];
        pillar.CollisionBox.MinX += 64;
        pillar.CollisionBox.MinZ += 64;
        pillar.CollisionBox.MaxZ -= 64;
        pillar.CollisionBox.MaxX -= 64;
        return new()
        {
            TypeID = 30,
            Mesh = pillar,
        };
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
