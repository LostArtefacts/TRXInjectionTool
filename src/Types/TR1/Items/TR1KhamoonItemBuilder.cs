using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Items;

public class TR1KhamoonItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level khamoon = _control1.Read($"Resources/{TR1LevelNames.KHAMOON}");
        InjectionData data = InjectionData.Create(InjectionType.ItemRotation, "khamoon_itemrots");

        data.ItemEdits = new()
        {
            SetAngle(khamoon, 33, 16384),
            SetAngle(khamoon, 34, -16384),
        };

        return new() { data };
    }
}
