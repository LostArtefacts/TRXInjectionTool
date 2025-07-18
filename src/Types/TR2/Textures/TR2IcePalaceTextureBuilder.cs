using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2IcePalaceTextureBuilder : TextureBuilder
{
    public override string ID => "palace_textures";

    public override List<InjectionData> Build()
    {
        TR2Level level = _control2.Read($"Resources/{TR2LevelNames.CHICKEN}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.CHICKEN);

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
                RoomIndex = 31,
                VertexIndex = level.Rooms[31].Mesh.Rectangles[133].Vertices[0],
                VertexChange = new() { Y = 256 },
            },
            new()
            {
                RoomIndex = 31,
                VertexIndex = level.Rooms[31].Mesh.Rectangles[133].Vertices[3],
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
                RoomIndex = 31,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 31,
                SourceIndex = 16,
                Vertices = new()
                {
                    level.Rooms[31].Mesh.Rectangles[124].Vertices[3],
                    level.Rooms[31].Mesh.Rectangles[137].Vertices[3],
                    level.Rooms[31].Mesh.Rectangles[137].Vertices[2],
                }
            },
        };
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR2Level level)
    {
        return new()
        {
            Reface(level, 31, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1356, 136),
            Reface(level, 31, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1356, 140),
            Reface(level, 31, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1356, 144),
            Reface(level, 31, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1356, 147),
            Reface(level, 31, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1356, 150),
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(52, TRMeshFaceType.TexturedTriangle, 3, 2),
        };
    }
}
