using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2FathomsTextureBuilder : TextureBuilder
{
    public override string ID => "fathoms_textures";

    public override List<InjectionData> Build()
    {
        TR2Level level = _control2.Read($"Resources/{TR2LevelNames.FATHOMS}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.FATHOMS);

        data.RoomEdits.AddRange(CreateShifts(level));
        data.RoomEdits.AddRange(CreateFillers(level));
        data.RoomEdits.AddRange(CreateRotations());

        return new() { data };
    }

    private static List<TRRoomTextureMove> CreateShifts(TR2Level level)
    {
        return new()
        {
            new()
            {
                RoomIndex = 12,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 24,
                VertexRemap = new()
                {
                    new()
                    {
                        Index = 0,
                        NewVertexIndex = level.Rooms[12].Mesh.Rectangles[38].Vertices[2],
                    },
                    new()
                    {
                        Index = 3,
                        NewVertexIndex = level.Rooms[12].Mesh.Rectangles[5].Vertices[3],
                    }
                }
            },
            new()
            {
                RoomIndex = 2,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 454,
                VertexRemap = new()
                {
                    new()
                    {
                        Index = 2,
                        NewVertexIndex = level.Rooms[2].Mesh.Rectangles[430].Vertices[3],
                    },
                    new()
                    {
                        Index = 3,
                        NewVertexIndex = level.Rooms[2].Mesh.Rectangles[434].Vertices[0],
                    }
                }
            },
        };
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR2Level level)
    {
        return new()
        {
            new()
            {
                RoomIndex = 12,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 12,
                SourceIndex = 39,
                Vertices = new()
                {
                    level.Rooms[12].Mesh.Rectangles[39].Vertices[0],
                    level.Rooms[12].Mesh.Rectangles[5].Vertices[3],
                    level.Rooms[12].Mesh.Rectangles[5].Vertices[2],
                    level.Rooms[12].Mesh.Rectangles[39].Vertices[1],
                }
            },
            new()
            {
                RoomIndex = 12,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 12,
                SourceIndex = 39,
                Vertices = new()
                {
                    level.Rooms[12].Mesh.Rectangles[4].Vertices[3],
                    level.Rooms[12].Mesh.Rectangles[37].Vertices[2],
                    level.Rooms[12].Mesh.Rectangles[37].Vertices[1],
                    level.Rooms[12].Mesh.Rectangles[4].Vertices[0],
                }
            },
            new()
            {
                RoomIndex = 2,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 2,
                SourceIndex = 454,
                Vertices = new()
                {
                    level.Rooms[2].Mesh.Rectangles[454].Vertices[2],
                    level.Rooms[2].Mesh.Rectangles[454].Vertices[3],
                    level.Rooms[2].Mesh.Rectangles[434].Vertices[0],
                    level.Rooms[2].Mesh.Rectangles[430].Vertices[3],
                }
            },
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(11, TRMeshFaceType.TexturedQuad, 47, 3),
        };
    }
}
