using Newtonsoft.Json;
using System.Drawing;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Model;

namespace TRXInjectionTool.Types.TR2.Lara;

public static class TR2GunUtils
{
    public static void ConvertFlatFaces(TR2Level level, List<Color> sourcePalette)
    {
        var ids = level.Models.Values.SelectMany(m => m.Meshes)
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
        level.Models.Values.SelectMany(m => m.Meshes)
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

    public static void ReplaceGlovedHands(TR2Level level)
    {
        // Remove gloves from gun anim hands.
        var handsImg = new TRImage("Resources/TR2/lara_hands.png");
        var handsMap = JsonConvert.DeserializeObject<List<TexMap>>
            (File.ReadAllText("Resources/TR2/lara_hands.json"));
        foreach (var map in handsMap)
        {
            var tex = handsImg.Export(map.TexA);
            var id = tex.GenerateID();
            foreach (var objTex in level.ObjectTextures)
            {
                var tile = new TRImage(level.Images16[objTex.Atlas].Pixels);
                var img = tile.Export(objTex.Bounds);
                if (img.GenerateID() == id)
                {
                    var cleanHand = handsImg.Export(map.TexB);
                    tile.Import(cleanHand, objTex.Position);
                    level.Images16[objTex.Atlas].Pixels = tile.ToRGB555();
                }
            }
        }

        var verts = new List<ushort> { 2, 3, 6, 7 };
        var baseFace = level.Models[TR2Type.LaraPistolAnim_H]
            .Meshes[10].TexturedRectangles.Find(t => t.Vertices.All(verts.Contains));
        foreach (var type in new[] { TR2Type.LaraHarpoonAnim_H, TR2Type.LaraM16Anim_H, TR2Type.LaraGrenadeAnim_H })
        {
            var face = level.Models[type]
                .Meshes[10].TexturedRectangles.Find(t => t.Vertices.All(verts.Contains));
            face.Texture = baseFace.Texture;
        }
    }

    private class TexMap
    {
        public Rectangle TexA { get; set; }
        public Rectangle TexB { get; set; }
    }
}
