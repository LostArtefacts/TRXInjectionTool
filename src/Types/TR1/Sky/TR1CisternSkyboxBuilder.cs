using TRLevelControl.Helpers;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Sky;

public class TR1CisternSkyboxBuilder : TR1ColosseumSkyboxBuilder
{
    public override string ID => "cistern_skybox";

    public override List<InjectionData> Build()
    {
        var level = CreateBaseLevel();
        var data = InjectionData.Create(level, InjectionType.Skybox, ID);
        CreateDefaultTests(data, TR1LevelNames.CISTERN);
        AdjustMidasGap(data);
        return new() { data };
    }

    private static void AdjustMidasGap(InjectionData data)
    {
        var verts = new List<List<ushort>>
        {
            new() { 6, 7, 1, 0 },
            new() { 7, 10, 2, 1 },
            new() { 10, 11, 9, 2 },
            new() { 11, 8, 4, 9 },
            new() { 8, 5, 3, 4 },
            new() { 5, 6, 0, 3 },
            new() { 8, 7, 6, 5 },
            new() { 11, 10, 7, 8 }
        };
        data.RoomEdits.AddRange(verts.Select(v => new TRRoomTextureCreate
        {
            RoomIndex = 138,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 137,
            SourceIndex = 2,
            Vertices = v,
        }));

        var cistern = _control1.Read($"Resources/{TR1LevelNames.CISTERN}");
        for (ushort i = 0; i < 12; i++)
        {
            data.RoomEdits.Add(new TRRoomVertexMove
            {
                RoomIndex = 138,
                VertexChange = new(),
                VertexIndex = i,
                ShadeChange = (short)(8192 - cistern.Rooms[138].Mesh.Vertices[i].Lighting),
            });
        }
    }
}
