using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Misc;

public class TR3GlobeBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        var srcLevel = _control3.Read("Resources/TR3/TITLE.TR2");
        CreateModelLevel(srcLevel, TR3Type.Globe_M_H);
        var dstLevel = _control3.Read($"Resources/{TR3LevelNames.JUNGLE}");
        ResetLevel(dstLevel);

        dstLevel.Models[TR3Type.Globe_M_H] = srcLevel.Models[TR3Type.Globe_M_H];
        dstLevel.Images8 = srcLevel.Images8;
        dstLevel.Images16 = srcLevel.Images16;
        dstLevel.Palette = srcLevel.Palette;
        dstLevel.ObjectTextures = srcLevel.ObjectTextures;

        var data = InjectionData.Create(dstLevel, InjectionType.General, "globe_model");

        foreach (var (id, sfx) in srcLevel.SoundEffects)
        {
            data.SFX.Add(new()
            {
                ID = (short)id,
                Chance = sfx.Chance,
                Characteristics = sfx.GetFlags(),
                Volume = sfx.Volume,
                SampleOffset = sfx.SampleID,
            });
            data.SFX[^1].LoadSFX(TRGameVersion.TR2);
        }

        return [data];
    }
}
