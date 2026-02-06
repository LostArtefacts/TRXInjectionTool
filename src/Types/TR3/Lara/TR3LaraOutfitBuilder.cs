using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;

namespace TRXInjectionTool.Types.TR3.Lara;

public class TR3LaraOutfitBuilder : OutfitBuilder
{
    private const short _barefootID = 374;
    private const int _skinBase = 393;

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

        return level;
    }

    protected override TRSFXData GetBarefootSFX()
    {
        var gym = _control1.Read($"Resources/{TR1LevelNames.ASSAULT}");
        var tr1Feet = gym.SoundEffects[TR1SFX.LaraFeet];
        var data = TRSFXData.Create(_barefootID, tr1Feet);

        var jungle = _control3.Read($"Resources/TR3/{TR3LevelNames.JUNGLE}");
        var tr3Feet = jungle.SoundEffects[TR3SFX.LaraFeet];
        data.Volume = (ushort)(tr3Feet.Volume << 7);

        return data;
    }
}
