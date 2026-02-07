using TRImageControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Lara;

public class TR3LaraGunBuilder : InjectionBuilder
{
    // These IDs aren't defined in TRLevelControl as doing so would affect
    // normal level IO (sound map limit).
    private static readonly Dictionary<TR1SFX, short> _tr1SoundIDs = new()
    {
        [TR1SFX.LaraMagnums] = 370,
    };
    private static readonly Dictionary<TR2SFX, short> _tr2SoundIDs = new()
    {
        [TR2SFX.LaraFireMagnums] = 371,
        [TR2SFX.M16Fire] = 372,
        [TR2SFX.M16Stop] = 373,
    };

    private static readonly List<TR3SFX> _tr3SoundIDs =
    [
        TR3SFX.LaraShotgun, TR3SFX.LaraShotgunShell, TR3SFX.LaraReload,
        TR3SFX.LaraMiniLoad, TR3SFX.LaraMiniLock, TR3SFX.LaraMiniFire,
        TR3SFX.BazookaFire, TR3SFX.LaraHarpoonFire, TR3SFX.LaraHarpoonFireWater,
        TR3SFX.LaraHarpoonLoad, TR3SFX.LaraHarpoonLoadWater,
    ];

    private static readonly List<TR3Type> _animTypes =
    [
        TR3Type.LaraMagnumAnim_H,
        TR3Type.LaraAutoAnim_H,
        TR3Type.LaraM16Anim_H,
    ];

    public override string ID => "tr3_lara_guns";

    public override List<InjectionData> Build()
    {
        var jungleLevel = _control3.Read($"Resources/{TR3LevelNames.JUNGLE}");

        var result = new List<InjectionData>();

        foreach (var isGym in new[] { false, true })
        {
            var gunLevel = _control2.Read($"Resources/TR3/Lara/Guns/{(isGym ? "gym" : string.Empty)}guns.tr2");
            var level = CreateLevel(gunLevel, isGym);

            var data = InjectionData.Create(level, InjectionType.General, $"lara{(isGym ? "_gym" : string.Empty)}_guns");
            result.Add(data);

            data.Images.AddRange(gunLevel.Images16.Select(i =>
            {
                var img = new TRImage(i.Pixels);
                return new TRTexImage32 { Pixels = img.ToRGBA() };
            }));

            AddGunSounds(data, isGym);
        }

        return result;
    }

    private static TR3Level CreateLevel(TR2Level gunLevel, bool gym)
    {
        var level = _control3.Read($"Resources/{TR3LevelNames.JUNGLE}");
        ResetLevel(level);

        foreach (var (type, model) in gunLevel.Models)
        {
            level.Models[(TR3Type)(int)type] = model;
        }

        level.ObjectTextures = gunLevel.ObjectTextures;
        UpdateAnimCommands(level);

        return level;
    }

    private static void UpdateAnimCommands(TR3Level level)
    {
        foreach (var fx in _animTypes.SelectMany(t => level.Models[t].Animations
            .SelectMany(a => a.Commands.OfType<TRSFXCommand>())))
        {
            if (_tr1SoundIDs.TryGetValue((TR1SFX)fx.SoundID, out var id))
            {
                fx.SoundID = id;
            }
        }
        level.SoundEffects.Remove(TR3SFX.LaraHolster);
    }

    private static void AddGunSounds(InjectionData data, bool gym)
    {
        var level = _control1.Read($"Resources/{TR1LevelNames.PYRAMID}");
        foreach (var (id1, id2) in _tr1SoundIDs)
        {
            var fx = level.SoundEffects[id1];
            if (id1 == TR1SFX.LaraMagnums)
            {
                fx.Mode = TR1SFXMode.Ambient;
                fx.Volume = 24576;
            }
            data.SFX.Add(TRSFXData.Create(id2, fx));
        }

        var wall = _control2.Read($"Resources/{TR2LevelNames.GW}");
        foreach (var (id1, id2) in _tr2SoundIDs)
        {
            var fx = wall.SoundEffects[id1];
            if (id1 == TR2SFX.M16Fire)
            {
                fx.Volume = 24576;
            }
            data.SFX.Add(TRSFXData.Create(id2, fx));
        }

        if (gym)
        {
            var jungle = _control3.Read($"Resources/{TR3LevelNames.JUNGLE}");
            data.SFX.AddRange(_tr3SoundIDs.Select(s => TRSFXData.Create(s, jungle.SoundEffects[s])));
        }
    }
}
