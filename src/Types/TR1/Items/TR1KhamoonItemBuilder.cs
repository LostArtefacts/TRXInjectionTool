using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Items;

public class TR1KhamoonItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        var level = _control1.Read($"Resources/{TR1LevelNames.KHAMOON}");
        var data = InjectionData.Create(TRGameVersion.TR1, InjectionType.ItemRotation, "khamoon_itemrots");
        CreateDefaultTests(data, TR1LevelNames.KHAMOON);

        data.ItemEdits.Add(SetAngle(level, 64, 16384));

        return new() { data };
    }
}
