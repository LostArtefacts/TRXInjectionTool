using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2FloatingTextureBuilder : TextureBuilder
{
    public override string ID => "floating_textures";

    public override List<InjectionData> Build()
    {
        TR2Level level = _control2.Read($"Resources/{TR2LevelNames.FLOATER}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.FLOATER);

        data.RoomEdits.AddRange(CreateVertexShifts(level));
        data.RoomEdits.AddRange(CreateShifts(level));
        data.RoomEdits.AddRange(CreateFillers(level));
        data.RoomEdits.AddRange(CreateRefacings(level));
        data.RoomEdits.AddRange(CreateRotations());

        return new() { data };
    }

    private static List<TRRoomVertexMove> CreateVertexShifts(TR2Level level)
    {
        return new()
        {
            new()
            {
                RoomIndex = 32,
                VertexIndex = level.Rooms[32].Mesh.Rectangles[1].Vertices[0],
                VertexChange = new() { Y = -768 },
            },
            new()
            {
                RoomIndex = 32,
                VertexIndex = level.Rooms[32].Mesh.Rectangles[1].Vertices[1],
                VertexChange = new() { Y = -1024 },
            },
        };
    }

    private static List<TRRoomTextureEdit> CreateShifts(TR2Level level)
    {
        var vert = level.Rooms[58].Mesh.Vertices[level.Rooms[58].Mesh.Rectangles[2].Vertices[1]];

        return new()
        {
            new TRRoomVertexCreate
            {
                RoomIndex = 58,
                Vertex = new()
                {
                    Lighting = vert.Lighting,
                    Vertex = new()
                    {
                        X = (short)(vert.Vertex.X + 1024),
                        Y = vert.Vertex.Y,
                        Z = vert.Vertex.Z,
                    },
                },
            },
            new TRRoomTextureMove
            {
                RoomIndex = 58,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 4,
                VertexRemap = new()
                {
                    new()
                    {
                        Index = 2,
                        NewVertexIndex = (ushort)level.Rooms[58].Mesh.Vertices.Count,
                    },
                    new()
                    {
                        Index = 3,
                        NewVertexIndex = level.Rooms[58].Mesh.Rectangles[2].Vertices[1],
                    }
                }
            },
            new TRRoomTextureMove
            {
                RoomIndex = 133,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 13,
                VertexRemap = new()
                {
                    new()
                    {
                        Index = 2,
                        NewVertexIndex = level.Rooms[133].Mesh.Rectangles[23].Vertices[3],
                    },
                    new()
                    {
                        Index = 3,
                        NewVertexIndex = level.Rooms[133].Mesh.Rectangles[8].Vertices[2],
                    }
                }
            },
        };
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR2Level level)
    {
        return new()
        {
            new()
            {
                RoomIndex = 58,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 58,
                SourceIndex = 3,
                Vertices = new()
                {
                    level.Rooms[58].Mesh.Rectangles[2].Vertices[2],
                    level.Rooms[58].Mesh.Rectangles[2].Vertices[1],
                    (ushort)level.Rooms[58].Mesh.Vertices.Count,
                    level.Rooms[58].Mesh.Rectangles[1].Vertices[3],
                }
            },
            new()
            {
                RoomIndex = 133,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 133,
                SourceIndex = 13,
                Vertices = new()
                {
                    level.Rooms[133].Mesh.Rectangles[8].Vertices[2],
                    level.Rooms[133].Mesh.Rectangles[23].Vertices[3],
                    level.Rooms[133].Mesh.Rectangles[22].Vertices[3],
                    level.Rooms[133].Mesh.Rectangles[12].Vertices[0],
                }
            },
        };
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR2Level level)
    {
        return new()
        {
            Reface(level, 27, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1565, 8),
            Reface(level, 32, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1518, 2),
            Reface(level, 41, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1565, 49),
            Reface(level, 41, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1674, 71),
            Reface(level, 58, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1565, 4),
            Reface(level, 60, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedQuad, 1667, 13),
            Reface(level, 60, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedQuad, 1654, 14),
            Reface(level, 73, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1756, 22),
            Reface(level, 133, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1713, 22),
            Reface(level, 133, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1711, 23),
            Reface(level, 157, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1518, 52),
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(98, TRMeshFaceType.TexturedQuad, 27, 1),
            Rotate(58, TRMeshFaceType.TexturedQuad, 4, 3),
            Rotate(60, TRMeshFaceType.TexturedTriangle, 14, 1),
            Rotate(73, TRMeshFaceType.TexturedTriangle, 5, 2),
            Rotate(73, TRMeshFaceType.TexturedTriangle, 6, 2),
            Rotate(101, TRMeshFaceType.TexturedTriangle, 0, 2),
            Rotate(101, TRMeshFaceType.TexturedTriangle, 13, 2),
        };
    }
}
