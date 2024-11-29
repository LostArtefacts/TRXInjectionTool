using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Items;

public class TR2OperaItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level opera = _control2.Read($"Resources/{TR2LevelNames.OPERA}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.ItemRotation, "opera_itemrots");

        data.ItemEdits = new()
        {
            SetAngle(opera, 118, 16384),
            SetAngle(opera, 82, 16384),
        };

        return new() { data };
    }
}
