using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1HiveTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level hive = _control1.Read($"Resources/{TR1LevelNames.HIVE}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.TextureFix, "hive_textures");
        CreateDefaultTests(data, TR1LevelNames.HIVE);

        data.RoomEdits.AddRange(CreateRefacings());
        data.RoomEdits.AddRange(CreateRotations());

        FixPassport(hive, data);
        FixBigPod(data, TR1LevelNames.HIVE);

        return new() { data };
    }

    private static List<TRRoomTextureReface> CreateRefacings()
    {
        return new()
        {
            new()
            {
                RoomIndex = 18,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 18,
                SourceIndex = 295,
                TargetIndex = 210,
            },
            new()
            {
                RoomIndex = 18,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 18,
                SourceIndex = 299,
                TargetIndex = 205,
            },
            new()
            {
                RoomIndex = 8,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 8,
                SourceIndex = 378,
                TargetIndex = 382,
            },
            new()
            {
                RoomIndex = 8,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 8,
                SourceIndex = 378,
                TargetIndex = 386,
            },
            new()
            {
                RoomIndex = 13,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 13,
                SourceIndex = 534,
                TargetIndex = 566,
            }
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(18, TRMeshFaceType.TexturedQuad, 210, 3),
            Rotate(18, TRMeshFaceType.TexturedQuad, 205, 1),
            Rotate(8, TRMeshFaceType.TexturedQuad, 382, 3),
            Rotate(8, TRMeshFaceType.TexturedQuad, 386, 3),
        };
    }
}
