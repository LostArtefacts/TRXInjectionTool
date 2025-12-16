using System.Drawing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2HSHTextureBuilder : TextureBuilder
{
    public override string ID => "house_textures";

    public override List<InjectionData> Build()
    {
        InjectionData data = CreateBaseData();

        TR2Level house = _control2.Read($"Resources/{TR2LevelNames.HOME}");
        FixTransparentTextures(house, data);

        data.RoomEdits.AddRange(CreateVertexShifts(house));
        data.RoomEdits.AddRange(CreateShifts(house));
        data.RoomEdits.AddRange(CreateFillers(house));
        data.RoomEdits.AddRange(CreateRefacings(house));
        data.RoomEdits.AddRange(CreateRotations());

        FixPassport(house, data);
        FixPushButton(data);

        return [data];
    }

    private static List<TRRoomVertexMove> CreateVertexShifts(TR2Level level)
    {
        var mesh27 = level.Rooms[27].Mesh;
        return
        [
            new()
            {
                RoomIndex = 61,
                VertexIndex = level.Rooms[61].Mesh.Rectangles[14].Vertices[0],
                VertexChange = new() { Y = -256 },
            },
            new()
            {
                RoomIndex = 61,
                VertexIndex = level.Rooms[61].Mesh.Rectangles[14].Vertices[1],
                VertexChange = new() { Y = -256 },
            },
            new()
            {
                RoomIndex = 63,
                VertexIndex = level.Rooms[63].Mesh.Rectangles[48].Vertices[1],
                VertexChange = new() { Y = -768 },
            },
            new()
            {
                RoomIndex = 63,
                VertexIndex = level.Rooms[63].Mesh.Rectangles[48].Vertices[2],
                VertexChange = new() { Y = -768 },
            },
            new()
            {
                RoomIndex = 63,
                VertexIndex = level.Rooms[63].Mesh.Rectangles[26].Vertices[2],
                VertexChange = new() { Y = -768 },
            },
            new()
            {
                RoomIndex = 27,
                VertexIndex = mesh27.Rectangles[51].Vertices[0],
                VertexChange = new(),
                ShadeChange = ShiftLighting(mesh27, 51, 0, 42, 1),
            },
            new()
            {
                RoomIndex = 27,
                VertexIndex = mesh27.Rectangles[51].Vertices[3],
                VertexChange = new(),
                ShadeChange = ShiftLighting(mesh27, 51, 3, 59, 3),
            },
            new()
            {
                RoomIndex = 27,
                VertexIndex = mesh27.Rectangles[51].Vertices[1],
                VertexChange = new(),
                ShadeChange = ShiftLighting(mesh27, 51, 1, 41, 1),
            },
            new()
            {
                RoomIndex = 27,
                VertexIndex = mesh27.Rectangles[51].Vertices[2],
                VertexChange = new(),
                ShadeChange = ShiftLighting(mesh27, 51, 2, 59, 0),
            },
        ];
    }

    private static List<TRRoomTextureEdit> CreateShifts(TR2Level level)
    {
        var vert = level.Rooms[63].Mesh.Vertices[level.Rooms[63].Mesh.Rectangles[48].Vertices[2]];

        return
        [
            new TRRoomTextureMove
            {
                RoomIndex = 61,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 15,
                VertexRemap =
                [
                    new()
                    {
                        Index = 0,
                        NewVertexIndex = level.Rooms[61].Mesh.Rectangles[15].Vertices[1],
                    },
                    new()
                    {
                        Index = 1,
                        NewVertexIndex = level.Rooms[61].Mesh.Rectangles[15].Vertices[0],
                    },
                    new()
                    {
                        Index = 2,
                        NewVertexIndex = level.Rooms[61].Mesh.Rectangles[15].Vertices[3],
                    },
                    new()
                    {
                        Index = 3,
                        NewVertexIndex = level.Rooms[61].Mesh.Rectangles[15].Vertices[3],
                    },
                ]
            },

            new TRRoomVertexCreate
            {
                RoomIndex = 63,
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
            },
            new TRRoomTextureMove
            {
                RoomIndex = 63,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 48,
                VertexRemap =
                [
                    new()
                    {
                        Index = 0,
                        NewVertexIndex = level.Rooms[63].Mesh.Rectangles[69].Vertices[1],
                    },
                    new()
                    {
                        Index = 3,
                        NewVertexIndex = (ushort)level.Rooms[63].Mesh.Vertices.Count,
                    },
                ]
            },
            new TRRoomTextureMove
            {
                RoomIndex = 63,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 26,
                VertexRemap =
                [
                    new()
                    {
                        Index = 0,
                        NewVertexIndex = (ushort)level.Rooms[63].Mesh.Vertices.Count,
                    },
                    new()
                    {
                        Index = 3,
                        NewVertexIndex = level.Rooms[63].Mesh.Rectangles[2].Vertices[0],
                    },
                ]
            },
            new TRRoomTextureMove
            {
                RoomIndex = 27,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 49,
                VertexRemap =
                [
                    new()
                    {
                        Index = 0,
                        NewVertexIndex = level.Rooms[27].Mesh.Rectangles[40].Vertices[1],
                    },
                    new()
                    {
                        Index = 1,
                        NewVertexIndex = level.Rooms[27].Mesh.Rectangles[58].Vertices[0],
                    },
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
                RoomIndex = 63,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 63,
                SourceIndex = 69,
                Vertices =
                [
                    level.Rooms[63].Mesh.Rectangles[69].Vertices[1],
                    (ushort)level.Rooms[63].Mesh.Vertices.Count,
                    level.Rooms[63].Mesh.Rectangles[45].Vertices[1],
                    level.Rooms[63].Mesh.Rectangles[45].Vertices[0],
                ]
            },
            new()
            {
                RoomIndex = 63,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 63,
                SourceIndex = 69,
                Vertices =
                [
                    (ushort)level.Rooms[63].Mesh.Vertices.Count,
                    level.Rooms[63].Mesh.Rectangles[2].Vertices[0],                    
                    level.Rooms[63].Mesh.Rectangles[2].Vertices[3],
                    level.Rooms[63].Mesh.Rectangles[45].Vertices[1],
                ]
            },
            CreateFace(27, 27, 49,TRMeshFaceType.TexturedQuad,
            [
                level.Rooms[27].Mesh.Rectangles[51].Vertices[0],
                level.Rooms[27].Mesh.Rectangles[51].Vertices[3],
                level.Rooms[27].Mesh.Rectangles[58].Vertices[0],
                level.Rooms[27].Mesh.Rectangles[40].Vertices[1],
            ]),
        ];
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR2Level level)
    {
        return
        [
            Reface(level, 9, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedQuad, 1388, 0),
            Reface(level, 61, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1359, 14),
            Reface(level, 61, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1359, 18),
        ];
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return
        [
            Rotate(2, TRMeshFaceType.TexturedTriangle, 1, 2),
            Rotate(34, TRMeshFaceType.TexturedQuad, 38, 3),
            Rotate(54, TRMeshFaceType.TexturedQuad, 15, 3),
        ];
    }

    private static void FixTransparentTextures(TR2Level level, InjectionData data)
    {
        FixLaraTransparency(level, data);
        FixTransparentPixels(level, data,
            level.Rooms[52].Mesh.Rectangles[70], Color.FromArgb(148, 32, 0));
    }

    private InjectionData CreateBaseData()
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.HOME}");
        var palette = level.Palette.ToList();
        ResetLevel(level, 1);
        level.Palette = palette;

        FixHomeWindows(level, TR2LevelNames.HOME);
        FixToilets(level, TR2LevelNames.HOME);
        FixHomeStatues(level);

        level.Models[TR2Type.BreakableWindow1].Animations.Clear();

        InjectionData data = InjectionData.Create(level, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.HOME);

        data.SetMeshOnlyModel((uint)TR2Type.BreakableWindow1);

        return data;
    }
}
