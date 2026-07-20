using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;

namespace TRXInjectionTool.Types.TR4.Lara;

public class TR4LaraOutfitBuilder : OutfitBuilder
{
    private const short _barefootStepID = 370;
    private const short _barefootLandID = 371;
    private const int _skinBase = 465;
    private const int _gameJointsBase = 502;

    public override string ID => "tr4-lara-outfits";

    protected override TRLevelBase CreateLevel(TR2Level outfitLevel)
    {
        var level = _control4.Read($"Resources/TR4/{TR4LevelNames.SETH}");
        ResetLevel(level);

        var skinId = _skinBase;
        for (int i = 0; i < _outfitCount; i++)
        {
            level.Models[(TR4Type)skinId++] = outfitLevel.Models[(TR2Type)(_modelBase + i)];
        }

        skinId = _skinBase + _maxOutfits;
        level.Models[(TR4Type)skinId++] = outfitLevel.Models[(TR2Type)_outfitExtras];
        level.Models[(TR4Type)skinId++] = outfitLevel.Models[(TR2Type)_outfitGuns3];
        level.Models[(TR4Type)skinId++] = outfitLevel.Models[(TR2Type)_outfitLegs];

        for (int i = 0; i < _jointsCount; i++)
        {
            level.Models[(TR4Type)(_gameJointsBase + i)] = outfitLevel.Models[(TR2Type)(_jointsBase + i)];
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
