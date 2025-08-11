using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public partial class TR1CisternTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level cistern = _control1.Read($"Resources/{TR1LevelNames.CISTERN}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.TextureFix, "cistern_textures");
        CreateDefaultTests(data, TR1LevelNames.CISTERN);

        data.RoomEdits.AddRange(CreateFillers(cistern));
        data.RoomEdits.AddRange(CreateRotations());
        data.RoomEdits.AddRange(CreateRefacings(cistern));
        data.RoomEdits.AddRange(FixCatwalks(cistern));

        FixPassport(cistern, data);

        return new() { data };
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR1Level cistern)
    {
        return new()
        {
            new()
            {
                RoomIndex = 3,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 9,
                SourceIndex = 66,
                Vertices = new()
                {
                    cistern.Rooms[3].Mesh.Rectangles[105].Vertices[1],
                    cistern.Rooms[3].Mesh.Rectangles[85].Vertices[1],
                    cistern.Rooms[3].Mesh.Rectangles[83].Vertices[2],
                    cistern.Rooms[3].Mesh.Rectangles[83].Vertices[1],
                }
            }
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(9, TRMeshFaceType.TexturedQuad, 21, 1),
            Rotate(9, TRMeshFaceType.TexturedQuad, 19, 3),
            Rotate(12, TRMeshFaceType.TexturedQuad, 32, 2),
            Rotate(12, TRMeshFaceType.TexturedQuad, 60, 2),
            Rotate(22, TRMeshFaceType.TexturedQuad, 3, 1),
            Rotate(102, TRMeshFaceType.TexturedTriangle, 4, 2),
            Rotate(111, TRMeshFaceType.TexturedQuad, 147, 1),
            Rotate(131, TRMeshFaceType.TexturedQuad, 192, 1),
        };
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR1Level cistern)
    {
        return new()
        {
            Reface(cistern, 6, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 52, 2),
            Reface(cistern, 9, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 48, 19),
            Reface(cistern, 9, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 48, 48),
            Reface(cistern, 9, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 49, 53),
            Reface(cistern, 9, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 53, 67),
            Reface(cistern, 12, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 34, 29),
            Reface(cistern, 12, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 49, 32),
            Reface(cistern, 12, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 49, 60),
            Reface(cistern, 22, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedQuad, 55, 0),
            Reface(cistern, 22, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 55, 36),
        };
    }
}
