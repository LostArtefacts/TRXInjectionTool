using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Items;

public class TR1MinesItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level mines = _control1.Read($"Resources/{TR1LevelNames.MINES}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.ItemRotation, "mines_itemrots");

        data.ItemEdits = new()
        {
            SetAngle(mines, 71, 16384),
        };

        return new() { data };
    }
}
