using Newtonsoft.Json;
using System.Drawing;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;

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

    public static void ConvertFlatFaces(TR2Level level, List<Color> sourcePalette, IEnumerable<TRModel> models)
    {
        var ids = models.SelectMany(m => m.Meshes)
            .SelectMany(m => m.ColouredFaces)
            .Select(f => f.Texture >> 8)
            .Distinct();

        var map = new Dictionary<int, int>();
        var regions = new List<TRTextileRegion>();
        int i = 0;
        foreach (var id in ids)
        {
            var img = new TRImage(8, 8);
            img.Fill(sourcePalette[id]);
            map[id] = level.ObjectTextures.Count + regions.Count;

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

        var basePalette = level.Palette.Select(c => c.ToTR1Color()).ToList();
        var packer = new TR2TexturePacker(level);
        packer.AddRectangles(regions);
        packer.Pack(true);

        level.ObjectTextures.AddRange(regions.Select(r => r.Segments.First().Texture as TRObjectTexture));
        models.SelectMany(m => m.Meshes)
            .ToList().ForEach(m =>
            {
                m.ColouredRectangles.ForEach(f => f.Texture = (ushort)map[f.Texture >> 8]);
                m.ColouredTriangles.ForEach(f => f.Texture = (ushort)map[f.Texture >> 8]);
                m.TexturedRectangles.AddRange(m.ColouredRectangles);
                m.TexturedTriangles.AddRange(m.ColouredTriangles);
                m.ColouredRectangles.Clear();
                m.ColouredTriangles.Clear();
            });
    }

    public static void ConvertFlatFaces(TR3Level level, List<Color> sourcePalette, IEnumerable<TRModel> models)
    {
        var ids = models.SelectMany(m => m.Meshes)
            .SelectMany(m => m.ColouredFaces)
            .Select(f => f.Texture >> 8)
            .Distinct();

        var map = new Dictionary<int, int>();
        var regions = new List<TRTextileRegion>();
        int i = 0;
        foreach (var id in ids)
        {
            var img = new TRImage(8, 8);
            img.Fill(sourcePalette[id]);
            map[id] = level.ObjectTextures.Count + regions.Count;

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

        var basePalette = level.Palette.Select(c => c.ToTR1Color()).ToList();
        var packer = new TR3TexturePacker(level);
        packer.AddRectangles(regions);
        packer.Pack(true);

        level.ObjectTextures.AddRange(regions.Select(r => r.Segments.First().Texture as TRObjectTexture));
        models.SelectMany(m => m.Meshes)
            .ToList().ForEach(m =>
            {
                m.ColouredRectangles.ForEach(f => f.Texture = (ushort)map[f.Texture >> 8]);
                m.ColouredTriangles.ForEach(f => f.Texture = (ushort)map[f.Texture >> 8]);
                m.TexturedRectangles.AddRange(m.ColouredRectangles);
                m.TexturedTriangles.AddRange(m.ColouredTriangles);
                m.ColouredRectangles.Clear();
                m.ColouredTriangles.Clear();
            });
    }
}
