using TRXInjectionTool;
using TRLevelControl.Model;
using TRXInjectionTool.Types.TRX.Sparks;

[assembly: TRXPlugin(SdkInfo.FormatMajor)]

namespace TRXInjectionTool.Types.TRX;

public class Manifest : IBuilderPackManifest
{
    public void Register()
    {
        // Slots between TR3's bat sprites (40) and misc sprites (60) to
        // preserve the published zip's entry order.
        AssetPublisher.Register(TRGameVersion.TR3, 50, new SparksBuilder());
    }
}
