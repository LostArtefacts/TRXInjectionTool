using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Items;

public class TR2RigItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level rig = _control2.Read($"Resources/{TR2LevelNames.RIG}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.ItemRotation, "rig_itemrots");
        CreateDefaultTests(data, TR2LevelNames.RIG);

        data.ItemEdits = new()
        {
            SetAngle(rig, 30, -16384),
        };

        return new() { data };
    }
}
