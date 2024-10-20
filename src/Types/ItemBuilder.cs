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
}
