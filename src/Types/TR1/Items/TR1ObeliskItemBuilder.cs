using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Items;

public class TR1ObeliskItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level obelisk = _control1.Read($@"Resources\{TR1LevelNames.OBELISK}");
        InjectionData data = InjectionData.Create(InjectionType.ItemRotation, "obelisk_itemrots");

        data.ItemEdits = new()
        {
            SetAngle(obelisk, 15, 16384),
            SetAngle(obelisk, 17, -16384),
        };

        return new() { data };
    }
}
