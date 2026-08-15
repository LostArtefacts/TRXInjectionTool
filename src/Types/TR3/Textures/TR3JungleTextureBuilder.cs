using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Textures;

public class TR3JungleTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = CreateBaseData();
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.JUNGLE}");
        CreateDefaultTests(data, $"TR3/{TR3LevelNames.JUNGLE}");
        data.RoomEdits.AddRange(FixDoubleSided());
        data.RoomEdits.AddRange(CreateRotations());
        data.RoomEdits.AddRange(CreateRefacings(level));
        data.RoomEdits.AddRange(FixPit87(level));
        data.RoomEdits.AddRange(FixLadder122(level));

        return [data];
    }

    private static IEnumerable<TRRoomTextureEdit> FixDoubleSided()
    {
        // Fix z-fighting in the canopy near the first secret
        foreach (var face in new short[] { 107, 109, 111, 132, 134, 152, 154, 158, 175, 177, 197 })
        {
            yield return DoubleSided(4, TRMeshFaceType.TexturedQuad, face, false);
        }
        foreach (var face in new short[] { 99, 117 })
        {
            yield return DoubleSided(4, TRMeshFaceType.TexturedTriangle, face, false);
        }

        // More z-fighting in room 59
        foreach (var face in new short[] { 1, 2 })
        {
            yield return DoubleSided(59, TRMeshFaceType.TexturedTriangle, face, false);
        }
    }

    private static IEnumerable<TRRoomTextureEdit> CreateRotations()
    {
        yield return Rotate(19, TRMeshFaceType.TexturedTriangle, 0, 1);
        yield return Rotate(77, TRMeshFaceType.TexturedTriangle, 0, 1);
    }

    private static IEnumerable<TRRoomTextureEdit> CreateRefacings(TR3Level level)
    {
        yield return Reface(level, 134, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedQuad, 1620, 30);
    }

    private static IEnumerable<TRRoomTextureEdit> FixPit87(TR3Level level)
    {
        // The pit in room 87 has broken lighting attributes. Simplest to recreate and remap the faces.
        const short roomIdx = 87;
        var room = level.Rooms[roomIdx];
        var mesh = room.Mesh;
        var vtxMap = new Dictionary<ushort, ushort>();

        foreach (var vertIdx in mesh.Rectangles[0].Vertices)
        {
            var vtx = room.Mesh.Vertices[vertIdx];
            vtxMap[vertIdx] = (ushort)room.Mesh.Vertices.Count;
            yield return CreateVertex(roomIdx, room, vtx, shift: 0);
        }

        for (short i = 0; i < mesh.Rectangles.Count; i++)
        {
            var face = mesh.Rectangles[i];
            var verts = face.Vertices.FindAll(vtxMap.ContainsKey);
            if (verts.Count == 0)
            {
                continue;
            }

            yield return new TRRoomTextureMove
            {
                RoomIndex = roomIdx,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = i,
                VertexRemap = [.. verts.Select(v => new TRRoomVertexRemap
                {
                    Index = (short)face.Vertices.IndexOf(v),
                    NewVertexIndex = vtxMap[v],
                })],
            };
        }
    }

    private static IEnumerable<TRRoomTextureEdit> FixLadder122(TR3Level level)
    {
        var mesh = level.Rooms[121].StaticMeshes[0].Clone();
        yield return new TRRoomStatic3DCreate
        {
            RoomIndex = 121,
            ID = 13,
            StaticMesh = new()
            {
                X = mesh.X,
                Y = mesh.Y,
                Z = mesh.Z,
                Angle = mesh.Angle,
                Intensity = mesh.Colour,
            },
        };

        mesh.Y = level.Rooms[121].StaticMeshes[1].Y;
        yield return new TRRoomStatic3DEdit
        {
            RoomIndex = 121,
            MeshIndex = 0,
            StaticMesh = new()
            {
                X = mesh.X,
                Y = mesh.Y,
                Z = mesh.Z,
                Angle = mesh.Angle,
                Intensity = mesh.Colour,
            },
        };
    }

    private static InjectionData CreateBaseData()
    {
        // Make a half-sized ladder clone to fix z-fighting in room 122
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.JUNGLE}");
        var ladder = level.StaticMeshes[TR3Type.SceneryBase + 10];

        var packer = new TR3TexturePacker(level);
        var regions = packer.GetMeshRegions([ladder.Mesh])
            .Values.SelectMany(v => v).ToList();
        var originalInfos = level.ObjectTextures.ToList();

        ResetLevel(level, 1);
        packer = new(level);
        packer.AddRectangles(regions);
        packer.Pack(true);
        GenerateImages8(level, [.. level.Palette.Select(c => c.ToTR1Color())]);

        level.StaticMeshes[TR3Type.SceneryBase + 13] = ladder;
        level.ObjectTextures.AddRange(regions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));
        ladder.Mesh.TexturedFaces
            .ToList().ForEach(f =>
            {
                f.Texture = (ushort)level.ObjectTextures.IndexOf(originalInfos[f.Texture]);
            });

        var texInfo = level.ObjectTextures[2];
        texInfo.Size = new(texInfo.Size.Width / 2, texInfo.Size.Height);
        var halfY = ladder.Mesh.Vertices[9].Y;
        foreach (var vert in new[] { 18, 20, 21, 24, 26, 27 })
        {
            ladder.Mesh.Vertices[vert].Y = halfY;
        }

        ladder.Mesh.TexturedTriangles.RemoveAll(t => t.Vertices.All(v => ladder.Mesh.Vertices[v].Y >= halfY));
        ladder.Mesh.TexturedRectangles.RemoveAll(t => t.Vertices.All(v => ladder.Mesh.Vertices[v].Y >= halfY));

        return InjectionData.Create(level, InjectionType.TextureFix, "jungle_textures");
    }
}
