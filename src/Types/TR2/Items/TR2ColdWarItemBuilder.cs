using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Items;

public class TR2ColdWarItemBuilder : ItemBuilder
{
    public override string ID => "coldwar_itemrots";

    public override List<InjectionData> Build()
    {
        var coldWar = _control2.Read($"Resources/{TR2LevelNames.COLDWAR}");
        var data = InjectionData.Create(TRGameVersion.TR2, InjectionType.ItemRotation, ID);
        CreateDefaultTests(data, TR2LevelNames.COLDWAR);

        data.ItemPosEdits =
        [
            SetAngle(coldWar, 0, -32768),
            SetAngle(coldWar, 70, 16384),
            SetAngle(coldWar, 72, 16384),
            SetAngle(coldWar, 79, 16384),
            SetAngle(coldWar, 80, 16384),
            SetAngle(coldWar, 87, 16384),
            SetAngle(coldWar, 139, -32768),
            SetAngle(coldWar, 140, -32768),
            SetAngle(coldWar, 155, -32768),
        ];

        return [data];
    }
}
