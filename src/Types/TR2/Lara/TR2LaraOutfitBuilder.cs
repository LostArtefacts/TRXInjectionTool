using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;

namespace TRXInjectionTool.Types.TR2.Lara;

public class TR2LaraOutfitBuilder : OutfitBuilder
{
    private const short _barefootID = 375;

    public override string ID => "tr2-lara-outfits";

    protected override TRLevelBase CreateLevel(TR2Level outfitLevel)
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.GW}");
        ResetLevel(level);

        level.Models[(TR2Type)270] = outfitLevel.Models[(TR2Type)270]; // skin 1
        level.Models[(TR2Type)271] = outfitLevel.Models[(TR2Type)271]; // skin 2
        level.Models[(TR2Type)272] = outfitLevel.Models[(TR2Type)272]; // skin extras
        level.Models[(TR2Type)273] = outfitLevel.Models[(TR2Type)274]; // guns
        level.Models[(TR2Type)274] = outfitLevel.Models[(TR2Type)277]; // legs

        return level;
    }

    protected override TRSFXData GetBarefootSFX()
    {
        var wall = _control2.Read($"Resources/{TR2LevelNames.GW}");
        var tr2Feet = wall.SoundEffects[TR2SFX.LaraFeet];
        return new()
        {
            ID = _barefootID,
            Chance = tr2Feet.Chance,
            Characteristics = tr2Feet.GetFlags(),
            Volume = tr2Feet.Volume,
            Data = [.. Enumerable.Range(0, 4).Select(i => File.ReadAllBytes($"Resources/TR2/Barefoot/{i}.wav"))],
        };
    }
}
