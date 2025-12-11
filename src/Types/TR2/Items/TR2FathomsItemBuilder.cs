using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Items;

public class TR2FathomsItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.FATHOMS}");
        var data = InjectionData.Create(TRGameVersion.TR2, InjectionType.ItemRotation, "fathoms_itemrots");
        CreateDefaultTests(data, TR2LevelNames.FATHOMS);

        data.ItemPosEdits =
        [
            SetAngle(level, 42, -32768),
        ];

        return [data];
    }
}
