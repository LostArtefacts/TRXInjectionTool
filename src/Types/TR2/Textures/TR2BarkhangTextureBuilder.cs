using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2BarkhangTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level barkhang = _control2.Read($"Resources/{TR2LevelNames.MONASTERY}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.TextureFix, "barkhang_textures");
        CreateDefaultTests(data, TR2LevelNames.MONASTERY);

        data.MeshEdits.Add(
            FixStaticMeshPosition(barkhang.StaticMeshes, TR2Type.Architecture7, new() { X = 5 }));

        return new() { data };
    }
}
