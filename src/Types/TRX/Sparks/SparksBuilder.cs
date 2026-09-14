using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;
using TRXInjectionTool.Model;

namespace TRXInjectionTool.Types.TRX.Sparks;

public class SparksBuilder : InjectionBuilder, IPublisher
{
    public override string ID => "sparks_gfx";

    // The name the sparks answer to where a game keeps no sprites of this
    // kind: TR1 and TR2 have no slot to give them, and one past what their
    // files use is read as a static sprite rather than an object.
    private const string _symbol = "sparks_gfx";

    private static readonly Dictionary<TRGameVersion, int> _gameMap = new()
    {
        [TRGameVersion.TR3] = (int)TR3Type.MiscSprites_S_H,
        [TRGameVersion.TR4] = (int)TR4Type.DefaultSprites,
    };

    private static readonly TRGameVersion[] _namedGames =
    [
        TRGameVersion.TR1,
        TRGameVersion.TR2,
    ];

    public override List<InjectionData> Build()
    {
        var level = CreateLevel();
        var slotted = _gameMap.Select(kvp =>
        {
            var data = InjectionData.Create(level, InjectionType.General, ID);
            data.SpriteSequences[0].SpriteID = kvp.Value;
            data.GameVersion = kvp.Key;
            return data;
        });

        var named = _namedGames.Select(version =>
        {
            var data = InjectionData.Create(level, InjectionType.General, ID);
            data.GameVersion = version;
            data.Symbols.Add(new()
            {
                Context = SymbolContext.Objects,
                Slot = data.SpriteSequences[0].SpriteID,
                Name = _symbol,
            });
            return data;
        });

        return [.. slotted, .. named];
    }

    private static TR3Level CreateLevel()
    {
        var level3 = _control3.Read($"Resources/TR3/{TR3LevelNames.JUNGLE}");
        var level4 = _control4.Read($"Resources/TR4/{TR4LevelNames.SETH}");

        var sparks3 = level3.Sprites[TR3Type.MiscSprites_S_H];
        var sparks4 = level4.Sprites[TR4Type.DefaultSprites];

        var sequence = new TRSpriteSequence();
        sequence.Textures.AddRange(sparks4.Textures.GetRange(0, 11));  // explosion, water, footprint, particle
        sequence.Textures.AddRange(sparks3.Textures.GetRange(18, 8));  // shields
        sequence.Textures.AddRange(sparks4.Textures.GetRange(16, 12)); // rope, D/R, menu
        sequence.Textures.AddRange(sparks3.Textures.GetRange(13, 4));  // unknown
        sequence.Textures.AddRange(sparks4.Textures.GetRange(11, 5));  // unknown
        sequence.Textures.AddRange(sparks4.Textures.GetRange(28, 5));  // unknown

        sparks3.Textures.RemoveRange(10, 3); // no need to pack the fish; see TR3FishSpriteBuilder

        var packer3 = new TR3TexturePacker(level3);
        var packer4 = new TR4TexturePacker(level4, TRGroupPackingMode.Object);

        ResetLevel(level3, 1);
        var packer = new TR3TexturePacker(level3);
        packer.AddRectangles(packer3.GetSpriteRegions(sparks3).Values.SelectMany(r => r));
        packer.AddRectangles(packer4.GetSpriteRegions(sparks4).Values.SelectMany(r => r));

        packer.Pack(true);
        level3.Sprites[TR3Type.MiscSprites_S_H] = sequence;

        return level3;
    }

    public TRLevelBase Publish()
        => CreateLevel();

    public string GetPublishedName()
        => $"{ID}.tr2";
}
