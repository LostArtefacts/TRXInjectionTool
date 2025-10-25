using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Items;

public class TR2FurnaceItemBuilder : ItemBuilder
{
    public override string ID => "furnace_itemrots";

    public override List<InjectionData> Build()
    {
        TR2Level furnace = _control2.Read($"Resources/{TR2LevelNames.FURNACE}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.ItemRotation, ID);
        CreateDefaultTests(data, TR2LevelNames.FURNACE);

        data.ItemPosEdits = new()
        {
            SetAngle(furnace, 7, 16384),
            SetAngle(furnace, 9, -32768),
            SetAngle(furnace, 22, -32768),
            SetAngle(furnace, 57, 16384),
            SetAngle(furnace, 73, -32768),
            SetAngle(furnace, 82, -32768),
            SetAngle(furnace, 88, 16384),
            SetAngle(furnace, 172, 16384),
        };

        return new() { data };
    }
}
