using TRDataControl;
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
        TR2LaraGunBuilder.AddGunSounds(data);
        return [data];
    }

    private static TR2Level Createlevel()
    {
        TR2Level level = _control2.Read($"Resources/{TR2LevelNames.VENICE}");
        ImportMagnums(level);
        ImportTR3Rifle(level, TR2Type.LaraMP5Anim_H);
        var basePalette = level.Palette16.Select(c => c.ToTR1Color()).ToList();

        var gunTypes = new[]
        {
            TR2Type.M16_M_H, TR2Type.GrenadeLauncher_M_H, TR2Type.Harpoon_M_H,
            TR2Type.LaraM16Anim_H, TR2Type.LaraGrenadeAnim_H, TR2Type.LaraHarpoonAnim_H,
            TR2Type.Gunflare_H, TR2Type.M16Gunflare_H, TR2Type.HarpoonProjectile_H, TR2Type.GrenadeProjectile_H,
            TR2Type.LaraMagnumAnim_H, TR2Type.Magnums_M_H, TR2Type.MagnumAmmo_M_H,
            TR2Type.LaraDeagleAnim_H, TR2Type.Deagle_M_H, TR2Type.DeagleAmmo_M_H,
            TR2Type.LaraMP5Anim_H, TR2Type.MP5_M_H, TR2Type.MP5Ammo_M_H,
            TR2Type.LaraMP5Anim_H, TR2Type.MP5_M_H, TR2Type.MP5Ammo_M_H,
        };

        CreateModelLevel(level, gunTypes);

        TR2Level vegas = _control2.Read($"Resources/{TR2LevelNames.VEGAS}");
        foreach (var sfx in vegas.SoundEffects.Keys)
        {
            level.SoundEffects.Remove(sfx);
        }

        TR2GunUtils.FixHolsterSFX(level, false);
        TR2GunUtils.ConvertFlatFaces(level, basePalette);
        GenerateImages8(level, vegas.Palette.Select(c => c.ToTR1Color()).ToList());

        return level;
    }

    private static void ImportMagnums(TR2Level level)
    {
        new TR2DataImporter
        {
            Level = level,
            DataFolder = "Resources/TR2/Objects",
            TypesToImport = [TR2Type.LaraMagnumAnim_H, TR2Type.LaraDeagleAnim_H, TR2Type.LaraMP5Anim_H],
        }.Import();
        level.Models[TR2Type.Magnums_M_H].Meshes[0].TexturedTriangles.Clear();
        TR2LaraGunBuilder.FixGloves(level, TR2Type.LaraDeagleAnim_H);
        TR2LaraGunBuilder.FixGloves(level, TR2Type.LaraMP5Anim_H);
    }

    private static void ImportTR3Rifle(TR2Level level, TR2Type type)
    {
        new TR2DataImporter
        {
            Level = level,
            DataFolder = "Resources/TR2/Objects",
            TypesToImport = [type],
        }.Import();
        var handA = level.Models[type].Meshes[10];
        var handB = level.Models[TR2Type.LaraShotgunAnim_H].Meshes[10];
        handA.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 8));
        handA.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 8));
        handA.TexturedTriangles.AddRange(handB.TexturedTriangles.Where(f => f.Vertices.All(v => v < 8)));
        handA.TexturedRectangles.AddRange(handB.TexturedRectangles.Where(f => f.Vertices.All(v => v < 8)));
    }
}
