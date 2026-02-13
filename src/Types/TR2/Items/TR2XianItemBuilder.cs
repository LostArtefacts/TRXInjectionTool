using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Items;

public class TR2XianItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level xian = _control2.Read($"Resources/{TR2LevelNames.XIAN}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.ItemRotation, "xian_itemrots");
        CreateDefaultTests(data, TR2LevelNames.XIAN);

        data.ItemPosEdits =
        [
            SetAngle(xian, 0, 16384),
            SetAngle(xian, 88, 16384),
            SetAngle(xian, 103, -16384),
            SetAngle(xian, 137, -32768),
            SetAngle(xian, 160, 16384),
            SetAngle(xian, 217, 16384),
        ];

        // Move switches and spike ceilings into their correct rooms.
        data.ItemPosEdits.AddRange(xian.Entities
            .Select((item, idx) => new { item, idx })
            .Where(x => x.item.Room == 81 && !TR2TypeUtilities.IsAnyPickupType(x.item.TypeID))
            .Select(x =>
            {
                var room = x.item.TypeID == TR2Type.WallSwitch ? 74 : 82;
                return MoveToRoom(xian, (short)x.idx, (short)room);
            }));

        return [data];
    }
}
