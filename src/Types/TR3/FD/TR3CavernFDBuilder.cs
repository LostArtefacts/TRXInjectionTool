using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.FD;

public class TR3CavernFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.FDFix, "cavern_fd");
        CreateDefaultTests(data, $"TR3/{TR3LevelNames.WILLIE}");
        FixSlopeSoftlock(data);

        return [data];
    }

    private static void FixSlopeSoftlock(InjectionData data)
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.WILLIE}");
        var sector = level.Rooms[12].GetSector(4, 7, TRUnit.Sector);
        var fd = level.FloorData[sector.FDIndex][0] as FDTriangulationEntry;
        fd.C11 = 4;
        fd.H1 = 3;
        data.FloorEdits.Add(new()
        {
            RoomIndex = 12,
            X = 4,
            Z = 7,
            Fixes = [new FDTrigCreateFix { Entries = [fd] }],
        });

        var mesh = level.Rooms[12].Mesh;
        var faceMap = new Dictionary<short, short>
        {
            [30] = 2,
            [32] = 1,
            [33] = 1,
        };
        foreach (var (face, vertex) in faceMap)
        {
            data.RoomEdits.Add(new TRRoomTextureMove
            {
                RoomIndex = 12,
                FaceType = TRMeshFaceType.TexturedTriangle,
                TargetIndex = face,
                VertexRemap = [new()
                {
                    Index = vertex,
                    NewVertexIndex = mesh.Rectangles[13].Vertices[0],
                }],
            });
        }
    }
}
