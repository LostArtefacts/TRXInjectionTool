using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Misc;

public class TR1GunGlowBuilder : InjectionBuilder
{
    public override string ID => "gun_glow";

    public override List<InjectionData> Build()
    {
        var wall = _control2.Read($"Resources/{TR2LevelNames.GW}");
        var sequence = wall.Sprites[TR2Type.Glow_S_H];
        var packer2 = new TR2TexturePacker(wall);
        var regions = packer2.GetSpriteRegions(sequence)
            .Values.SelectMany(r => r);

        var caves = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        ResetLevel(caves, 1);

        var packer1 = new TR1TexturePacker(caves);
        packer1.AddRectangles(regions);
        packer1.Pack(true);

        caves.Sprites[TR1Type.Glow_S_H] = sequence;

        var data = InjectionData.Create(caves, InjectionType.General, ID);
        return new() { data };
    }
}
