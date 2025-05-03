using System.Drawing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1EgyptTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level egypt = _control1.Read($"Resources/{TR1LevelNames.EGYPT}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.TextureFix, "egypt_textures");
        CreateDefaultTests(data, TR1LevelNames.EGYPT);

        data.RoomEdits.AddRange(CreateRefacings());
        data.RoomEdits.AddRange(CreateRotations());
        data.RoomEdits.AddRange(CreateShifts(egypt));
        data.RoomEdits.AddRange(FixVases(egypt));

        FixTransparentTextures(egypt, data);

        return new() { data };
    }

    private static List<TRRoomTextureReface> CreateRefacings()
    {
        return new()
        {
            new()
            {
                RoomIndex = 46,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 46,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 17,
                TargetIndex = 20
            },
            new()
            {
                RoomIndex = 47,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 47,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 28,
                TargetIndex = 1
            }
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(47, TRMeshFaceType.TexturedQuad, 1, 3),
            Rotate(47, TRMeshFaceType.TexturedQuad, 35, 1),
        };
    }

    private static List<TRRoomTextureMove> CreateShifts(TR1Level egypt)
    {
        return new()
        {
            new()
            {
                RoomIndex = 98,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 2,
                VertexRemap = new()
                {
                    new()
                    {
                        NewVertexIndex = egypt.Rooms[98].Mesh.Rectangles[1].Vertices[2],
                    },
                    new()
                    {
                        Index = 1,
                        NewVertexIndex = egypt.Rooms[98].Mesh.Rectangles[1].Vertices[1],
                    },
                }
            }
        };
    }

    private static IEnumerable<TRRoomVertexMove> FixVases(TR1Level egypt)
    {
        return egypt.Rooms
            .SelectMany(r => r.Mesh.Sprites.Where(s => s.ID == TR1Type.Debris0).Select(s => new TRRoomVertexMove
            {
                RoomIndex = (short)egypt.Rooms.IndexOf(r),
                VertexIndex = (ushort)s.Vertex,
                VertexChange = new() { Y = -27 },
            }));
    }

    private static void FixTransparentTextures(TR1Level egypt, InjectionData data)
    {
        TR1CommonTextureBuilder.FixTransparentPixels(egypt, data,
            egypt.Rooms[25].Mesh.Rectangles[32], Color.FromArgb(188, 140, 64));
        TR1CommonTextureBuilder.FixTransparentPixels(egypt, data,
            egypt.Rooms[39].Mesh.Rectangles[125], Color.FromArgb(188, 140, 64));
    }
}
