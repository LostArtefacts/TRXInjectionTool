using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2CatacombsTextureBuilder : TextureBuilder
{
    public override string ID => "catacombs_textures";

    public override List<InjectionData> Build()
    {
        TR2Level level = _control2.Read($"Resources/{TR2LevelNames.COT}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.COT);

        data.RoomEdits.AddRange(CreateVertexShifts(level));
        data.RoomEdits.AddRange(CreateFillers(level));
        data.RoomEdits.AddRange(CreateRefacings(level));
        data.RoomEdits.AddRange(CreateRotations());

        return new() { data };
    }

    private static List<TRRoomVertexMove> CreateVertexShifts(TR2Level level)
    {
        return new()
        {
            new()
            {
                RoomIndex = 40,
                VertexIndex = level.Rooms[40].Mesh.Rectangles[133].Vertices[0],
                VertexChange = new() { Y = 256 },
            },
            new()
            {
                RoomIndex = 40,
                VertexIndex = level.Rooms[40].Mesh.Rectangles[133].Vertices[3],
                VertexChange = new() { Y = 256 },
            },
        };
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR2Level level)
    {
        return new()
        {
            new()
            {
                RoomIndex = 40,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 40,
                SourceIndex = 16,
                Vertices = new()
                {
                    level.Rooms[40].Mesh.Rectangles[124].Vertices[3],
                    level.Rooms[40].Mesh.Rectangles[137].Vertices[3],
                    level.Rooms[40].Mesh.Rectangles[137].Vertices[2],
                }
            },
        };
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR2Level level)
    {
        return new()
        {
            Reface(level, 40, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1251, 136),
            Reface(level, 40, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1251, 140),
            Reface(level, 40, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1251, 144),
            Reface(level, 40, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1251, 147),
            Reface(level, 40, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1251, 150),
            Reface(level, 56, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1313, 150),
            Reface(level, 79, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1313, 150),
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(11, TRMeshFaceType.TexturedQuad, 213, 3),
            Rotate(56, TRMeshFaceType.TexturedQuad, 150, 3),
            Rotate(79, TRMeshFaceType.TexturedQuad, 150, 3),
        };
    }
}
