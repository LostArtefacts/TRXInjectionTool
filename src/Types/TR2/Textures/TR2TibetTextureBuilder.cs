using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2TibetTextureBuilder : TextureBuilder
{
    public override string ID => "tibet_textures";

    public override List<InjectionData> Build()
    {
        TR2Level level = _control2.Read($"Resources/{TR2LevelNames.TIBET}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.TIBET);

        data.RoomEdits.AddRange(CreateFillers(level));
        data.RoomEdits.AddRange(CreateRefacings(level));
        data.RoomEdits.AddRange(CreateRotations());

        FixPassport(level, data);

        return new() { data };
    }

    private static List<TRRoomTextureEdit> CreateFillers(TR2Level level)
    {
        var vert = level.Rooms[143].Mesh.Vertices[level.Rooms[143].Mesh.Rectangles[403].Vertices[1]];
        return new()
        {
            new TRRoomVertexCreate
            {
                RoomIndex = 143,
                Vertex = new()
                {
                    Lighting = vert.Lighting,
                    Vertex = new()
                    {
                        X = vert.Vertex.X,
                        Y = (short)(vert.Vertex.Y + 768),
                        Z = vert.Vertex.Z,
                    },
                },
            },
            new TRRoomTextureCreate
            {
                RoomIndex = 143,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 143,
                SourceIndex = 427,
                Vertices = new()
                {
                    level.Rooms[143].Mesh.Rectangles[403].Vertices[2],
                    level.Rooms[143].Mesh.Rectangles[403].Vertices[1],
                    (ushort)level.Rooms[143].Mesh.Vertices.Count,
                    level.Rooms[143].Mesh.Rectangles[427].Vertices[2],
                }
            },
        };
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR2Level level)
    {
        return new()
        {
            Reface(level, 12, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1292, 78),
            Reface(level, 28, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 1361, 1),
            Reface(level, 40, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1288, 157),
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(12, TRMeshFaceType.TexturedTriangle, 11, 2),
            Rotate(12, TRMeshFaceType.TexturedTriangle, 13, 2),
            Rotate(12, TRMeshFaceType.TexturedTriangle, 14, 2),
            Rotate(12, TRMeshFaceType.TexturedTriangle, 18, 2),
            Rotate(12, TRMeshFaceType.TexturedTriangle, 21, 2),
            Rotate(12, TRMeshFaceType.TexturedTriangle, 23, 2),
            Rotate(12, TRMeshFaceType.TexturedTriangle, 24, 2),
            Rotate(12, TRMeshFaceType.TexturedTriangle, 26, 2),
            Rotate(12, TRMeshFaceType.TexturedTriangle, 27, 2),
            Rotate(15, TRMeshFaceType.TexturedTriangle, 3, 2),
            Rotate(15, TRMeshFaceType.TexturedTriangle, 4, 2),
            Rotate(28, TRMeshFaceType.TexturedTriangle, 1, 1),
        };
    }
}
