using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Items;

public class TR2TibetItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level tibet = _control2.Read($"Resources/{TR2LevelNames.TIBET}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.ItemRotation, "tibet_itemrots");
        CreateDefaultTests(data, TR2LevelNames.TIBET);

        data.ItemPosEdits = new()
        {
            SetAngle(tibet, 0, -32768),
            SetAngle(tibet, 48, 16384),
            SetAngle(tibet, 68, 16384),
        };

        return new() { data };
    }
}
