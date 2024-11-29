using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Items;

public class TR2BarkhangItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level barkhang = _control2.Read($"Resources/{TR2LevelNames.MONASTERY}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.ItemRotation, "barkhang_itemrots");

        data.ItemEdits = new()
        {
            SetAngle(barkhang, 31, -16384),
            SetAngle(barkhang, 97, 16384),
            SetAngle(barkhang, 176, 16384),
            SetAngle(barkhang, 202, 16384),
        };

        return new() { data };
    }
}
