using TRImageControl;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types;

public abstract class OutfitBuilder : InjectionBuilder
{
    protected const int _modelBase = 302;
    protected const int _outfitCount = 23;
    protected const int _maxOutfits = 32;
    protected const int _outfitExtras = 334;
    protected const int _outfitGuns1 = 335;
    protected const int _outfitGuns2 = 336;
    protected const int _outfitGuns3 = 337;
    protected const int _outfitLegs = 338;

    public override List<InjectionData> Build()
    {
        var outfitLevel = _control2.Read("Resources/outfits.tr2");
        var level = CreateLevel(outfitLevel);
        level.ObjectTextures = outfitLevel.ObjectTextures;

        // Cache blend mode attribute, otherwise lost during level flattening.
        var blendModes = level.ObjectTextures.Select((o, i) => new { Mode = o.BlendingMode, Idx = i })
            .ToList();

        var data = InjectionData.Create(level, InjectionType.General, "lara_outfits");
        data.Images.AddRange(outfitLevel.Images16.Select(i =>
        {
            var img = new TRImage(i.Pixels);
            return new TRTexImage32 { Pixels = img.ToRGBA() };
        }));

        blendModes.ForEach(o => data.ObjectTextures[o.Idx].Attribute = (ushort)o.Mode);

        data.SFX.Add(GetBarefootSFX());

        return [data];
    }

    protected abstract TRLevelBase CreateLevel(TR2Level outfitLevel);
    protected abstract TRSFXData GetBarefootSFX();
}
