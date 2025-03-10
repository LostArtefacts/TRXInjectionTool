using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Items;

public class TR2DivingItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level diving = _control2.Read($"Resources/{TR2LevelNames.DA}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.ItemRotation, "diving_itemrots");
        CreateDefaultTests(data, TR2LevelNames.DA);

        data.ItemEdits = new()
        {
            SetAngle(diving, 58, -32768),
            SetAngle(diving, 94, -32768),
            SetAngle(diving, 120, 16384),
            SetAngle(diving, 124, -16384),
        };

        return new() { data };
    }
}
