using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2WallTextureBuilder : TextureBuilder
{
    public override string ID => "wall_textures";

    public override List<InjectionData> Build()
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.GW}");
        var data = InjectionData.Create(TRGameVersion.TR2, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.GW);

        data.RoomEdits.AddRange(CreateRefacings(level));
        data.RoomEdits.AddRange(CreateRotations());

        FixPassport(level, data);

        return new() { data };
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR2Level level)
    {
        return new()
        {
            Reface(level, 30, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 1206, 2),
            Reface(level, 30, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 1206, 3),
            Reface(level, 30, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 1217, 4),
            Reface(level, 59, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1196, 200),
            Reface(level, 59, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1196, 201),
            Reface(level, 59, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1196, 206),
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(28, TRMeshFaceType.TexturedTriangle, 0, 1),
            Rotate(28, TRMeshFaceType.TexturedTriangle, 1, 2),
            Rotate(30, TRMeshFaceType.TexturedTriangle, 1, 2),
            Rotate(30, TRMeshFaceType.TexturedTriangle, 2, 2),
            Rotate(30, TRMeshFaceType.TexturedTriangle, 4, 2),
            Rotate(36, TRMeshFaceType.TexturedTriangle, 2, 1),
            Rotate(36, TRMeshFaceType.TexturedTriangle, 9, 1),
            Rotate(36, TRMeshFaceType.TexturedTriangle, 24, 1),
            Rotate(36, TRMeshFaceType.TexturedTriangle, 33, 2),
            Rotate(36, TRMeshFaceType.TexturedTriangle, 41, 2),
            Rotate(36, TRMeshFaceType.TexturedTriangle, 55, 2),
            Rotate(59, TRMeshFaceType.TexturedTriangle, 21, 2),
        };
    }
}
