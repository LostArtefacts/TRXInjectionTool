using System.Drawing;
using TRImageControl;
using TRImageControl.Packing;
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
        InjectionData data = CreateBaseData();

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

    private InjectionData CreateBaseData()
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.VENICE}");
        var lantern = level.StaticMeshes[TR2Type.SceneryBase + 30];

        var verts = new ushort[] { 10, 11, 14, 15 };
        var face = lantern.Mesh.TexturedRectangles.First(f => f.Vertices.All(verts.Contains));
        lantern.Mesh.TexturedRectangles.Add(new()
        {
            Texture = face.Texture,
            Vertices = [11, 10, 14, 15],
        });

        verts = [10, 11, 17, 18];
        face = lantern.Mesh.TexturedRectangles.First(f => f.Vertices.All(verts.Contains));
        var newVertList = new List<List<ushort>>
        {
            new() { 17, 18, 10, 11 },
            new() { 16, 17, 11, 15 },
            new() { 17, 16, 12, 8 },
            new() { 16, 19, 13, 12 },
            new() { 19, 18, 9, 13 },
            new() { 19, 16, 15, 14 },
            new() { 18, 19, 14, 10 },
        };
        lantern.Mesh.TexturedRectangles.AddRange(newVertList.Select(v => new TRMeshFace
        {
            Type = TRFaceType.Rectangle,
            Texture = face.Texture,
            Vertices = v,
        }));

        var packer = new TR2TexturePacker(level);
        var regions = packer.GetMeshRegions([lantern.Mesh])
            .Values.SelectMany(v => v);
        var originalInfos = level.ObjectTextures.ToList();

        var dataLevel = _control2.Read($"Resources/{TR2LevelNames.VENICE}");
        ResetLevel(dataLevel, 1);
        var levelPacker = new TR2TexturePacker(dataLevel);
        levelPacker.AddRectangles(regions);
        levelPacker.Pack(true);
        dataLevel.StaticMeshes[TR2Type.SceneryBase + 30] = lantern;
        GenerateImages8(dataLevel, dataLevel.Palette.Select(c => c.ToTR1Color()).ToList());

        dataLevel.ObjectTextures.AddRange(regions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));
        lantern.Mesh.TexturedFaces
            .ToList().ForEach(f =>
            {
                f.Texture = (ushort)dataLevel.ObjectTextures.IndexOf(originalInfos[f.Texture]);
            });

        var data = InjectionData.Create(dataLevel, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.VENICE);
        return data;
    }
}
