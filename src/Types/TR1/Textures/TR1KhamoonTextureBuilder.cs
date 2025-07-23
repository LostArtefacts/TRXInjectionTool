using System.Drawing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1KhamoonTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level khamoon = _control1.Read($"Resources/{TR1LevelNames.KHAMOON}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.TextureFix, "khamoon_textures");
        CreateDefaultTests(data, TR1LevelNames.KHAMOON);

        data.RoomEdits.AddRange(CreateFillers(khamoon));
        data.RoomEdits.AddRange(CreateRefacings());
        data.RoomEdits.AddRange(CreateRotations());
        data.RoomEdits.AddRange(CreateVertexShifts(khamoon));

        FixTransparentTextures(khamoon, data);
        FixPassport(khamoon, data);

        return new() { data };
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR1Level khamoon)
    {
        return new()
        {
            new()
            {
                RoomIndex = 58,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = GetSource(khamoon, TRMeshFaceType.TexturedTriangle, 240).Room,
                SourceIndex = GetSource(khamoon, TRMeshFaceType.TexturedTriangle, 240).Face,
                Vertices = new()
                {
                    khamoon.Rooms[58].Mesh.Rectangles[13].Vertices[3],
                    khamoon.Rooms[58].Mesh.Rectangles[21].Vertices[3],
                    khamoon.Rooms[58].Mesh.Rectangles[21].Vertices[2],
                }
            },
        };
    }

    private static List<TRRoomTextureReface> CreateRefacings()
    {
        return new()
        {
            new()
            {
                RoomIndex = 47,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 47,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 17,
                TargetIndex = 20
            },
            new()
            {
                RoomIndex = 48,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 48,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 24,
                TargetIndex = 1
            },
            new()
            {
                RoomIndex = 60,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 54,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 78,
                TargetIndex = 78
            }
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(51, TRMeshFaceType.TexturedQuad, 16, 1),
            Rotate(64, TRMeshFaceType.TexturedQuad, 16, 1),
            Rotate(48, TRMeshFaceType.TexturedQuad, 1, 3),
        };
    }

    private static List<TRRoomVertexMove> CreateVertexShifts(TR1Level khamoon)
    {
        return new()
        {
            new()
            {
                RoomIndex = 60,
                VertexIndex = khamoon.Rooms[60].Mesh.Rectangles[108].Vertices[0],
                VertexChange = new() { Y = -768 }
            },
            new()
            {
                RoomIndex = 60,
                VertexIndex = khamoon.Rooms[60].Mesh.Rectangles[108].Vertices[1],
                VertexChange = new() { Y = -768 }
            }
        };
    }

    private static void FixTransparentTextures(TR1Level khamoon, InjectionData data)
    {
        FixTransparentPixels(khamoon, data,
            khamoon.Rooms[20].Mesh.Rectangles[77], Color.FromArgb(188, 140, 64));
        FixTransparentPixels(khamoon, data,
            khamoon.Rooms[20].Mesh.Rectangles[78], Color.FromArgb(188, 140, 64));
    }
}
