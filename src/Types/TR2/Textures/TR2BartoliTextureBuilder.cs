using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2BartoliTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level bartoli = _control2.Read($"Resources/{TR2LevelNames.BARTOLI}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.TextureFix, "bartoli_textures");
        CreateDefaultTests(data, TR2LevelNames.BARTOLI);

        data.MeshEdits.Add(
            FixStaticMeshPosition(bartoli.StaticMeshes, TR2Type.Architecture4, new() { Z = 27 }));

        return new() { data };
    }
}
