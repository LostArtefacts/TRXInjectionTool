using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Items;

public class TR1EgyptItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level egypt = _control1.Read($"Resources/{TR1LevelNames.EGYPT}");

        return new()
        {
            CreateItemRots(egypt),
            FixMeshPositions(egypt),
        };
    }

    private static InjectionData CreateItemRots(TR1Level level)
    {
        var data = InjectionData.Create(TRGameVersion.TR1, InjectionType.ItemRotation, "egypt_itemrots");
        CreateDefaultTests(data, TR1LevelNames.EGYPT);

        data.ItemPosEdits.Add(SetAngle(level, 52, 16384));
        data.ItemPosEdits.Add(SetAngle(level, 99, -16384));
        data.ItemPosEdits.Add(SetAngle(level, 185, -16384));
        data.ItemPosEdits.Add(SetAngle(level, 186, -32768));
        data.ItemPosEdits.Add(SetAngle(level, 190, 16384));
        data.ItemPosEdits.Add(SetAngle(level, 191, 16384));

        return data;
    }

    private static InjectionData FixMeshPositions(TR1Level egypt)
    {
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.General, "egypt_meshfixes");
        CreateDefaultTests(data, TR1LevelNames.EGYPT);

        data.MeshEdits.Add(FixEgyptToppledChair(TR1Type.Architecture7, egypt));
        data.StaticMeshEdits.Add(FixEgyptPillar(egypt));

        return data;
    }
}
