using TRImageControl;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types;

public abstract class OutfitBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        var outfitLevel = _control2.Read("Resources/outfits.tr2");
        var level = CreateLevel(outfitLevel);
        level.ObjectTextures = outfitLevel.ObjectTextures;

        var data = InjectionData.Create(level, InjectionType.General, "lara_outfits");
        data.Images.AddRange(outfitLevel.Images16.Select(i =>
        {
            var img = new TRImage(i.Pixels);
            return new TRTexImage32 { Pixels = img.ToRGBA() };
        }));

        data.SFX.Add(GetBarefootSFX());

        return [data];
    }

    protected abstract TRLevelBase CreateLevel(TR2Level outfitLevel);
    protected abstract TRSFXData GetBarefootSFX();
}
