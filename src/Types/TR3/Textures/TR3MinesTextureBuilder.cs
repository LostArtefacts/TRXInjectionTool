using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Textures;

public class TR3MinesTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.TextureFix, "mines_textures");
        CreateDefaultTests(data, $"TR3/{TR3LevelNames.RXTECH}");
        data.AnimTextureEdits.AddRange(FixCartTracks());
        return [data];
    }

    private static IEnumerable<TRAnimTextureEdit> FixCartTracks()
    {
        // The PC version has four unused animation ranges for the cart tracks.
        // Replace the first known texture in these with actual placed map textures.
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.RXTECH}");
        var remap = new Dictionary<ushort, ushort>
        {
            [2167] = 1576, // Snow, NW_Clockwise
            [2171] = 1635, // Snow, NE_AntiClockwise
            [2179] = 1945, // Metal, NW_Clockwise
            [2175] = 1900, // Metal, NE_AntiClockwise
        };

        for (int i = 0; i < level.AnimatedTextures.Count; i++)
        {
            var range = level.AnimatedTextures[i];
            for (int j = 0; j < range.Textures.Count; j++)
            {
                if (!remap.TryGetValue(range.Textures[j], out var newTexture))
                {
                    continue;
                }

                range.Textures[j] = newTexture;
                yield return new()
                {
                    Index = i,
                    Textures = range.Textures,
                };
                break;
            }
        }
    }
}
