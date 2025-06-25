using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1Cut3TextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level cut3 = _control1.Read($"Resources/{TR1LevelNames.MINES_CUT}");
        InjectionData data = CreateBaseData();
        CreateDefaultTests(data, TR1LevelNames.MINES_CUT);

        data.RoomEdits.AddRange(CreateRotations());
        FixPlatformDoorArea(data, cut3);

        return new() { data };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(21, TRMeshFaceType.TexturedTriangle, 7, 2),
            Rotate(6, TRMeshFaceType.TexturedTriangle, 7, 2),
        };
    }

    private static void FixPlatformDoorArea(InjectionData data, TR1Level cut3)
    {
        foreach (short room in new short[] { 14, 7 })
        {
            var mesh = cut3.Rooms[room].Mesh;
            var baseShade = mesh.Vertices[mesh.Rectangles[82].Vertices[2]].Lighting;
            data.RoomEdits.AddRange(new List<TRRoomVertexMove>()
            {
                new()
                {
                    RoomIndex = room,
                    VertexIndex = mesh.Rectangles[40].Vertices[2],
                    VertexChange = new() { Y = -1536 },
                    ShadeChange = (short)(baseShade - mesh.Vertices[mesh.Rectangles[40].Vertices[2]].Lighting),
                },
                new()
                {
                    RoomIndex = room,
                    VertexIndex = mesh.Rectangles[40].Vertices[3],
                    VertexChange = new() { Y = -1536 },
                    ShadeChange = (short)(baseShade - mesh.Vertices[mesh.Rectangles[40].Vertices[3]].Lighting),
                },
                new()
                {
                    RoomIndex = room,
                    VertexIndex = mesh.Rectangles[39].Vertices[0],
                    VertexChange = new() { Y = -1024 },
                    ShadeChange = (short)(baseShade - mesh.Vertices[mesh.Rectangles[39].Vertices[0]].Lighting),
                },
                new()
                {
                    RoomIndex = room,
                    VertexIndex = mesh.Rectangles[39].Vertices[1],
                    VertexChange = new() { Y = -1024 },
                    ShadeChange = (short)(baseShade - mesh.Vertices[mesh.Rectangles[39].Vertices[1]].Lighting),
                },
            });

            data.RoomEdits.Add(new TRRoomTextureCreate
            {
                RoomIndex = room,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = room,
                SourceIndex = 68,
                Vertices = new()
                {
                    mesh.Rectangles[40].Vertices[3],
                    mesh.Rectangles[40].Vertices[2],
                    mesh.Rectangles[39].Vertices[1],
                    mesh.Rectangles[39].Vertices[0],
                }
            });

            data.RoomEdits.Add(Reface(cut3, room, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 39, 40));
            data.RoomEdits.Add(Reface(cut3, room, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 14, 39));

            var shadeFixes = new[] { 66, 44, 39, 33, 3 }
                .SelectMany(i => mesh.Rectangles[i].Vertices.GetRange(0, 2))
                .Select(v => mesh.Vertices[v])
                .Distinct()
                .Select(v => new TRRoomVertexMove
                {
                    RoomIndex = room,
                    VertexIndex = (ushort)mesh.Vertices.IndexOf(v),
                    VertexChange = new(),
                    ShadeChange = (short)(baseShade - v.Lighting),
                });
            data.RoomEdits.AddRange(shadeFixes);
        }
    }

    private static InjectionData CreateBaseData()
    {
        TR1Level baseLevel = CreateAtlantisContinuityLevel(TR1Type.SceneryBase + 17);
        InjectionData data = InjectionData.Create(baseLevel, InjectionType.TextureFix, "cut3_textures");

        data.RoomEdits.Add(new TRRoomStatic3DCreate
        {
            ID = 17,
            RoomIndex = 6,
            StaticMesh = new()
            {
                X = 51712,
                Y = -20404,
                Z = 51712,
                Intensity = 7936,
            }
        });
        data.RoomEdits.Add(new TRRoomStatic3DCreate
        {
            ID = 17,
            RoomIndex = 21,
            StaticMesh = new()
            {
                X = 51712,
                Y = -20404,
                Z = 51712,
                Intensity = 4096,
            }
        });


        data.RoomEdits.Add(new TRRoomStatic3DCreate
        {
            ID = 18,
            RoomIndex = 10,
            StaticMesh = new()
            {
                X = 64000 - 512,
                Y = -15616,
                Z = 51712 - 512,
                Angle = 16384,
                Intensity = 7936,
            }
        });
        data.RoomEdits.Add(new TRRoomStatic3DCreate
        {
            ID = 18,
            RoomIndex = 19,
            StaticMesh = new()
            {
                X = 64000 - 512,
                Y = -15616,
                Z = 51712 - 512,
                Angle = 16384,
                Intensity = 4096,
            }
        });

        return data;
    }
}
