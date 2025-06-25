using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1Cut4TextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level cut4 = _control1.Read($"Resources/{TR1LevelNames.ATLANTIS_CUT}");
        InjectionData data = CreateBaseData();
        CreateDefaultTests(data, TR1LevelNames.ATLANTIS_CUT);

        data.RoomEdits.AddRange(CreateRefacings(cut4));
        data.RoomEdits.AddRange(CreateRotations());

        FixMissingDoors(cut4, data);

        return new() { data };
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR1Level cut4)
    {
        return new()
        {
            new()
            {
                RoomIndex = 9,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 9,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 15,
                TargetIndex = 21
            },
            Reface(cut4, 2, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 2, 65),
            Reface(cut4, 4, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 2, 73),
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(15, TRMeshFaceType.TexturedTriangle, 0, 1),
            Rotate(16, TRMeshFaceType.TexturedTriangle, 7, 2),
            Rotate(16, TRMeshFaceType.TexturedQuad, 43, 1),
        };
    }

    private static void FixMissingDoors(TR1Level cut4, InjectionData data)
    {
        TR1Entity door = cut4.Entities.Find(e => e.TypeID == TR1Type.Door1).Clone() as TR1Entity;
        door.X = 64000;
        door.Y = -15616;
        door.Z = 51712;
        door.Room = 14;

        data.FloorEdits.Add(new()
        {
            RoomIndex = 14,
            Fixes = new()
            {
                new FDTrigItem
                {
                    Item = door,
                }
            },
        });

        var baseShade = cut4.Rooms[9].Mesh.Vertices[cut4.Rooms[9].Mesh.Rectangles[67].Vertices[2]].Lighting;
        data.RoomEdits.AddRange(new List<TRRoomVertexMove>()
        {
            new()
            {
                RoomIndex = 9,
                VertexIndex = cut4.Rooms[9].Mesh.Rectangles[40].Vertices[2],
                VertexChange = new() { Y = -1536 },
                ShadeChange = (short)(baseShade - cut4.Rooms[9].Mesh.Vertices[cut4.Rooms[9].Mesh.Rectangles[40].Vertices[2]].Lighting),
            },
            new()
            {
                RoomIndex = 9,
                VertexIndex = cut4.Rooms[9].Mesh.Rectangles[40].Vertices[3],
                VertexChange = new() { Y = -1536 },
                ShadeChange = (short)(baseShade - cut4.Rooms[9].Mesh.Vertices[cut4.Rooms[9].Mesh.Rectangles[40].Vertices[3]].Lighting),
            },
            new()
            {
                RoomIndex = 9,
                VertexIndex = cut4.Rooms[9].Mesh.Rectangles[39].Vertices[0],
                VertexChange = new() { Y = -1024 },
            },
            new()
            {
                RoomIndex = 9,
                VertexIndex = cut4.Rooms[9].Mesh.Rectangles[39].Vertices[1],
                VertexChange = new() { Y = -1024 },
            },
        });

        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 68,
            Vertices = new()
            {
                cut4.Rooms[9].Mesh.Rectangles[40].Vertices[3],
                cut4.Rooms[9].Mesh.Rectangles[40].Vertices[2],
                cut4.Rooms[9].Mesh.Rectangles[39].Vertices[1],
                cut4.Rooms[9].Mesh.Rectangles[39].Vertices[0],
            }
        });

        data.RoomEdits.Add(Reface(cut4, 9, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 30, 40));
        data.RoomEdits.Add(Reface(cut4, 9, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 14, 39));
    }

    private static InjectionData CreateBaseData()
    {
        TR1Level pyramid = _control1.Read($"Resources/{TR1LevelNames.PYRAMID}");
        CreateModelLevel(pyramid, TR1Type.ThorLightning);
        pyramid.SoundEffects.Clear();

        InjectionData data = InjectionData.Create(pyramid, InjectionType.TextureFix, "cut4_textures");
        data.FloorEdits.Add(new()
        {
            RoomIndex = 16,
            Fixes = new()
            {
                new FDTrigItem
                {
                    Item = new()
                    {
                        TypeID = TR1Type.ThorLightning,
                        X = 51712,
                        Y = -20480,
                        Z = 51712,
                        Room = 16,
                        Intensity = -1,
                    },
                }
            },
        });

        return data;
    }
}
