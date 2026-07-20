using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;

namespace TRXInjectionTool.Types.TR2.Lara;

public class TR2LaraOutfitBuilder : OutfitBuilder
{
    private const short _barefootStepID = 375;
    private const short _barefootLandID = 377;
    private const int _skinBase = 302;
    private const int _gameJointsBase = 340;

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

        for (int i = 0; i < _jointsCount; i++)
        {
            level.Models[(TR2Type)(_gameJointsBase + i)] = outfitLevel.Models[(TR2Type)(_jointsBase + i)];
        }

        return level;
    }

    protected override List<TRSFXData> GetBarefootSFX()
    {
        var wall = _control2.Read($"Resources/{TR2LevelNames.GW}");
        var gym1 = _control1.Read($"Resources/{TR1LevelNames.ASSAULT}");
        var tr2Feet = wall.SoundEffects[TR2SFX.LaraFeet];
        var tr1Land = TRSFXData.Create(_barefootLandID, gym1.SoundEffects[TR1SFX.LaraLand]);
        tr1Land.Volume = wall.SoundEffects[TR2SFX.LaraLand].Volume;
        return
        [
            new()
            {
                ID = _barefootStepID,
                Chance = tr2Feet.Chance,
                Characteristics = tr2Feet.GetFlags(),
                Volume = tr2Feet.Volume,
                Data = [.. Enumerable.Range(0, 4).Select(i => File.ReadAllBytes($"Resources/TR2/Barefoot/{i}.wav"))],
            },
            tr1Land,
        ];
    }
}
