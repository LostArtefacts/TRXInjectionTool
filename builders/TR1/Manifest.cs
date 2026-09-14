using TRLevelControl.Model;
using TRXInjectionTool.Types.TR1.Lara;
using TRXInjectionTool.Types.TR1.Misc;

namespace TRXInjectionTool.Types.TR1;

public class Manifest : IBuilderPackManifest
{
    public void Register()
    {
        AssetPublisher.Register(TRGameVersion.TR1, 10, new TR1FontBuilder());
        AssetPublisher.Register(TRGameVersion.TR1, 20, new TR1PDABuilder());
        AssetPublisher.Register(TRGameVersion.TR1, 30, new TR1MiscSpritesBuilder());
        AssetPublisher.RegisterLara(TRGameVersion.TR1, new TR1LaraAnimBuilder());
    }
}
