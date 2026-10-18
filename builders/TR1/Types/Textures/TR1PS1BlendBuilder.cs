using TRLevelControl.Model;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1PS1BlendBuilder : PS1BlendBuilder
{
    public override string ID => "tr1_ps1_blend";

    protected override TRGameVersion GameVersion => TRGameVersion.TR1;

    protected override Dictionary<string, (string FileName, string Prefix)> Levels { get; } = new()
    {
        ["cut1"] = ("CUT1.PHD", "cut1"),
        ["cut3"] = ("CUT3.PHD", "cut3"),
        ["cut4"] = ("CUT4.PHD", "cut4"),
        ["gym"] = ("GYM.PHD", "gym"),
        ["level10a"] = ("LEVEL10A.PHD", "mines"),
        ["level10b"] = ("LEVEL10B.PHD", "atlantis"),
        ["level10c"] = ("LEVEL10C.PHD", "pyramid"),
        ["level2"] = ("LEVEL2.PHD", "vilcabamba"),
        ["level3a"] = ("LEVEL3A.PHD", "valley"),
        ["level3b"] = ("LEVEL3B.PHD", "qualopec"),
        ["level4"] = ("LEVEL4.PHD", "folly"),
        ["level5"] = ("LEVEL5.PHD", "colosseum"),
        ["level6"] = ("LEVEL6.PHD", "midas"),
        ["level7a"] = ("LEVEL7A.PHD", "cistern"),
        ["level7b"] = ("LEVEL7B.PHD", "tihocan"),
        ["level8a"] = ("LEVEL8A.PHD", "khamoon"),
        ["level8b"] = ("LEVEL8B.PHD", "obelisk"),
        ["level8c"] = ("LEVEL8C.PHD", "sanctuary"),
    };
}
