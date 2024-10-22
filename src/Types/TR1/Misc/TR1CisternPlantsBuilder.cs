using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Misc;

public class TR1CisternPlantsBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        // This disables the animation on the underwater plant, by resetting it to
        // only have one texture.
        TR1Level cistern = _control1.Read($@"Resources\{TR1LevelNames.CISTERN}");
        TRSpriteSequence plant = cistern.Sprites[TR1Type.Plant2];
        plant.Textures = new() { plant.Textures[0] };

        TR1TexturePacker packer = new(cistern);
        TRTextileRegion region = packer.GetSpriteRegions(plant).Values.First().First();

        ResetLevel(cistern, 1);

        packer = new(cistern);
        packer.AddRectangle(region);
        packer.Pack(true);

        cistern.Sprites[TR1Type.Plant2] = plant;

        InjectionData data = InjectionData.Create(cistern, InjectionType.DisableAnimSprite, "cistern_plants");
        return new() { data };
    }
}
