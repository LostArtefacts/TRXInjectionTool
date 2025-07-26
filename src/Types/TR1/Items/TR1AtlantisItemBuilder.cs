using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Items;

public class TR1AtlantisItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        var level = _control1.Read($"Resources/{TR1LevelNames.ATLANTIS}");
        var data = InjectionData.Create(TRGameVersion.TR1, InjectionType.ItemRotation, "atlantis_itemrots");
        CreateDefaultTests(data, TR1LevelNames.ATLANTIS);

        data.ItemEdits.Add(SetAngle(level, 57, -16384));
        data.ItemEdits.Add(SetAngle(level, 58, -16384));
        data.ItemEdits.Add(SetAngle(level, 90, -16384));

        return new() { data };
    }
}
