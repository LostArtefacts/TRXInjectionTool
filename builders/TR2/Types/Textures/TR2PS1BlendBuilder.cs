using TRLevelControl.Model;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2PS1BlendBuilder : PS1BlendBuilder
{
    public override string ID => "tr2_ps1_blend";

    protected override TRGameVersion GameVersion => TRGameVersion.TR2;

    protected override IEnumerable<KeyValuePair<string, string>> Levels
        => _tr2NameMap.Concat(_tr2CutNameMap).Append(new("TITLE.TR2", "title"));
}
