using TRImageControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Lara;

public class TR2LaraGunBuilder : InjectionBuilder
{
    // These IDs aren't defined in TRLevelControl as doing so would affect
    // normal level IO (sound map limit).
    private static readonly Dictionary<TR1SFX, short> _tr1SoundIDs = new()
    {
        [TR1SFX.LaraMagnums] = 370,
        [TR1SFX.Explosion] = 373,
    };

    private static readonly Dictionary<TR3SFX, short> _tr3SoundIDs = new()
    {
        [TR3SFX.DessertEagleFire] = 371,
        [TR3SFX.HecklerFire] = 372,
        [TR3SFX.BazookaFire] = 374,
    };

    private static readonly List<TR2Type> _animTypes =
    [
        TR2Type.LaraMagnumAnim_H,
        TR2Type.LaraDeagleAnim_H,
        TR2Type.LaraMP5Anim_H,
        TR2Type.LaraRocketAnim_H,
    ];

    public override string ID => "tr2_lara_guns";

    private enum LevelType
    {
        Normal,
        Gym,
        House,
        Vegas,
    }

    private static string GetTypeName(LevelType type)
        => type == LevelType.Normal ? string.Empty : type.ToString().ToLower();

    public override List<InjectionData> Build()
    {
        var result = new List<InjectionData>();
        foreach (var type in Enum.GetValues<LevelType>())
        {
            var name = GetTypeName(type);
            var gunLevel = _control2.Read($"Resources/TR2/Lara/Guns/{name}guns.tr2");
            var level = CreateLevel(gunLevel, type);            

            var data = InjectionData.Create(level, InjectionType.General,
                $"lara_{name}{(name.Length > 0 ? "_" : string.Empty)}guns");
            result.Add(data);

            data.Images.AddRange(gunLevel.Images16.Select(i =>
            {
                var img = new TRImage(i.Pixels);
                return new TRTexImage32 { Pixels = img.ToRGBA() };
            }));

            AddGunSounds(data, type);
        }

        return result;
    }

    private static TR2Level CreateLevel(TR2Level gunLevel, LevelType levelType)
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.GW}");
        ResetLevel(level);

        foreach (var (type, model) in gunLevel.Models)
        {
            level.Models[type] = model;
        }
        foreach (var (type, sprite) in gunLevel.Sprites)
        {
            level.Sprites[type] = sprite;
        }

        level.ObjectTextures = gunLevel.ObjectTextures;

        UpdateAnimCommands(level, levelType);

        return level;
    }

    private static void UpdateAnimCommands(TR2Level level, LevelType levelType)
    {
        foreach (var type in new[] { TR2Type.LaraMP5Anim_H, TR2Type.LaraRocketAnim_H })
        {
            var equipAnim = level.Models[type].Animations[1];
            if (equipAnim.Commands.Count > 0)
            {
                (equipAnim.Commands[0] as TRSFXCommand).SoundID = (short)TR2SFX.LaraDraw;
            }
        }

        foreach (var fx in _animTypes.SelectMany(t => level.Models[t].Animations
            .SelectMany(a => a.Commands.OfType<TRSFXCommand>())))
        {
            if (_tr1SoundIDs.TryGetValue((TR1SFX)fx.SoundID, out var id1))
            {
                fx.SoundID = id1;
            }
            else if (_tr3SoundIDs.TryGetValue((TR3SFX)fx.SoundID, out var id3))
            {
                fx.SoundID = id3;
            }
        }

        if (levelType == LevelType.Gym)
        {
            TR2GunUtils.FixHolsterSFX(level, false);
        }
    }

    private static void AddGunSounds(InjectionData data, LevelType levelType)
    {
        var level = _control1.Read($"Resources/{TR1LevelNames.PYRAMID}");
        foreach (var (id1, id2) in _tr1SoundIDs)
        {
            var fx = level.SoundEffects[id1];
            if (id1 == TR1SFX.LaraMagnums)
            {
                fx.Mode = TR1SFXMode.Ambient;
            }
            data.SFX.Add(TRSFXData.Create(id2, fx));
        }

        var level3 = _control3.Read($"Resources/{TR3LevelNames.JUNGLE}");
        foreach (var (id3, id1) in _tr3SoundIDs)
        {
            var fx = level3.SoundEffects[id3];
            if (id3 == TR3SFX.HecklerFire)
            {
                fx.Volume = 230;
            }
            data.SFX.Add(TRSFXData.Create(id1, fx));
        }

        if (levelType == LevelType.Gym)
        {
            var venice = _control2.Read($"Resources/{TR2LevelNames.VENICE}");
            data.SFX.Add(TRSFXData.Create(TR2SFX.GlassBreak, venice.SoundEffects[TR2SFX.GlassBreak]));
        }
    }
}
