using System.Drawing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2VeniceTextureBuilder : TextureBuilder
{
    public override string ID => "venice_textures";

    public override List<InjectionData> Build()
    {
        TR2Level venice = _control2.Read($"Resources/{TR2LevelNames.VENICE}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.VENICE);

        data.RoomEdits.AddRange(CreateFillers(venice));
        data.RoomEdits.AddRange(CreateRefacings(venice));
        data.RoomEdits.AddRange(CreateRotations());
        data.RoomEdits.AddRange(FixPolePositions(venice));
        data.MeshEdits.Add(
            FixStaticMeshPosition(venice.StaticMeshes, TR2Type.Architecture4, new() { Z = 32 }));
        FixTransparentTextures(venice, data);
        FixPassport(venice, data);
        FixPushButton(data);

        return [data];
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR2Level level)
    {
        return
        [
            new()
            {
                RoomIndex = 42,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 43,
                SourceIndex = 46,
                Vertices =
                [
                    level.Rooms[42].Mesh.Rectangles[1].Vertices[3],
                    level.Rooms[42].Mesh.Rectangles[1].Vertices[2],
                    level.Rooms[42].Mesh.Rectangles[0].Vertices[1],
                    level.Rooms[42].Mesh.Rectangles[0].Vertices[0],
                ]
            },
            new()
            {
                RoomIndex = 42,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 43,
                SourceIndex = 48,
                Vertices =
                [
                    level.Rooms[42].Mesh.Rectangles[4].Vertices[3],
                    level.Rooms[42].Mesh.Rectangles[4].Vertices[2],
                    level.Rooms[42].Mesh.Rectangles[3].Vertices[1],
                    level.Rooms[42].Mesh.Rectangles[3].Vertices[0],
                ]
            },
            new()
            {
                RoomIndex = 42,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 43,
                SourceIndex = 50,
                Vertices =
                [
                    level.Rooms[42].Mesh.Rectangles[6].Vertices[3],
                    level.Rooms[42].Mesh.Rectangles[6].Vertices[2],
                    level.Rooms[42].Mesh.Rectangles[5].Vertices[1],
                    level.Rooms[42].Mesh.Rectangles[5].Vertices[0],
                ]
            },
        ];
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR2Level level)
    {
        return
        [
            Reface(level, 36, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1574, 0),
            Reface(level, 36, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1574, 5),
            Reface(level, 42, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1599, 1),
            Reface(level, 42, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1600, 4),
            Reface(level, 42, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1601, 6),
            Reface(level, 77, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1513, 65),
        ];
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return
        [
            Rotate(36, TRMeshFaceType.TexturedQuad, 22, 2),
            Rotate(36, TRMeshFaceType.TexturedQuad, 88, 2),
        ];
    }

    private static void FixTransparentTextures(TR2Level level, InjectionData data)
    {
        FixTransparentPixels(level, data,
            level.Rooms[0].Mesh.Rectangles[56], Color.FromArgb(139, 131, 41));
    }

    private static List<TRRoomTextureEdit> FixPolePositions(TR2Level level)
    {
        var mesh = level.Rooms[11].StaticMeshes[0];
        return
        [
            new TRRoomStatic3DCreate
            {
                RoomIndex = 11,
                ID = 21,
                StaticMesh = new()
                {
                    Angle = mesh.Angle,
                    X = mesh.X,
                    Y = mesh.Y + 1030,
                    Z = mesh.Z,
                    Intensity = mesh.Intensity1,
                },
            },

            .. _gondolaPoleShifts.Select(s => ShiftStatic(level, s)),
        ];
    }

    protected static readonly List<StaticShift> _gondolaPoleShifts =
    [
        new(11, 0, new TRVertex32 { Y = 4 }),

        new(19, 4, new TRVertex32 { Y = -2 }),
        new(19, 5, new TRVertex32 { Y = -6 }),

        new(74, 4, new TRVertex32 { Y = -390 }),
        new(74, 5, new TRVertex32 { Y = -2 }),

        new(106, 0, new TRVertex32 { Y = 4 }),
        new(106, 1, new TRVertex32 { Y = 522 }),
    ];
}
