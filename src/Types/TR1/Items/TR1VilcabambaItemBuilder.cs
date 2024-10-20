using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Items;

public class TR1VilcabambaItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level vilcabamba = _control1.Read($@"Resources\{TR1LevelNames.VILCABAMBA}");
        InjectionData data = InjectionData.Create(InjectionType.ItemRotation, "vilcabamba_itemrots");

        data.ItemEdits = new()
        {
            SetAngle(vilcabamba, 4, -16384),
        };

        return new() { data };
    }
}
