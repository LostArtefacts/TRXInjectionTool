using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Items;

public class TR2BarkhangItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        var barkhang = _control2.Read($"Resources/{TR2LevelNames.MONASTERY}");
        var data = InjectionData.Create(TRGameVersion.TR2, InjectionType.ItemRotation, "barkhang_itemrots");
        CreateDefaultTests(data, TR2LevelNames.MONASTERY);

        data.ItemPosEdits =
        [
            SetAngle(barkhang, 31, -16384),
            SetAngle(barkhang, 97, 16384),
            SetAngle(barkhang, 168, -32768),
            SetAngle(barkhang, 176, 16384),
            SetAngle(barkhang, 202, 16384),
        ];

        return [data];
    }
}
