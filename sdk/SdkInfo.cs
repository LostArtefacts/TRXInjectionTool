namespace TRXInjectionTool;

public static class SdkInfo
{
    // The TRXI container major version the SDK writes. Bumped only on a
    // breaking container/framing redesign; per-chunk versions evolve without
    // touching it. Being a const, it is baked into plugin assemblies at
    // compile time via [assembly: TRXPlugin(...)], which is what lets the
    // host detect a plugin built against another format generation.
    public const uint FormatMajor = Format.Container.FormatMajor;
}

// Every plugin assembly must carry this attribute; the host refuses to load
// builder assemblies whose baked format major differs from its own.
[AttributeUsage(AttributeTargets.Assembly)]
public class TRXPluginAttribute : Attribute
{
    public uint FormatMajor { get; }

    public TRXPluginAttribute(uint formatMajor)
    {
        FormatMajor = formatMajor;
    }
}
