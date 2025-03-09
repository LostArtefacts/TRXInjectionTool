using TRImageControl.Packing;
using TRImageControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;
using System.Drawing;

namespace TRXInjectionTool.Types.TR1.Sky;

public class TR1ColosseumSkyboxBuilder : InjectionBuilder
{
    private static readonly Dictionary<string, string> _imageIDs = new()
    {
        ["6063df8273ce88d50da6c6b20fd48802"] = "Resources/TR1/Colosseum/sky1.png",
        ["74827eb9fffd8d7cac2ce1807bd37bce"] = "Resources/TR1/Colosseum/sky2.png",
    };

    public override List<InjectionData> Build()
    {
        TR1Level caves = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        TR2Level gold = _control2.Read($"Resources/{TR2LevelNames.FOOLGOLD}");

        ResetLevel(caves, 1);

        TRModel sky = gold.Models[TR2Type.Skybox_H];
        caves.Models[TR1Type.Unused01] = sky;
        caves.Palette = InitialisePalette8(sky, gold.Palette16);

        Color topColour = Color.FromArgb(24, 16, 48);

        PackTextures(caves, gold, sky, _imageIDs);

        // Replace the flat faces at the top of the mesh with a texture.
        TR1TexturePacker packer = new(caves);
        TRImage img = new(16, 16);
        img.Write((c, x, y) => topColour);
        TRTextileRegion region = new(new()
        {
            Texture = new TRObjectTexture(0, 0, 16, 16),
        }, img);
        packer.AddRectangle(region);
        packer.Pack(true);

        List<TRMeshFace> topTriangles = sky.Meshes[0].ColouredTriangles.Where(f => f.Vertices.Contains(32)).ToList();
        sky.Meshes[0].TexturedTriangles.AddRange(topTriangles);
        sky.Meshes[0].ColouredTriangles.RemoveAll(topTriangles.Contains);
        topTriangles.ForEach(t => t.Texture = (ushort)caves.ObjectTextures.Count);

        caves.ObjectTextures.Add(region.Segments.First().Texture as TRObjectTexture);

        InjectionData data = InjectionData.Create(caves, InjectionType.Skybox, "colosseum_skybox");
        return new() { data };
    }
}
