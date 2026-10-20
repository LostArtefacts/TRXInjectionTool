using System.Drawing;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Applicability;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR4.Textures;

public class TR4BurialTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = CreateBaseData();
        var level = _control4.Read($"Resources/TR4/{TR4LevelNames.BURIAL}");
        data.RoomEdits.AddRange(CreateShifts(level));
        data.RoomEdits.AddRange(CreateFillers(level));
        data.RoomEdits.AddRange(CreateRefacings(level));
        data.RoomEdits.AddRange(CreateRotations());
        data.RoomEdits.AddRange(FixRoom114(level));

        FixMummyDoor(level, data);

        return [data];
    }

    private static IEnumerable<TRRoomTextureMove> CreateShifts(TR4Level level)
    {
        yield return CreateQuadShift(58, 148,
        [
            new()
            {
                Index = 1,
                NewVertexIndex = level.Rooms[58].Mesh.Rectangles[169].Vertices[2],
            },
            new()
            {
                Index = 2,
                NewVertexIndex = level.Rooms[58].Mesh.Rectangles[124].Vertices[3],
            },
        ]);
        
    }

    private static IEnumerable<TRRoomTextureCreate> CreateFillers(TR4Level level)
    {
        yield return CreateFace(58, 58, 124, TRMeshFaceType.TexturedQuad,
        [
            level.Rooms[58].Mesh.Rectangles[146].Vertices[0],
            level.Rooms[58].Mesh.Rectangles[146].Vertices[3],
            level.Rooms[58].Mesh.Rectangles[124].Vertices[3],
            level.Rooms[58].Mesh.Rectangles[169].Vertices[2],
        ]);
    }

    private static IEnumerable<TRRoomTextureReface> CreateRefacings(TR4Level level)
    {
        yield return Reface(level, 12, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 2192, 5);
        yield return Reface(level, 88, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 2191, 1);
        yield return Reface(level, 114, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 2208, 85); 
    }

    private static IEnumerable<TRRoomTextureRotate> CreateRotations()
    {
        yield return Rotate(12, TRMeshFaceType.TexturedQuad, 5, 3);
        yield return Rotate(88, TRMeshFaceType.TexturedQuad, 1, 1);
    }

    private static IEnumerable<TRRoomTextureEdit> FixRoom114(TR4Level level)
    {
        const short roomIdx = 114;
        var room = level.Rooms[roomIdx];
        var vtxPos = new List<ushort>();

        TRRoomVertexCreate MakeVertex(int faceIdx, int vertIdx)
        {
            var vtx = room.Mesh.Vertices[room.Mesh.Rectangles[faceIdx].Vertices[vertIdx]];
            vtxPos.Add((ushort)room.Mesh.Vertices.Count);
            var vertex = CreateVertex(roomIdx, room, vtx, shift: -TRConsts.Step1);
            return vertex;
        }

        yield return MakeVertex(85, 2);

        yield return new TRRoomVertexMove
        {
            RoomIndex = roomIdx,
            VertexIndex = room.Mesh.Rectangles[83].Vertices[0],
            VertexChange = new() { Y = -TRConsts.Step1 },
        };

        yield return CreateQuadShift(roomIdx, 82,
        [
            new()
            {
                Index = 2,
                NewVertexIndex = room.Mesh.Rectangles[81].Vertices[0],
            },
        ]);
        yield return CreateQuadShift(roomIdx, 83,
        [
            new()
            {
                Index = 1,
                NewVertexIndex = room.Mesh.Rectangles[81].Vertices[0],
            },
        ]);
        yield return CreateQuadShift(roomIdx, 85,
        [
            new()
            {
                Index = 2,
                NewVertexIndex = vtxPos[0],
            },
            new()
            {
                Index = 3,
                NewVertexIndex = room.Mesh.Rectangles[86].Vertices[3],
            },
        ]);
        yield return CreateQuadShift(roomIdx, 83,
        [
            new()
            {
                Index = 2,
                NewVertexIndex = room.Mesh.Rectangles[80].Vertices[2],
            },
            new()
            {
                Index = 3,
                NewVertexIndex = vtxPos[0],
            },
        ]);

        var source = GetSource(level, TRMeshFaceType.TexturedQuad, 2366);
        yield return CreateFace(roomIdx, source.Room, (ushort)source.Face, TRMeshFaceType.TexturedQuad,
        [
            room.Mesh.Rectangles[85].Vertices[2],
            room.Mesh.Rectangles[85].Vertices[3],
            room.Mesh.Rectangles[86].Vertices[3],
            vtxPos[0],
        ]);
        yield return CreateFace(roomIdx, source.Room, (ushort)source.Face, TRMeshFaceType.TexturedQuad,
        [
            room.Mesh.Rectangles[83].Vertices[2],
            room.Mesh.Rectangles[83].Vertices[3],
            vtxPos[0],
            room.Mesh.Rectangles[80].Vertices[2],
        ]);
    }

    private static void FixMummyDoor(TR4Level level, InjectionData data)
    {
        const int textureID = 2260;
        var texInfo = level.ObjectTextures[textureID];
        var tile = new TRImage(level.Images.GetImage32(texInfo.Atlas).Pixels);
        var img = tile.Export(texInfo.Bounds);

        var magenta = new[]
        {
            Color.FromArgb(253, 3, 249),
            Color.FromArgb(253, 3, 250),
        };
        img.Write((c, x, y) => magenta.Contains(c) ? Color.Transparent : c);

        data.TextureOverwrites.Add(new()
        {
            Page = texInfo.Atlas,
            X = (byte)texInfo.Bounds.X,
            Y = (byte)texInfo.Bounds.Y,
            Width = (ushort)texInfo.Size.Width,
            Height = (ushort)texInfo.Size.Height,
            Data = img.ToRGBA(),
        });

        data.ApplicabilityTests.Add(new TextureTest(textureID, texInfo));
    }

    private static InjectionData CreateBaseData()
    {
        var level = _control4.Read($"Resources/TR4/{TR4LevelNames.BURIAL}");
        var stairs = level.Models[TR4Type.Animating6];

        var packer = new TR4TexturePacker(level, TRGroupPackingMode.All);
        var regions = packer.GetMeshRegions(stairs.Meshes)
            .Values.SelectMany(v => v);
        var originalInfos = level.ObjectTextures.ToList();

        var jungle = _control3.Read($"Resources/TR3/{TR3LevelNames.JUNGLE}");
        ResetLevel(jungle, 1);
        var packer3 = new TR3TexturePacker(jungle);
        packer3.AddRectangles(regions);
        packer3.Pack(true);

        jungle.ObjectTextures.AddRange(regions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));
        stairs.Meshes
            .SelectMany(m => m.TexturedFaces)
            .Distinct()
            .ToList()
            .ForEach(f =>
            {
                f.Texture = (ushort)jungle.ObjectTextures.IndexOf(originalInfos[f.Texture]);
            });

        ResetLevel(level);
        level.Models[TR4Type.Animating6] = stairs;
        level.ObjectTextures.AddRange(regions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));

        var image = new TRImage(jungle.Images16[0].Pixels);
        level.Images.Objects.Images16.Add(new() { Pixels = image.ToRGB555() });
        level.Images.Objects.Images32.Add(new() { Pixels = image.ToRGB32() });

        foreach (var face in stairs.Meshes.SelectMany(m => m.TexturedFaces))
        {
            face.UseAlphaBlending = false;
        }

        var data = InjectionData.Create(level, InjectionType.TextureFix, "burial_textures");
        CreateDefaultTests(data, $"TR4/{TR4LevelNames.BURIAL}");
        return data;
    }
}
