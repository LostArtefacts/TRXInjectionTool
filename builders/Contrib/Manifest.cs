using TRXInjectionTool;

[assembly: TRXPlugin(SdkInfo.FormatMajor)]

namespace TRXInjectionTool.Types.Contrib;

// Builders for content the games never carried. Nothing here is published
// alongside the stock assets, so the pack registers no publisher entries.
public class Manifest : IBuilderPackManifest
{
    public void Register()
    {
    }
}
