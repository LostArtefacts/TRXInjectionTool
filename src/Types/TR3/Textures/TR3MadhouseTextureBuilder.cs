using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Textures;

public class TR3MadhouseTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.TextureFix, "zoo_textures");
        CreateDefaultTests(data, $"TR3/{TR3LevelNames.MADHOUSE}");

        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.MADHOUSE}");
        FixSpikes(data, level);

        return [data];
    }

    private static void FixSpikes(InjectionData data, TR3Level level)
    {
        var mesh = level.Models[TR3Type.TeethSpikesOrBarbedWire].Meshes[0];
        var maxY = mesh.Vertices.Max(v => v.Y);

        var edit = new TRMeshEdit
        {
            ModelID = (uint)TR3Type.TeethSpikesOrBarbedWire,
            VertexEdits = [],
        };
        data.MeshEdits.Add(edit);

        for (short i = 0; i < mesh.Vertices.Count; i++)
        {
            var vertex = mesh.Vertices[i];
            if (vertex.Y < maxY)
            {
                continue;
            }

            edit.VertexEdits.Add(new()
            {
                Index = i,
                Change = new() { Y = TRConsts.Step1 / 2 },
            });
        }
    }
}
