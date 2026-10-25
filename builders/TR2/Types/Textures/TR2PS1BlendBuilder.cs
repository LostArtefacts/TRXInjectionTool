using TRLevelControl.Model;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2PS1BlendBuilder : PS1BlendBuilder
{
    public override string ID => "tr2_ps1_blend";

    protected override TRGameVersion GameVersion => TRGameVersion.TR2;

    protected override IEnumerable<KeyValuePair<string, string>> Levels
        => _tr2NameMap.Concat(_tr2CutNameMap).Append(new("TITLE.TR2", "title"));

    // The PlayStation marks these faces semi-transparent by mistake: the edge
    // of the door to the main chamber of Temple of Xian, and a triangle on
    // the foreheads of Bartoli's followers in the Xian cutscene.
    protected override IEnumerable<(string Level, uint ObjectID)> SolidObjects =>
    [
        ("emprtomb", (uint)TR2Type.Door1),
        ("cut4", (uint)TR2Type.CutsceneActor9),
        ("cut4", (uint)TR2Type.CutsceneActor10),
    ];
}
