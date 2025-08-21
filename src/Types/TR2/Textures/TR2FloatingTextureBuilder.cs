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
        var level = _control2.Read($"Resources/{TR2LevelNames.FLOATER}");
        var data = CreateBaseData();

        data.RoomEdits.AddRange(CreateVertexShifts(level));
        data.RoomEdits.AddRange(CreateShifts(level));
        data.RoomEdits.AddRange(CreateFillers(level));
        data.RoomEdits.AddRange(CreateRefacings(level));
        data.RoomEdits.AddRange(CreateRotations());

        FixRoom29(level, data);
        FixPassport(level, data);

        return [data];
    }

    private static List<TRRoomVertexMove> CreateVertexShifts(TR2Level level)
    {
        return
        [
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
        ];
    }

    private static List<TRRoomTextureEdit> CreateShifts(TR2Level level)
    {
        var vert = level.Rooms[58].Mesh.Vertices[level.Rooms[58].Mesh.Rectangles[2].Vertices[1]];

        return
        [
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
                VertexRemap =
                [
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
                ]
            },
            new TRRoomTextureMove
            {
                RoomIndex = 133,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 13,
                VertexRemap =
                [
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
                ]
            },
        ];
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR2Level level)
    {
        return
        [
            new()
            {
                RoomIndex = 58,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 58,
                SourceIndex = 3,
                Vertices =
                [
                    level.Rooms[58].Mesh.Rectangles[2].Vertices[2],
                    level.Rooms[58].Mesh.Rectangles[2].Vertices[1],
                    (ushort)level.Rooms[58].Mesh.Vertices.Count,
                    level.Rooms[58].Mesh.Rectangles[1].Vertices[3],
                ]
            },
            new()
            {
                RoomIndex = 133,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 133,
                SourceIndex = 13,
                Vertices =
                [
                    level.Rooms[133].Mesh.Rectangles[8].Vertices[2],
                    level.Rooms[133].Mesh.Rectangles[23].Vertices[3],
                    level.Rooms[133].Mesh.Rectangles[22].Vertices[3],
                    level.Rooms[133].Mesh.Rectangles[12].Vertices[0],
                ]
            },
        ];
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR2Level level)
    {
        return
        [
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
        ];
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return
        [
            Rotate(98, TRMeshFaceType.TexturedQuad, 27, 1),
            Rotate(58, TRMeshFaceType.TexturedQuad, 4, 3),
            Rotate(60, TRMeshFaceType.TexturedTriangle, 14, 1),
            Rotate(73, TRMeshFaceType.TexturedTriangle, 5, 2),
            Rotate(73, TRMeshFaceType.TexturedTriangle, 6, 2),
            Rotate(101, TRMeshFaceType.TexturedTriangle, 0, 2),
            Rotate(101, TRMeshFaceType.TexturedTriangle, 13, 2),
        ];
    }

    private static void FixRoom29(TR2Level level, InjectionData data)
    {
        data.VisPortalEdits.Add(FDBuilder.DeletePortal(level.Rooms, 29, 1));
        data.VisPortalEdits.Add(FDBuilder.DeletePortal(level.Rooms, 131, 0));
        var faces = new[] { 53, 70, 87 };
        data.RoomEdits.AddRange(faces.Select(face =>
            Reface(level, 29, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1509, (short)face)));

        for (int i = 2; i < 4; i++)
        {
            var room = level.Rooms[131];
            var vert = room.Mesh.Vertices[room.Mesh.Rectangles[17].Vertices[i]].Clone() as TR2RoomVertex;
            data.RoomEdits.Add(CreateVertex(131, room, vert, -1, 1024));
            room.Mesh.Rectangles[17].Vertices[i] = (ushort)(room.Mesh.Vertices.Count - 1);
        }

        var mesh = level.Rooms[131].Mesh;
        data.RoomEdits.Add(CreateQuadShift(131, 11,
        [
            new(2, mesh.Rectangles[9].Vertices[2]),
            new(3, mesh.Rectangles[17].Vertices[2]),
        ]));
        data.RoomEdits.Add(CreateQuadShift(131, 17,
        [
            new(2, mesh.Rectangles[17].Vertices[2]),
            new(3, mesh.Rectangles[17].Vertices[3]),
        ]));
        data.RoomEdits.Add(CreateQuadShift(131, 23,
        [
            new(2, mesh.Rectangles[17].Vertices[3]),
            new(3, mesh.Rectangles[28].Vertices[3]),
        ]));

        data.RoomEdits.Add(CreateFace(131, 131, 9, TRMeshFaceType.TexturedQuad,
        [
            mesh.Rectangles[9].Vertices[1], mesh.Rectangles[8].Vertices[0],
            mesh.Rectangles[17].Vertices[2], mesh.Rectangles[9].Vertices[2],
        ]));
        data.RoomEdits.Add(CreateFace(131, 131, 9, TRMeshFaceType.TexturedQuad,
        [
            mesh.Rectangles[16].Vertices[1], mesh.Rectangles[16].Vertices[0],
            mesh.Rectangles[17].Vertices[3], mesh.Rectangles[17].Vertices[2],
        ]));
        data.RoomEdits.Add(CreateFace(131, 131, 9, TRMeshFaceType.TexturedQuad,
        [
            mesh.Rectangles[22].Vertices[1], mesh.Rectangles[22].Vertices[0],
            mesh.Rectangles[28].Vertices[3], mesh.Rectangles[17].Vertices[3],
        ]));

        foreach (var face in new[] { 11, 17, 23 })
        {
            data.RoomEdits.Add(Reface(level, 131, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1559, (short)face));
            data.RoomEdits.Add(Rotate(131, TRMeshFaceType.TexturedQuad, (short)face, 2));
        }

        for (int i = 2; i < 4; i++)
        {
            var room = level.Rooms[29];
            var vert = room.Mesh.Vertices[room.Mesh.Rectangles[67].Vertices[i]].Clone() as TR2RoomVertex;
            data.RoomEdits.Add(CreateVertex(29, room, vert, -1, -2048));
        }

        mesh = level.Rooms[29].Mesh;
        data.RoomEdits.Add(CreateFace(29, 29, 30, TRMeshFaceType.TexturedQuad,
        [
            mesh.Rectangles[53].Vertices[1], mesh.Rectangles[53].Vertices[0],
            mesh.Rectangles[30].Vertices[3], (ushort)(mesh.Vertices.Count - 2),
        ]));
        data.RoomEdits.Add(CreateFace(29, 29, 30, TRMeshFaceType.TexturedQuad,
        [
            mesh.Rectangles[70].Vertices[1], mesh.Rectangles[70].Vertices[0],
            (ushort)(mesh.Vertices.Count - 2), (ushort)(mesh.Vertices.Count - 1),
        ]));
        data.RoomEdits.Add(CreateFace(29, 29, 30, TRMeshFaceType.TexturedQuad,
        [
            mesh.Rectangles[87].Vertices[1], mesh.Rectangles[87].Vertices[0],
            (ushort)(mesh.Vertices.Count - 1), mesh.Rectangles[107].Vertices[2],
        ]));

        for (ushort x = 2; x < 5; x++)
        {
            var sector = TRRoomSectorExt.CloneFrom(level.Rooms[29].GetSector(x, 8, TRUnit.Sector));
            sector.Ceiling = -4608;
            sector.RoomAbove = -1;
            data.FloorEdits.Add(new()
            {
                RoomIndex = 29,
                X = x,
                Z = 8,
                Fixes =
                [
                    new FDSectorOverwrite { Sector = sector },
                ]
            });
        }
    }

    private InjectionData CreateBaseData()
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.FLOATER}");
        CreateModelLevel(level, TR2Type.LiftingDoor3);
        level.SoundEffects.Clear();

        var mesh = level.Models[TR2Type.LiftingDoor3].Meshes[0];
        var faces = mesh.TexturedRectangles.ToList();

        for (int i = 2; i < 5; i++)
        {
            var tex = level.ObjectTextures[i].Clone();
            tex.UVMode = TRUVMode.NE_AntiClockwise;
            level.ObjectTextures.Add(tex);
        }

        var map = new Dictionary<ushort, ushort>
        {
            [0] = 1,
            [1] = 0,
            [2] = (ushort)(level.ObjectTextures.Count - 3),
            [3] = (ushort)(level.ObjectTextures.Count - 2),
            [4] = (ushort)(level.ObjectTextures.Count - 1),
        };

        mesh.TexturedRectangles.AddRange(faces.Select(f => new TRMeshFace
        {
            Type = TRFaceType.Rectangle,
            Texture = map.TryGetValue(f.Texture, out ushort value) ? value : f.Texture,
            Vertices =
            [
                f.Vertices[1], f.Vertices[0], f.Vertices[3], f.Vertices[2],
            ]
        }));

        mesh.Vertices.ForEach(v => v.X += (short)(v.X == -1 ? -1 : 1));

        var verts = new[] { 0, 1, 4, 5 };
        mesh.Vertices.AddRange(verts.Select(v => new TRVertex
        {
            X = mesh.Vertices[v].X,
            Y = (short)(mesh.Vertices[v].Y - 96),
            Z = mesh.Vertices[v].Z,
        }));
        mesh.Normals.AddRange(verts.Select(v => mesh.Normals[v].Clone()));

        mesh.TexturedRectangles.Add(new()
        {
            Type = TRFaceType.Rectangle,
            Texture = 3,
            Vertices = [ 9, 11, 10, 8 ]
        });

        var data = InjectionData.Create(level, InjectionType.TextureFix, ID);
        data.SetMeshOnlyModel((uint)TR2Type.LiftingDoor3);
        CreateDefaultTests(data, TR2LevelNames.FLOATER);
        return data;
    }
}
