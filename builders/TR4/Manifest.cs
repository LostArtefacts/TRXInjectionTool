using TRXInjectionTool;
using TRLevelControl.Model;
using TRXInjectionTool.Types.TR4.Lara;
using TRXInjectionTool.Types.TR4.Misc;

[assembly: TRXPlugin(SdkInfo.BinIteration)]

namespace TRXInjectionTool.Types.TR4;

public class Manifest : IBuilderPackManifest
{
    public void Register()
    {
        AssetPublisher.Register(TRGameVersion.TR4, 10, new TR4FontBuilder());
        AssetPublisher.Register(TRGameVersion.TR4, 20, new TR4InventoryBuilder());
        AssetPublisher.RegisterLara(TRGameVersion.TR4, new TR4LaraAnimBuilder());
    }
}
