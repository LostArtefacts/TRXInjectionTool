using System.Drawing;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2BartoliTextureBuilder : TextureBuilder
{
    public override string ID => "bartoli_textures";

    public override List<InjectionData> Build()
    {
        TR2Level bartoli = _control2.Read($"Resources/{TR2LevelNames.BARTOLI}");
        InjectionData data = CreateBaseData();

        data.RoomEdits.AddRange(CreateRefacings(bartoli));
        data.RoomEdits.AddRange(CreateVertexShifts(bartoli));
        data.RoomEdits.AddRange(CreateRotations());
        data.RoomEdits.AddRange(FixPolePositions(bartoli));
        data.MeshEdits.Add(
            FixStaticMeshPosition(bartoli.StaticMeshes, TR2Type.Architecture4, new() { Z = 32 }));
        FixTransparentTextures(bartoli, data);
        FixPassport(bartoli, data);

        return [data];
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR2Level level)
    {
        return
        [
            Reface(level, 17, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1573, 3),
            Reface(level, 17, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1573, 4),
            Reface(level, 29, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1665, 46),            
            Reface(level, 46, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1711, 48),            
            Reface(level, 51, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1573, 166),
            Reface(level, 56, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1623, 14),
            Reface(level, 56, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1623, 33),
            Reface(level, 56, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1643, 34),
            Reface(level, 121, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1589, 78),
            Reface(level, 121, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1589, 87),
            Reface(level, 121, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1589, 98),
            Reface(level, 121, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1589, 107),
            Reface(level, 121, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1588, 114),
            Reface(level, 121, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1588, 116),
            Reface(level, 121, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1588, 117),
            Reface(level, 127, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1744, 57),
            Reface(level, 131, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1735, 7),
            Reface(level, 145, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1579, 40),
        ];
    }

    private static List<TRRoomTextureEdit> CreateVertexShifts(TR2Level level)
    {
        var vert = level.Rooms[121].Mesh.Vertices[level.Rooms[121].Mesh.Rectangles[13].Vertices[2]];
        vert.Vertex.Y += 256;

        var result = new List<TRRoomTextureEdit>
        {
            new TRRoomVertexCreate
            {
                RoomIndex = 121,
                Vertex = new()
                {
                    Lighting = vert.Lighting,
                    Vertex = vert.Vertex,
                },
            },
            new TRRoomTextureMove
            {
                RoomIndex = 121,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 12,
                VertexRemap =
                [
                    new()
                    {
                        Index = 1,
                        NewVertexIndex = (ushort)level.Rooms[121].Mesh.Vertices.Count,
                    },
                ]
            },
            new TRRoomTextureMove
            {
                RoomIndex = 121,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 13,
                VertexRemap =
                [
                    new()
                    {
                        Index = 2,
                        NewVertexIndex = (ushort)level.Rooms[121].Mesh.Vertices.Count,
                    },
                ]
            },

            new TRRoomVertexMove
            {
                RoomIndex = 17,
                VertexIndex = level.Rooms[17].Mesh.Rectangles[4].Vertices[2],
                VertexChange = new() { Y = 1024 },
            },
            new TRRoomVertexMove
            {
                RoomIndex = 17,
                VertexIndex = level.Rooms[17].Mesh.Rectangles[4].Vertices[3],
                VertexChange = new() { Y = 1024 },
            },
            new TRRoomVertexMove
            {
                RoomIndex = 17,
                VertexIndex = level.Rooms[17].Mesh.Rectangles[5].Vertices[2],
                VertexChange = new() { Y = -768 },
            },
            new TRRoomVertexMove
            {
                RoomIndex = 17,
                VertexIndex = level.Rooms[17].Mesh.Rectangles[5].Vertices[3],
                VertexChange = new() { Y = -768 },
            },
            new TRRoomVertexMove
            {
                RoomIndex = 108,
                VertexIndex = level.Rooms[108].Mesh.Rectangles[9].Vertices[2],
                VertexChange = new() { Y = 256 },
            },
        };

        var columnShifts = new[] { 9, 3, 5, 15, 23, 31, 39, 47, 55, 70, 82, 95, 107, }
            .Select(f => new TRRoomVertexMove
            {
                RoomIndex = 108,
                VertexIndex = level.Rooms[108].Mesh.Rectangles[f].Vertices[3],
                VertexChange = new() { Y = 256 },
            });
        result.AddRange(columnShifts);

        columnShifts = new[] { 13, 10, 7, 2 }
            .Select(f => new TRRoomVertexMove
            {
                RoomIndex = 121,
                VertexIndex = level.Rooms[121].Mesh.Rectangles[f].Vertices[3],
                VertexChange = new() { Y = 256 },
            });
        result.AddRange(columnShifts);

        return result;
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return
        [
            Rotate(46, TRMeshFaceType.TexturedQuad, 48, 3),
            Rotate(73, TRMeshFaceType.TexturedQuad, 119, 3),
            Rotate(143, TRMeshFaceType.TexturedQuad, 116, 3),
            Rotate(121, TRMeshFaceType.TexturedQuad, 114, 2),
            Rotate(121, TRMeshFaceType.TexturedQuad, 116, 2),
            Rotate(121, TRMeshFaceType.TexturedQuad, 117, 2),
            Rotate(127, TRMeshFaceType.TexturedQuad, 57, 3),
            Rotate(131, TRMeshFaceType.TexturedQuad, 7, 2),
            Rotate(146, TRMeshFaceType.TexturedQuad, 52, 3),
        ];
    }

    private InjectionData CreateBaseData()
    {
        TR2Level level = _control2.Read($"Resources/{TR2LevelNames.BARTOLI}");
        var leverStatic = level.StaticMeshes[TR2Type.SceneryBase + 13];
        var palette = level.Palette.Select(c => c.ToTR1Color()).ToList();

        var packer = new TR2TexturePacker(level);
        var regions = packer.GetMeshRegions(new[] { leverStatic.Mesh })
            .Values.SelectMany(v => v);
        var originalInfos = level.ObjectTextures.ToList();
        ResetLevel(level, 1);

        packer = new(level);
        packer.AddRectangles(regions);
        packer.Pack(true);

        level.StaticMeshes[TR2Type.SceneryBase + 13] = leverStatic;
        level.ObjectTextures.AddRange(regions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));
        leverStatic.Mesh.TexturedFaces.ToList()
            .ForEach(f =>
            {
                f.Texture = (ushort)level.ObjectTextures.IndexOf(originalInfos[f.Texture]);
            });
        GenerateImages8(level, palette);

        TRObjectTexture texInfo = null;
        var faces = leverStatic.Mesh.TexturedRectangles.ToList();
        foreach (var face in faces)
        {
            if (texInfo == null)
            {
                texInfo = level.ObjectTextures[face.Texture].Clone();
                texInfo.UVMode = TRUVMode.NE_AntiClockwise;
                level.ObjectTextures.Add(texInfo);
            }
            var altFace = face.Clone();
            altFace.SwapVertices(0, 1);
            altFace.SwapVertices(2, 3);
            altFace.Texture = (ushort)(level.ObjectTextures.Count - 1);
            leverStatic.Mesh.TexturedRectangles.Add(altFace);
        }

        leverStatic.Mesh.Vertices.ForEach(v =>
        {
            v.Z -= 14;
            if (v.Y < -77)
            {
                v.Y -= 10;
            }
            if (v.X < 0)
            {
                v.X -= 10;
            }
        });

        var data = InjectionData.Create(level, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.BARTOLI);

        return data;
    }

    private static void FixTransparentTextures(TR2Level level, InjectionData data)
    {
        FixTransparentPixels(level, data,
            level.Rooms[107].Mesh.Rectangles[60], Color.FromArgb(139, 131, 41));
    }

    private static IEnumerable<TRRoomTextureEdit> FixPolePositions(TR2Level level)
    {
        return _gondolaPoleShifts.Select(s => ShiftStatic(level, s));
    }

    protected static readonly List<StaticShift> _gondolaPoleShifts =
    [
        new(27, 0, new() { Y = -2 }),
        new(27, 1, new() { Y = -2 }),
        new(27, 2, new() { Y = -6 }),
        new(27, 3, new() { Y = -6 }),

        new(28, 0, new() { Y = 4 }),
        new(28, 1, new() { Y = 4 }),

        new(46, 0, new() { Y = -2 }),
        new(46, 1, new() { Y = -2 }),
        new(46, 2, new() { Y = -6 }),
        new(46, 3, new() { Y = -6 }),

        new(73, 0, new() { Y = -2 }),
        new(73, 1, new() { Y = -2 }),
        new(73, 2, new() { Y = -2 }),
        new(73, 3, new() { Y = -6 }),
        new(73, 4, new() { Y = -6 }),
        new(73, 5, new() { Y = -6 }),

        new(87, 0, new() { Y = -2 }),
        new(87, 1, new() { Y = -2 }),
        new(87, 2, new() { Y = -2 }),
        new(87, 3, new() { Y = -6 }),
        new(87, 4, new() { Y = -6 }),
        new(87, 5, new() { Y = -6 }),

        new(106, 0, new() { Y = -6 }),
        new(106, 1, new() { Y = -6 }),
        new(106, 2, new() { Y = -2 }),
        new(106, 3, new() { Y = -2 }),
        new(106, 4, new() { Y = -2 }),
        new(106, 5, new() { Y = -2 }),
        new(106, 6, new() { Y = -6 }),
        new(106, 7, new() { Y = -6 }),

        new(126, 0, new() { Y = 4 }),
        new(126, 1, new() { Y = 4 }),
        new(126, 2, new() { Y = 4 }),

        new(127, 0, new() { Y = 4 }),
        new(127, 1, new() { Y = 4 }),

        new(134, 0, new() { Y = 4 }),
        new(134, 1, new() { Y = 4 }),
        new(134, 2, new() { Y = 4 }),

        new(142, 0, new() { Y = 4 }),
        new(142, 1, new() { Y = 4 }),
        new(142, 2, new() { Y = 4 }),

        new(143, 0, new() { Y = -2 }),
        new(143, 1, new() { Y = -2 }),
        new(143, 2, new() { Y = -2 }),
        new(143, 3, new() { Y = -6 }),
        new(143, 4, new() { Y = -6 }),
        new(143, 5, new() { Y = -6 }),

        new(154, 0, new() { Y = 4 }),
        new(154, 1, new() { Y = 522 }),
        new(154, 2, new() { Y = 4 }),
        new(154, 3, new() { Y = 522 }),
        new(154, 4, new() { Y = 4 }),
        new(154, 5, new() { Y = 4 }),
        new(154, 6, new() { Y = 138 }),
        new(154, 7, new() { Y = 266 }),

        new(163, 4, new() { Y = -6 }),
        new(163, 5, new() { Y = -2 }),

        new(164, 0, new() { Y = 522 }),
        new(164, 1, new() { Y = 522 }),
        new(164, 2, new() { Y = 4 }),
        new(164, 3, new() { Y = 4 }),
        new(164, 4, new() { Y = 266 }),
        new(164, 5, new() { Y = 4 }),
    ];
}
