using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Items;

public class TR2VegasItemBuilder : ItemBuilder
{
    public override string ID => "vegas_itemrots";

    public override List<InjectionData> Build()
    {
        var vegas = _control2.Read($"Resources/{TR2LevelNames.VEGAS}");
        var data = InjectionData.Create(TRGameVersion.TR2, InjectionType.ItemRotation, ID);
        CreateDefaultTests(data, TR2LevelNames.VEGAS);

        data.ItemPosEdits =
        [
            SetAngle(vegas, 7, -16384),
            SetAngle(vegas, 10, -32768),
            SetAngle(vegas, 16, 16384),
            SetAngle(vegas, 42, 16384),
            SetAngle(vegas, 43, 16384),
            SetAngle(vegas, 45, -16384),
            SetAngle(vegas, 48, -16384),
            SetAngle(vegas, 55, -32768),
            SetAngle(vegas, 76, -16384),
            SetAngle(vegas, 80, -32768),
            SetAngle(vegas, 81, -32768),
            SetAngle(vegas, 82, -32768),
            SetAngle(vegas, 118, -32768),
            SetAngle(vegas, 134, -32768),
            SetAngle(vegas, 162, -32768),
            SetAngle(vegas, 167, -16384),
            SetAngle(vegas, 168, 16384),
        ];

        return [data];
    }
}
