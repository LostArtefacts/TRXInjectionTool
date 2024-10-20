using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Items;

public class TR1FollyItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level folly = _control1.Read($@"Resources\{TR1LevelNames.FOLLY}");
        InjectionData data = InjectionData.Create(InjectionType.ItemRotation, "folly_itemrots");

        data.ItemEdits = new()
        {
            SetAngle(folly, 71, -16384),
            SetAngle(folly, 89, -16384),
            SetAngle(folly, 98, -16384),
            SetAngle(folly, 106, -32768),
        };

        return new() { data };
    }
}
