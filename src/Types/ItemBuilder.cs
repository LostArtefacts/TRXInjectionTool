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
}
