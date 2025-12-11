using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Items;

public class TR2KingdomItemBuilder : ItemBuilder
{
    public override string ID => "kingdom_itemrots";

    public override List<InjectionData> Build()
    {
        var kingdom = _control2.Read($"Resources/{TR2LevelNames.KINGDOM}");
        var data = InjectionData.Create(TRGameVersion.TR2, InjectionType.ItemRotation, ID);
        CreateDefaultTests(data, TR2LevelNames.KINGDOM);

        data.ItemPosEdits =
        [
            SetAngle(kingdom, 11, -16384),
            SetAngle(kingdom, 12, 16384),
            SetAngle(kingdom, 19, -32768),
            SetAngle(kingdom, 24, -16384),
            SetAngle(kingdom, 25, 16384),
            SetAngle(kingdom, 26, 16384),
            SetAngle(kingdom, 44, 16384),
            SetAngle(kingdom, 73, -16384),
            SetAngle(kingdom, 113, -32768),
            SetAngle(kingdom, 116, -16384),
            SetAngle(kingdom, 121, 16384),
            SetAngle(kingdom, 126, 16384),
            SetAngle(kingdom, 132, -16384),
            SetAngle(kingdom, 133, -32768),
            SetAngle(kingdom, 139, -32768),
            SetAngle(kingdom, 163, -16384),
            SetAngle(kingdom, 168, -32768),
            SetAngle(kingdom, 170, -16384),
        ];

        return [data];
    }
}
