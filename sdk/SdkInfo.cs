namespace TRXInjectionTool;

public static class SdkInfo
{
    // The injection .bin layout iteration written by InjectionIO. Bumped
    // whenever the serialized format changes. Being a const, it is baked
    // into plugin assemblies at compile time via [assembly: TRXPlugin(...)],
    // which is what lets the host detect a plugin built against another
    // format revision.
    public const uint BinIteration = 12;
}

// Every plugin assembly must carry this attribute; the host refuses to load
// builder assemblies whose baked iteration differs from its own.
[AttributeUsage(AttributeTargets.Assembly)]
public class TRXPluginAttribute : Attribute
{
    public uint BinIteration { get; }

    public TRXPluginAttribute(uint binIteration)
    {
        BinIteration = binIteration;
    }
}
