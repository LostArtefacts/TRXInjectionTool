using TRLevelControl.Model;
using TRXInjectionTool.Types.TR2.Lara;
using TRXInjectionTool.Types.TR2.Misc;
using TRXInjectionTool.Types.TR2.Objects;

namespace TRXInjectionTool.Types.TR2;

public class Manifest : IBuilderPackManifest
{
    public void Register()
    {
        AssetPublisher.Register(TRGameVersion.TR2, 10, new TR2FontBuilder());
        AssetPublisher.Register(TRGameVersion.TR2, 20, new TR2PDABuilder());
        AssetPublisher.Register(TRGameVersion.TR2, 30, new TR2OGSecretBuilder());
        AssetPublisher.Register(TRGameVersion.TR2, 40, new TR2GMSecretBuilder());
        AssetPublisher.Register(TRGameVersion.TR2, 50, new TR2MiscSpritesBuilder());
        AssetPublisher.RegisterLara(TRGameVersion.TR2, new TR2LaraAnimBuilder());
    }
}
