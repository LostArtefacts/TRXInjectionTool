using TRLevelControl.Model;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2PS1BlendBuilder : PS1BlendBuilder
{
    public override string ID => "tr2_ps1_blend";

    protected override TRGameVersion GameVersion => TRGameVersion.TR2;

    protected override Dictionary<string, (string FileName, string Prefix)> Levels { get; } = new()
    {
        ["assault"] = ("ASSAULT.TR2", "gym"),
        ["boat"] = ("BOAT.TR2", "venice"),
        ["catacomb"] = ("CATACOMB.TR2", "catacombs"),
        ["cut3"] = ("CUT3.TR2", "cut3"),
        ["cut4"] = ("CUT4.TR2", "cut4"),
        ["deck"] = ("DECK.TR2", "deck"),
        ["emprtomb"] = ("EMPRTOMB.TR2", "xian"),
        ["floating"] = ("FLOATING.TR2", "floating"),
        ["house"] = ("HOUSE.TR2", "house"),
        ["icecave"] = ("ICECAVE.TR2", "palace"),
        ["keel"] = ("KEEL.TR2", "wreck"),
        ["living"] = ("LIVING.TR2", "living"),
        ["monastry"] = ("MONASTRY.TR2", "barkhang"),
        ["opera"] = ("OPERA.TR2", "opera"),
        ["platform"] = ("PLATFORM.TR2", "diving"),
        ["rig"] = ("RIG.TR2", "rig"),
        ["skidoo"] = ("SKIDOO.TR2", "tibet"),
        ["title"] = ("TITLE.TR2", "title"),
        ["unwater"] = ("UNWATER.TR2", "fathoms"),
        ["venice"] = ("VENICE.TR2", "bartoli"),
        ["wall"] = ("WALL.TR2", "wall"),
        ["xian"] = ("XIAN.TR2", "lair"),
    };
}
