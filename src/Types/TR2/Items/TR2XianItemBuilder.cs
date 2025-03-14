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

        data.ItemEdits = new()
        {
            SetAngle(xian, 0, 16384),
            SetAngle(xian, 88, 16384),
            SetAngle(xian, 103, -16384),
            SetAngle(xian, 137, -32768),
            SetAngle(xian, 160, 16384),
            SetAngle(xian, 217, 16384),
        };

        return new() { data };
    }
}
