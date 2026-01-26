using TRDataControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Lara;

public class TR2LaraGunBuilder : InjectionBuilder, IPublisher
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

    public override List<InjectionData> Build()
    {
        var result = new List<InjectionData>();
        foreach (var typeStr in new[] { string.Empty, "Unwater" })
        {
            var level = CreateLevel(typeStr);
            var data = InjectionData.Create(level, InjectionType.General,
                $"lara_{typeStr.ToLower()}{(typeStr.Length > 0 ? "_" : string.Empty)}guns");
            AddGunSounds(data);
            result.Add(data);
        }

        return result;
    }

    private static TR2Level CreateLevel(string typeStr)
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.GW}");
        ResetLevel(level, 1);
        level.Models[TR2Type.Lara] = new()
        {
            Meshes = [new() { Normals = [] }],
        };

        HandleTR1Guns(level, typeStr);
        level.Models.Remove(TR2Type.Lara);

        level.SoundEffects.Remove(TR2SFX.LaraFeet);
        level.SoundEffects.Remove(TR2SFX.LaraWetFeet);

        return level;
    }

    private static void HandleTR1Guns(TR2Level level, string typeStr)
    {
        var dataDir = "Resources/TR2/Objects";
        if (typeStr.Length > 0)
        {
            var gymDir = dataDir + $"/{typeStr}";
            var data = new TR2DataProvider();
            foreach (var dep in _animTypes.SelectMany(t => data.GetDependencies(t)))
            {
                var file = Path.Combine(dataDir, TR2TypeUtilities.GetName(dep).ToUpper() + ".TRB");
                File.Copy(file, Path.Combine(gymDir, Path.GetFileName(file)), true);
            }
            dataDir = gymDir;
        }

        new TR2DataImporter
        {
            Level = level,
            DataFolder = dataDir,
            TypesToImport = _animTypes,
        }.Import();

        level.Models[TR2Type.Magnums_M_H].Meshes[0].TexturedTriangles.Clear();

        FixGloves(level, TR2Type.LaraDeagleAnim_H);
        FixGloves(level, TR2Type.LaraMP5Anim_H);
        FixGloves(level, TR2Type.LaraRocketAnim_H);

        AmendAnimSFX(level);
    }

    public static void AmendAnimSFX(TR2Level level)
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
    }

    public static void FixGloves(TR2Level level, TR2Type type)
    {
        // Gloves are messed up, can't work out why. Do this for now until
        // proper controlled outfits are implemented.
        var handA = level.Models[type].Meshes[10];
        var handB = level.Models[TR2Type.LaraMagnumAnim_H].Meshes[10];
        handA.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 8));
        handA.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 8));
        handA.TexturedTriangles.AddRange(handB.TexturedTriangles.Where(f => f.Vertices.All(v => v < 8)));
        handA.TexturedRectangles.AddRange(handB.TexturedRectangles.Where(f => f.Vertices.All(v => v < 8)));
    }

    public static void AddGunSounds(InjectionData data)
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
    }

    public string GetPublishedName()
        => "lara_guns.tr2";

    public TRLevelBase Publish()
        => CreateLevel(string.Empty);
}
