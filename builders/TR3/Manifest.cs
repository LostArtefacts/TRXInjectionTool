using TRXInjectionTool;
using TRLevelControl.Model;
using TRXInjectionTool.Types.TR3.Lara;
using TRXInjectionTool.Types.TR3.Misc;

[assembly: TRXPlugin(SdkInfo.FormatMajor)]

namespace TRXInjectionTool.Types.TR3;

public class Manifest : IBuilderPackManifest
{
    public void Register()
    {
        AssetPublisher.Register(TRGameVersion.TR3, 10, new TR3FontBuilder());
        AssetPublisher.Register(TRGameVersion.TR3, 20, new TR3PDABuilder());
        AssetPublisher.Register(TRGameVersion.TR3, 30, new TR3FishSpritesBuilder());
        AssetPublisher.Register(TRGameVersion.TR3, 40, new TR3BatSpritesBuilder());
        // Order 50 is SparksBuilder, registered by the TRX pack's manifest.
        AssetPublisher.Register(TRGameVersion.TR3, 60, new TR3MiscSpritesBuilder());
        AssetPublisher.RegisterLara(TRGameVersion.TR3, new TR3LaraAnimBuilder());
    }
}
