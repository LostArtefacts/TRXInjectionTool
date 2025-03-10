using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1FollyTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level folly = _control1.Read($"Resources/{TR1LevelNames.FOLLY}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.TextureFix, "folly_textures");
        CreateDefaultTests(data, TR1LevelNames.FOLLY);

        data.RoomEdits.AddRange(CreateFillers(folly));
        data.RoomEdits.AddRange(CreateRefacings());
        data.RoomEdits.AddRange(CreateRotations());
        data.RoomEdits.AddRange(CreateShifts(folly));

        return new() { data };
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR1Level folly)
    {
        return new()
        {
            new()
            {
                RoomIndex = 35,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 35,
                SourceIndex = 7,
                Vertices = new()
                {
                    folly.Rooms[35].Mesh.Rectangles[7].Vertices[1],
                    folly.Rooms[35].Mesh.Rectangles[12].Vertices[0],
                    folly.Rooms[35].Mesh.Rectangles[12].Vertices[3],
                    folly.Rooms[35].Mesh.Rectangles[7].Vertices[2],
                }
            },
        };
    }

    private static List<TRRoomTextureReface> CreateRefacings()
    {
        return new()
        {
            new()
            {
                RoomIndex = 18,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 18,
                SourceFaceType = TRMeshFaceType.TexturedTriangle,
                SourceIndex = 4,
                TargetIndex = 0
            },
            new()
            {
                RoomIndex = 35,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 35,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 6,
                TargetIndex = 10
            },
            new()
            {
                RoomIndex = 1,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 1,
                SourceFaceType = TRMeshFaceType.TexturedTriangle,
                SourceIndex = 8,
                TargetIndex = 4
            },
            new()
            {
                RoomIndex = 4,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 4,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 74,
                TargetIndex = 62
            }
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(1, TRMeshFaceType.TexturedTriangle, 4, 2),
            Rotate(1, TRMeshFaceType.TexturedTriangle, 8, 2),
            Rotate(3, TRMeshFaceType.TexturedQuad, 208, 1),
            Rotate(4, TRMeshFaceType.TexturedQuad, 62, 1),
        };
    }

    private static List<TRRoomTextureMove> CreateShifts(TR1Level folly)
    {
        return new()
        {
            new()
            {
                RoomIndex = 35,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 10,
                VertexRemap = new()
                {
                    new()
                    {
                        NewVertexIndex = folly.Rooms[35].Mesh.Rectangles[6].Vertices[1]
                    },
                    new()
                    {
                        Index = 1,
                        NewVertexIndex = folly.Rooms[35].Mesh.Rectangles[11].Vertices[0]
                    }
                }
            },
        };
    }
}
