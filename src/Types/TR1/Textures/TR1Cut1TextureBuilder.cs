using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1Cut1TextureBuilder : TextureBuilder
{
    public override string ID => "cut1_textures";

    public override List<InjectionData> Build()
    {
        var level = _control1.Read($"Resources/{TR1LevelNames.QUALOPEC_CUT}");
        var data = InjectionData.Create(TRGameVersion.TR1, InjectionType.TextureFix, ID);

        data.RoomEdits.AddRange(CreateFillers(level));

        return [data];
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR1Level level)
    {
        return
        [
            new()
            {
                RoomIndex = 3,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 6,
                SourceIndex = 107,
                Vertices =
                [
                    level.Rooms[3].Mesh.Triangles[18].Vertices[0],
                    level.Rooms[3].Mesh.Rectangles[37].Vertices[0],
                    level.Rooms[3].Mesh.Rectangles[37].Vertices[3],
                ]
            },
            new()
            {
                RoomIndex = 6,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 6,
                SourceIndex = 107,
                Vertices =
                [
                    level.Rooms[6].Mesh.Rectangles[137].Vertices[1],
                    level.Rooms[6].Mesh.Rectangles[138].Vertices[3],
                    level.Rooms[6].Mesh.Rectangles[138].Vertices[2],
                ]
            },
            new()
            {
                RoomIndex = 6,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 6,
                SourceIndex = 107,
                Vertices =
                [
                    level.Rooms[6].Mesh.Rectangles[138].Vertices[2],
                    level.Rooms[6].Mesh.Rectangles[125].Vertices[0],
                    level.Rooms[6].Mesh.Rectangles[125].Vertices[3],
                ]
            },
        ];
    }
}
