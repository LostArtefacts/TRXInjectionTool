using TRDataControl;
using TRImageControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;
using TRXInjectionTool.Types.TR2.Lara;

namespace TRXInjectionTool.Types.TR3.Lara;

public class TR3LaraGymGunBuilder : InjectionBuilder
{
    public override string ID => "tr3_lara_gym_guns";

    public override List<InjectionData> Build()
    {
        var gym = Createlevel();
        var data = InjectionData.Create(gym, InjectionType.General, "lara_gym_guns");
        TR3LaraGunBuilder.AddGunSounds(data, true);
        return [data];
    }

    private static TR3Level Createlevel()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.NEVADA}");
        new TR3DataImporter
        {
            Level = level,
            DataFolder = "Resources/TR3/Objects",
            TypesToImport = [TR3Type.LaraSkin_H_Home],
        }.Import();

        var basePalette = level.Palette16.Select(c => c.ToTR1Color()).ToList();
        TR2GunUtils.ConvertFlatFaces(level, basePalette);

        FixLegs(level, TR3Type.LaraUziAnimation_H);
        FixLegs(level, TR3Type.LaraDeagleAnimation_H);

        ImportMagnums(level);
        ImportTR2Rifle(level, TR3Type.LaraM16Anim_H);

        var hips = level.Models[TR3Type.Lara].Meshes[0];
        hips.TexturedRectangles.Clear();
        hips.TexturedTriangles.Clear();
        hips.Vertices.Clear();
        hips.Normals.Clear();

        var gunTypes = new[]
        {
            TR3Type.LaraShotgunAnimation_H, TR3Type.Shotgun_M_H, TR3Type.ShotgunAmmo_M_H,
            TR3Type.LaraDeagleAnimation_H, TR3Type.Deagle_M_H,  TR3Type.DeagleAmmo_M_H,
            TR3Type.LaraUziAnimation_H, TR3Type.Uzis_M_H, TR3Type.UziAmmo_M_H,
            TR3Type.LaraHarpoonAnimation_H, TR3Type.Harpoon_M_H, TR3Type.Harpoons_M_H, TR3Type.HarpoonSingle2,
            TR3Type.LaraMP5Animation_H, TR3Type.MP5_M_H, TR3Type.MP5Ammo_M_H, TR3Type.GunflareMP5_H,
            TR3Type.LaraGrenadeAnimation_H, TR3Type.GrenadeLauncher_M_H, TR3Type.Grenades_M_H, TR3Type.GrenadeSingle,
            TR3Type.LaraRocketAnimation_H, TR3Type.RocketLauncher_M_H, TR3Type.Rockets_M_H, TR3Type.RocketSingle,
            TR3Type.LaraMagnumAnim_H, TR3Type.Magnums_M_H, TR3Type.MagnumAmmo_M_H,
            TR3Type.LaraAutoAnim_H, TR3Type.Autos_M_H, TR3Type.AutoAmmo_M_H,
            TR3Type.LaraM16Anim_H, TR3Type.M16_M_H, TR3Type.M16Ammo_M_H,
        };

        CreateModelLevel(level, gunTypes);
        
        GenerateImages8(level, [.. level.Palette.Select(c => c.ToTR1Color())]);
        level.SoundEffects.Clear();
        return level;
    }

    private static void FixLegs(TR3Level level, TR3Type type)
    {
        static bool pred1(TRMeshFace f) => f.Vertices.All(v => v < 26);
        static bool pred2(TRMeshFace f) => f.Vertices.All(v => v < 18);
        static bool pred3(TRMeshFace f) => f.Vertices.All(v => v > 12 && v < 21);
        foreach (var legIdx in new[] { 1, 4 })
        {
            var magLeg = level.Models[type].Meshes[legIdx];
            var defLeg = level.Models[TR3Type.LaraSkin_H].Meshes[legIdx].Clone();
            level.Models[type].Meshes[legIdx] = defLeg;
            if (type == TR3Type.LaraDeagleAnimation_H && legIdx == 1)
            {
                continue;
            }

            var native = type == TR3Type.LaraUziAnimation_H || type == TR3Type.LaraDeagleAnimation_H;
            magLeg.TexturedTriangles.RemoveAll(native ? pred2 : pred1);
            magLeg.TexturedRectangles.RemoveAll(native ? pred2 : pred1);
            if (native)
            {
                defLeg.TexturedTriangles.RemoveAll(pred3);
                defLeg.TexturedRectangles.RemoveAll(pred3);
            }

            var newFaces = magLeg.TexturedFaces.ToList();
            var newVerts = newFaces.SelectMany(f => f.Vertices)
                .Distinct().ToList();
            var map = new Dictionary<ushort, ushort>();
            foreach (var vert in newVerts)
            {
                map[vert] = (ushort)defLeg.Vertices.Count;
                defLeg.Vertices.Add(magLeg.Vertices[vert]);
                defLeg.Normals.Add(magLeg.Normals[vert]);
            }

            foreach (var face in newFaces)
            {
                for (int i = 0; i < face.Vertices.Count; i++)
                {
                    face.Vertices[i] = map[face.Vertices[i]];
                }
                if (face.Type == TRFaceType.Triangle)
                {
                    defLeg.TexturedTriangles.Add(face);
                }
                else
                {
                    defLeg.TexturedRectangles.Add(face);
                }
            }
        }
    }

    private static void ImportMagnums(TR3Level level)
    {
        var types = new[] { TR3Type.LaraMagnumAnim_H, TR3Type.LaraAutoAnim_H };
        new TR3DataImporter
        {
            Level = level,
            DataFolder = "Resources/TR3/Objects",
            TypesToImport = [.. types],
        }.Import();
        level.Models[TR3Type.Magnums_M_H].Meshes[0].TexturedTriangles.Clear();
        level.Models[TR3Type.Autos_M_H].Meshes[0].TexturedTriangles.Clear();

        foreach (var type in types)
        {
            FixLegs(level, type);
        }
    }

    private static void ImportTR2Rifle(TR3Level level, TR3Type type)
    {
        new TR3DataImporter
        {
            Level = level,
            DataFolder = "Resources/TR3/Objects",
            TypesToImport = [type],
        }.Import();
        var handA = level.Models[type].Meshes[10];
        var handB = level.Models[TR3Type.LaraShotgunAnimation_H].Meshes[10];
        handA.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 8));
        handA.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 8));
        handA.TexturedTriangles.AddRange(handB.TexturedTriangles.Where(f => f.Vertices.All(v => v < 8)));
        handA.TexturedRectangles.AddRange(handB.TexturedRectangles.Where(f => f.Vertices.All(v => v < 8)));
    }
}
