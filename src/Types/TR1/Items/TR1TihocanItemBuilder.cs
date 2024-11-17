using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Items;

public class TR1TihocanItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level tihocan = _control1.Read($"Resources/{TR1LevelNames.TIHOCAN}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.ItemRotation, "tihocan_itemrots");

        tihocan.Entities[12].Z -= TRConsts.Step4;
        data.ItemEdits = new()
        {
            SetAngle(tihocan, 10, -32768),
            SetAngle(tihocan, 12, -32768),
            SetAngle(tihocan, 63, 16384),
            SetAngle(tihocan, 65, 16384),
        };

        return new() { data };
    }
}
