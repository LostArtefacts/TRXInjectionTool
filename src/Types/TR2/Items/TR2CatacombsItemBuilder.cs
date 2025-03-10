using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Items;

public class TR2CatacombsItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level catacombs = _control2.Read($"Resources/{TR2LevelNames.COT}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.ItemRotation, "catacombs_itemrots");
        CreateDefaultTests(data, TR2LevelNames.COT);

        data.ItemEdits = new()
        {
            SetAngle(catacombs, 9, 16384),
            SetAngle(catacombs, 47, -32768),
            SetAngle(catacombs, 112, -32768),
        };

        return new() { data };
    }
}
