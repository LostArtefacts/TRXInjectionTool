using System.Drawing;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Model;

namespace TRXInjectionTool.Util;

public static class TRFaceConverter
{
    public static void ConvertFlatFaces(TR2Level level, List<Color> sourcePalette)
    {
        ConvertFlatFaces(level, sourcePalette, level.Models.Values);
    }

    public static void ConvertFlatFaces(TR3Level level, List<Color> sourcePalette)
    {
        ConvertFlatFaces(level, sourcePalette, level.Models.Values);
    }

    public static void ConvertFlatFaces<TLevel>(
        TLevel level,
        IReadOnlyList<Color> sourcePalette,
        IEnumerable<TRModel> models)
        where TLevel : TRLevelBase
    {
        ConvertFlatFaces(level, sourcePalette, models
            .SelectMany(m => m.Meshes)
            .ToList());
    }

    // Turns the flat-coloured faces of the meshes into textured ones, each
    // drawn from a swatch of its colour. The palette index of a flat face sits
    // in the high byte of its texture in TR2 and TR3, which is the default, and
    // in the texture itself in TR1.
    public static void ConvertFlatFaces<TLevel>(
        TLevel level,
        IReadOnlyList<Color> sourcePalette,
        List<TRMesh> meshList,
        Func<ushort, int> paletteIndex = null)
        where TLevel : TRLevelBase
    {
        paletteIndex ??= texture => texture >> 8;
        TRTexturePacker packer = level switch
        {
            TR2Level level2 => new TR2TexturePacker(level2),
            TR3Level level3 => new TR3TexturePacker(level3),
            _ => throw new InvalidOperationException("Only TR2/TR3 levels are supported."),
        };

        var objectTextures = level.ObjectTextures;
        var ids = meshList
            .SelectMany(m => m.ColouredFaces)
            .Select(f => paletteIndex(f.Texture))
            .Distinct()
            .ToList();
        if (ids.Count == 0)
        {
            return;
        }

        var map = new Dictionary<int, int>();
        var regions = new List<TRTextileRegion>();
        int i = 0;
        foreach (var id in ids)
        {
            var img = new TRImage(8, 8);
            img.Fill(sourcePalette[id]);
            map[id] = objectTextures.Count + regions.Count;

            var texInfo = new TRObjectTexture(0, 0, 8, 8);
            regions.Add(new()
            {
                Image = img,
                Bounds = texInfo.Bounds,
                Segments = new()
                {
                    new()
                    {
                        Index = i++,
                        Texture = texInfo,
                    }
                }
            });
        }

        packer.AddRectangles(regions);
        packer.Pack(true);
        objectTextures.AddRange(regions.Select(r => r.Segments.First().Texture as TRObjectTexture));
        foreach (var mesh in meshList)
        {
            mesh.ColouredRectangles.ForEach(f => f.Texture = (ushort)map[paletteIndex(f.Texture)]);
            mesh.ColouredTriangles.ForEach(f => f.Texture = (ushort)map[paletteIndex(f.Texture)]);
            mesh.TexturedRectangles.AddRange(mesh.ColouredRectangles);
            mesh.TexturedTriangles.AddRange(mesh.ColouredTriangles);
            mesh.ColouredRectangles.Clear();
            mesh.ColouredTriangles.Clear();
        }
    }
}
