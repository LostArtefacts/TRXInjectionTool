using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Items;

public class TR2VeniceItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.VENICE}");
        var data = InjectionData.Create(TRGameVersion.TR2, InjectionType.ItemRotation, "venice_itemrots");
        CreateDefaultTests(data, TR2LevelNames.VENICE);

        data.ItemPosEdits =
        [
            SetAngle(level, 44, 16384),
            SetAngle(level, 100, 16384),
        ];

        return [data];
    }
}
