using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;

namespace TRXInjectionTool.Types.TR4.Lara;

public class TR4LaraOutfitBuilder : OutfitBuilder
{
    private const short _barefootStepID = 370;
    private const short _barefootLandID = 371;
    private const int _skinBase = 465;

    public override string ID => "tr4-lara-outfits";

    // The crowbar is worn on Lara's right hand as a skin extra mesh (so its
    // presence survives a save/reload). Its geometry is not in the outfits
    // resource, so graft it in from the in-game crowbar pickup object.
    protected override void ModifyOutfits(TR2Level outfitLevel)
    {
        var coastal = _control4.Read($"Resources/TR4/{TR4LevelNames.COASTAL}");

        // The crowbar hand meshswap is the plain hand with the crowbar appended;
        // isolate the crowbar so it lands in the exact OG grip pose. LM_HAND_R
        // is mesh index 10.
        const int handMeshIdx = 10;
        var animHand = coastal.Models[TR4Type.LaraCrowbarAnim].Meshes[handMeshIdx];
        var baseHand = coastal.Models[TR4Type.LaraSkin].Meshes[handMeshIdx];
        var crowbarMesh = ExtractAddedGeometry(animHand, baseHand.Vertices.Count);

        // Object-texture Atlas is a global index over Rooms ++ Objects ++ Bump.
        var pages = new List<TRTexImage32>();
        pages.AddRange(coastal.Images.Rooms.Images32);
        pages.AddRange(coastal.Images.Objects.Images32);
        pages.AddRange(coastal.Images.Bump.Images32);

        GraftExtraMesh(
            outfitLevel, _outfitExtras, crowbarMesh, coastal.ObjectTextures, pages);
    }

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
