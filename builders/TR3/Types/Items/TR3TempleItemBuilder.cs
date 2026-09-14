using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Items;

public class TR3TempleItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.ItemRotation, "temple_itemrots");
        CreateDefaultTests(data, $"TR3/{TR3LevelNames.RUINS}");

        data.ItemPosEdits.Add(FixLaraPos());
        return [data];
    }

    private static TRItemPosEdit FixLaraPos()
    {
        // Lara is embedded in the floor, so the camera pops up at the start once the engine
        // corrects her position.
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.RUINS}");
        var laraIdx = level.Entities.FindIndex(e => e.TypeID == TR3Type.Lara);
        var lara = level.Entities[laraIdx];
        lara.Y -= TRConsts.Step1 / 2;
        return MoveToRoom(level, (short)laraIdx, lara.Room);
    }
}
