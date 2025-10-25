using TRLevelControl.Model;
using TRXInjectionTool.Actions;

namespace TRXInjectionTool.Types;

public abstract class ItemBuilder : InjectionBuilder
{
    protected static TRItemPosEdit SetAngle(TR1Level level, short itemIndex, short angle)
    {
        TR1Entity item = level.Entities[itemIndex].Clone() as TR1Entity;
        item.Angle = angle;

        return new()
        {
            Index = itemIndex,
            Item = item,
        };
    }

    public static TRItemPosEdit SetAngle(TR2Level level, short itemIndex, short angle)
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
