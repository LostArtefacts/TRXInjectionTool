using TRDataControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Lara;

public class TR2LaraGunBuilder : InjectionBuilder, IPublisher
{
    // These IDs aren't defined in TRLevelControl as doing so would affect
    // normal level IO (sound map limit).
    private static readonly Dictionary<TR1SFX, short> _soundIDs = new()
    {
        [TR1SFX.LaraMagnums] = 370,
    };

    private static readonly List<TR2Type> _animTypes =
    [
        TR2Type.LaraMagnumAnim_H,
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

        foreach (var fx in _animTypes.SelectMany(t => level.Models[t].Animations
            .SelectMany(a => a.Commands.OfType<TRSFXCommand>())))
        {
            if (_soundIDs.TryGetValue((TR1SFX)fx.SoundID, out var id))
            {
                fx.SoundID = id;
            }
        }
    }

    public static void AddGunSounds(InjectionData data)
    {
        var level = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        foreach (var (id1, id2) in _soundIDs)
        {
            var fx = level.SoundEffects[id1];
            fx.Mode = TR1SFXMode.Ambient;

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
