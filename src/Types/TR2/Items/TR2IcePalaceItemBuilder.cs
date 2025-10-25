using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Items;

public class TR2IcePalaceItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level palace = _control2.Read($"Resources/{TR2LevelNames.CHICKEN}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.ItemRotation, "palace_itemrots");
        CreateDefaultTests(data, TR2LevelNames.CHICKEN);

        data.ItemPosEdits = new()
        {
            SetAngle(palace, 90, -32768),
            SetAngle(palace, 93, -32768),
            SetAngle(palace, 151, -16384),
        };

        return new() { data };
    }
}
