using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Misc;

public class TR2PickupAidBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        var floater = _control2.Read($"Resources/{TR2LevelNames.FLOATER}");
        var twinkle = floater.Sprites[TR2Type.XianGuardSparkles_S_H];

        var packer = new TR2TexturePacker(floater);
        var regions = packer.GetSpriteRegions(twinkle)
            .Values.SelectMany(r => r);

        ResetLevel(floater, 1);

        packer = new(floater);
        packer.AddRectangles(regions);
        packer.Pack(true);

        foreach (var texture in twinkle.Textures)
        {
            texture.Alignment.Left -= 40;
            texture.Alignment.Right += 40;
            texture.Alignment.Top -= 40;
        }

        floater.Sprites[TR2Type.PickupAid] = twinkle;

        var data = InjectionData.Create(floater, InjectionType.General, "pickup_aid");
        return [data];
    }
}
