using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;

namespace TRXInjectionTool.Types.TR3.Lara;

public class TR3LaraOutfitBuilder : OutfitBuilder
{
    private const short _barefootID = 374;

    public override string ID => "tr3-lara-outfits";

    protected override TRLevelBase CreateLevel(TR2Level outfitLevel)
    {
        var level = _control3.Read($"Resources/{TR3LevelNames.JUNGLE}");
        ResetLevel(level);

        level.Models[(TR3Type)393] = outfitLevel.Models[(TR2Type)270]; // skin 1
        level.Models[(TR3Type)394] = outfitLevel.Models[(TR2Type)271]; // skin 2
        level.Models[(TR3Type)395] = outfitLevel.Models[(TR2Type)272]; // skin extras
        level.Models[(TR3Type)396] = outfitLevel.Models[(TR2Type)275]; // guns
        level.Models[(TR3Type)397] = outfitLevel.Models[(TR2Type)277]; // legs

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
