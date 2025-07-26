using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Items;

public class TR1StrongholdItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level level = _control1.Read($"Resources/{TR1LevelNames.STRONGHOLD}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.ItemRotation, "stronghold_itemrots");
        CreateDefaultTests(data, TR1LevelNames.STRONGHOLD);

        data.ItemEdits.Add(SetAngle(level, 10, -32768));
        data.ItemEdits.Add(SetAngle(level, 36, 16384));
        data.ItemEdits.Add(SetAngle(level, 78, -32768));
        data.ItemEdits.Add(SetAngle(level, 118, 16384));
        data.ItemEdits.Add(SetAngle(level, 159, -32768));

        return new() { data };
    }
}
