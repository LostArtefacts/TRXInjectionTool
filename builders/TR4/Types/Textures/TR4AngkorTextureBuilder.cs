using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR4.Textures;

public class TR4AngkorTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR4, InjectionType.TextureFix, "angkor_textures");
        CreateDefaultTests(data, $"TR4/{TR4LevelNames.ANGKOR}");
        data.RoomEdits.Add(FixRoom60Portal());

        return [data];
    }

    private static TRRoomTextureDoubleSided FixRoom60Portal()
    {
        return new()
        {
            RoomIndex = 60,
            FaceType = TRMeshFaceType.TexturedQuad,
            TargetIndex = 12,
            DoubleSided = true,
        };
    }
}
