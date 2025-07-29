using System.Drawing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1CatTextureBuilder : TextureBuilder
{
    public override string ID => "cat_textures";

    public override List<InjectionData> Build()
    {
        TR1Level cat = _control1.Read($"Resources/{TR1LevelNames.CAT}");
        InjectionData data = CreateBaseData();

        data.RoomEdits.AddRange(CreateFillers(cat));
        data.RoomEdits.AddRange(CreateRefacings(cat));
        data.RoomEdits.AddRange(CreateRotations());
        data.RoomEdits.AddRange(FixVases(cat));
        data.RoomEdits.AddRange(FixCatPositions(cat));

        FixTransparentTextures(cat, data);
        FixPassport(cat, data);

        return new() { data };
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR1Level cat)
    {
        return new()
        {
            new()
            {
                RoomIndex = 75,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 75,
                SourceIndex = 4,
                Vertices = new()
                {
                    cat.Rooms[75].Mesh.Rectangles[196].Vertices[2],
                    cat.Rooms[75].Mesh.Rectangles[174].Vertices[3],
                    cat.Rooms[75].Mesh.Rectangles[196].Vertices[3],
                }
            },
        };
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR1Level cat)
    {
        return new()
        {
            new()
            {
                RoomIndex = 50,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 50,
                SourceIndex = 56,
                TargetIndex = 81,
            },
            new()
            {
                RoomIndex = 71,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 76,
                SourceIndex = 81,
                TargetIndex = 76,
            },
            new()
            {
                RoomIndex = 87,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 87,
                SourceIndex = 47,
                TargetIndex = 100,
            },
            Reface(cat, 32, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 45, 254),
            Reface(cat, 98, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedQuad, 16, 2),
            Reface(cat, 98, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 17, 72),
            Reface(cat, 98, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 208, 74),
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(70, TRMeshFaceType.TexturedQuad, 98, 3),
            Rotate(78, TRMeshFaceType.TexturedQuad, 112, 3),
            Rotate(71, TRMeshFaceType.TexturedQuad, 73, 3),
            Rotate(76, TRMeshFaceType.TexturedQuad, 78, 3),
            Rotate(96, TRMeshFaceType.TexturedQuad, 5, 3),
            Rotate(96, TRMeshFaceType.TexturedQuad, 73, 3),
            Rotate(96, TRMeshFaceType.TexturedQuad, 27, 1),
        };
    }

    private static IEnumerable<TRRoomVertexMove> FixVases(TR1Level cat)
    {
        return cat.Rooms
            .SelectMany(r => r.Mesh.Sprites.Where(s => s.ID == TR1Type.Debris0).Select(s => new TRRoomVertexMove
            {
                RoomIndex = (short)cat.Rooms.IndexOf(r),
                VertexIndex = (ushort)s.Vertex,
                VertexChange = new() { Y = -27 },
            }));
    }

    private static IEnumerable<TRRoomStatic3DEdit> FixCatPositions(TR1Level cat)
    {
        var map = new Dictionary<short, short>
        {
            [6] = 0,
            [23] = 0,
        };
        foreach (var (room, mesh) in map)
        {
            cat.Rooms[room].StaticMeshes[mesh].Y += 128;
        }

        return map.Select(kvp => new TRRoomStatic3DEdit
        {
            RoomIndex = kvp.Key,
            MeshIndex = kvp.Value,
            StaticMesh = cat.Rooms[kvp.Key].StaticMeshes[kvp.Value],
        });
    }

    private static void FixTransparentTextures(TR1Level cat, InjectionData data)
    {
        FixTransparentPixels(cat, data,
            cat.Rooms[2].Mesh.Rectangles[128], Color.FromArgb(188, 140, 64));
        FixTransparentPixels(cat, data,
            cat.Rooms[7].Mesh.Rectangles[19], Color.FromArgb(188, 140, 64));
        FixTransparentPixels(cat, data,
            cat.Rooms[95].Mesh.Rectangles[7], Color.FromArgb(252, 228, 140));
    }

    private InjectionData CreateBaseData()
    {
        var level = _control1.Read($"Resources/{TR1LevelNames.CAT}");
        ResetLevel(level, 1);
        FixCatStatue(level);

        var data = InjectionData.Create(level, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR1LevelNames.CAT);

        return data;
    }
}
