using TRLevelControl.Model;
using TRXInjectionTool;
using TRXInjectionTool.Control;

[assembly: TRXPlugin(SdkInfo.FormatMajor)]

namespace ExamplePlugin;

// Minimal proof that builders can live outside the host assembly: produces a
// valid (if empty) injection without needing any level data. Real plugins
// read from the shared Resources/ tree and write to the shared Output/ tree,
// both relative to the directory the tool is launched from; use a nested
// directory (e.g. Resources/ExamplePlugin/...) for plugin-specific data.
public class ExampleBuilder : InjectionBuilder
{
    public override string ID => "example_plugin";

    public override List<InjectionData> Build()
    {
        return [InjectionData.Create(TRGameVersion.TR1, InjectionType.General, "example_plugin")];
    }
}
