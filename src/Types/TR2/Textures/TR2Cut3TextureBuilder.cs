using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public partial class TR2Cut3TextureBuilder : TextureBuilder
{
    public override string ID => "cut3_textures";

    public override List<InjectionData> Build()
    {
        TR2Level level = _control2.Read($"Resources/{TR2LevelNames.DA_CUT}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.DA_CUT);

        data.RoomEdits.AddRange(CreateRefacings(level));
        data.RoomEdits.AddRange(CreateRotations());
        data.RoomEdits.AddRange(FixCatwalks(level));

        return new() { data };
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR2Level level)
    {
        return new()
        {
            Reface(level, 8, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1088, 39),
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(8, TRMeshFaceType.TexturedQuad, 39, 3),
        };
    }
}
