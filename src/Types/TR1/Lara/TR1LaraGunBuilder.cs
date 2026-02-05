using TRImageControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Lara;

public class TR1LaraGunBuilder : InjectionBuilder
{
    private static readonly List<TR1SFX> _gymSoundsIDs =
    [
        TR1SFX.LaraShotgun,
        TR1SFX.LaraMagnums,
        TR1SFX.LaraUziFire,
        TR1SFX.MenuGuns,
    ];

    // These IDs aren't defined in TRLevelControl as doing so would affect
    // normal level IO (sound map limit).
    private static readonly Dictionary<TR2SFX, short> _tr2SoundIDs = new()
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

    private static readonly Dictionary<TR3SFX, short> _tr3SoundIDs = new()
    {
        [TR3SFX.DessertEagleFire] = 269,
        [TR3SFX.HecklerFire] = 270,
        [TR3SFX.BazookaFire] = 271,
    };

    private static readonly List<TR1Type> _animTypes =
    [
        TR1Type.LaraM16Anim_H,
        TR1Type.LaraGrenadeAnim_H,
        TR1Type.LaraHarpoonAnim_H,
        TR1Type.LaraFlareAnim_H,
        TR1Type.LaraAutoAnim_H,
        TR1Type.LaraDeagleAnim_H,
        TR1Type.LaraMP5Anim_H,
        TR1Type.LaraRocketAnim_H,
    ];

    public override string ID => "tr1_lara_guns";

    public override List<InjectionData> Build()
    {
        var result = new List<InjectionData>();

        foreach (var isGym in new[] { false, true })
        {
            var gunLevel = _control2.Read($"Resources/TR1/Lara/Guns/{(isGym ? "gym" : string.Empty)}guns.tr2");
            var level = CreateLevel(gunLevel, isGym);

            var data = InjectionData.Create(level, InjectionType.General, $"lara{(isGym ? "_gym" : string.Empty)}_guns");
            result.Add(data);

            data.Images.AddRange(gunLevel.Images16.Select(i =>
            {
                var img = new TRImage(i.Pixels);
                return new TRTexImage32 { Pixels = img.ToRGBA() };
            }));

            AddGunSounds(data, isGym);
            AddFlareSounds(data);
        }

        return result;
    }

    private static TR1Level CreateLevel(TR2Level gunLevel, bool gym)
    {
        var level = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        ResetLevel(level);

        foreach (var (type, model) in gunLevel.Models)
        {
            level.Models[(TR1Type)(int)type] = model;
        }
        foreach (var (type, sprite) in gunLevel.Sprites)
        {
            level.Sprites[(TR1Type)(int)type] = sprite;
        }

        level.ObjectTextures = gunLevel.ObjectTextures;
        UpdateAnimCommands(level);

        return level;
    }

    private static void UpdateAnimCommands(TR1Level level)
    {
        foreach (var fx in _animTypes.SelectMany(t => level.Models[t].Animations
            .SelectMany(a => a.Commands.OfType<TRSFXCommand>())))
        {
            if (_tr2SoundIDs.TryGetValue((TR2SFX)fx.SoundID, out var id2))
            {
                fx.SoundID = id2;
            }
            else if (_tr3SoundIDs.TryGetValue((TR3SFX)fx.SoundID, out var id3))
            {
                fx.SoundID = id3;
            }
        }

        foreach (var type in _animTypes)
        {
            if (type == TR1Type.LaraAutoAnim_H || type == TR1Type.LaraDeagleAnim_H)
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
    }

    private static void AddGunSounds(InjectionData data, bool isGym)
    {
        var level2 = _control2.Read($"Resources/{TR2LevelNames.GW}");
        foreach (var (id2, id1) in _tr2SoundIDs)
        {
            var fx = level2.SoundEffects[id2];
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

            data.SFX.Add(TRSFXData.Create(id1, fx));
            if (id2 == TR2SFX.M16Fire)
            {
                data.SFX[^1].Data = [File.ReadAllBytes("Resources/TR1/Lara/Guns/m16.wav")];
            }
        }

        var level3 = _control3.Read($"Resources/{TR3LevelNames.JUNGLE}");
        foreach (var (id3, id1) in _tr3SoundIDs)
        {
            var fx = level3.SoundEffects[id3];
            switch (id3)
            {
                case TR3SFX.DessertEagleFire:
                    fx.Mode = TR3SFXMode.Restart;
                    break;
                case TR3SFX.HecklerFire:
                    fx.Mode = TR3SFXMode.Wait;
                    fx.Volume = 230;
                    break;
                default:
                    break;
            }
            data.SFX.Add(TRSFXData.Create(id1, fx));
        }

        var pyramid = _control1.Read($"Resources/{TR1LevelNames.PYRAMID}");
        {
            // Explosion for rocket launcher
            var explosion = pyramid.SoundEffects[TR1SFX.Explosion];
            data.SFX.Add(TRSFXData.Create(TR1SFX.Explosion, explosion));
        }

        {
            // Explosion for grenade launcher. Use mode 3 to imply Normal in the engine.
            // The provided wav is a combination of AtlanteanExplode and Explosion1 as
            // TR2's samples sound out of place.
            var explosion = pyramid.SoundEffects[TR1SFX.AtlanteanExplode];
            explosion.Mode = (TR1SFXMode)3; // Normal
            explosion.Samples = [File.ReadAllBytes("Resources/TR1/Lara/Guns/grenade.wav")];
            data.SFX.Add(TRSFXData.Create(272, explosion));
        }

        if (isGym)
        {
            data.SFX.AddRange(_gymSoundsIDs.Select(
                id => TRSFXData.Create(id, pyramid.SoundEffects[id])));
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
}
