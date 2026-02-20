using TRImageControl;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types;

public abstract class OutfitBuilder : InjectionBuilder
{
    protected const int _modelBase = 302;
    protected const int _outfitCount = 20;
    protected const int _maxOutfits = 32;
    protected const int _outfitExtras = 322;
    protected const int _outfitGuns1 = 323;
    protected const int _outfitGuns2 = 324;
    protected const int _outfitGuns3 = 325;
    protected const int _outfitLegs = 326;

    private const ushort _reflective = 8;

    public override List<InjectionData> Build()
    {
        var outfitLevel = _control2.Read("Resources/outfits.tr2");
        var level = CreateLevel(outfitLevel);
        level.ObjectTextures = outfitLevel.ObjectTextures;

        // Cache reflective attribute, otherwise lost during level flattening
        var alphaTexInfos = level.ObjectTextures.Select((o, i) => new { Info = o, Idx = i })
            .Where(o => (ushort)o.Info.BlendingMode == _reflective)
            .ToList();

        var data = InjectionData.Create(level, InjectionType.General, "lara_outfits");
        data.Images.AddRange(outfitLevel.Images16.Select(i =>
        {
            var img = new TRImage(i.Pixels);
            return new TRTexImage32 { Pixels = img.ToRGBA() };
        }));

        alphaTexInfos.ForEach(o => data.ObjectTextures[o.Idx].Attribute = _reflective);

        data.SFX.Add(GetBarefootSFX());

        return [data];
    }

    protected abstract TRLevelBase CreateLevel(TR2Level outfitLevel);
    protected abstract TRSFXData GetBarefootSFX();
}
