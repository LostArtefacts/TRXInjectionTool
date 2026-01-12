using System.Drawing;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Sky;

public class TR1ColosseumSkyboxBuilder : InjectionBuilder
{
    private static readonly List<short> _skyRooms = [18, 19, 74, 77, 78, 79, 81, 82, 83];

    protected static readonly Dictionary<string, string> _imageIDs = new()
    {
        ["6063df8273ce88d50da6c6b20fd48802"] = "Resources/TR1/Colosseum/sky1.png",
        ["74827eb9fffd8d7cac2ce1807bd37bce"] = "Resources/TR1/Colosseum/sky2.png",
    };

    public override string ID => "colosseum_skybox";

    public override List<InjectionData> Build()
    {
        var level = CreateBaseLevel();
        var data = InjectionData.Create(level, InjectionType.Skybox, ID);
        CreateDefaultTests(data, TR1LevelNames.COLOSSEUM);

        var colly = _control1.Read($"Resources/{TR1LevelNames.COLOSSEUM}");
        data.FloorEdits.AddRange(FDBuilder.AddRoomFlags(_skyRooms, TRRoomFlag.Skybox, colly.Rooms));

        return [data];
    }

    protected static TR1Level CreateBaseLevel()
    {
        var caves = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        var gold = _control2.Read($"Resources/{TR2LevelNames.FOOLGOLD}");

        ResetLevel(caves, 1);

        var sky = gold.Models[TR2Type.Skybox_H];
        caves.Models[TR1Type.Unused01] = sky;
        caves.Palette = InitialisePalette8(sky, gold.Palette16);

        var topColour = Color.FromArgb(24, 16, 48);

        PackTextures(caves, gold, sky, _imageIDs);

        // Replace the flat faces at the top of the mesh with a texture.
        var packer = new TR1TexturePacker(caves);
        var img = new TRImage(16, 16);
        img.Write((c, x, y) => topColour);
        var region = new TRTextileRegion(new()
        {
            Texture = new TRObjectTexture(0, 0, 16, 16),
        }, img);
        packer.AddRectangle(region);
        packer.Pack(true);

        var topTriangles = sky.Meshes[0].ColouredTriangles.Where(f => f.Vertices.Contains(32)).ToList();
        sky.Meshes[0].TexturedTriangles.AddRange(topTriangles);
        sky.Meshes[0].ColouredTriangles.RemoveAll(topTriangles.Contains);
        topTriangles.ForEach(t => t.Texture = (ushort)caves.ObjectTextures.Count);

        caves.ObjectTextures.Add(region.Segments.First().Texture as TRObjectTexture);

        return caves;
    }
}
