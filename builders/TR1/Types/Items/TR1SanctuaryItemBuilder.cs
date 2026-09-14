using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Items;

public class TR1SanctuaryItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        var sanctuary = _control1.Read($"Resources/{TR1LevelNames.SANCTUARY}");
        return
        [
            CreateItemRots(sanctuary),
            FixMeshPositions(sanctuary),
        ];
    }

    private static InjectionData CreateItemRots(TR1Level level)
    {
        var data = InjectionData.Create(TRGameVersion.TR1, InjectionType.ItemRotation, "sanctuary_itemrots");
        CreateDefaultTests(data, TR1LevelNames.SANCTUARY);

        data.ItemPosEdits.Add(SetAngle(level, 4, 16384));
        data.ItemPosEdits.Add(SetAngle(level, 38, -16384));
        data.ItemPosEdits.Add(SetAngle(level, 46, -32768));
        data.ItemPosEdits.Add(SetAngle(level, 54, -16384));
        data.ItemPosEdits.Add(SetAngle(level, 67, -16384));
        data.ItemPosEdits.Add(SetAngle(level, 73, -16384));

        return data;
    }

    private static InjectionData FixMeshPositions(TR1Level level)
    {
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.General, "sanctuary_meshfixes");
        CreateDefaultTests(data, TR1LevelNames.SANCTUARY);

        FixStatics(level, data);

        return data;
    }

    private static void FixStatics(TR1Level level, InjectionData data)
    {
        data.StaticMeshEdits.Add(ReduceEarBounds(level));
    }

    private static TRStaticMeshEdit ReduceEarBounds(TR1Level level)
    {
        var ear = level.StaticMeshes[TR1Type.SceneryBase + 43];
        ear.CollisionBox.MinX += 256;
        return new()
        {
            TypeID = 43,
            Mesh = ear,
        };
    }
}
