using TRDataControl;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Lara;

public class TR2LaraHSHGunBuilder : InjectionBuilder
{
    public override string ID => "lara_house_guns";

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
        var basePalette = level.Palette16.Select(c => c.ToTR1Color()).ToList();
        TR2DataImporter importer = new()
        {
            Level = level,
            DataFolder = "Resources/TR2/Objects",
            TypesToImport = new()
            {
                TR2Type.LaraPistolAnim_H_Home,
                TR2Type.LaraAutoAnim_H_Home,
                TR2Type.LaraUziAnim_H_Home,
            },
        };
        importer.Import();

        ImportMagnums(level);

        level.Models[TR2Type.Lara].Meshes[0].TexturedRectangles.Clear();
        level.Models[TR2Type.Lara].Meshes[0].TexturedTriangles.Clear();
        level.Models[TR2Type.Lara].Meshes[0].ColouredRectangles.Clear();
        level.Models[TR2Type.Lara].Meshes[0].ColouredTriangles.Clear();

        var gunTypes = new[]
        {
            TR2Type.Pistols_M_H, TR2Type.Autos_M_H, TR2Type.Uzi_M_H,
            TR2Type.M16_M_H, TR2Type.GrenadeLauncher_M_H, TR2Type.Harpoon_M_H,
            TR2Type.LaraPistolAnim_H, TR2Type.LaraAutoAnim_H, TR2Type.LaraUziAnim_H,
            TR2Type.LaraM16Anim_H, TR2Type.LaraGrenadeAnim_H, TR2Type.LaraHarpoonAnim_H,
            TR2Type.M16Gunflare_H, TR2Type.GrenadeProjectile_H, TR2Type.HarpoonProjectile_H,
            TR2Type.LaraMagnumAnim_H, TR2Type.Magnums_M_H, TR2Type.MagnumAmmo_M_H,
        };

        CreateModelLevel(level, gunTypes);
        TR2GunUtils.ReplaceGlovedHands(level);

        TR2Level hsh = _control2.Read($"Resources/{TR2LevelNames.HOME}");
        foreach (var sfx in hsh.SoundEffects.Keys)
        {
            level.SoundEffects.Remove(sfx);
        }

        TR2GunUtils.FixHolsterSFX(level, false);
        TR2GunUtils.ConvertFlatFaces(level, basePalette);
        AddPistolsSprite(level);
        GenerateImages8(level, hsh.Palette.Select(c => c.ToTR1Color()).ToList());

        return level;
    }

    private static void ImportMagnums(TR2Level level)
    {
        new TR2DataImporter
        {
            Level = level,
            DataFolder = "Resources/TR2/Objects",
            TypesToImport = [TR2Type.LaraMagnumAnim_H],
        }.Import();
        level.Models[TR2Type.Magnums_M_H].Meshes[0].TexturedTriangles.Clear();

        static bool pred(TRMeshFace f) => f.Vertices.All(v => v < 13 || (v >= 21 && v <= 25));
        foreach (var legIdx in new[] { 1, 4 })
        {
            var magLeg = level.Models[TR2Type.LaraMagnumAnim_H].Meshes[legIdx];
            var defLeg = level.Models[TR2Type.LaraPistolAnim_H].Meshes[legIdx];

            magLeg.TexturedTriangles.RemoveAll(pred);
            magLeg.TexturedRectangles.RemoveAll(pred);
            magLeg.TexturedTriangles.AddRange(defLeg.TexturedTriangles.Where(pred));
            magLeg.TexturedRectangles.AddRange(defLeg.TexturedRectangles.Where(pred));
        }
    }

    private static void AddPistolsSprite(TR2Level level)
    {
        var wall = _control2.Read($"Resources/{TR2LevelNames.GW}");
        var sprite = wall.Sprites[TR2Type.Pistols_S_P];

        var packer = new TR2TexturePacker(wall);
        var regions = packer.GetSpriteRegions(sprite)
            .Values.SelectMany(r => r)
            .ToList();

        packer = new(level);
        packer.AddRectangles(regions);
        packer.Pack(true);

        level.Sprites[TR2Type.Pistols_S_P] = sprite;
    }
}
