using TRDataControl;
using TRImageControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Lara;

public class TR2LaraGymGunBuilder : InjectionBuilder
{
    public override string ID => "lara_gym_guns";

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
        var basePalette = level.Palette.Select(c => c.ToTR1Color()).ToList();
        TR2DataImporter importer = new()
        {
            Level = level,
            DataFolder = "Resources/TR2/Objects",
            TypesToImport = new()
            {
                TR2Type.LaraPistolAnim_H_Assault,
                TR2Type.LaraAutoAnim_H_Assault,
                TR2Type.LaraUziAnim_H_Assault,
            },
        };
        importer.Import();

        ImportMagnums(level);
        ImportDeagle(level);
        ImportTR3Rifle(level, TR2Type.LaraMP5Anim_H);
        ImportTR3Rifle(level, TR2Type.LaraRocketAnim_H);

        var gunTypes = new[]
        {
            TR2Type.Pistols_M_H, TR2Type.Autos_M_H, TR2Type.Uzi_M_H, TR2Type.Shotgun_M_H,
            TR2Type.M16_M_H, TR2Type.GrenadeLauncher_M_H, TR2Type.Harpoon_M_H,
            TR2Type.LaraPistolAnim_H, TR2Type.LaraAutoAnim_H, TR2Type.LaraUziAnim_H, TR2Type.LaraShotgunAnim_H,
            TR2Type.LaraM16Anim_H, TR2Type.LaraGrenadeAnim_H, TR2Type.LaraHarpoonAnim_H,
            TR2Type.Gunflare_H, TR2Type.M16Gunflare_H,
            TR2Type.LaraMagnumAnim_H, TR2Type.Magnums_M_H, TR2Type.MagnumAmmo_M_H,
            TR2Type.LaraDeagleAnim_H, TR2Type.Deagle_M_H, TR2Type.DeagleAmmo_M_H,
            TR2Type.LaraMP5Anim_H, TR2Type.MP5_M_H, TR2Type.MP5Ammo_M_H,
            TR2Type.LaraRocketAnim_H, TR2Type.RocketLauncher_M_H, TR2Type.RocketAmmo_M_H, TR2Type.RocketProjectile_H,
        };

        var glassSFX = level.SoundEffects[TR2SFX.GlassBreak];
        CreateModelLevel(level, gunTypes);

        TR2Level gym = _control2.Read($"Resources/{TR2LevelNames.ASSAULT}");
        level.SoundEffects[TR2SFX.GlassBreak] = glassSFX;
        foreach (var sfx in gym.SoundEffects.Keys)
        {
            level.SoundEffects.Remove(sfx);
        }

        TR2GunUtils.FixHolsterSFX(level, false);
        TR2GunUtils.ConvertFlatFaces(level, basePalette);
        GenerateImages8(level, gym.Palette.Select(c => c.ToTR1Color()).ToList());

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

        static bool pred1(TRMeshFace f) => f.Vertices.All(v => v < 13 || (v >= 21 && v <= 25));
        static bool pred2(TRMeshFace f) => f.Vertices.All(v => v >= 20);
        foreach (var legIdx in new[] { 1, 4 })
        {
            var magLeg = level.Models[TR2Type.LaraMagnumAnim_H].Meshes[legIdx];
            var defLeg = level.Models[TR2Type.LaraPistolAnim_H].Meshes[legIdx].Clone();

            magLeg.TexturedTriangles.RemoveAll(pred1);
            magLeg.TexturedRectangles.RemoveAll(pred1);
            defLeg.TexturedTriangles.RemoveAll(pred2);
            defLeg.TexturedRectangles.RemoveAll(pred2);

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

            level.Models[TR2Type.LaraMagnumAnim_H].Meshes[legIdx] = defLeg;
        }
    }

    private static void ImportDeagle(TR2Level level)
    {
        new TR2DataImporter
        {
            Level = level,
            DataFolder = "Resources/TR2/Objects",
            TypesToImport = [TR2Type.LaraDeagleAnim_H],
        }.Import();

        static bool pred1(TRMeshFace f) => f.Vertices.All(v => v < 18);
        static bool pred2(TRMeshFace f) => f.Vertices.All(v => v >= 20);
        foreach (var legIdx in new[] { 1, 4 })
        {
            var magLeg = level.Models[TR2Type.LaraDeagleAnim_H].Meshes[legIdx];
            var defLeg = level.Models[TR2Type.LaraPistolAnim_H].Meshes[legIdx].Clone();

            if (legIdx == 1)
            {
                var remove = new ushort[] { 22,23,24,25,26,27,28,30,31,33,46,47,48,49,34,37,38,39,41,42,43,44 };
                defLeg.TexturedTriangles.RemoveAll(f => f.Vertices.All(remove.Contains));
                defLeg.TexturedRectangles.RemoveAll(f => f.Vertices.All(remove.Contains));
            }
            else
            {
                magLeg.TexturedTriangles.RemoveAll(pred1);
                magLeg.TexturedRectangles.RemoveAll(pred1);
                defLeg.TexturedTriangles.RemoveAll(pred2);
                defLeg.TexturedRectangles.RemoveAll(pred2);

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

            level.Models[TR2Type.LaraDeagleAnim_H].Meshes[legIdx] = defLeg;
        }
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
