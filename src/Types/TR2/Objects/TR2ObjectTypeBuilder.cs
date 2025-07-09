using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Objects;

public class TR2ObjectTypeBuilder : InjectionBuilder
{
    private static readonly Dictionary<string, List<TRObjectTypeEdit>> _edits = new()
    {
        ["coldwar_objects"] = new()
        {
            new()
            {
                BaseType = (int)TR2Type.MonkWithLongStick,
                TargetType = (int)TR2Type.MonkWithNoShadow,
            },
        },
        ["furnace_objects"] = new()
        {
            new()
            {
                BaseType = (int)TR2Type.Spider,
                TargetType = (int)TR2Type.Wolf,
            },
            new()
            {
                BaseType = (int)TR2Type.GiantSpider,
                TargetType = (int)TR2Type.Bear,
            },
        },
    };

    public override List<InjectionData> Build()
    {
        return _edits.Select(kvp =>
        {
            InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.General, kvp.Key);
            data.ObjectTypeEdits.AddRange(kvp.Value);
            return data;
        }).ToList();
    }
}
