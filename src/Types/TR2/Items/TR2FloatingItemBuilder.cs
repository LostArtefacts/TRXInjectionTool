using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
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
            .. SwapSecrets(floating),
        ];

        return [data];
    }

    private static IEnumerable<TRItemPosEdit> SwapSecrets(TR2Level level)
    {
        var stoneIdx = (short)level.Entities.FindIndex(e => e.TypeID == TR2Type.StoneSecret_S_P);
        var jadeIdx = (short)level.Entities.FindIndex(e => e.TypeID == TR2Type.JadeSecret_S_P);

        var jade = level.Entities[jadeIdx];
        yield return new()
        {
            Index = stoneIdx,
            Item = new()
            {
                Angle = jade.Angle,
                X = jade.X,
                Y = jade.Y,
                Z = jade.Z,
                Room = jade.Room,
            },
        };

        var stone = level.Entities[stoneIdx];
        yield return new()
        {
            Index = jadeIdx,
            Item = new()
            {
                Angle = stone.Angle,
                X = stone.X,
                Y = stone.Y,
                Z = stone.Z,
                Room = stone.Room,
            },
        };
    }
}
