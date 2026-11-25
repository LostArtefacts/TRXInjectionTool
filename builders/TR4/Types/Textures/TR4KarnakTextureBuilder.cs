using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR4.Textures;

public class TR4KarnakTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR4, InjectionType.TextureFix, "karnak_textures");
        CreateDefaultTests(data, $"TR4/{TR4LevelNames.KARNAK}");
        var level = _control4.Read($"Resources/TR4/{TR4LevelNames.KARNAK}");

        data.MeshEdits.Add(new()
        {
            ModelID = (uint)(TR4Type.SceneryBase + 22),
            FaceRotations = [new() { FaceType = TRMeshFaceType.TexturedQuad, FaceIndex = 30, Rotations = 3 }],
        });

        data.MeshEdits.Add(CloseBowl(level.Models[TR4Type.Animating4].Meshes[2],
            (uint)TR4Type.Animating4, 2, 0, [0, 8, 16, 24, 32, 40]));
        data.MeshEdits.Add(CloseBowl(level.Models[TR4Type.Animating5].Meshes[0],
            (uint)TR4Type.Animating5, 0, 6, [41, 49, 57, 65, 73, 81]));
        data.MeshEdits.Add(CloseBowl(level.StaticMeshes[TR4Type.SceneryBase + 14].Mesh,
            (uint)(TR4Type.SceneryBase + 14), 0, 8, [22, 30, 38, 46, 54, 62]));

        return [data];
    }

    // Closes the open ring under a bowl with a fan of triangles. The ring
    // lists its vertices in the order the side faces run around it; the new
    // triangles run the other way, so they face out of the bowl. They take
    // the texture of a triangle on the bowl's inner base.
    private static TRMeshEdit CloseBowl(TRMesh mesh, uint modelID, short meshIndex, int baseTriangle, ushort[] ring)
    {
        var texture = mesh.TexturedTriangles[baseTriangle].Texture;
        return new()
        {
            ModelID = modelID,
            MeshIndex = meshIndex,
            FaceAdditions = [.. Enumerable.Range(1, ring.Length - 2).Select(i => new TRMeshFaceAddition
            {
                FaceType = TRMeshFaceType.TexturedTriangle,
                Texture = texture,
                Vertices = [ring[0], ring[i + 1], ring[i]],
            })],
        };
    }
}
