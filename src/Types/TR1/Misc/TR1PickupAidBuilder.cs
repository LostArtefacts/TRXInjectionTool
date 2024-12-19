using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Misc;

public class TR1PickupAidBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level midas = _control1.Read($"Resources/{TR1LevelNames.MIDAS}");
        TRSpriteSequence twinkle = midas.Sprites[TR1Type.Sparkles_S_H];

        TR1TexturePacker packer = new(midas);
        IEnumerable<TRTextileRegion> regions = packer.GetSpriteRegions(twinkle)
            .Values.SelectMany(r => r);

        ResetLevel(midas, 1);

        packer = new(midas);
        packer.AddRectangles(regions);
        packer.Pack(true);

        foreach (TRSpriteTexture texture in twinkle.Textures)
        {
            texture.Alignment.Left -= 40;
            texture.Alignment.Right += 40;
            texture.Alignment.Top -= 40;
        }

        midas.Sprites[TR1Type.Unused02] = twinkle;

        InjectionData data = InjectionData.Create(midas, InjectionType.General, "pickup_aid");
        return new() { data };
    }
}
