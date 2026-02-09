using System.Drawing;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Sky;

public class TR1ValleySkyboxBuilder : InjectionBuilder
{
    private static readonly List<short> _skyRooms = [6, 32, 34, 39, 52, 53, 54, 55, 64, 65, 66];

    private static readonly Dictionary<string, string> _imageIDs = new()
    {
        ["54bbd1a9a9c26f1f961a494463ac2bea"] = "Resources/TR1/Valley/sky1.png",
        ["156b3c5749ea413af11d32823deedc9a"] = "Resources/TR1/Valley/sky2.png",
        ["41b4862cdb163d7db3477adeffcc31f4"] = "Resources/TR1/Valley/sky3.png",
        ["94816542d35e6ef2a3ddef2465964709"] = "Resources/TR1/Valley/sky4.png",
        ["3f3c0b3b7060dcfc14d1d9fffd0fd97e"] = "Resources/TR1/Valley/sky5.png",
    };

    public override List<InjectionData> Build()
    {
        TR1Level caves = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        TR3Level chamber = _control3.Read($"Resources/{TR3LevelNames.WILLIE}");

        ResetLevel(caves, 1);

        TRModel sky = chamber.Models[TR3Type.Skybox_H];
        caves.Models[TR1Type.Unused01] = sky;
        caves.Palette = InitialisePalette8(sky, chamber.Palette16);

        // Reset the blue at the top of the mesh to match our replacement textures (OG TR3 is wrong).
        Color topColour = new TRImage("Resources/TR1/Valley/sky1.png").GetPixel(0, 0);
        caves.Palette[1] = topColour.ToTRColour();

        // Everything touching vertex 32 is the bottom of the mesh so should be black.
        sky.Meshes[0].ColouredTriangles
            .Where(f => f.Vertices.Contains(32))
            .ToList()
            .ForEach(f => f.Texture = 2);

        // Rotate so the moon is above the temple
        sky.Animations[0].Frames[0].Rotations[0].Y = 448;

        PackTextures(caves, chamber, sky, _imageIDs);

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

        List<TRMeshFace> topTriangles = sky.Meshes[0].ColouredTriangles.Where(f => !f.Vertices.Contains(32)).ToList();
        List<TRMeshFace> topRectangles = new(sky.Meshes[0].ColouredRectangles);
        sky.Meshes[0].TexturedTriangles.AddRange(topTriangles);
        sky.Meshes[0].TexturedRectangles.AddRange(topRectangles);
        sky.Meshes[0].ColouredTriangles.RemoveAll(topTriangles.Contains);
        sky.Meshes[0].ColouredRectangles.Clear();
        topTriangles.ForEach(t => t.Texture = (ushort)caves.ObjectTextures.Count);
        topRectangles.ForEach(t => t.Texture = (ushort)caves.ObjectTextures.Count);

        caves.ObjectTextures.Add(region.Segments.First().Texture as TRObjectTexture);

        InjectionData data = InjectionData.Create(caves, InjectionType.Skybox, "valley_skybox");
        CreateDefaultTests(data, TR1LevelNames.VALLEY);

        var valley = _control1.Read($"Resources/{TR1LevelNames.VALLEY}");
        data.FloorEdits.AddRange(FDBuilder.AddRoomFlags(_skyRooms, TRRoomFlag.Skybox, valley.Rooms));

        return [data];
    }
}
