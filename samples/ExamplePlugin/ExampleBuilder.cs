using TRLevelControl.Model;
using TRXInjectionTool;
using TRXInjectionTool.Control;

namespace ExamplePlugin;

// Minimal proof that builders can live outside the host assembly: produces a
// valid (if empty) injection without needing any level data on disk.
public class ExampleBuilder : InjectionBuilder
{
    public override string ID => "example_plugin";

    public override List<InjectionData> Build()
    {
        return [InjectionData.Create(TRGameVersion.TR1, InjectionType.General, "example_plugin")];
    }
}
