using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;

namespace TRXInjectionTool.Types.TR2.Lara;

public class TR2LaraOutfitBuilder : OutfitBuilder
{
    private const short _barefootID = 375;
    private const int _skinBase = 302;

    public override string ID => "tr2-lara-outfits";

    protected override TRLevelBase CreateLevel(TR2Level outfitLevel)
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.GW}");
        ResetLevel(level);

        var skinId = _skinBase;
        for (int i = 0; i < _outfitCount; i++)
        {
            level.Models[(TR2Type)skinId++] = outfitLevel.Models[(TR2Type)(_modelBase + i)];
        }

        skinId = _skinBase + _maxOutfits;
        level.Models[(TR2Type)skinId++] = outfitLevel.Models[(TR2Type)_outfitExtras];
        level.Models[(TR2Type)skinId++] = outfitLevel.Models[(TR2Type)_outfitGuns2];
        level.Models[(TR2Type)skinId++] = outfitLevel.Models[(TR2Type)_outfitLegs];

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
