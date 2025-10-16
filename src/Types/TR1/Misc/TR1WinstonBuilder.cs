using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Misc;

public class TR1WinstonBuilder : InjectionBuilder
{

    public override List<InjectionData> Build()
    {
        var tr2Level = CreateWinstonLevel(TR2LevelNames.ASSAULT);
        var tr1Level = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        ResetLevel(tr1Level);

        tr1Level.Models[TR1Type.Winston] = tr2Level.Models[TR2Type.Winston];
        tr1Level.Images8 = tr2Level.Images8;
        tr1Level.Palette = tr2Level.Palette;
        tr1Level.ObjectTextures = tr2Level.ObjectTextures;

        var data = InjectionData.Create(tr1Level, InjectionType.General, "winston_model");

        foreach (var (id, sfx) in tr2Level.SoundEffects)
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
