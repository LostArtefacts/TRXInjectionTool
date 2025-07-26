using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Items;

public class TR1CatItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level cat = _control1.Read($"Resources/{TR1LevelNames.CAT}");

        return new()
        {
            CreateItemRots(cat),
            FixMeshPositions(cat),
        };
    }

    private static InjectionData CreateItemRots(TR1Level cat)
    {
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.ItemRotation, "cat_itemrots");
        CreateDefaultTests(data, TR1LevelNames.CAT);

        data.ItemEdits.Add(SetAngle(cat, 44, 16384));
        data.ItemEdits.Add(SetAngle(cat, 93, 16384));
        data.ItemEdits.Add(SetAngle(cat, 94, 16384));
        data.ItemEdits.Add(SetAngle(cat, 127, -16384));
        data.ItemEdits.Add(SetAngle(cat, 153, -16384));
        data.ItemEdits.Add(SetAngle(cat, 159, 16384));
        data.ItemEdits.Add(SetAngle(cat, 171, -32768));

        // Rotate and shift door 180 to put the invisible wall on the other side of it.
        cat.Entities[180].X += TRConsts.Step4;
        data.ItemEdits.Add(SetAngle(cat, 180, 16384));

        return data;
    }

    private static InjectionData FixMeshPositions(TR1Level cat)
    {
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.General, "cat_meshfixes");
        CreateDefaultTests(data, TR1LevelNames.CAT);

        data.MeshEdits.Add(FixEgyptToppledChair(TR1Type.Architecture7, cat));

        return data;
    }
}
