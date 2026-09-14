using System.Drawing;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Misc;

public class TR2ExplosionBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level wall = _control2.Read($"Resources/{TR2LevelNames.GW}");
        TRSpriteSequence explosion = wall.Sprites[TR2Type.Explosion_S_H];

        TR2TexturePacker packer = new(wall);
        IEnumerable<TRTextileRegion> regions = packer.GetSpriteRegions(explosion)
            .Values.SelectMany(r => r);

        List<Color> basePalette = new(wall.Palette.Select(c => c.ToTR1Color()));
        ResetLevel(wall, 1);

        packer = new(wall);
        packer.AddRectangles(regions);
        packer.Pack(true);

        wall.Sprites[TR2Type.Explosion_S_H] = explosion;
        GenerateImages8(wall, basePalette);

        InjectionData data = InjectionData.Create(wall, InjectionType.General, "explosion");
        return new() { data };
    }
}
