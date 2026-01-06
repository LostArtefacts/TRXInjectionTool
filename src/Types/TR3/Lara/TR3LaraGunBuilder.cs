using TRDataControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Lara;

public class TR3LaraGunBuilder : InjectionBuilder, IPublisher
{
    // These IDs aren't defined in TRLevelControl as doing so would affect
    // normal level IO (sound map limit).
    private static readonly Dictionary<TR1SFX, short> _tr1SoundIDs = new()
    {
        [TR1SFX.LaraMagnums] = 370,
    };

    private static readonly List<TR3Type> _animTypes =
    [
        TR3Type.LaraMagnumAnim_H,
    ];

    public override string ID => "tr3_lara_guns";

    public override List<InjectionData> Build()
    {
        var result = new List<InjectionData>();
        foreach (var typeStr in new[] { string.Empty, "Coastal", "London", "Nevada", "Antarc" })
        {
            var level = CreateLevel(typeStr);
            var data = InjectionData.Create(level, InjectionType.General,
                $"lara_{typeStr.ToLower()}{(typeStr.Length > 0 ? "_" : string.Empty)}guns");
            AddGunSounds(data);
            result.Add(data);
        }

        return result;
    }

    private static TR3Level CreateLevel(string typeStr)
    {
        var level = _control3.Read($"Resources/{TR3LevelNames.JUNGLE}");
        ResetLevel(level, 1);
        level.Models[TR3Type.Lara] = new()
        {
            Meshes = [new() { Normals = [] }],
        };

        HandleTR1Guns(level, typeStr);
        level.Models.Remove(TR3Type.Lara);

        return level;
    }

    private static void HandleTR1Guns(TR3Level level, string typeStr)
    {
        var dataDir = "Resources/TR3/Objects";
        if (typeStr.Length > 0)
        {
            var gymDir = dataDir + $"/{typeStr}";
            var data = new TR3DataProvider();
            foreach (var dep in _animTypes.SelectMany(t => data.GetDependencies(t)))
            {
                var file = Path.Combine(dataDir, TR3TypeUtilities.GetName(dep).ToUpper() + ".TRB");
                File.Copy(file, Path.Combine(gymDir, Path.GetFileName(file)), true);
            }
            dataDir = gymDir;
        }

        new TR3DataImporter
        {
            Level = level,
            DataFolder = dataDir,
            TypesToImport = _animTypes,
        }.Import();

        level.Models[TR3Type.Magnums_M_H].Meshes[0].TexturedTriangles.Clear();

        //FixGloves(level, TR2Type.LaraDeagleAnim_H);
        //FixGloves(level, TR2Type.LaraMP5Anim_H);
        //FixGloves(level, TR2Type.LaraRocketAnim_H);

        foreach (var fx in _animTypes.SelectMany(t => level.Models[t].Animations
            .SelectMany(a => a.Commands.OfType<TRSFXCommand>())))
        {
            if (_tr1SoundIDs.TryGetValue((TR1SFX)fx.SoundID, out var id))
            {
                fx.SoundID = id;
            }
        }
    }

    public static void FixGloves(TR3Level level, TR3Type type)
    {
        // Gloves are messed up, can't work out why. Do this for now until
        // proper controlled outfits are implemented.
        var handA = level.Models[type].Meshes[10];
        var handB = level.Models[TR3Type.LaraMagnumAnim_H].Meshes[10];
        handA.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 8));
        handA.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 8));
        handA.TexturedTriangles.AddRange(handB.TexturedTriangles.Where(f => f.Vertices.All(v => v < 8)));
        handA.TexturedRectangles.AddRange(handB.TexturedRectangles.Where(f => f.Vertices.All(v => v < 8)));
    }

    public static void AddGunSounds(InjectionData data)
    {
        var level = _control1.Read($"Resources/{TR1LevelNames.PYRAMID}");
        var pistols = _control3.Read($"Resources/{TR3LevelNames.JUNGLE}").SoundEffects[TR3SFX.LaraFire];
        foreach (var (id1, id2) in _tr1SoundIDs)
        {
            var fx = level.SoundEffects[id1];
            if (id1 == TR1SFX.LaraMagnums)
            {
                fx.Mode = TR1SFXMode.Ambient;
                fx.Volume = 24576;
            }

            data.SFX.Add(new()
            {
                ID = id2,
                Chance = fx.Chance,
                Characteristics = fx.GetFlags(),
                Volume = fx.Volume,
                Data = fx.Samples,
            });
        }
    }

    public string GetPublishedName()
        => "lara_guns.tr2";

    public TRLevelBase Publish()
        => CreateLevel(string.Empty);
}
