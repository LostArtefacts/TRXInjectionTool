using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2DeckTextureBuilder : TextureBuilder
{
    public override string ID => "deck_textures";

    public override List<InjectionData> Build()
    {
        TR2Level level = _control2.Read($"Resources/{TR2LevelNames.DECK}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.DECK);

        data.RoomEdits.AddRange(CreateShifts(level));
        data.RoomEdits.AddRange(CreateFillers(level));
        data.RoomEdits.AddRange(CreateRefacings(level));
        data.RoomEdits.AddRange(CreateRotations());
        FixRoom80(level, data);

        FixPassport(level, data);
        FixPushButton(data);

        return [data];
    }

    private static List<TRRoomTextureMove> CreateShifts(TR2Level level)
    {
        return
        [
            new()
            {
                RoomIndex = 84,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 51,
                VertexRemap =
                [
                    new()
                    {
                        Index = 2,
                        NewVertexIndex = level.Rooms[84].Mesh.Rectangles[49].Vertices[3],
                    },
                    new()
                    {
                        Index = 3,
                        NewVertexIndex = level.Rooms[84].Mesh.Rectangles[53].Vertices[2],
                    }
                ]
            },
            new()
            {
                RoomIndex = 36,
                FaceType = TRMeshFaceType.TexturedTriangle,
                TargetIndex = 14,
                VertexRemap =
                [
                    new()
                    {
                        Index = 2,
                        NewVertexIndex = level.Rooms[36].Mesh.Rectangles[54].Vertices[1],
                    },
                ]
            },
            CreateQuadShift(36, 56,
            [
                new(1, level.Rooms[36].Mesh.Rectangles[52].Vertices[0]),
                new(2, level.Rooms[36].Mesh.Rectangles[52].Vertices[3]),
            ]),
            CreateQuadShift(36, 58,
            [
                new(2, level.Rooms[36].Mesh.Rectangles[52].Vertices[0]),
            ])
        ];
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR2Level level)
    {
        return
        [
            new()
            {
                RoomIndex = 84,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 84,
                SourceIndex = 50,
                Vertices =
                [
                    level.Rooms[84].Mesh.Rectangles[54].Vertices[1],
                    level.Rooms[84].Mesh.Rectangles[50].Vertices[0],
                    level.Rooms[84].Mesh.Rectangles[50].Vertices[3],
                    level.Rooms[84].Mesh.Rectangles[54].Vertices[2],
                ]
            },
        ];
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR2Level level)
    {
        return
        [
            Reface(level, 47, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1643, 164),
            Reface(level, 47, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1637, 166),
            Reface(level, 60, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1592, 79),
            Reface(level, 60, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1618, 138),
            Reface(level, 68, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1681, 117),
            Reface(level, 113, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1637, 1),
        ];
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return
        [
            Rotate(1, TRMeshFaceType.TexturedTriangle, 1, 2),
            Rotate(60, TRMeshFaceType.TexturedQuad, 138, 3),
            Rotate(85, TRMeshFaceType.TexturedTriangle, 1, 2),
        ];
    }

    private static void FixRoom80(TR2Level level, InjectionData data)
    {
        var vert = level.Rooms[80].Mesh.Vertices[level.Rooms[80].Mesh.Rectangles[49].Vertices[1]];
        data.RoomEdits.Add(new TRRoomVertexCreate
        {
            RoomIndex = 80,
            Vertex = new()
            {
                Lighting = vert.Lighting,
                Vertex = new()
                {
                    X = vert.Vertex.X,
                    Y = (short)(vert.Vertex.Y - 768),
                    Z = vert.Vertex.Z,
                },
            },
        });
        data.RoomEdits.Add(new TRRoomVertexCreate
        {
            RoomIndex = 80,
            Vertex = new()
            {
                Lighting = vert.Lighting,
                Vertex = new()
                {
                    X = vert.Vertex.X,
                    Y = (short)(vert.Vertex.Y - 2560),
                    Z = vert.Vertex.Z,
                },
            },
        });

        data.RoomEdits.Add(new TRRoomTextureMove
        {
            RoomIndex = 80,
            FaceType = TRMeshFaceType.TexturedQuad,
            TargetIndex = 49,
            VertexRemap =
            [
                new()
                {
                    Index = 0,
                    NewVertexIndex = (ushort)level.Rooms[80].Mesh.Vertices.Count,
                },
                new()
                {
                    Index = 3,
                    NewVertexIndex = level.Rooms[80].Mesh.Rectangles[70].Vertices[1],
                }
            ]
        });
        data.RoomEdits.Add(new TRRoomTextureMove
        {
            RoomIndex = 80,
            FaceType = TRMeshFaceType.TexturedQuad,
            TargetIndex = 5,
            VertexRemap =
            [
                new()
                {
                    Index = 0,
                    NewVertexIndex = level.Rooms[80].Mesh.Rectangles[2].Vertices[3],
                },
                new()
                {
                    Index = 3,
                    NewVertexIndex = (ushort)level.Rooms[80].Mesh.Vertices.Count,
                }
            ]
        });

        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 80,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 80,
            SourceIndex = 49,
            Vertices =
            [
                level.Rooms[80].Mesh.Rectangles[72].Vertices[1],
                (ushort)(level.Rooms[80].Mesh.Vertices.Count + 1),
                (ushort)level.Rooms[80].Mesh.Vertices.Count,
                level.Rooms[80].Mesh.Rectangles[72].Vertices[2],
            ]
        });
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 80,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 80,
            SourceIndex = 49,
            Vertices =
            [

                level.Rooms[80].Mesh.Rectangles[4].Vertices[0],
                level.Rooms[80].Mesh.Rectangles[4].Vertices[3],
                (ushort)level.Rooms[80].Mesh.Vertices.Count,
                (ushort)(level.Rooms[80].Mesh.Vertices.Count + 1),
            ]
        });

        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 80,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 80,
            SourceIndex = 49,
            Vertices =
            [

                level.Rooms[80].Mesh.Rectangles[1].Vertices[3],
                (ushort)(level.Rooms[80].Mesh.Vertices.Count + 1),
                level.Rooms[80].Mesh.Rectangles[71].Vertices[2],
                level.Rooms[80].Mesh.Rectangles[71].Vertices[1],
            ]
        });
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 80,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 80,
            SourceIndex = 49,
            Vertices =
            [
                level.Rooms[80].Mesh.Rectangles[1].Vertices[3],
                level.Rooms[80].Mesh.Rectangles[1].Vertices[2],
                level.Rooms[80].Mesh.Rectangles[3].Vertices[3],
                (ushort)(level.Rooms[80].Mesh.Vertices.Count + 1),
            ]
        });
    }
}
