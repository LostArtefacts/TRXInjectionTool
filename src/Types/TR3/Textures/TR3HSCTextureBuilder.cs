using System.Drawing;
using TRImageControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Textures;

public class TR3HSCTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.HSC}");
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.TextureFix, "compound_textures");
        CreateDefaultTests(data, $"TR3/{TR3LevelNames.HSC}");

        data.RoomEdits.AddRange(CreateRefacings(level));
        data.TextureOverwrites.Add(FixGrating(level, 1859));

        return [data];
    }

    private static IEnumerable<TRRoomTextureReface> CreateRefacings(TR3Level level)
    {
        // Fix the conveyor belt only being partially animated
        var beltMapA = new Dictionary<short, List<short>>
        {
            [176] = [0, 3, 8, 12, 16, 21, 25, 29, 34, 38],
            [103] = [17, 20, 26],
        };
        foreach (var (room, faces) in beltMapA)
        {
            foreach (var face in faces)
            {
                yield return Reface(level, room, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1978, face);
            }
        }

        var beltMapB = new Dictionary<short, List<short>>
        {
            [176] = [44, 47, 51, 54, 58, 62, 65, 69, 73, 76],
            [103] = [19, 22, 33, 36],
        };
        foreach (var (room, faces) in beltMapB)
        {
            foreach (var face in faces)
            {
                yield return Reface(level, room, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1979, face);
            }
        }

        // Wrong face on window in room 105
        yield return Reface(level, 105, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1930, 4);

        // Fix z-fighting in room 179
        foreach (var face in new[] { 11, 15, 19 })
        {
            yield return Reface(level, 179, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1912, (short)face);
        }
    }

    public static TRTextureOverwrite FixGrating(TR3Level level, int textureId)
    {
        var texInfo = level.ObjectTextures[textureId];
        var tile = new TRImage(level.Images16[texInfo.Atlas].Pixels);
        var img = tile.Export(texInfo.Bounds);

        var magenta = new[]
        {
            Color.FromArgb(164, 106, 148),
            Color.FromArgb(123, 24, 98),
            Color.FromArgb(131, 32, 115),
            Color.FromArgb(90, 32, 82),
            Color.FromArgb(98, 24, 74),
            Color.FromArgb(98, 32, 82),
        };
        img.Write((c, x, y) =>
        {
            if (c.A == 0)
            {
                return img.GetPixel(x + 1, y);
            }
            if (magenta.Contains(c))
            {
                return img.GetPixel(x, y - 1);
            }
            return c;
        });

        return new()
        {
            Page = texInfo.Atlas,
            X = (byte)texInfo.Bounds.X,
            Y = (byte)texInfo.Bounds.Y,
            Width = (ushort)texInfo.Size.Width,
            Height = (ushort)texInfo.Size.Height,
            Data = img.ToRGBA(),
        };
    }
}
