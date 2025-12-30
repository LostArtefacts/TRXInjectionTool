using System.Diagnostics;
using System.Drawing;
using TRDataControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Lara;

public class TR1LaraGunBuilder : InjectionBuilder, IPublisher
{
    // These IDs aren't defined in TRLevelControl as doing so would affect
    // normal level IO (sound map limit).
    private static readonly Dictionary<TR2SFX, short> _soundIDs = new()
    {
        [TR2SFX.M16Fire] = 259,
        [TR2SFX.M16Stop] = 260,
        [TR2SFX.LaraMiniLoad] = 261,
        [TR2SFX.LaraMiniLock] = 262,
        [TR2SFX.LaraMiniFire] = 263,
        [TR2SFX.LaraHarpoonFire] = 264,
        [TR2SFX.LaraHarpoonLoad] = 265,
        [TR2SFX.LaraHarpoonLoadWater] = 266,
        [TR2SFX.LaraHarpoonFireWater] = 267,
        [TR2SFX.LaraFireMagnums] = 268,
    };

    private static readonly List<TR1Type> _animTypes =
    [
        TR1Type.LaraM16Anim_H,
        TR1Type.LaraGrenadeAnim_H,
        TR1Type.LaraHarpoonAnim_H,
        TR1Type.LaraFlareAnim_H,
        TR1Type.LaraAutoAnim_H,
    ];

    public override string ID => "tr1_lara_guns";

    public override List<InjectionData> Build()
    {
        var level = CreateLevel(false);
        var data = InjectionData.Create(level, InjectionType.General, "lara_guns");
        AddGunSounds(data);
        AddFlareSounds(data);

        var level2 = TR1LaraGymGunBuilder.CreateLevel();
        HandleTR2Guns(level2, true);
        var data2 = InjectionData.Create(level2, InjectionType.General, "lara_gym_guns");
        AddGunSounds(data2);
        AddFlareSounds(data2);

        return [data, data2];
    }

    private static TR1Level CreateLevel(bool gym)
    {
        var level = _control1.Read($"Resources/{TR1LevelNames.CAVES}");

        HandleShotgun(level);
        HandleTR2Guns(level, gym);

        return level;
    }

    private static void HandleShotgun(TR1Level level)
    {
        var model = level.Models[TR1Type.LaraShotgunAnim_H];
        var oldBack = model.Meshes[7];
        oldBack.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 30));
        oldBack.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 30));
        oldBack.ColouredRectangles.RemoveAll(f => f.Vertices.All(v => v < 30));
        oldBack.Vertices.RemoveRange(0, 30);
        oldBack.Normals.RemoveRange(0, 30);
        oldBack.TexturedFaces.Concat(oldBack.ColouredFaces).ToList().ForEach(f =>
        {
            for (int i = 0; i < f.Vertices.Count; i++)
            {
                f.Vertices[i] -= 30;
            }
        });

        {
            // Fix glove
            var gloveFace = model.Meshes[10].ColouredRectangles[1];
            gloveFace.Texture = level.Models[TR1Type.Lara].Meshes[10].TexturedRectangles[3].Texture;
            model.Meshes[10].ColouredRectangles.Remove(gloveFace);
            model.Meshes[10].TexturedRectangles.Add(gloveFace);
        }

        var hips = level.Models[TR1Type.Lara].Meshes[0];
        CreateModelLevel(level, TR1Type.LaraShotgunAnim_H);

        hips.TexturedFaces.Concat(hips.ColouredFaces)
            .ToList().ForEach(f => f.Texture = 0);
        hips.ColouredRectangles.AddRange(hips.TexturedRectangles);
        hips.ColouredTriangles.AddRange(hips.TexturedTriangles);
        hips.TexturedRectangles.Clear();
        hips.TexturedTriangles.Clear();

        model = level.Models[TR1Type.LaraShotgunAnim_H];
        oldBack = model.Meshes[7];
        for (int i = 0; i < 15; i++)
        {
            if (model.Meshes[i] == null || i == 7)
            {
                model.Meshes[i] = hips;
            }
        }

        oldBack.Vertices.ForEach(v =>
        {
            v.Y += 204;
            v.Z += 25;
        });
        oldBack.Centre = new() { X = 42, Y = 99, Z = 70 };
        oldBack.CollRadius = 105;
        model.Meshes[14] = oldBack;

        var wall = _control2.Read($"Resources/{TR2LevelNames.GW}");
        model.Animations = wall.Models[TR2Type.LaraShotgunAnim_H].Animations;
        // Fix using holster SFX on draw
        (model.Animations[1].Commands[0] as TRSFXCommand).SoundID = (short)TR1SFX.LaraDraw;
    }

    private static void HandleTR2Guns(TR1Level level, bool gym)
    {
        var dataDir = "Resources/TR1/Lara/Guns";
        if (gym)
        {
            var gymDir = dataDir + "/Gym";
            var data = new TR1DataProvider();
            foreach (var dep in _animTypes.SelectMany(t => data.GetDependencies(t)))
            {
                var file = Path.Combine(dataDir, TR1TypeUtilities.GetName(dep).ToUpper() + ".TRB");
                File.Copy(file, Path.Combine(gymDir, Path.GetFileName(file)), true);
            }
            dataDir = gymDir;
        }

        new TR1DataImporter
        {
            Level = level,
            DataFolder = dataDir,
            TypesToImport = _animTypes,
        }.Import();

        foreach (var fx in _animTypes.SelectMany(t => level.Models[t].Animations
            .SelectMany(a => a.Commands.OfType<TRSFXCommand>())))
        {
            if (_soundIDs.TryGetValue((TR2SFX)fx.SoundID, out var id))
            {
                fx.SoundID = id;
            }
        }

        foreach (var type in _animTypes)
        {
            if (type == TR1Type.LaraAutoAnim_H)
            {
                continue;
            }

            var equipAnim = level.Models[type].Animations[type == TR1Type.LaraGrenadeAnim_H ? 0 : 1];
            if (equipAnim.Commands.Count > 0)
            {
                (equipAnim.Commands[0] as TRSFXCommand).SoundID = (short)TR1SFX.LaraDraw;
            }

            if (type == TR1Type.LaraHarpoonAnim_H)
            {
                foreach (var id in new[] { 9, 10 })
                {
                    var uwUnequipAnim = level.Models[type].Animations[id];
                    uwUnequipAnim.Commands.Add(new TRSFXCommand
                    {
                        SoundID = (short)TR1SFX.LaraHolster,
                        FrameNumber = 20,
                    });
                }
            }
        }

        if (gym)
        {
            var col = Color.FromArgb(220, 160, 100);
            var i = level.Palette.FindIndex(c => c.Red == col.R && c.Green == col.G && c.Blue == col.B);
            Debug.Assert(i > 0);
            level.Models[TR1Type.LaraFlareAnim_H].Meshes[13].ColouredRectangles[0].Texture = (ushort)i;
        }
    }

    private static void AddGunSounds(InjectionData data)
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.GW}");
        foreach (var (id2, id1) in _soundIDs)
        {
            var fx = level.SoundEffects[id2];
            switch (id2)
            {
                case TR2SFX.M16Fire:
                    fx.Mode = TR2SFXMode.Wait;
                    fx.Volume = 0x5800;
                    break;
                case TR2SFX.M16Stop:
                    fx.Volume = 0x5800;
                    break;
                case TR2SFX.LaraHarpoonFire:
                case TR2SFX.LaraHarpoonFireWater:
                case TR2SFX.LaraFireMagnums:
                    fx.Mode = TR2SFXMode.Restart;
                    break;
                default:
                    break;
            }

            data.SFX.Add(new()
            {
                ID = id1,
                Chance = fx.Chance,
                Characteristics = fx.GetFlags(),
                Volume = fx.Volume,
                SampleOffset = fx.SampleID,
            });

            if (id2 == TR2SFX.M16Fire)
            {
                data.SFX[^1].Data = [File.ReadAllBytes("Resources/TR1/Lara/Guns/m16.wav")];
            }
            else
            {
                data.SFX[^1].LoadSFX(TRGameVersion.TR2);
            }
        }
    }

    private static void AddFlareSounds(InjectionData data)
    {
        var sfxLevel = _control3.Read($"Resources/{TR3LevelNames.JUNGLE}");
        var soundMap = new Dictionary<TR3SFX, short>
        {
            [TR3SFX.LaraFlareIgnite] = 257,
            [TR3SFX.LaraFlareBurn] = 258,
        };

        foreach (var (tr2Id, tr1Id) in soundMap)
        {
            var sfx = sfxLevel.SoundEffects[tr2Id];
            data.SFX.Add(new()
            {
                ID = tr1Id,
                Chance = sfx.Chance,
                Characteristics = 1 << 2, // mode=wait
                Volume = 13106,
                SampleOffset = sfx.SampleID,
            });
            data.SFX[^1].LoadSFX(TRGameVersion.TR3);
            if (tr2Id == TR3SFX.LaraFlareBurn)
            {
                data.SFX[^1].Data[0] = File.ReadAllBytes("Resources/TR1/Lara/Guns/flare.wav");
            }
        }
    }

    public string GetPublishedName()
        => "lara_guns.phd";

    public TRLevelBase Publish()
        => CreateLevel(false);
}
