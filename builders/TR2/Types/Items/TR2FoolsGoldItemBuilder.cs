using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Items;

public class TR2FoolsGoldItemBuilder : ItemBuilder
{
    public override string ID => "fools_itemrots";

    public override List<InjectionData> Build()
    {
        var foolsGold = _control2.Read($"Resources/{TR2LevelNames.FOOLGOLD}");
        var data = InjectionData.Create(TRGameVersion.TR2, InjectionType.ItemRotation, ID);
        CreateDefaultTests(data, TR2LevelNames.FOOLGOLD);

        data.ItemPosEdits =
        [
            SetAngle(foolsGold, 29, 16384),
            SetAngle(foolsGold, 103, -16384),
            SetAngle(foolsGold, 104, -16384),
            SetAngle(foolsGold, 130, -32768),
            SetAngle(foolsGold, 152, -32768),
            SetAngle(foolsGold, 172, -32768),
            SetAngle(foolsGold, 173, -32768),
            SetAngle(foolsGold, 184, -16384),
            SetAngle(foolsGold, 185, -32768),
            SetAngle(foolsGold, 189, -32768),
        ];

        return [data];
    }
}
