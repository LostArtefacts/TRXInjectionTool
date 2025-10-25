using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Items;

public class TR2DeckItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level deck = _control2.Read($"Resources/{TR2LevelNames.DECK}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.ItemRotation, "deck_itemrots");
        CreateDefaultTests(data, TR2LevelNames.DECK);

        data.ItemPosEdits = new()
        {
            SetAngle(deck, 82, -16384),
        };

        return new() { data };
    }
}
