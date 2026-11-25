using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR4.Textures;

public class TR4HypostyleTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR4, InjectionType.TextureFix, "hypostyle_textures");
        CreateDefaultTests(data, $"TR4/{TR4LevelNames.HYPOSTYLE}");
        var level = _control4.Read($"Resources/TR4/{TR4LevelNames.HYPOSTYLE}");
        data.MeshEdits.Add(FixGlassPyramid(level));

        return [data];
    }

    // Each glass panel of the pyramid has a face on each side, apart from the
    // panel between vertices 114 and 115, which has no inner face. The panel
    // between vertices 128 and 129 is opaque, unlike the others.
    private static TRMeshEdit FixGlassPyramid(TR4Level level)
    {
        var mesh = level.Models[TR4Type.Animating3].Meshes[0];
        var glass = mesh.TexturedTriangles[34];
        return new()
        {
            ModelID = (uint)TR4Type.Animating3,
            FaceEffects =
            [
                new() { FaceType = TRMeshFaceType.TexturedTriangle, FaceIndex = 59, Effects = glass.Effects },
                new() { FaceType = TRMeshFaceType.TexturedTriangle, FaceIndex = 60, Effects = glass.Effects },
            ],
            FaceAdditions =
            [
                new()
                {
                    FaceType = TRMeshFaceType.TexturedTriangle,
                    Texture = glass.Texture,
                    Effects = glass.Effects,
                    Vertices = [81, 115, 114],
                },
            ],
        };
    }
}
