using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.FD;

public class TR1ValleyFDBuilder : FDBuilder
{
    private static readonly List<short> _windyRooms
        = new() { 6, 31, 32, 33, 34, 39, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66 };

    public override List<InjectionData> Build()
    {
        TR1Level valley = _control1.Read($"Resources/{TR1LevelNames.VALLEY}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.FDFix, "valley_fd");
        CreateDefaultTests(data, TR1LevelNames.VALLEY);
        FixSlopeSoftlock(valley, data);
        data.FloorEdits.AddRange(AddRoomFlags(_windyRooms, TRRoomFlag.Wind, valley.Rooms));
        FixWaterfalls(valley, data);

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

    private static void FixWaterfalls(TR1Level valley, InjectionData data)
    {
        for (ushort z = 4; z < 10; z++)
        {
            var trigger = GetTrigger(valley, 88, 6, z);
            trigger.OneShot = true;
            trigger.TrigType = FDTrigType.AntiTrigger;
            data.FloorEdits.Add(MakeTrigger(valley, 88, 6, z, trigger));
        }

        {
            var trigger = GetTrigger(valley, 0, 6, 1);
            trigger.Actions.AddRange(new[] { 15, 16, 17 }
                .Select(i => new FDActionItem { Parameter = (short)i }));
            data.FloorEdits.Add(MakeTrigger(valley, 0, 6, 1, trigger));
        }
    }
}
