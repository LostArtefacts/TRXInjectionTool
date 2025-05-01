using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.FD;

public class TR2BarkhangFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level barkhang = _control2.Read($"Resources/{TR2LevelNames.MONASTERY}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.FDFix, "barkhang_fd");
        CreateDefaultTests(data, TR2LevelNames.MONASTERY);
        FixSlopeSoftlock(barkhang, data);

        return new() { data };
    }

    private static void FixSlopeSoftlock(TR2Level barkhang, InjectionData data)
    {
        data.FloorEdits.Add(MakeSlant(barkhang, 96, 12, 4, -1, 3));

        var mesh = barkhang.Rooms[96].Mesh;
        data.RoomEdits.Add(new TRRoomVertexMove
        {
            RoomIndex = 96,
            VertexIndex = mesh.Rectangles[133].Vertices[1],
            VertexChange = new() { Y = -512 }
        });
        data.RoomEdits.Add(new TRRoomVertexMove
        {
            RoomIndex = 96,
            VertexIndex = mesh.Rectangles[133].Vertices[3],
            VertexChange = new() { Y = 512 }
        });
    }
}
