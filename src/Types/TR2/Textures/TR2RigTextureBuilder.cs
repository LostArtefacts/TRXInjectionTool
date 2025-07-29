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
        TR2Level level = _control2.Read($"Resources/{TR2LevelNames.RIG}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.RIG);

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

        return new() { data };
    }

    private static List<TRRoomTextureMove> CreateShifts(TR2Level level)
    {
        return new()
        {
            new()
            {
                RoomIndex = 35,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 42,
                VertexRemap = new()
                {
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
                }
            },
            new()
            {
                RoomIndex = 81,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 33,
                VertexRemap = new()
                {
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
                RoomIndex = 35,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 35,
                SourceIndex = 56,
                Vertices = new()
                {
                    level.Rooms[35].Mesh.Rectangles[56].Vertices[1],
                    level.Rooms[35].Mesh.Rectangles[30].Vertices[0],
                    level.Rooms[35].Mesh.Rectangles[30].Vertices[3],
                    level.Rooms[35].Mesh.Rectangles[56].Vertices[2],
                }
            },
            new()
            {
                RoomIndex = 81,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 81,
                SourceIndex = 43,
                Vertices = new()
                {
                    level.Rooms[81].Mesh.Rectangles[43].Vertices[1],
                    level.Rooms[81].Mesh.Rectangles[25].Vertices[0],
                    level.Rooms[81].Mesh.Rectangles[25].Vertices[3],
                    level.Rooms[81].Mesh.Rectangles[43].Vertices[2],
                }
            },
        };
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR2Level level)
    {
        return new()
        {
            Reface(level, 11, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1622, 137),
            Reface(level, 33, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 1736, 0),
            Reface(level, 35, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1702, 42),
            Reface(level, 81, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1702, 33),
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(33, TRMeshFaceType.TexturedTriangle, 0, 2),
        };
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
            VertexRemap = new()
            {
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
            }
        });
        data.RoomEdits.Add(new TRRoomTextureMove
        {
            FaceType = TRMeshFaceType.TexturedQuad,
            RoomIndex = 11,
            TargetIndex = 135,
            VertexRemap = new()
            {
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
            }
        });


        var sector = TRRoomSectorExt.CloneFrom(level.Rooms[11].GetSector(2, 16, TRUnit.Sector));
        sector.Ceiling += 1024;
        data.FloorEdits.Add(new()
        {
            RoomIndex = 11,
            X = 2,
            Z = 16,
            Fixes = new()
            {
                new FDSectorOverwrite
                {
                    Sector = sector,
                },
                new FDTrigCreateFix
                {
                    Entries = new()
                    {
                        new FDSlantEntry
                        {
                            Type = FDSlantType.Ceiling,
                            ZSlant = 3,
                        }
                    }
                }
            }
        });
    }

    private static void FixTransparentTextures(TR2Level level, InjectionData data)
    {
        FixTransparentPixels(level, data,
            level.Rooms[96].Mesh.Rectangles[27], Color.FromArgb(246, 238, 213));
    }
}
