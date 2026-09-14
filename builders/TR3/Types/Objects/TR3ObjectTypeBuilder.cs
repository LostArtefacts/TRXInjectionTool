using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Objects;

public class TR3ObjectTypeBuilder : InjectionBuilder
{
    private const TR3Type _dolphin = (TR3Type)429;

    private static readonly Dictionary<string, List<TRObjectTypeEdit>> _edits = new()
    {
        ["undersea_objects"] =
        [
            new()
            {
                BaseType = (int)TR3Type.KillerWhale,
                TargetType = (int)_dolphin,
            },
        ],
    };

    public override List<InjectionData> Build()
    {
        return [.. _edits.Select(kvp =>
        {
            var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.General, kvp.Key);
            data.ObjectTypeEdits.AddRange(kvp.Value);
            return data;
        })];
    }
}
