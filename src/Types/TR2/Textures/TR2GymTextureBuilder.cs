using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2GymTextureBuilder : TextureBuilder
{
    public override string ID => "gym_textures";

    public override List<InjectionData> Build()
    {
        InjectionData data = CreateBaseData();

        TR2Level gym = _control2.Read($"Resources/{TR2LevelNames.ASSAULT}");
        FixLaraTransparency(gym, data);

        data.RoomEdits.AddRange(CreateVertexShifts(gym));
        data.RoomEdits.AddRange(CreateShifts(gym));
        data.RoomEdits.AddRange(CreateRefacings(gym));
        data.RoomEdits.AddRange(CreateRotations());

        return new() { data };
    }

    private static List<TRRoomVertexMove> CreateVertexShifts(TR2Level level)
    {
        return new()
        {
            new()
            {
                RoomIndex = 65,
                VertexIndex = level.Rooms[65].Mesh.Rectangles[14].Vertices[0],
                VertexChange = new() { Y = -256 },
            },
            new()
            {
                RoomIndex = 65,
                VertexIndex = level.Rooms[65].Mesh.Rectangles[14].Vertices[1],
                VertexChange = new() { Y = -256 },
            },
        };
    }

    private static List<TRRoomTextureMove> CreateShifts(TR2Level level)
    {
        return new()
        {
            new TRRoomTextureMove
            {
                RoomIndex = 65,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 15,
                VertexRemap = new()
                {
                    new()
                    {
                        Index = 0,
                        NewVertexIndex = level.Rooms[65].Mesh.Rectangles[15].Vertices[1],
                    },
                    new()
                    {
                        Index = 1,
                        NewVertexIndex = level.Rooms[65].Mesh.Rectangles[15].Vertices[0],
                    },
                    new()
                    {
                        Index = 2,
                        NewVertexIndex = level.Rooms[65].Mesh.Rectangles[15].Vertices[3],
                    },
                    new()
                    {
                        Index = 3,
                        NewVertexIndex = level.Rooms[65].Mesh.Rectangles[15].Vertices[3],
                    },
                }
            },
        };
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR2Level level)
    {
        return new()
        {
            Reface(level, 9, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedQuad, 867, 0),
            Reface(level, 65, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 838, 14),
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(36, TRMeshFaceType.TexturedQuad, 38, 3),
            Rotate(57, TRMeshFaceType.TexturedQuad, 15, 3),
        };
    }

    private InjectionData CreateBaseData()
    {
        TR2Level level = CreateWinstonLevel(TR2LevelNames.ASSAULT);
        // Current injection limitation, do not replace SFX
        level.SoundEffects.Clear();

        FixHomeWindows(level, TR2LevelNames.ASSAULT);
        FixHomeStatue(level);

        InjectionData data = InjectionData.Create(level, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.ASSAULT);
        return data;
    }
}
