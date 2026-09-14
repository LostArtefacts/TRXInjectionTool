using TRImageControl.Packing;
using TRImageControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;
using System.Drawing;

namespace TRXInjectionTool.Types.TR1.Sky;

public class TR1ObeliskSkyboxBuilder : InjectionBuilder
{
    private static readonly List<short> _skyRooms = [59];

    public override List<InjectionData> Build()
    {
        TR1Level caves = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        TR3Level chamber = _control3.Read($"Resources/{TR3LevelNames.WILLIE}");

        ResetLevel(caves, 1);

        TRModel sky = chamber.Models[TR3Type.Skybox_H];
        caves.Models[TR1Type.Unused01] = sky;

        Color topColour = Color.FromArgb(90, 136, 173);

        TR1TexturePacker packer = new(caves);
        TRImage img = new(16, 16);
        img.Write((c, x, y) => topColour);
        TRTextileRegion region = new(new()
        {
            Texture = new TRObjectTexture(0, 0, 16, 16),
        }, img);
        packer.AddRectangle(region);
        packer.Pack(true);

        TRMesh mesh = sky.Meshes[0];
        List<TRMeshFace> topTriangles = mesh.ColouredTriangles.Where(f => f.Vertices.Contains(65)).ToList();
        mesh.TexturedTriangles.AddRange(topTriangles);
        mesh.TexturedRectangles.AddRange(mesh.ColouredRectangles);
        topTriangles.ForEach(t => t.Texture = (ushort)caves.ObjectTextures.Count);
        mesh.TexturedRectangles.ForEach(t => t.Texture = (ushort)caves.ObjectTextures.Count);
        mesh.ColouredTriangles.RemoveAll(topTriangles.Contains);
        mesh.ColouredRectangles.Clear();

        caves.Palette[2] = new();
        mesh.ColouredTriangles.ForEach(f => f.Texture = 2);        

        caves.ObjectTextures.Add(region.Segments.First().Texture as TRObjectTexture);

        for (int i = 33; i <= 48; i++)
        {
            mesh.Vertices[i].Y += 400;
        }

        InjectionData data = InjectionData.Create(caves, InjectionType.Skybox, "obelisk_skybox");
        CreateDefaultTests(data, TR1LevelNames.OBELISK);

        var obelisk = _control1.Read($"Resources/{TR1LevelNames.OBELISK}");
        data.FloorEdits.AddRange(FDBuilder.AddRoomFlags(_skyRooms, TRRoomFlag.Skybox, obelisk.Rooms));

        return [data];
    }
}
