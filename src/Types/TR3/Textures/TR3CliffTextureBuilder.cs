using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Textures;

public class TR3CliffTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.TextureFix, "cliff_textures");

        data.RoomEdits.AddRange(CreateRotations());

        return [data];
    }

    private static IEnumerable<TRRoomTextureRotate> CreateRotations()
    {
        yield return Rotate(22, TRMeshFaceType.TexturedQuad, 94, 2);
        yield return Rotate(23, TRMeshFaceType.TexturedQuad, 5, 2);
    }
}
