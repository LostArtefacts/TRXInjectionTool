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
        data.MeshEdits.Add(
            FixStaticMeshPosition(bartoli.StaticMeshes, TR2Type.Architecture4, new() { Z = 27 }));
        FixTransparentTextures(bartoli, data);
        FixPassport(bartoli, data);

        return new() { data };
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR2Level level)
    {
        return new()
        {
            Reface(level, 17, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1573, 3),
            Reface(level, 17, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1573, 4),
            Reface(level, 29, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1665, 46),            
            Reface(level, 46, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1711, 48),            
            Reface(level, 51, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1573, 166),
            Reface(level, 56, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1623, 14),
            Reface(level, 56, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1623, 33),
            Reface(level, 56, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1643, 34),
            Reface(level, 127, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1744, 57),
            Reface(level, 131, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1735, 7),
            Reface(level, 145, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1579, 40),
        };
    }

    private static List<TRRoomVertexMove> CreateVertexShifts(TR2Level level)
    {
        var result = new List<TRRoomVertexMove>
        {
            new()
            {
                RoomIndex = 17,
                VertexIndex = level.Rooms[17].Mesh.Rectangles[4].Vertices[2],
                VertexChange = new() { Y = 1024 },
            },
            new()
            {
                RoomIndex = 17,
                VertexIndex = level.Rooms[17].Mesh.Rectangles[4].Vertices[3],
                VertexChange = new() { Y = 1024 },
            },
            new()
            {
                RoomIndex = 17,
                VertexIndex = level.Rooms[17].Mesh.Rectangles[5].Vertices[2],
                VertexChange = new() { Y = -768 },
            },
            new()
            {
                RoomIndex = 17,
                VertexIndex = level.Rooms[17].Mesh.Rectangles[5].Vertices[3],
                VertexChange = new() { Y = -768 },
            },
            new()
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
        return result;
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(46, TRMeshFaceType.TexturedQuad, 48, 3),
            Rotate(73, TRMeshFaceType.TexturedQuad, 119, 3),
            Rotate(143, TRMeshFaceType.TexturedQuad, 116, 3),
            Rotate(127, TRMeshFaceType.TexturedQuad, 57, 3),
            Rotate(131, TRMeshFaceType.TexturedQuad, 7, 2),
            Rotate(146, TRMeshFaceType.TexturedQuad, 52, 3),
        };
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
}
