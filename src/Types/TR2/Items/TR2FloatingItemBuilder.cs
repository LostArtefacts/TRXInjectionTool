using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Items;

public class TR2FloatingItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level floating = _control2.Read($"Resources/{TR2LevelNames.FLOATER}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.ItemRotation, "floating_itemrots");
        CreateDefaultTests(data, TR2LevelNames.FLOATER);

        data.ItemPosEdits =
        [
            SetAngle(floating, 1, -32768),
            MoveToRoom(floating, 95, 117), // Move the switch to the correct room
        ];

        return [data];
    }
}
