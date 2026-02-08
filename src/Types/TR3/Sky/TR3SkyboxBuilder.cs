using System.Diagnostics;
using System.Drawing;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Sky;

public class TR3SkyboxBuilder : InjectionBuilder
{
    private static readonly List<SkyboxSetup> _setups =
    [
        new("gym_sky", TR3LevelNames.ASSAULT),     // Lara's Home
        new("india_sky", TR3LevelNames.JUNGLE),    // Jungle, Temple, Temple cutscene, Ganges
        new("coastal_sky", TR3LevelNames.COASTAL), // Coastal Village
        new("crash_sky", TR3LevelNames.CRASH),     // Coastal cutscene, Crash Site
        new("rapids_sky", TR3LevelNames.MADUBU),   // Madubu
        new("london_sky", TR3LevelNames.THAMES),   // Thames cutscene, City
        new("nevada_sky", TR3LevelNames.NEVADA),   // Nevada Desert
        new("area51_sky", TR3LevelNames.AREA51),   // HSC, Area51
        new("antarc_sky", TR3LevelNames.ANTARC),   // Antarctica, Antarctica cutscene
        new("cavern_sky", TR3LevelNames.WILLIE),   // Meteorite Cavern
        new("scotland_sky", "SCOTLAND.TR2"),       // Fling, Will's Den
    ];

    public override string ID => "tr3_skyboxes";

    public override List<InjectionData> Build()
    {
        return [.. _setups.Select(s => s.GenerateInjection())];
    }

    private class SkyboxSetup(string binName, string levelName)
    {
        public string BinName { get; set; } = binName;
        public string LevelName { get; set; } = levelName;

        public InjectionData GenerateInjection()
        {
            var level = _control3.Read($"Resources/TR3/{LevelName}");
            var palette = level.Palette16.Select(c => c.ToColor()).ToList();
            CreateModelLevel(level, TR3Type.Skybox_H);
            FixSkybox(level, palette);
            return InjectionData.Create(level, InjectionType.General, BinName);
        }

        private static void FixSkybox(TR3Level level, List<Color> palette)
        {
            // For upper faces, use whatever colour is on the existing flat faces.
            // For lower, select the bottom-most pixel of the lowest textured face.
            var mesh = level.Models[TR3Type.Skybox_H].Meshes[0];
            var (_, highY) = GetExtremeFace(mesh.TexturedFaces, mesh.Vertices, true);
            var (lowFace, lowY) = GetExtremeFace(mesh.TexturedFaces, mesh.Vertices, false);

            var upperFaces = mesh.ColouredFaces.Where(f => f.Vertices.All(v => mesh.Vertices[v].Y <= highY)).ToList();
            var lowerFaces = mesh.ColouredFaces.Where(f => f.Vertices.All(v => mesh.Vertices[v].Y >= lowY)).ToList();
            Debug.Assert(upperFaces.Count + lowerFaces.Count == mesh.ColouredFaces.Count());

            var regions = new List<TRTextileRegion>();
            ushort top = 0;
            ushort bottom = 0;
            {
                var texInfo = level.ObjectTextures[lowFace.Texture];
                var img = new TRImage(level.Images16[texInfo.Atlas].Pixels)
                    .Export(texInfo.Bounds);
                var colour = img.GetPixel(0, img.Height - 1);
                bottom = (ushort)(level.ObjectTextures.Count + regions.Count);
                regions.Add(CreateSwatch(colour, bottom));
            }
            {
                var colour = palette[upperFaces.First().Texture >> 8];
                top = (ushort)(level.ObjectTextures.Count + regions.Count);
                regions.Add(CreateSwatch(colour, top));
            }

            var packer = new TR3TexturePacker(level);
            packer.AddRectangles(regions);
            packer.Pack(true);

            level.ObjectTextures.AddRange(regions.Select(r => r.Segments.First().Texture as TRObjectTexture));

            upperFaces.ForEach(f => f.Texture = top);
            lowerFaces.ForEach(f => f.Texture = bottom);
            mesh.TexturedRectangles.AddRange(mesh.ColouredRectangles);
            mesh.TexturedTriangles.AddRange(mesh.ColouredTriangles);
            mesh.ColouredRectangles.Clear();
            mesh.ColouredTriangles.Clear();
        }

        private static (TRFace Face, int Y) GetExtremeFace
            (IEnumerable<TRFace> faces, List<TRVertex> vertices, bool highest)
        {
            var result = faces.Select(f =>
            {
                var v = (highest
                        ? f.Vertices.OrderBy(i => vertices[i].Y)
                        : f.Vertices.OrderByDescending(i => vertices[i].Y))
                    .First();

                return (Face: f, vertices[v].Y);
            });
            return highest
                ? result.OrderBy(t => t.Y).First()
                : result.OrderByDescending(t => t.Y).First();
        }

        private static TRTextileRegion CreateSwatch(Color colour, ushort index)
        {
            var img = new TRImage(8, 8);
            img.Fill(colour);

            var tex = new TRObjectTexture(0, 0, 8, 8);
            return new()
            {
                Image = img,
                Bounds = tex.Bounds,
                Segments = [ new() { Index = index, Texture = tex }],
            };
        }
    }
}
