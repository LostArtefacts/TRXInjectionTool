using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR4.Textures;

public class TR4TitleTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR4, InjectionType.TextureFix, "title_textures");
        CreateDefaultTests(data, "TR4/title.tr4");
        data.RoomEdits.Add(FixRoom30());

        return [data];
    }

    private static TRRoomTextureReface FixRoom30()
    {
        var level = _control4.Read($"Resources/TR4/title.tr4");
        var tex = level.Rooms[30].Mesh.Rectangles[9].Texture;
        return Reface(level, 30, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, tex, 14);
    }
}
