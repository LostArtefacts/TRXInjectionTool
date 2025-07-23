using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2VeniceTextureBuilder : TextureBuilder
{
    public override string ID => "venice_textures";

    public override List<InjectionData> Build()
    {
        TR2Level venice = _control2.Read($"Resources/{TR2LevelNames.VENICE}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.VENICE);

        data.RoomEdits.AddRange(CreateFillers(venice));
        data.RoomEdits.AddRange(CreateRefacings(venice));
        data.RoomEdits.AddRange(CreateRotations());
        data.MeshEdits.Add(
            FixStaticMeshPosition(venice.StaticMeshes, TR2Type.Architecture4, new() { Z = 27 }));

        FixPassport(venice, data);

        return new() { data };
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR2Level level)
    {
        return new()
        {
            new()
            {
                RoomIndex = 42,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 43,
                SourceIndex = 46,
                Vertices = new()
                {
                    level.Rooms[42].Mesh.Rectangles[1].Vertices[3],
                    level.Rooms[42].Mesh.Rectangles[1].Vertices[2],
                    level.Rooms[42].Mesh.Rectangles[0].Vertices[1],
                    level.Rooms[42].Mesh.Rectangles[0].Vertices[0],
                }
            },
            new()
            {
                RoomIndex = 42,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 43,
                SourceIndex = 48,
                Vertices = new()
                {
                    level.Rooms[42].Mesh.Rectangles[4].Vertices[3],
                    level.Rooms[42].Mesh.Rectangles[4].Vertices[2],
                    level.Rooms[42].Mesh.Rectangles[3].Vertices[1],
                    level.Rooms[42].Mesh.Rectangles[3].Vertices[0],
                }
            },
            new()
            {
                RoomIndex = 42,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 43,
                SourceIndex = 50,
                Vertices = new()
                {
                    level.Rooms[42].Mesh.Rectangles[6].Vertices[3],
                    level.Rooms[42].Mesh.Rectangles[6].Vertices[2],
                    level.Rooms[42].Mesh.Rectangles[5].Vertices[1],
                    level.Rooms[42].Mesh.Rectangles[5].Vertices[0],
                }
            },
        };
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR2Level level)
    {
        return new()
        {
            Reface(level, 36, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1574, 0),
            Reface(level, 36, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1574, 5),
            Reface(level, 42, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1599, 1),
            Reface(level, 42, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1600, 4),
            Reface(level, 42, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1601, 6),
            Reface(level, 77, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1513, 65),
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(36, TRMeshFaceType.TexturedQuad, 22, 2),
            Rotate(36, TRMeshFaceType.TexturedQuad, 88, 2),
        };
    }
}
