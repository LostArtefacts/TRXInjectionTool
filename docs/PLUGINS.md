# Builder plugins

Injection builders can live outside this repository. The tool is split into:

- **`sdk/` (TRXInjection.Sdk)** — the builder API: `InjectionBuilder`,
  `InjectionData`, `InjectionIO`, the edit actions and applicability tests.
  Builder plugins reference this project (with `Private=false`, so the host's
  copy is used at runtime).
- **`src/` (TRXInjectionTool)** — the host CLI. Builders compiled into the
  host under `TRXInjectionTool.Types.*` behave as before.

## Writing a plugin

Create a class library targeting `net8.0`, reference the SDK, and subclass
`InjectionBuilder`:

```csharp
public class MyBuilder : InjectionBuilder
{
    public override string ID => "my_builder";
    public override List<InjectionData> Build() => ...;
}
```

See `samples/ExamplePlugin` for a complete minimal plugin.

## Loading

Drop the plugin DLL (plus any private dependencies) into a `Plugins/`
directory beside the executable (e.g. `out/Plugins/`). Each DLL loads in its
own `AssemblyLoadContext`; assemblies the host already provides (the SDK,
TombIO libraries and their dependencies) resolve to the host's copies, so do
not ship those with the plugin. Plugin builders appear in `--list` and in the
interactive menu grouped under the plugin assembly's name.

## Current limitations

- Resource paths (`Resources/...`) resolve against the working directory, so
  a plugin needing level data or textures must be run where those resources
  exist. Per-plugin resource roots are a planned follow-up.
- The `.bin` output format is the TRX injection format (`TRXJ`); the SDK does
  not yet expose an exporter seam for other engines.
