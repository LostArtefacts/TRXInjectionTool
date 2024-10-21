using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Sky;

public class TR1ObeliskSkyboxBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level caves = _control1.Read($@"Resources\{TR1LevelNames.CAVES}");
        TR3Level chamber = _control3.Read($@"Resources\{TR3LevelNames.WILLIE}");

        ResetLevel(caves);

        TRModel sky = chamber.Models[TR3Type.Skybox_H];
        caves.Models[TR1Type.Unused01] = sky;

        caves.Palette[1] = new()
        {
            Red = 90,
            Green = 136,
            Blue = 173,
        };

        TRMesh mesh = sky.Meshes[0];
        mesh.ColouredTriangles.ForEach(f => f.Texture = (ushort)(f.Vertices.Contains(65) ? 1 : 2));
        mesh.ColouredRectangles.ForEach(f => f.Texture = 1);
        mesh.TexturedRectangles.ForEach(f => f.Texture = (ushort)(f.Vertices[0] <= 15 ? 1 : 2));

        mesh.ColouredRectangles.AddRange(mesh.TexturedRectangles);
        mesh.TexturedRectangles.Clear();

        for (int i = 33; i <= 48; i++)
        {
            mesh.Vertices[i].Y += 400;
        }

        InjectionData data = InjectionData.Create(caves, InjectionType.Skybox, "obelisk_skybox");
        return new() { data };
    }
}
