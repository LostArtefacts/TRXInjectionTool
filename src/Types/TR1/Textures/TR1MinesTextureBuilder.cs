using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1MinesTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level mines = _control1.Read($"Resources/{TR1LevelNames.MINES}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.TextureFix, "mines_textures");

        data.RoomEdits.AddRange(CreateFillers(mines));
        data.RoomEdits.AddRange(CreateRefacings());
        data.RoomEdits.AddRange(CreateRotations());
        data.RoomEdits.AddRange(CreateShifts(mines));

        return new() { data };
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR1Level mines)
    {
        return new()
        {
            new()
            {
                RoomIndex = 35,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 35,
                SourceIndex = 2,
                Vertices = new()
                {
                    mines.Rooms[35].Mesh.Rectangles[4].Vertices[1],
                    mines.Rooms[35].Mesh.Rectangles[0].Vertices[0],
                    mines.Rooms[35].Mesh.Rectangles[0].Vertices[3],
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
                RoomIndex = 69,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 69,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 172,
                TargetIndex = 174
            },
            new()
            {
                RoomIndex = 23,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 23,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 99,
                TargetIndex = 89
            },
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(23, TRMeshFaceType.TexturedTriangle, 8, 2),
            Rotate(23, TRMeshFaceType.TexturedTriangle, 6, 2),
            Rotate(24, TRMeshFaceType.TexturedTriangle, 0, 1),
        };
    }

    private static List<TRRoomTextureMove> CreateShifts(TR1Level mines)
    {
        return new()
        {
            new()
            {
                RoomIndex = 55,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 8,
                VertexRemap = new()
                {
                    new()
                    {
                        NewVertexIndex = mines.Rooms[55].Mesh.Rectangles[9].Vertices[3],
                    },
                    new()
                    {
                        Index = 1,
                        NewVertexIndex = mines.Rooms[55].Mesh.Rectangles[9].Vertices[2],
                    },
                }
            }
        };
    }
}
