using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Items;

public class TR2CatacombsItemBuilder : ItemBuilder
{
    public override string ID => "catacombs_itemrots";

    public override List<InjectionData> Build()
    {
        TR2Level catacombs = _control2.Read($"Resources/{TR2LevelNames.COT}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.ItemRotation, ID);
        CreateDefaultTests(data, TR2LevelNames.COT);

        data.ItemEdits = new()
        {
            SetAngle(catacombs, 9, 16384),
            SetAngle(catacombs, 47, -32768),
            SetAngle(catacombs, 85, 16384),
            SetAngle(catacombs, 86, 16384),
            SetAngle(catacombs, 112, -32768),
            SetAngle(catacombs, 136, 16384),
        };

        return new() { data };
    }
}
