using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Items;

public class TR1HiveItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level hive = _control1.Read($"Resources/{TR1LevelNames.HIVE}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.ItemRotation, "hive_itemrots");

        data.ItemEdits = new()
        {
            SetAngle(hive, 6, -16384),
            SetAngle(hive, 179, -16384),
        };

        return new() { data };
    }
}
