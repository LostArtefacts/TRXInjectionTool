using TRImageControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Lara;

public class TR2LaraVegasGunBuilder : InjectionBuilder
{
    public override string ID => "lara_vegas_guns";

    public override List<InjectionData> Build()
    {
        TR2Level gym = Createlevel();
        InjectionData data = InjectionData.Create(gym, InjectionType.General, ID);
        return new() { data };
    }

    private static TR2Level Createlevel()
    {
        TR2Level level = _control2.Read($"Resources/{TR2LevelNames.VENICE}");
        var basePalette = level.Palette16.Select(c => c.ToTR1Color()).ToList();

        var gunTypes = new[]
        {
            TR2Type.M16_M_H, TR2Type.GrenadeLauncher_M_H, TR2Type.Harpoon_M_H,
            TR2Type.LaraM16Anim_H, TR2Type.LaraGrenadeAnim_H, TR2Type.LaraHarpoonAnim_H,
            TR2Type.Gunflare_H, TR2Type.M16Gunflare_H, TR2Type.HarpoonProjectile_H, TR2Type.GrenadeProjectile_H,
        };

        CreateModelLevel(level, gunTypes);

        TR2Level vegas = _control2.Read($"Resources/{TR2LevelNames.VEGAS}");
        foreach (var sfx in vegas.SoundEffects.Keys)
        {
            level.SoundEffects.Remove(sfx);
        }

        TR2GunUtils.ConvertFlatFaces(level, basePalette);
        GenerateImages8(level, vegas.Palette.Select(c => c.ToTR1Color()).ToList());

        return level;
    }
}
