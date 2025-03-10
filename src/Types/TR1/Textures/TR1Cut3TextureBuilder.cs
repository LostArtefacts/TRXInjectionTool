using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1Cut3TextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.TextureFix, "cut3_textures");
        CreateDefaultTests(data, TR1LevelNames.MINES_CUT);

        data.RoomEdits.AddRange(CreateRotations());

        return new() { data };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(21, TRMeshFaceType.TexturedTriangle, 7, 2),
            Rotate(6, TRMeshFaceType.TexturedTriangle, 7, 2),
        };
    }
}
