using System.Drawing;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1VilcabambaTextureBuilder : TextureBuilder
{
    public override string ID => "vilcabamba_textures";

    public override List<InjectionData> Build()
    {
        TR1Level vilcabamba = _control1.Read($"Resources/{TR1LevelNames.VILCABAMBA}");
        var data = CreateBaseData();

        data.RoomEdits.AddRange(CreateVertexShifts(vilcabamba));
        data.RoomEdits.AddRange(CreateShifts(vilcabamba));
        data.RoomEdits.AddRange(CreateFillers(vilcabamba));
        data.RoomEdits.AddRange(CreateRefacings());
        data.RoomEdits.AddRange(CreateRotations());

        FixBatTransparency(vilcabamba, data);
        FixWolfTransparency(vilcabamba, data);
        FixPassport(vilcabamba, data);

        return new() { data };
    }

    private static List<TRRoomVertexMove> CreateVertexShifts(TR1Level level)
    {
        return new()
        {
            new()
            {
                RoomIndex = 41,
                VertexIndex = level.Rooms[41].Mesh.Rectangles[73].Vertices[1],
                VertexChange = new() { Y = -512 }
            },
            new()
            {
                RoomIndex = 41,
                VertexIndex = level.Rooms[41].Mesh.Rectangles[73].Vertices[2],
                VertexChange = new() { Y = -512 }
            },
            new()
            {
                RoomIndex = 36,
                VertexIndex = level.Rooms[36].Mesh.Rectangles[187].Vertices[0],
                VertexChange = new() { Y = -512 }
            },
            new()
            {
                RoomIndex = 36,
                VertexIndex = level.Rooms[36].Mesh.Rectangles[187].Vertices[0],
                VertexChange = new(),
                ShadeChange = 2048,
            },
        };
    }

    private static List<TRRoomTextureEdit> CreateShifts(TR1Level level)
    {
        var vtx = level.Rooms[36].Mesh.Vertices[level.Rooms[36].Mesh.Rectangles[187].Vertices[1]];
        vtx.Vertex.Y -= 512;

        return new()
        {
            new TRRoomVertexCreate
            {
                RoomIndex = 36,
                Vertex = vtx,
            },
            new TRRoomTextureMove
            {
                RoomIndex = 36,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 187,
                VertexRemap = new()
                {
                    new()
                    {
                        Index = 1,
                        NewVertexIndex = (ushort)level.Rooms[36].Mesh.Vertices.Count,
                    },
                }
            },
            new TRRoomTextureMove
            {
                RoomIndex = 36,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 185,
                VertexRemap = new()
                {
                    new()
                    {
                        Index = 0,
                        NewVertexIndex = (ushort)level.Rooms[36].Mesh.Vertices.Count,
                    },
                }
            }
        };
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR1Level vilcabamba)
    {
        return new()
        {
            new()
            {
                RoomIndex = 15,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 15,
                SourceIndex = 22,
                Vertices = new()
                {
                    vilcabamba.Rooms[15].Mesh.Rectangles[163].Vertices[1],
                    vilcabamba.Rooms[15].Mesh.Rectangles[163].Vertices[0],
                    vilcabamba.Rooms[15].Mesh.Rectangles[166].Vertices[0],
                },
            },
            new()
            {
                RoomIndex = 21,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 5,
                Vertices = new()
                {
                    vilcabamba.Rooms[21].Mesh.Rectangles[4].Vertices[3],
                    vilcabamba.Rooms[21].Mesh.Rectangles[4].Vertices[2],
                    vilcabamba.Rooms[21].Mesh.Rectangles[5].Vertices[1],
                    vilcabamba.Rooms[21].Mesh.Rectangles[5].Vertices[0],
                },
            },
            new()
            {
                RoomIndex = 36,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = GetSource(vilcabamba, TRMeshFaceType.TexturedTriangle, 98).Room,
                SourceIndex = GetSource(vilcabamba, TRMeshFaceType.TexturedTriangle, 98).Face,
                Vertices = new()
                {
                    vilcabamba.Rooms[36].Mesh.Rectangles[187].Vertices[1],
                    vilcabamba.Rooms[36].Mesh.Rectangles[187].Vertices[2],
                    (ushort)vilcabamba.Rooms[36].Mesh.Vertices.Count,                    
                },
            },
            new()
            {
                RoomIndex = 36,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = GetSource(vilcabamba, TRMeshFaceType.TexturedTriangle, 76).Room,
                SourceIndex = GetSource(vilcabamba, TRMeshFaceType.TexturedTriangle, 76).Face,
                Vertices = new()
                {
                    (ushort)vilcabamba.Rooms[36].Mesh.Vertices.Count,
                    vilcabamba.Rooms[36].Mesh.Rectangles[185].Vertices[3],
                    vilcabamba.Rooms[36].Mesh.Rectangles[185].Vertices[0],                    
                },
            },
        };
    }

    private static List<TRRoomTextureReface> CreateRefacings()
    {
        return new()
        {
            new()
            {
                RoomIndex = 26,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 26,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 425,
                TargetIndex = 403,
            },
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(15, TRMeshFaceType.TexturedQuad, 191, 3),
            Rotate(26, TRMeshFaceType.TexturedTriangle, 0, 1),
            Rotate(73, TRMeshFaceType.TexturedQuad, 3, 3),
        };
    }

    private InjectionData CreateBaseData()
    {
        var level = _control1.Read($"Resources/{TR1LevelNames.VILCABAMBA}");
        var statue = level.StaticMeshes[TR1Type.SceneryBase + 30];

        var packer = new TR1TexturePacker(level);
        var regions = packer.GetMeshRegions(new[] { statue.Mesh })
            .Values.SelectMany(v => v);
        var originalInfos = level.ObjectTextures.ToList();
        ResetLevel(level, 1);

        packer = new(level);
        packer.AddRectangles(regions);
        packer.Pack(true);

        level.StaticMeshes[TR1Type.SceneryBase + 30] = statue;
        level.ObjectTextures.AddRange(regions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));
        statue.Mesh.TexturedFaces.ToList()
            .ForEach(f =>
            {
                f.Texture = (ushort)level.ObjectTextures.IndexOf(originalInfos[f.Texture]);
            });

        statue.Mesh.TexturedRectangles.Add(new()
        {
            Type = TRFaceType.Rectangle,
            Texture = statue.Mesh.TexturedRectangles[9].Texture,
            Vertices = new() { 20, 23, 22, 21 }
        });

        var img = GetImage(statue.Mesh.TexturedRectangles[13].Texture, level);
        img.Write((c, x, y) => c.A == 0 ? Color.FromArgb(52, 52, 40) : c);
        ImportImage(statue.Mesh.TexturedRectangles[13].Texture, img, level);

        var data = InjectionData.Create(level, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR1LevelNames.VILCABAMBA);

        return data;
    }
}
