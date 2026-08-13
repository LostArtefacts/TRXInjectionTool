using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;

namespace TRXInjectionTool.Types.TR3.Lara;

public class TR3LaraOutfitBuilder : OutfitBuilder
{
    private const short _barefootStepID = 374;
    private const short _barefootLandID = 375;
    private const int _skinBase = 393;
    private const int _gameJointsBase = 433;

    public override string ID => "tr3-lara-outfits";

    protected override TRLevelBase CreateLevel(TR2Level outfitLevel)
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.JUNGLE}");
        ResetLevel(level);

        var skinId = _skinBase;
        for (int i = 0; i < _outfitCount; i++)
        {
            level.Models[(TR3Type)skinId++] = outfitLevel.Models[(TR2Type)(_modelBase + i)];
        }

        skinId = _skinBase + _maxOutfits;
        level.Models[(TR3Type)skinId++] = outfitLevel.Models[(TR2Type)_outfitExtras];
        level.Models[(TR3Type)skinId++] = outfitLevel.Models[(TR2Type)_outfitGuns3];
        level.Models[(TR3Type)skinId++] = outfitLevel.Models[(TR2Type)_outfitLegs];

        for (int i = 0; i < _jointsCount; i++)
        {
            level.Models[(TR3Type)(_gameJointsBase + i)] = outfitLevel.Models[(TR2Type)(_jointsBase + i)];
        }

        return level;
    }

    protected override List<TRSFXData> GetBarefootSFX()
    {
        var gym = _control1.Read($"Resources/{TR1LevelNames.ASSAULT}");
        var stepData = TRSFXData.Create(_barefootStepID, gym.SoundEffects[TR1SFX.LaraFeet]);
        var landData = TRSFXData.Create(_barefootLandID, gym.SoundEffects[TR1SFX.LaraLand]);

        var jungle = _control3.Read($"Resources/TR3/{TR3LevelNames.JUNGLE}");
        stepData.Volume = (ushort)(jungle.SoundEffects[TR3SFX.LaraFeet].Volume << 7);
        landData.Volume = (ushort)(jungle.SoundEffects[TR3SFX.LaraLand].Volume << 7);

        return [stepData, landData];
    }
}
