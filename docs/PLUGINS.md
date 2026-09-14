# Builder plugins

Injection builders are plugins. The repository splits into:

- **`sdk/` (TRXInjection.Sdk)** — the builder API: `InjectionBuilder`,
  `InjectionData`, `InjectionIO`, the edit actions, applicability tests, the
  shared abstract builder bases (`TextureBuilder`, `LaraBuilder`, ...) and
  `AssetPublisher`.
- **`src/` (TRXInjectionTool)** — the host CLI. It contains no builders; it
  discovers them in DLLs under a `Plugins/` directory beside the executable.
- **`builders/`** — the first-party builder packs (`TRXBuilders.TR1` ...
  `TRXBuilders.TRX`), ordinary plugins that `just build` deploys into
  `out/Plugins/`.

## Writing a plugin

Create a class library targeting `net8.0`, reference the SDK (with
`Private=false`, so the host's copy is used at runtime), and subclass
`InjectionBuilder`:

```csharp
[assembly: TRXPlugin(SdkInfo.FormatMajor)]

public class MyBuilder : InjectionBuilder
{
    public override string ID => "my_builder";
    public override List<InjectionData> Build() => ...;
}
```

The assembly-level `TRXPlugin` stamp is mandatory. `SdkInfo.FormatMajor` is
a constant, so the TRXI container generation the plugin was compiled against
is baked into its DLL; the host refuses plugins whose baked major differs
from the format it writes, failing at load time with a clear message instead
of at run time with a stale-layout `.bin`. Per-chunk format versions evolve
without invalidating plugins.

See `samples/ExamplePlugin` for a complete minimal plugin and the
`builders/` packs for real ones.

## Loading

Drop the plugin DLL into `Plugins/` beside the executable. All plugins load
into one shared `AssemblyLoadContext`, so plugins may reference each other
(the TR1 pack reuses the TR2 pack's pickup builder, for example); assemblies
the host already provides (the SDK, TombIO libraries and their dependencies)
resolve to the host's copies and must not be shipped with a plugin. Plugin
builders appear in `--list` and in the interactive menu grouped by their
`TRXInjectionTool.Types.*` namespace, or under the assembly name for other
namespaces.

## Resources and output

There is exactly one `Resources/` and one `Output/` directory, shared by the
host and every plugin, both resolved relative to the directory the tool is
launched from. Plugins are free to reference nested paths within them
(e.g. `Resources/MyPack/...`) for plugin-specific data; injections land in
the usual `Output/<game>/...` tree via `InjectionBuilder.MakeOutputPath`.

## Published assets

Zip entry order in the published asset archives is byte-significant, so
publishers are not discovered implicitly: a pack registers them with
explicit order keys through an `IBuilderPackManifest` implementation, which
the host runs for every loaded assembly. See `builders/*/Manifest.cs` — the
TRX pack's `SparksBuilder` slotting into the TR3 archive shows a manifest
registering across games.

## Current limitations

- The `.bin` output format is TRXI (see `docs/FORMAT.md`), written through
  the `IInjectionExporter` seam; the legacy TRXJ writer remains available via
  `--legacy-trxj` until the TRX-side cutover completes.
