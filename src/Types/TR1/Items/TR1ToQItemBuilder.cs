using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Items;

public class TR1ToQItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level qualopec = _control1.Read($"Resources/{TR1LevelNames.QUALOPEC}");
        InjectionData data = InjectionData.Create(InjectionType.ItemRotation, "qualopec_itemrots");

        data.ItemEdits = new()
        {
            SetAngle(qualopec, 20, -16384),
            SetAngle(qualopec, 28, 16384),
        };

        return new() { data };
    }
}
