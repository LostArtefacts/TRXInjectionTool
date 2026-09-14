using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Items;

public class TR2LivingItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.LQ}");
        var data = InjectionData.Create(TRGameVersion.TR2, InjectionType.ItemRotation, "living_itemrots");
        CreateDefaultTests(data, TR2LevelNames.LQ);

        data.ItemPosEdits =
        [
            SetAngle(level, 2, -32768),
        ];

        return [data];
    }
}
