using System.Drawing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public partial class TR2RigTextureBuilder : TextureBuilder
{
    public override string ID => "rig_textures";

    public override List<InjectionData> Build()
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.RIG}");
        var data = CreateBaseData();

        data.RoomEdits.AddRange(CreateShifts(level));
        data.RoomEdits.AddRange(CreateFillers(level));
        data.RoomEdits.AddRange(CreateRefacings(level));
        data.RoomEdits.AddRange(CreateRotations());
        data.RoomEdits.AddRange(FixCatwalks(level));
        FixThinWall(level, data);
        FixTransparentTextures(level, data);
        FixPassport(level, data);
        FixPushButton(data);
        FixWheelDoor(data, TR2LevelNames.RIG);
        FixSlidingOffshoreDoor(data, TR2LevelNames.RIG);
        FixPlaneStatics(level, data);
        FixOxygenTanks(level, data);

        return [data];
    }

    private InjectionData CreateBaseData()
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.RIG}");
        var types = TR2TypeUtilities.BreakableWindows()
            .FindAll(t => level.Models.ContainsKey(t));
        CreateModelLevel(level, [.. types]);
        level.SoundEffects.Clear();

        FixTR2Windows(level);

        var data = InjectionData.Create(level, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.RIG);
        foreach (var type in types)
        {
            data.SetMeshOnlyModel((uint)type);
        }
        return data;
    }

    private static List<TRRoomTextureMove> CreateShifts(TR2Level level)
    {
        return
        [
            new()
            {
                RoomIndex = 35,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 42,
                VertexRemap =
                [
                    new()
                    {
                        Index = 0,
                        NewVertexIndex = level.Rooms[35].Mesh.Rectangles[54].Vertices[1],
                    },
                    new()
                    {
                        Index = 1,
                        NewVertexIndex = level.Rooms[35].Mesh.Rectangles[28].Vertices[0],
                    }
                ]
            },
            new()
            {
                RoomIndex = 81,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 33,
                VertexRemap =
                [
                    new()
                    {
                        Index = 0,
                        NewVertexIndex = level.Rooms[81].Mesh.Rectangles[41].Vertices[1],
                    },
                    new()
                    {
                        Index = 1,
                        NewVertexIndex = level.Rooms[81].Mesh.Rectangles[23].Vertices[0],
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
                RoomIndex = 35,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 35,
                SourceIndex = 56,
                Vertices =
                [
                    level.Rooms[35].Mesh.Rectangles[56].Vertices[1],
                    level.Rooms[35].Mesh.Rectangles[30].Vertices[0],
                    level.Rooms[35].Mesh.Rectangles[30].Vertices[3],
                    level.Rooms[35].Mesh.Rectangles[56].Vertices[2],
                ]
            },
            new()
            {
                RoomIndex = 81,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 81,
                SourceIndex = 43,
                Vertices =
                [
                    level.Rooms[81].Mesh.Rectangles[43].Vertices[1],
                    level.Rooms[81].Mesh.Rectangles[25].Vertices[0],
                    level.Rooms[81].Mesh.Rectangles[25].Vertices[3],
                    level.Rooms[81].Mesh.Rectangles[43].Vertices[2],
                ]
            },
        ];
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR2Level level)
    {
        return
        [
            Reface(level, 11, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1622, 137),
            Reface(level, 33, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 1736, 0),
            Reface(level, 35, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1702, 42),
            Reface(level, 81, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1702, 33),
        ];
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return
        [
            Rotate(33, TRMeshFaceType.TexturedTriangle, 0, 2),
        ];
    }

    private static void FixThinWall(TR2Level level, InjectionData data)
    {
        var vert = level.Rooms[11].Mesh.Vertices[level.Rooms[11].Mesh.Rectangles[77].Vertices[1]];
        data.RoomEdits.Add(new TRRoomVertexCreate
        {
            RoomIndex = 11,
            Vertex = new()
            {
                Lighting = vert.Lighting,
                Vertex = new()
                {
                    X = vert.Vertex.X,
                    Y = (short)(vert.Vertex.Y + 256),
                    Z = vert.Vertex.Z,
                },
            },
        });

        data.RoomEdits.Add(new TRRoomTextureMove
        {
            FaceType = TRMeshFaceType.TexturedQuad,
            RoomIndex = 11,
            TargetIndex = 83,
            VertexRemap =
            [
                new()
                {
                    Index = 0,
                    NewVertexIndex = (ushort)level.Rooms[11].Mesh.Vertices.Count,
                },
                new()
                {
                    Index = 1,
                    NewVertexIndex = level.Rooms[11].Mesh.Rectangles[135].Vertices[2],
                }
            ]
        });
        data.RoomEdits.Add(new TRRoomTextureMove
        {
            FaceType = TRMeshFaceType.TexturedQuad,
            RoomIndex = 11,
            TargetIndex = 135,
            VertexRemap =
            [
                new()
                {
                    Index = 0,
                    NewVertexIndex = level.Rooms[11].Mesh.Rectangles[77].Vertices[1],
                },
                new()
                {
                    Index = 3,
                    NewVertexIndex = (ushort)level.Rooms[11].Mesh.Vertices.Count,
                }
            ]
        });


        var sector = TRRoomSectorExt.CloneFrom(level.Rooms[11].GetSector(2, 16, TRUnit.Sector));
        sector.Ceiling += 1024;
        data.FloorEdits.Add(new()
        {
            RoomIndex = 11,
            X = 2,
            Z = 16,
            Fixes =
            [
                new FDSectorOverwrite
                {
                    Sector = sector,
                },
                new FDTrigCreateFix
                {
                    Entries =
                    [
                        new FDSlantEntry
                        {
                            Type = FDSlantType.Ceiling,
                            ZSlant = 3,
                        }
                    ]
                }
            ]
        });
    }

    private static void FixTransparentTextures(TR2Level level, InjectionData data)
    {
        FixTransparentPixels(level, data,
            level.Rooms[96].Mesh.Rectangles[27], Color.FromArgb(246, 238, 213));
    }

    private static void FixPlaneStatics(TR2Level level, InjectionData data)
    {
        data.RoomEdits.Add(CreateQuadShift(92, 4,
        [
            new(0, level.Rooms[92].Mesh.Rectangles[4].Vertices[3]),
            new(1, level.Rooms[92].Mesh.Rectangles[4].Vertices[2]),
            new(2, level.Rooms[92].Mesh.Rectangles[4].Vertices[1]),
            new(3, level.Rooms[92].Mesh.Rectangles[4].Vertices[0]),
        ]));
        data.RoomEdits.Add(CreateQuadShift(92, 27,
        [
            new(0, level.Rooms[92].Mesh.Rectangles[27].Vertices[3]),
            new(1, level.Rooms[92].Mesh.Rectangles[27].Vertices[2]),
            new(2, level.Rooms[92].Mesh.Rectangles[27].Vertices[1]),
            new(3, level.Rooms[92].Mesh.Rectangles[27].Vertices[0]),
        ]));

        foreach (var face in new[] { 27, 5, 30, 7 })
        {
            data.RoomEdits.Add(CreateQuadShift(95, (short)face,
            [
                new(0, level.Rooms[95].Mesh.Rectangles[20].Vertices[1]),
                new(1, level.Rooms[95].Mesh.Rectangles[20].Vertices[0]),
                new(2, level.Rooms[95].Mesh.Rectangles[20].Vertices[3]),
                new(3, level.Rooms[95].Mesh.Rectangles[20].Vertices[2]),
            ]));
        }

        data.RoomEdits.Add(CreateQuadShift(2, 212,
        [
            new(0, level.Rooms[2].Mesh.Rectangles[211].Vertices[1]),
            new(1, level.Rooms[2].Mesh.Rectangles[211].Vertices[0]),
            new(2, level.Rooms[2].Mesh.Rectangles[211].Vertices[3]),
            new(3, level.Rooms[2].Mesh.Rectangles[211].Vertices[2]),
        ]));
        data.RoomEdits.Add(CreateQuadShift(96, 33,
        [
            new(0, level.Rooms[96].Mesh.Rectangles[31].Vertices[1]),
            new(1, level.Rooms[96].Mesh.Rectangles[31].Vertices[0]),
            new(2, level.Rooms[96].Mesh.Rectangles[31].Vertices[3]),
            new(3, level.Rooms[96].Mesh.Rectangles[31].Vertices[2]),
        ]));

        data.MeshEdits.AddRange(new[] { 17, 18 }.Select(type =>
            new TRMeshEdit
            {
                ModelID = (uint)((int)TR2Type.SceneryBase + type),
                VertexEdits = new[] { 0, 2, 3, 5}.Select(i =>
                    new TRVertexEdit
                    {
                        Index = (short)i,
                        Change = new()
                        {
                            X = 3,
                            Z = (short)((type == 17 && (i == 2 || i == 5)) ? 6 : 0),
                            Y = (short)(i == 3 || i == 5 ? -1 : 0),
                        }
                    }).ToList(),            
            }));
    }

    private static void FixOxygenTanks(TR2Level level, InjectionData data)
    {
        var mesh = level.Rooms[34].StaticMeshes[^1];
        data.RoomEdits.Add(new TRRoomStatic3DCreate
        {
            ID = 33,            
            RoomIndex = level.Rooms[34].AlternateRoom,
            StaticMesh = new()
            {
                X = mesh.X,
                Y = mesh.Y,
                Z = mesh.Z,
                Angle = mesh.Angle,
                Intensity = mesh.Intensity1,
            }
        });
    }
}
