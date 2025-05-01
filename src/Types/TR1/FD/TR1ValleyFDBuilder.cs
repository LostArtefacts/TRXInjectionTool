using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.FD;

public class TR1ValleyFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level valley = _control1.Read($"Resources/{TR1LevelNames.VALLEY}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.FDFix, "valley_fd");
        CreateDefaultTests(data, TR1LevelNames.VALLEY);
        FixSlopeSoftlock(valley, data);

        return new() { data };
    }

    private static void FixSlopeSoftlock(TR1Level valley, InjectionData data)
    {
        // Adjust the slopes in room 58 to avoid Lara getting stuck.
        data.FloorEdits.Add(MakeSlant(valley, 58, 4, 8, -5, -5));
        data.FloorEdits.Add(MakeSlant(valley, 58, 4, 9, -11, 0));

        // The rest is needed to move textures to fit the new geometry.
        var mesh = valley.Rooms[58].Mesh;
        data.RoomEdits.Add(new TRRoomVertexMove
        {
            RoomIndex = 58,
            VertexIndex = mesh.Rectangles[48].Vertices[2],
            VertexChange = new() { Y = -512 }
        });
        data.RoomEdits.Add(new TRRoomVertexMove
        {
            RoomIndex = 58,
            VertexIndex = mesh.Rectangles[49].Vertices[1],
            VertexChange = new() { Y = 256 }
        });
        data.RoomEdits.Add(new TRRoomVertexMove
        {
            RoomIndex = 58,
            VertexIndex = mesh.Rectangles[49].Vertices[3],
            VertexChange = new() { Y = -256 }
        });
        data.RoomEdits.Add(new TRRoomTextureMove
        {
            RoomIndex = 58,
            FaceType = TRMeshFaceType.TexturedQuad,
            TargetIndex = 46,
            VertexRemap = new()
            {
                new()
                {
                    Index = 3,
                    NewVertexIndex = mesh.Rectangles[49].Vertices[3],
                }
            }
        });

        data.RoomEdits.Add(new TRRoomTextureMove
        {
            RoomIndex = 58,
            FaceType = TRMeshFaceType.TexturedQuad,
            TargetIndex = 51,
            VertexRemap = new()
            {
                new()
                {
                    Index = 0,
                    NewVertexIndex = mesh.Rectangles[46].Vertices[0],
                },
                new()
                {
                    Index = 1,
                    NewVertexIndex = mesh.Rectangles[49].Vertices[3],
                },
                new()
                {
                    Index = 2,
                    NewVertexIndex = mesh.Rectangles[61].Vertices[1],
                },
                new()
                {
                    Index = 3,
                    NewVertexIndex = mesh.Rectangles[61].Vertices[0],
                },
            }
        });

        data.RoomEdits.Add(new TRRoomTextureMove
        {
            RoomIndex = 58,
            FaceType = TRMeshFaceType.TexturedTriangle,
            TargetIndex = 8,
            VertexRemap = new()
            {
                new()
                {
                    Index = 0,
                    NewVertexIndex = mesh.Rectangles[49].Vertices[3],
                },
                new()
                {
                    Index = 1,
                    NewVertexIndex = mesh.Rectangles[46].Vertices[2],
                },
                new()
                {
                    Index = 2,
                    NewVertexIndex = mesh.Rectangles[49].Vertices[0],
                },
            }
        });

        data.RoomEdits.Add(new TRRoomTextureReface
        {
            RoomIndex = 58,
            FaceType = TRMeshFaceType.TexturedQuad,
            TargetIndex = 51,
            SourceRoom = 58,
            SourceFaceType = TRMeshFaceType.TexturedQuad,
            SourceIndex = 60,
        });
        data.RoomEdits.Add(new TRRoomTextureReface
        {
            RoomIndex = 58,
            FaceType = TRMeshFaceType.TexturedTriangle,
            TargetIndex = 8,
            SourceRoom = 1,
            SourceFaceType = TRMeshFaceType.TexturedTriangle,
            SourceIndex = 4,
        });

        mesh = valley.Rooms[56].Mesh;
        data.RoomEdits.Add(new TRRoomVertexMove
        {
            RoomIndex = 56,
            VertexIndex = mesh.Triangles[0].Vertices[1],
            VertexChange = new() { Y = 256 }
        });
    }
}
