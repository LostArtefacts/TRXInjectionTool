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
            FixMeshPositions(egypt),
        };
    }

    private static InjectionData FixMeshPositions(TR1Level egypt)
    {
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.General, "egypt_meshfixes");

        data.MeshEdits.Add(FixEgyptToppledChair(TR1Type.Architecture7, egypt));

        return data;
    }
}
