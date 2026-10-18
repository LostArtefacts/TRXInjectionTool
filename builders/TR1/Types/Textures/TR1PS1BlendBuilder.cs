using TRLevelControl.Model;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1PS1BlendBuilder : PS1BlendBuilder
{
    public override string ID => "tr1_ps1_blend";

    protected override TRGameVersion GameVersion => TRGameVersion.TR1;

    protected override IEnumerable<KeyValuePair<string, string>> Levels
        => _tr1NameMap.Concat(_tr1CutNameMap);
}
