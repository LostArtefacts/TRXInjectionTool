using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types;

public abstract class SecretBuilder : InjectionBuilder, IPublisher
{
    protected abstract string Name { get; }

    public override List<InjectionData> Build()
    {
        var level = CreateLevel();
        var data = InjectionData.Create(level, InjectionType.General, $"secret_models_{Name}");
        return [data];
    }

    private TR2Level CreateLevel()
    {
        var level = _control2.Read($"Resources/TR2/Secrets/{Name.ToUpper()}/wad.tr2");
        level.SoundEffects.Clear();
        level.Rooms.Clear();
        return level;
    }

    public TRLevelBase Publish()
        => CreateLevel();

    public string GetPublishedName()
        => $"secrets_{Name}.tr2";
}
