using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public partial class TR1StrongholdTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level stronghold = _control1.Read($"Resources/{TR1LevelNames.STRONGHOLD}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.TextureFix, "stronghold_textures");
        CreateDefaultTests(data, TR1LevelNames.STRONGHOLD);

        data.RoomEdits.AddRange(CreateFillers(stronghold));
        data.RoomEdits.AddRange(CreateRefacings());
        data.RoomEdits.AddRange(CreateRotations());
        data.RoomEdits.AddRange(CreateShifts(stronghold));

        FixMiscRooms(stronghold, data);
        FixPassport(stronghold, data);

        return new() { data };
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR1Level stronghold)
    {
        return new()
        {
            new()
            {
                RoomIndex = 63,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 63,
                SourceIndex = 18,
                Vertices = new()
                {
                    stronghold.Rooms[63].Mesh.Rectangles[51].Vertices[3],
                    stronghold.Rooms[63].Mesh.Rectangles[54].Vertices[2],
                    stronghold.Rooms[63].Mesh.Rectangles[54].Vertices[1],
                }
            }
        };
    }

    private static List<TRRoomTextureReface> CreateRefacings()
    {
        return new()
        {
            new()
            {
                RoomIndex = 7,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 7,
                SourceIndex = 31,
                TargetIndex = 25,
            },
            new()
            {
                RoomIndex = 75,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 7,
                SourceIndex = 31,
                TargetIndex = 25,
            },
            new()
            {
                RoomIndex = 2,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 2,
                SourceIndex = 304,
                TargetIndex = 267,
            },
            new()
            {
                RoomIndex = 6,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 6,
                SourceIndex = 425,
                TargetIndex = 462,
            }
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(6, TRMeshFaceType.TexturedQuad, 462, 3),
            Rotate(18, TRMeshFaceType.TexturedTriangle, 9, 2),
            Rotate(18, TRMeshFaceType.TexturedTriangle, 15, 2),
            Rotate(27, TRMeshFaceType.TexturedTriangle, 3, 1),
        };
    }

    private static List<TRRoomTextureMove> CreateShifts(TR1Level stronghold)
    {
        return new()
        {
            new()
            {
                RoomIndex = 19,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 11,
                VertexRemap = new()
                {
                    new()
                    {
                        NewVertexIndex = stronghold.Rooms[19].Mesh.Rectangles[9].Vertices[0],
                    },
                    new()
                    {
                        Index = 3,
                        NewVertexIndex = stronghold.Rooms[19].Mesh.Rectangles[9].Vertices[1],
                    },
                }
            },
        };
    }
}
