using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2CatacombsTextureBuilder : TextureBuilder
{
    public override string ID => "catacombs_textures";

    public override List<InjectionData> Build()
    {
        TR2Level level = _control2.Read($"Resources/{TR2LevelNames.COT}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.COT);

        data.RoomEdits.AddRange(CreateVertexShifts(level));
        data.RoomEdits.AddRange(CreateFillers(level));
        data.RoomEdits.AddRange(CreateRefacings(level));
        data.RoomEdits.AddRange(CreateRotations());
        data.RoomEdits.AddRange(FixYetiRoom(level));

        FixPassport(level, data);

        return new() { data };
    }

    private static List<TRRoomVertexMove> CreateVertexShifts(TR2Level level)
    {
        return new()
        {
            new()
            {
                RoomIndex = 40,
                VertexIndex = level.Rooms[40].Mesh.Rectangles[133].Vertices[0],
                VertexChange = new() { Y = 256 },
            },
            new()
            {
                RoomIndex = 40,
                VertexIndex = level.Rooms[40].Mesh.Rectangles[133].Vertices[3],
                VertexChange = new() { Y = 256 },
            },
        };
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR2Level level)
    {
        return new()
        {
            new()
            {
                RoomIndex = 40,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 40,
                SourceIndex = 16,
                Vertices = new()
                {
                    level.Rooms[40].Mesh.Rectangles[124].Vertices[3],
                    level.Rooms[40].Mesh.Rectangles[137].Vertices[3],
                    level.Rooms[40].Mesh.Rectangles[137].Vertices[2],
                }
            },
        };
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR2Level level)
    {
        return new()
        {
            Reface(level, 40, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1251, 136),
            Reface(level, 40, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1251, 140),
            Reface(level, 40, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1251, 144),
            Reface(level, 40, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1251, 147),
            Reface(level, 40, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1251, 150),
            Reface(level, 56, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1313, 150),
            Reface(level, 79, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1313, 150),
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(11, TRMeshFaceType.TexturedQuad, 213, 3),
            Rotate(56, TRMeshFaceType.TexturedQuad, 150, 3),
            Rotate(79, TRMeshFaceType.TexturedQuad, 150, 3),
        };
    }

    private static IEnumerable<TRRoomTextureEdit> FixYetiRoom(TR2Level level)
    {
        var room = level.Rooms[41];
        foreach (var root in new[] { 110, 90, 106, 86, 102, 82, 98, 77 })
        {
            // Face removal isn't feasible/manageable so we hide these redundant
            // ones inside the adjacent blocks.
            var adj = room.Mesh.Rectangles[root + 2 + (root & 1)];
            yield return CreateQuadShift(41, (short)root, new()
            {
                new(2, adj.Vertices[1]),
                new(3, adj.Vertices[0]),
            });
        }

        const short roomIdx = 53;
        room = level.Rooms[roomIdx];

        var map = new List<(short Left, short Right, ushort Base)>
        {
            (187, 146, 148),
            (190, 151, 153),
            (193, 157, 159),
            (196, 163, 165),
            (199, 169, 171),
        };
        for (int i = 0; i < map.Count; i++)
        {
            var (leftIdx, rightIdx, baseIdx) = map[i];
            var baseLighting = room.Mesh.Vertices[room.Mesh.Rectangles[baseIdx].Vertices[3]].Lighting;

            if (i > 0)
            {
                foreach (var root in new[] { rightIdx, leftIdx })
                {
                    yield return CreateVertex(roomIdx, room,
                        room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[0]], baseLighting);
                    yield return SetLastVertexFlags(roomIdx, room);
                    yield return CreateFace(roomIdx, roomIdx, baseIdx, TRMeshFaceType.TexturedQuad, new[]
                    {
                        room.Mesh.Rectangles[root].Vertices[1],
                        room.Mesh.Rectangles[root].Vertices[0],
                        (ushort)(room.Mesh.Vertices.Count - 1),
                        root == rightIdx
                            ? room.Mesh.Rectangles[baseIdx].Vertices[2]
                            : (ushort)(room.Mesh.Vertices.Count - 2),
                    });
                }

                yield return CreateFace(roomIdx, roomIdx, baseIdx, TRMeshFaceType.TexturedQuad, new[]
                {
                    room.Mesh.Rectangles[leftIdx].Vertices[0],
                    room.Mesh.Rectangles[map[i - 1].Left].Vertices[3],
                    (ushort)(room.Mesh.Vertices.Count - 3),
                    (ushort)(room.Mesh.Vertices.Count - 1),
                });
            }

            if (i == map.Count - 1)
            {
                break;
            }

            foreach (var root in new[] { rightIdx, leftIdx })
            {
                yield return CreateVertex(roomIdx, room,
                    room.Mesh.Vertices[room.Mesh.Rectangles[root].Vertices[3]], baseLighting);
                yield return SetLastVertexFlags(roomIdx, room);
                yield return CreateFace(roomIdx, roomIdx, baseIdx, TRMeshFaceType.TexturedQuad, new[]
                {
                    room.Mesh.Rectangles[root].Vertices[3],
                    room.Mesh.Rectangles[root].Vertices[2],
                    root == rightIdx
                        ? room.Mesh.Rectangles[baseIdx].Vertices[3]
                        : (ushort)(room.Mesh.Vertices.Count - 2),
                    (ushort)(room.Mesh.Vertices.Count - 1),
                });
            }
        }
    }
}
