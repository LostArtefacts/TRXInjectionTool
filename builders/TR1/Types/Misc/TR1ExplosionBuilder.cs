using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Misc;

public class TR1ExplosionBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level atlantis = _control1.Read($"Resources/{TR1LevelNames.ATLANTIS}");
        TRSpriteSequence explosion = atlantis.Sprites[TR1Type.Explosion1_S_H];
        TR1SoundEffect sfx = atlantis.SoundEffects[TR1SFX.AtlanteanExplode];

        TR1TexturePacker packer = new(atlantis);
        IEnumerable<TRTextileRegion> regions = packer.GetSpriteRegions(explosion)
            .Values.SelectMany(r => r);

        ResetLevel(atlantis, 1);

        packer = new(atlantis);
        packer.AddRectangles(regions);
        packer.Pack(true);

        atlantis.Sprites[TR1Type.Explosion1_S_H] = explosion;
        atlantis.SoundEffects[TR1SFX.AtlanteanExplode] = sfx;

        InjectionData data = InjectionData.Create(atlantis, InjectionType.General, "explosion");
        return new() { data };
    }
}
