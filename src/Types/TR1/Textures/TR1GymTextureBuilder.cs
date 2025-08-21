using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1GymTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level gym = _control1.Read($"Resources/{TR1LevelNames.ASSAULT}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.TextureFix, "gym_textures");
        CreateDefaultTests(data, TR1LevelNames.ASSAULT);

        data.RoomEdits.AddRange(CreateRefacings(gym));
        data.RoomEdits.AddRange(CreateRotations());
        data.RoomEdits.AddRange(FixRoom11(gym));

        FixPassport(gym, data);

        return [data];
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR1Level level)
    {
        return
        [
            Reface(level, 9, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedQuad, 197, 0),
            Reface(level, 9, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedQuad, 197, 1),
        ];
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return
        [
            Rotate(9, TRMeshFaceType.TexturedTriangle, 1, 1),
        ];
    }

    private static IEnumerable<TRRoomTextureEdit> FixRoom11(TR1Level level)
    {
        var mesh = level.Rooms[11].Mesh;
        yield return CreateQuadShift(11, 72,
        [
            new(2, mesh.Rectangles[71].Vertices[2]),
            new(3, mesh.Rectangles[100].Vertices[3]),
        ]);

        yield return CreateVertex(11, level.Rooms[11], mesh.Vertices[mesh.Rectangles[120].Vertices[3]], -1, 512);
        yield return CreateQuadShift(11, 120,
        [
            new(2, mesh.Rectangles[122].Vertices[2]),
            new(3, (ushort)(mesh.Vertices.Count - 1)),
        ]);

        yield return CreateQuadShift(11, 158,
        [
            new(2, mesh.Rectangles[178].Vertices[2]),
            new(3, mesh.Rectangles[154].Vertices[3]),
        ]);
        yield return CreateQuadShift(11, 135,
        [
            new(2, mesh.Rectangles[110].Vertices[2]),
            new(3, mesh.Rectangles[164].Vertices[3]),
        ]);
    }
}
