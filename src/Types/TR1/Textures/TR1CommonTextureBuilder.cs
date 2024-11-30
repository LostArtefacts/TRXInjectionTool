using System.Diagnostics;
using System.Drawing;
using TRImageControl;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public static class TR1CommonTextureBuilder
{
    public static void FixWolfTransparency(TR1Level level, InjectionData data)
    {
        List<ushort> eyeVerts = new() { 20, 13, 12, 22 };
        TRMeshFace eyeFace = level.Models[TR1Type.Wolf]
            .Meshes[3]
            .TexturedRectangles.Find(t => t.Vertices.All(eyeVerts.Contains));

        FixTransparentPixels(level, data, eyeFace, Color.FromArgb(228, 228, 228));
    }

    public static void FixBatTransparency(TR1Level level, InjectionData data)
    {
        List<ushort> eyeVerts = new() { 0, 1, 3 };
        TRMeshFace eyeFace = level.Models[TR1Type.Bat]
            .Meshes[4]
            .TexturedTriangles.Find(t => t.Vertices.All(eyeVerts.Contains));

        FixTransparentPixels(level, data, eyeFace, Color.Black);
    }

    public static void FixTransparentPixels(TR1Level level, InjectionData data, TRFace face, Color fillColour)
    {
        Debug.Assert(face != null);

        TRObjectTexture texInfo = level.ObjectTextures[face.Texture];
        TRImage tile = new(level.Images8[texInfo.Atlas].Pixels, level.Palette);
        TRImage img = tile.Export(texInfo.Bounds);

        List<Color> palette = new()
        {
            Color.Magenta,
        };

        palette.AddRange(data.TextureOverwrites
            .SelectMany(o => o.Data)
            .Where(p => p != 0)
            .Distinct()
            .Select(p => Color.FromArgb(data.Palette[p].Red, data.Palette[p].Green, data.Palette[p].Blue)));

        img.Write((c, x, y) =>
        {
            c = c.A == 0 ? fillColour : c;
            if (!palette.Contains(c))
            {
                palette.Add(c);
            }

            return c;
        });

        while (palette.Count < 256)
        {
            palette.Add(Color.Black);
        }

        List<TRColour> trPalette = new(palette.Select(c => c.ToTRColour()));
        byte[] pixels = img.ToRGB(trPalette);

        for (int i = 0; i < data.Palette.Count; i++)
        {
            data.Palette[i].Red = trPalette[i].Red;
            data.Palette[i].Green = trPalette[i].Green;
            data.Palette[i].Blue = trPalette[i].Blue;
        }

        data.TextureOverwrites.Add(new()
        {
            Page = texInfo.Atlas,
            X = (byte)texInfo.Bounds.X,
            Y = (byte)texInfo.Bounds.Y,
            Width = (ushort)texInfo.Size.Width,
            Height = (ushort)texInfo.Size.Height,
            Data = pixels,
        });
    }
}
