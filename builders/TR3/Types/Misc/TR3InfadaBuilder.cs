using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Misc;

public class TR3InfadaBuilder : InjectionBuilder
{
    private static readonly TR3Type[] _types = [TR3Type.Infada_P, TR3Type.Infada_M_H];

    public override List<InjectionData> Build()
    {
        return [CreateData()];
    }

    private static InjectionData CreateData()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.CAVES}");
        var meshes = _types
            .SelectMany(t => level.Models[t].Meshes)
            .Distinct();
        foreach (var mesh in meshes)
        {
            TextureFlatFaces(mesh);
        }

        CreateModelLevel(level, _types);
        level.SoundEffects.Clear();

        var data = InjectionData.Create(level, InjectionType.General, "infada_meshes");
        foreach (var type in _types)
        {
            data.SetMeshOnlyModel((uint)type);
        }
        return data;
    }

    private static void TextureFlatFaces(TRMesh mesh)
    {
        if (mesh.ColouredTriangles.Count == 0)
        {
            return;
        }

        var texture = mesh.TexturedTriangles
            .GroupBy(f => f.Texture)
            .OrderByDescending(g => g.Count())
            .First().Key;
        mesh.ColouredTriangles.ForEach(f => f.Texture = texture);
        mesh.TexturedTriangles.AddRange(mesh.ColouredTriangles);
        mesh.ColouredTriangles.Clear();
    }
}
