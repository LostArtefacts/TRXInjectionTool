using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR4.Textures;

public class TR4HypostyleJeepBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR4, InjectionType.TextureFix, "hypostyle_jeep");
        CreateDefaultTests(data, $"TR4/{TR4LevelNames.HYPOSTYLE}");
        data.MeshEdits.AddRange(FixJeepWindows());

        return [data];
    }

    // The side windows of the jeep sample a teal patch of the crossbow
    // texture, which their additive blending turns green. A dark blue-grey
    // panel of the jeep body is close to the glass the same jeep uses in
    // Temple of Karnak.
    private static IEnumerable<TRMeshEdit> FixJeepWindows()
    {
        foreach (short meshIndex in new short[] { 2, 4 })
        {
            yield return new()
            {
                ModelID = (uint)TR4Type.Animating1,
                MeshIndex = meshIndex,
                FaceEdits =
                [
                    new()
                    {
                        ModelID = (uint)TR4Type.Animating1,
                        MeshIndex = 0,
                        FaceType = TRMeshFaceType.TexturedQuad,
                        FaceIndex = 46,
                        TargetFaceIndices = [0, 1],
                    },
                ],
            };
        }
    }
}
