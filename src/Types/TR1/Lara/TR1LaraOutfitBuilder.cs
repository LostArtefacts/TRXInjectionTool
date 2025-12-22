using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;

namespace TRXInjectionTool.Types.TR1.Lara;

public class TR1LaraOutfitBuilder : OutfitBuilder
{
    private const short _barefootID = 273;

    public override string ID => "tr1-lara-outfits";

    protected override TRLevelBase CreateLevel(TR2Level outfitLevel)
    {
        var level = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        ResetLevel(level);

        level.Models[(TR1Type)195] = outfitLevel.Models[(TR2Type)270]; // skin 1
        level.Models[(TR1Type)196] = outfitLevel.Models[(TR2Type)271]; // skin 2
        level.Models[(TR1Type)197] = outfitLevel.Models[(TR2Type)272]; // skin extras
        level.Models[(TR1Type)198] = outfitLevel.Models[(TR2Type)273]; // guns
        level.Models[(TR1Type)199] = outfitLevel.Models[(TR2Type)277]; // legs

        return level;
    }

    protected override TRSFXData GetBarefootSFX()
    {
        var gym = _control1.Read($"Resources/{TR1LevelNames.ASSAULT}");
        var tr1Feet = gym.SoundEffects[TR1SFX.LaraFeet];
        return TRSFXData.Create(_barefootID, tr1Feet);
    }
}
