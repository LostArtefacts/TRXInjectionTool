using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Items;

public class TR1SanctuaryItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level sanctuary = _control1.Read($"Resources/{TR1LevelNames.SANCTUARY}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.ItemRotation, "sanctuary_itemrots");
        CreateDefaultTests(data, TR1LevelNames.SANCTUARY);

        data.ItemEdits = new()
        {
            SetAngle(sanctuary, 38, -16384),
            SetAngle(sanctuary, 54, -16384),
            SetAngle(sanctuary, 67, -16384),
            SetAngle(sanctuary, 73, -16384),
        };

        return new() { data };
    }
}
