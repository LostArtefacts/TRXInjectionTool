using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1ToQTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level toq = _control1.Read($@"Resources\{TR1LevelNames.QUALOPEC}");
        InjectionData data = InjectionData.Create(InjectionType.TextureFix, "qualopec_textures");

        data.RoomEdits.AddRange(CreateRefacings());
        data.RoomEdits.AddRange(CreateRotations());
        data.RoomEdits.AddRange(CreateShifts(toq));

        return new() { data };
    }

    private static List<TRRoomTextureReface> CreateRefacings()
    {
        return new()
        {
            new()
            {
                RoomIndex = 8,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 8,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 97,
                TargetIndex = 87,
            },
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(5, TRMeshFaceType.TexturedQuad, 211, 2),
            Rotate(8, TRMeshFaceType.TexturedQuad, 87, 1),
        };
    }

    private static List<TRRoomTextureMove> CreateShifts(TR1Level toq)
    {
        return new()
        {
            new()
            {
                RoomIndex = 8,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 107,
                VertexRemap = new()
                {
                    new()
                    {
                        NewVertexIndex = toq.Rooms[8].Mesh.Rectangles[81].Vertices[3],
                    },
                    new()
                    {
                        Index = 1,
                        NewVertexIndex = toq.Rooms[8].Mesh.Rectangles[81].Vertices[2],
                    },
                }
            },

            new()
            {
                RoomIndex = 8,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 6,
                VertexRemap = new()
                {
                    new()
                    {
                        NewVertexIndex = toq.Rooms[8].Mesh.Rectangles[2].Vertices[3],
                    },
                    new()
                    {
                        Index = 1,
                        NewVertexIndex = toq.Rooms[8].Mesh.Rectangles[5].Vertices[0],
                    },
                }
            }
        };
    }
}
