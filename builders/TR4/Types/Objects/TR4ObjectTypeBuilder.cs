using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR4.Objects;

public class TR4ObjectTypeBuilder : InjectionBuilder
{
    const int _henchmanJeep = 542;

    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR4, InjectionType.General, "henchman_jeep");
        data.ObjectTypeEdits.Add(new()
        {
            BaseType = (int)TR4Type.EnemyJeep,
            TargetType = _henchmanJeep,
        });
        return [data];
    }
}
