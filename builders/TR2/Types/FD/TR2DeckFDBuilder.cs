using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.FD;

public class TR2DeckFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level deck = _control2.Read($"Resources/{TR2LevelNames.DECK}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.FDFix, "deck_fd");
        CreateDefaultTests(data, TR2LevelNames.DECK);

        data.VisPortalEdits.Add(DeletePortal(deck.Rooms, 17, 2));
        data.VisPortalEdits.Add(DeletePortal(deck.Rooms, 104, 2));

        return new() { data };
    }
}
