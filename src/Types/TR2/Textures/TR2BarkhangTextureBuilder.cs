using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2BarkhangTextureBuilder : TextureBuilder
{
    public override string ID => "barkhang_textures";

    public override List<InjectionData> Build()
    {
        TR2Level barkhang = _control2.Read($"Resources/{TR2LevelNames.MONASTERY}");
        InjectionData data = CreateBaseData();

        data.RoomEdits.AddRange(CreateVertexShifts(barkhang));
        data.RoomEdits.AddRange(CreateShifts(barkhang));
        data.RoomEdits.AddRange(CreateFillers(barkhang));
        data.RoomEdits.AddRange(CreateRefacings(barkhang));
        data.RoomEdits.AddRange(CreateRotations());
        data.MeshEdits.Add(
            FixStaticMeshPosition(barkhang.StaticMeshes, TR2Type.Architecture7, new() { X = 10 }));

        FixPassport(barkhang, data);

        return new() { data };
    }

    private static List<TRRoomVertexMove> CreateVertexShifts(TR2Level level)
    {
        return new()
        {
            new()
            {
                RoomIndex = 70,
                VertexIndex = level.Rooms[70].Mesh.Rectangles[113].Vertices[1],
                VertexChange = new() { Y = 256 },
            },
            new()
            {
                RoomIndex = 70,
                VertexIndex = level.Rooms[70].Mesh.Rectangles[113].Vertices[2],
                VertexChange = new() { Y = 256 },
            },
        };
    }

    private static List<TRRoomTextureMove> CreateShifts(TR2Level level)
    {
        return new()
        {
            new()
            {
                RoomIndex = 94,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 63,
                VertexRemap = new()
                {
                    new()
                    {
                        Index = 0,
                        NewVertexIndex = level.Rooms[94].Mesh.Rectangles[66].Vertices[2],
                    },
                    new()
                    {
                        Index = 3,
                        NewVertexIndex = level.Rooms[94].Mesh.Rectangles[52].Vertices[3],
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
                RoomIndex = 94,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 94,
                SourceIndex = 63,
                Vertices = new()
                {
                    level.Rooms[94].Mesh.Rectangles[67].Vertices[2],
                    level.Rooms[94].Mesh.Rectangles[67].Vertices[1],
                    level.Rooms[94].Mesh.Rectangles[53].Vertices[1],
                    level.Rooms[94].Mesh.Rectangles[53].Vertices[0],
                }
            },
        };
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR2Level level)
    {
        return new()
        {
            Reface(level, 45, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1370, 67),
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(23, TRMeshFaceType.TexturedQuad, 203, 2),
            Rotate(25, TRMeshFaceType.TexturedTriangle, 8, 2),
            Rotate(70, TRMeshFaceType.TexturedQuad, 112, 2),
            Rotate(87, TRMeshFaceType.TexturedTriangle, 57, 2),
        };
    }

    private InjectionData CreateBaseData()
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.MONASTERY}");
        FixPlaceholderBridges(level, TR2LevelNames.MONASTERY, new()
        {
            [TR2Type.SceneryBase + 21] = level.StaticMeshes[TR2Type.SceneryBase + 21],
            [TR2Type.SceneryBase + 22] = level.StaticMeshes[TR2Type.SceneryBase + 22],
            [TR2Type.SceneryBase + 23] = level.StaticMeshes[TR2Type.SceneryBase + 23],
        });

        var data = InjectionData.Create(level, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.MONASTERY);

        {
            level = _control2.Read($"Resources/{TR2LevelNames.MONASTERY}");
            var mesh = level.Rooms[40].Mesh;
            data.RoomEdits.Add(CreateFace(40, 113, 84, TRMeshFaceType.TexturedQuad, new[]
            {
                mesh.Rectangles[7].Vertices[1], mesh.Rectangles[27].Vertices[0],
                mesh.Rectangles[27].Vertices[3], mesh.Rectangles[7].Vertices[2],
            }));
            data.RoomEdits.Add(CreateFace(40, 113, 84, TRMeshFaceType.TexturedQuad, new[]
            {
                mesh.Rectangles[20].Vertices[1], mesh.Rectangles[8].Vertices[0],
                mesh.Rectangles[8].Vertices[3], mesh.Rectangles[20].Vertices[2],
            }));
        }

        return data;
    }
}
