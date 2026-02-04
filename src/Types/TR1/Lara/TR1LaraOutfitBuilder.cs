using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;

namespace TRXInjectionTool.Types.TR1.Lara;

public class TR1LaraOutfitBuilder : OutfitBuilder
{
    private const short _barefootID = 273;
    private const int _skinBase = 258;

    public override string ID => "tr1-lara-outfits";

    protected override TRLevelBase CreateLevel(TR2Level outfitLevel)
    {
        var level = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        ResetLevel(level);

        var skinId = _skinBase;
        for (int i = 0; i < _outfitCount; i++)
        {
            level.Models[(TR1Type)skinId++] = outfitLevel.Models[(TR2Type)(_modelBase + i)];
        }

        skinId = _skinBase + _maxOutfits;
        level.Models[(TR1Type)skinId++] = outfitLevel.Models[(TR2Type)_outfitExtras];
        level.Models[(TR1Type)skinId++] = outfitLevel.Models[(TR2Type)_outfitGuns1];
        level.Models[(TR1Type)skinId++] = outfitLevel.Models[(TR2Type)_outfitLegs];

        return level;
    }

    protected override TRSFXData GetBarefootSFX()
    {
        var gym = _control1.Read($"Resources/{TR1LevelNames.ASSAULT}");
        var tr1Feet = gym.SoundEffects[TR1SFX.LaraFeet];
        return TRSFXData.Create(_barefootID, tr1Feet);
    }
}
