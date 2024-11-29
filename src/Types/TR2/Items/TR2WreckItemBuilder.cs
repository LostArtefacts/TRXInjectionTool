using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Items;

public class TR2WreckItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level wreck = _control2.Read($"Resources/{TR2LevelNames.DORIA}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.ItemRotation, "wreck_itemrots");

        data.ItemEdits = new()
        {
            SetAngle(wreck, 64, -32768),
            SetAngle(wreck, 149, 16384),
            SetAngle(wreck, 183, 16384),
        };

        return new() { data };
    }
}
