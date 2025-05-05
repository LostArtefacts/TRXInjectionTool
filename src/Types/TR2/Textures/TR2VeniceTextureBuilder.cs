using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2VeniceTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level venice = _control2.Read($"Resources/{TR2LevelNames.VENICE}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.TextureFix, "venice_textures");
        CreateDefaultTests(data, TR2LevelNames.VENICE);

        data.MeshEdits.Add(
            FixStaticMeshPosition(venice.StaticMeshes, TR2Type.Architecture4, new() { Z = 27 }));

        return new() { data };
    }
}
