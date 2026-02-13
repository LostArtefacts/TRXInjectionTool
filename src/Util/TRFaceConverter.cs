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
        ConvertFlatFacesImpl(level, sourcePalette, models);
    }

    public static void ConvertFlatFaces(TR3Level level, List<Color> sourcePalette, IEnumerable<TRModel> models)
    {
        ConvertFlatFacesImpl(level, sourcePalette, models);
    }

    private static void ConvertFlatFacesImpl<TLevel>(
        TLevel level,
        IReadOnlyList<Color> sourcePalette,
        IEnumerable<TRModel> models)
        where TLevel : TRLevelBase
    {
        TRTexturePacker packer = level switch
        {
            TR2Level level2 => new TR2TexturePacker(level2),
            TR3Level level3 => new TR3TexturePacker(level3),
            _ => throw new InvalidOperationException("Only TR2/TR3 levels are supported."),
        };

        var objectTextures = level.ObjectTextures;
        var meshList = models
            .SelectMany(m => m.Meshes)
            .ToList();
        var ids = meshList
            .SelectMany(m => m.ColouredFaces)
            .Select(f => f.Texture >> 8)
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
            mesh.ColouredRectangles.ForEach(f => f.Texture = (ushort)map[f.Texture >> 8]);
            mesh.ColouredTriangles.ForEach(f => f.Texture = (ushort)map[f.Texture >> 8]);
            mesh.TexturedRectangles.AddRange(mesh.ColouredRectangles);
            mesh.TexturedTriangles.AddRange(mesh.ColouredTriangles);
            mesh.ColouredRectangles.Clear();
            mesh.ColouredTriangles.Clear();
        }
    }
}
