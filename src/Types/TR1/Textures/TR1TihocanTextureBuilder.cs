using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1TihocanTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level tihocan = _control1.Read($"Resources/{TR1LevelNames.TIHOCAN}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.TextureFix, "tihocan_textures");
        CreateDefaultTests(data, TR1LevelNames.TIHOCAN);

        data.RoomEdits.AddRange(CreateRefacings());
        data.RoomEdits.AddRange(CreateShifts(tihocan));

        FixPassport(tihocan, data);

        return new() { data };
    }

    private static List<TRRoomTextureReface> CreateRefacings()
    {
        return new()
        {
            new()
            {
                RoomIndex = 75,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 75,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 64,
                TargetIndex = 32
            },
            new()
            {
                RoomIndex = 89,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 89,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 66,
                TargetIndex = 1
            },
            new()
            {
                RoomIndex = 89,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 89,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 66,
                TargetIndex = 84
            }
        };
    }

    private static List<TRRoomTextureMove> CreateShifts(TR1Level tihocan)
    {
        return new()
        {
            new()
            {
                RoomIndex = 104,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 64,
                VertexRemap = new()
                {
                    new()
                    {
                        NewVertexIndex = tihocan.Rooms[104].Mesh.Rectangles[52].Vertices[0]
                    },
                    new()
                    {
                        Index = 3,
                        NewVertexIndex = tihocan.Rooms[104].Mesh.Rectangles[54].Vertices[0]
                    }
                }
            },
        };
    }
}
