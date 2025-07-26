using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Items;

public class TR1PyramidItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        var level = _control1.Read($"Resources/{TR1LevelNames.PYRAMID}");
        var data = InjectionData.Create(TRGameVersion.TR1, InjectionType.ItemRotation, "pyramid_itemrots");
        CreateDefaultTests(data, TR1LevelNames.PYRAMID);

        data.ItemEdits.Add(SetAngle(level, 95, -32768));
        data.ItemEdits.Add(SetAngle(level, 96, -32768));
        data.ItemEdits.Add(SetAngle(level, 99, -32768));

        return new() { data };
    }
}
