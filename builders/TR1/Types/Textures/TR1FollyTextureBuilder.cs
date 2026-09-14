using System.Drawing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1FollyTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level folly = _control1.Read($"Resources/{TR1LevelNames.FOLLY}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.TextureFix, "folly_textures");
        CreateDefaultTests(data, TR1LevelNames.FOLLY);

        data.RoomEdits.AddRange(CreateFillers(folly));
        data.RoomEdits.AddRange(CreateRefacings(folly));
        data.RoomEdits.AddRange(CreateRotations());
        data.RoomEdits.AddRange(CreateVertexShifts(folly));
        data.RoomEdits.AddRange(CreateShifts(folly));
        FixTransparentTextures(folly, data);
        FixPassport(folly, data);

        return [data];
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR1Level folly)
    {
        return
        [
            new()
            {
                RoomIndex = 35,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 35,
                SourceIndex = 7,
                Vertices =
                [
                    folly.Rooms[35].Mesh.Rectangles[7].Vertices[1],
                    folly.Rooms[35].Mesh.Rectangles[12].Vertices[0],
                    folly.Rooms[35].Mesh.Rectangles[12].Vertices[3],
                    folly.Rooms[35].Mesh.Rectangles[7].Vertices[2],
                ]
            },
        ];
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR1Level folly)
    {
        return
        [
            new()
            {
                RoomIndex = 18,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 18,
                SourceFaceType = TRMeshFaceType.TexturedTriangle,
                SourceIndex = 4,
                TargetIndex = 0
            },
            new()
            {
                RoomIndex = 35,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 35,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 6,
                TargetIndex = 10
            },
            new()
            {
                RoomIndex = 1,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 1,
                SourceFaceType = TRMeshFaceType.TexturedTriangle,
                SourceIndex = 8,
                TargetIndex = 4
            },
            new()
            {
                RoomIndex = 4,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 4,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 74,
                TargetIndex = 62
            },
            Reface(folly, 23, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 28, 205),
            Reface(folly, 23, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 28, 206),
        ];
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return
        [
            Rotate(1, TRMeshFaceType.TexturedTriangle, 4, 2),
            Rotate(1, TRMeshFaceType.TexturedTriangle, 8, 2),
            Rotate(3, TRMeshFaceType.TexturedQuad, 208, 1),
            Rotate(4, TRMeshFaceType.TexturedQuad, 62, 1),
            Rotate(18, TRMeshFaceType.TexturedQuad, 31, 3),
        ];
    }

    private static List<TRRoomVertexMove> CreateVertexShifts(TR1Level level)
    {
        var meshA = level.Rooms[14].Mesh;
        var meshB = level.Rooms[50].Mesh;

        var min = meshA.Vertices.Min(v => v.Vertex.X);
        var vertsA = meshA.Vertices.Where(v => v.Vertex.X == min);
        var vertsB = meshB.Vertices.FindAll(v => v.Vertex.X == min);

        return [.. vertsA.Select(v =>
        {
            var flipV = vertsB.Find(vt => vt.Vertex.IsEquivalent(v.Vertex));
            return new TRRoomVertexMove
            {
                RoomIndex = 14,
                VertexIndex = (ushort)meshA.Vertices.IndexOf(v),
                ShadeChange = (short)(flipV.Lighting - v.Lighting),
            };
        })];
    }

    private static List<TRRoomTextureMove> CreateShifts(TR1Level folly)
    {
        return
        [
            new()
            {
                RoomIndex = 35,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 10,
                VertexRemap =
                [
                    new()
                    {
                        NewVertexIndex = folly.Rooms[35].Mesh.Rectangles[6].Vertices[1]
                    },
                    new()
                    {
                        Index = 1,
                        NewVertexIndex = folly.Rooms[35].Mesh.Rectangles[11].Vertices[0]
                    }
                ]
            },
        ];
    }

    private static void FixTransparentTextures(TR1Level level, InjectionData data)
    {
        FixTransparentPixels(level, data,
            level.Rooms[25].Mesh.Rectangles[12], Color.FromArgb(112, 96, 72));
    }
}
