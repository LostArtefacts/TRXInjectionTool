using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Items;

public class TR1CatItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level cat = _control1.Read($@"Resources\{TR1LevelNames.CAT}");
        InjectionData data = InjectionData.Create(InjectionType.ItemRotation, "cat_itemrots");

        data.ItemEdits = new()
        {
            SetAngle(cat, 44, 16384),
            SetAngle(cat, 153, -16384),
            SetAngle(cat, 171, -32768),
        };

        return new() { data };
    }
}
