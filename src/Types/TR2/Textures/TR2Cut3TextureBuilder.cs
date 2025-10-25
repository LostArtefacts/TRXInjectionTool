using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types.TR2.Textures;

public partial class TR2Cut3TextureBuilder : TextureBuilder
{
    public override string ID => "cut3_textures";

    public override List<InjectionData> Build()
    {
        var data = CreateBaseData();
        var level = _control2.Read($"Resources/{TR2LevelNames.DA_CUT}");

        data.RoomEdits.AddRange(CreateRefacings(level));
        data.RoomEdits.AddRange(CreateRotations());
        data.RoomEdits.AddRange(FixCatwalks(level));

        return [data];
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR2Level level)
    {
        return
        [
            Reface(level, 8, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1088, 39),
        ];
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return
        [
            Rotate(8, TRMeshFaceType.TexturedQuad, 39, 3),
        ];
    }

    private InjectionData CreateBaseData()
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.DA_CUT}");
        CreateModelLevel(level, TR2Type.LaraPistolAnim_H, TR2Type.CutsceneActor3);

        {
            var mesh = level.Models[TR2Type.LaraPistolAnim_H].Meshes[1];
            mesh.ColouredRectangles[0].Texture = mesh.TexturedRectangles[10].Texture;
            mesh.ColouredRectangles[0].Rotate(3);
            mesh.TexturedRectangles.Add(mesh.ColouredRectangles[0]);
            mesh.ColouredRectangles.Clear();
        }

        {
            var mesh = level.Models[TR2Type.LaraPistolAnim_H].Meshes[4];
            mesh.ColouredRectangles[0].Texture = mesh.TexturedRectangles[9].Texture;
            mesh.ColouredRectangles[0].Rotate(2);
            mesh.TexturedRectangles.Add(mesh.ColouredRectangles[0]);
            mesh.ColouredRectangles.RemoveAt(0);

            var mesh2 = level.Models[TR2Type.LaraPistolAnim_H].Meshes[1];
            mesh.ColouredRectangles[0].Texture = mesh2.TexturedRectangles[9].Texture;
            mesh.TexturedRectangles.Add(mesh.ColouredRectangles[0]);
            mesh.ColouredRectangles.RemoveAt(0);

            mesh.ColouredTriangles.Clear();
            mesh.TexturedRectangles.Add(new()
            {
                Type = TRFaceType.Rectangle,
                Vertices = [35, 39, 40, 36],
                Texture = mesh2.TexturedRectangles[12].Texture,
            });
        }

        {
            var mesh = level.Models[TR2Type.CutsceneActor3].Meshes[1];
            mesh.TexturedTriangles.RemoveAt(34);
            mesh.TexturedTriangles.RemoveAt(33);
            mesh.TexturedRectangles.Add(new()
            {
                Type = TRFaceType.Rectangle,
                Vertices = [39, 35, 36, 40],
                Texture = mesh.TexturedRectangles[13].Texture,
            });

            mesh.TexturedRectangles[8].Texture = mesh.TexturedRectangles[11].Texture;
            mesh.TexturedRectangles[8].Rotate(3);
        }

        {
            var mesh = level.Models[TR2Type.CutsceneActor3].Meshes[4];
            mesh.TexturedTriangles.RemoveAt(34);
            mesh.TexturedTriangles.RemoveAt(33);
            mesh.TexturedRectangles.Add(new()
            {
                Type = TRFaceType.Rectangle,
                Vertices = [35, 39, 40, 36],
                Texture = mesh.TexturedRectangles[13].Texture,
            });

            mesh.TexturedRectangles[8].Texture = mesh.TexturedRectangles[11].Texture;
            mesh.TexturedRectangles[8].Rotate(2);
        }

        var data = InjectionData.Create(level, InjectionType.TextureFix, ID);
        data.SetMeshOnlyModel((uint)TR2Type.LaraPistolAnim_H);
        data.SetMeshOnlyModel((uint)TR2Type.CutsceneActor3);

        CreateDefaultTests(data, TR2LevelNames.DA_CUT);

        return data;
    }
}
