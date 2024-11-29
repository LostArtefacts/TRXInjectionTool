using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Items;

public class TR2HSHItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level house = _control2.Read($"Resources/{TR2LevelNames.HOME}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.ItemRotation, "house_itemrots");

        // Everything in the closet.
        foreach (TR2Entity item in house.Entities.Where(e => e.Room == 57))
        {
            short angle = (short)(item.TypeID == TR2Type.Shotgun_S_P ? -32768 : 16384);
            data.ItemEdits.Add(SetAngle(house, (short)house.Entities.IndexOf(item), angle));
        }

        return new() { data };
    }
}
