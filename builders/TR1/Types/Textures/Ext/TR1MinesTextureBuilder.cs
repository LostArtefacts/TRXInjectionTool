using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public partial class TR1MinesTextureBuilder
{
    private static void FixCabinRoom(TR1Level level, InjectionData data)
    {
        // The lowered cabin room has an internal roof that makes no sense. Raise it a click along
        // with all relevant vis portals and vertices, and fill in the gaps.
        foreach (var adjRoom in level.Rooms.Where(r =>
            r.Portals.Any(p => p.Normal.Y == 0 && p.AdjoiningRoom == 94 && p.Vertices.Min(v => v.Y) == -8192)))
        {
            var portal = adjRoom.Portals.Find(p => p.AdjoiningRoom == 94);
            data.VisPortalEdits.Add(new()
            {
                BaseRoom = (short)level.Rooms.IndexOf(adjRoom),
                LinkRoom = 94,
                PortalIndex = (ushort)adjRoom.Portals.IndexOf(portal),
                VertexChanges = [.. Enumerable.Range(0, 4).Select(i => new TRVertex
                {
                    Y = (short)(portal.Vertices[i].Y == -8192 ? -256 : 0),
                })],
            });
        }

        const short roomIdx = 95;
        var room = level.Rooms[roomIdx];
        var mesh = room.Mesh;
        for (ushort i = 0; i < room.Portals.Count; i++)
        {
            var portal = room.Portals[i];
            if (portal.Vertices.Min(v => v.Y) == -8192)
            {
                data.VisPortalEdits.Add(new()
                {
                    BaseRoom = 95,
                    LinkRoom = (short)portal.AdjoiningRoom,
                    PortalIndex = i,
                    VertexChanges = [.. Enumerable.Range(0, 4).Select(i => new TRVertex
                    {
                        Y = (short)(portal.Vertices[i].Y == -8192 ? -256 : 0),
                    })],
                });
            }
        }

        for (ushort x = 1; x < 3; x++)
        {
            for (ushort z = 1; z < 4; z++)
            {
                var sector = TRRoomSectorExt.CloneFrom(room.GetSector(x, z, TRUnit.Sector));
                sector.Ceiling -= 256;
                data.FloorEdits.Add(new()
                {
                    RoomIndex = roomIdx,
                    X = x,
                    Z = z,
                    Fixes =
                    [
                        new FDSectorOverwrite { Sector = sector },
                    ]
                });
            }
        }

        {
            var root = 17;
            data.RoomEdits.Add(CreateVertex(roomIdx, room, mesh.Vertices[mesh.Rectangles[root].Vertices[0]], -1, -256));
            data.RoomEdits.Add(CreateVertex(roomIdx, room, mesh.Vertices[mesh.Rectangles[root].Vertices[3]], -1, -256));
            data.RoomEdits.Add(CreateFace(roomIdx, 98, 6, TRMeshFaceType.TexturedQuad,
            [
                (ushort)(room.Mesh.Vertices.Count - 2),
                (ushort)(room.Mesh.Vertices.Count - 1),
                mesh.Rectangles[root].Vertices[3], mesh.Rectangles[root].Vertices[0]
            ]));
            data.RoomEdits.Add(CreateQuadShift(roomIdx, (short)root,
            [
                new(0, (ushort)(room.Mesh.Vertices.Count - 2)),
                new(3, (ushort)(room.Mesh.Vertices.Count - 1)),
            ]));
        }

        {
            var root = 15;
            data.RoomEdits.Add(CreateVertex(roomIdx, room, mesh.Vertices[mesh.Rectangles[root].Vertices[3]], -1, -256));
            data.RoomEdits.Add(CreateFace(roomIdx, 98, 4, TRMeshFaceType.TexturedQuad,
            [
                (ushort)(room.Mesh.Vertices.Count - 2),
                (ushort)(room.Mesh.Vertices.Count - 1),
                mesh.Rectangles[root].Vertices[3], mesh.Rectangles[root].Vertices[0]
            ]));
            data.RoomEdits.Add(CreateQuadShift(roomIdx, (short)root,
            [
                new(0, (ushort)(room.Mesh.Vertices.Count - 2)),
                new(3, (ushort)(room.Mesh.Vertices.Count - 1)),
            ]));
        }

        {
            var root = 12;
            data.RoomEdits.Add(CreateVertex(roomIdx, room, mesh.Vertices[mesh.Rectangles[root].Vertices[3]], -1, -256));
            data.RoomEdits.Add(CreateFace(roomIdx, 98, 4, TRMeshFaceType.TexturedQuad,
            [
                (ushort)(room.Mesh.Vertices.Count - 2),
                (ushort)(room.Mesh.Vertices.Count - 1),
                mesh.Rectangles[root].Vertices[3], mesh.Rectangles[root].Vertices[0]
            ]));

            data.RoomEdits.Add(CreateVertex(roomIdx, room, mesh.Vertices[mesh.Rectangles[root].Vertices[2]], -1, -256));
            data.RoomEdits.Add(CreateFace(roomIdx, 98, 6, TRMeshFaceType.TexturedQuad,
            [
                (ushort)(room.Mesh.Vertices.Count - 2),
                (ushort)(room.Mesh.Vertices.Count - 1),
                mesh.Rectangles[root].Vertices[2], mesh.Rectangles[root].Vertices[3]
            ]));

            data.RoomEdits.Add(CreateQuadShift(roomIdx, (short)root,
            [
                new(0, (ushort)(room.Mesh.Vertices.Count - 3)),
                new(2, (ushort)(room.Mesh.Vertices.Count - 1)),
                new(3, (ushort)(room.Mesh.Vertices.Count - 2)),
            ]));
        }

        {
            var root = 1;
            data.RoomEdits.Add(CreateVertex(roomIdx, room, mesh.Vertices[mesh.Rectangles[root].Vertices[2]], -1, -256));
            data.RoomEdits.Add(CreateFace(roomIdx, 98, 4, TRMeshFaceType.TexturedQuad,
            [
                (ushort)(room.Mesh.Vertices.Count - 2),
                (ushort)(room.Mesh.Vertices.Count - 1),
                mesh.Rectangles[root].Vertices[2], mesh.Rectangles[root].Vertices[3]
            ]));

            data.RoomEdits.Add(CreateVertex(roomIdx, room, mesh.Vertices[mesh.Rectangles[root].Vertices[1]], -1, -256));
            data.RoomEdits.Add(CreateFace(roomIdx, 98, 6, TRMeshFaceType.TexturedQuad,
            [
                (ushort)(room.Mesh.Vertices.Count - 2),
                (ushort)(room.Mesh.Vertices.Count - 1),
                mesh.Rectangles[root].Vertices[1], mesh.Rectangles[root].Vertices[2]
            ]));

            data.RoomEdits.Add(CreateQuadShift(roomIdx, (short)root,
            [
                new(3, (ushort)(room.Mesh.Vertices.Count - 3)),
                new(2, (ushort)(room.Mesh.Vertices.Count - 2)),
                new(1, (ushort)(room.Mesh.Vertices.Count - 1)),
            ]));
        }

        {
            var root = 5;
            data.RoomEdits.Add(CreateVertex(roomIdx, room, mesh.Vertices[mesh.Rectangles[root].Vertices[1]], -1, -256));
            data.RoomEdits.Add(CreateFace(roomIdx, 98, 4, TRMeshFaceType.TexturedQuad,
            [
                (ushort)(room.Mesh.Vertices.Count - 2),
                (ushort)(room.Mesh.Vertices.Count - 1),
                mesh.Rectangles[root].Vertices[1], mesh.Rectangles[root].Vertices[2]
            ]));
            data.RoomEdits.Add(CreateQuadShift(roomIdx, (short)root,
            [
                new(2, (ushort)(room.Mesh.Vertices.Count - 2)),
                new(1, (ushort)(room.Mesh.Vertices.Count - 1)),
            ]));
        }

        {
            var root = 8;
            data.RoomEdits.Add(CreateVertex(roomIdx, room, mesh.Vertices[mesh.Rectangles[root].Vertices[1]], -1, -256));
            data.RoomEdits.Add(CreateFace(roomIdx, 98, 6, TRMeshFaceType.TexturedQuad,
            [
                (ushort)(room.Mesh.Vertices.Count - 2),
                (ushort)(room.Mesh.Vertices.Count - 1),
                mesh.Rectangles[root].Vertices[1], mesh.Rectangles[root].Vertices[2]
            ]));

            data.RoomEdits.Add(CreateVertex(roomIdx, room, mesh.Vertices[mesh.Rectangles[root].Vertices[0]], -1, -256));
            data.RoomEdits.Add(CreateFace(roomIdx, 98, 4, TRMeshFaceType.TexturedQuad,
            [
                (ushort)(room.Mesh.Vertices.Count - 2),
                (ushort)(room.Mesh.Vertices.Count - 1),
                mesh.Rectangles[root].Vertices[0], mesh.Rectangles[root].Vertices[1]
            ]));

            data.RoomEdits.Add(CreateQuadShift(roomIdx, (short)root,
            [
                new(2, (ushort)(room.Mesh.Vertices.Count - 3)),
                new(1, (ushort)(room.Mesh.Vertices.Count - 2)),
                new(0, (ushort)(room.Mesh.Vertices.Count - 1)),
            ]));
        }

        {
            var root = 17;
            data.RoomEdits.Add(CreateFace(roomIdx, 98, 6, TRMeshFaceType.TexturedQuad,
            [
                (ushort)(room.Mesh.Vertices.Count - 1),
                (ushort)(room.Mesh.Vertices.Count - 10),
                mesh.Rectangles[root].Vertices[0], mesh.Rectangles[root].Vertices[1]
            ]));
            data.RoomEdits.Add(CreateQuadShift(roomIdx, (short)root,
            [
                new(1, (ushort)(room.Mesh.Vertices.Count - 1)),
            ]));
        }

        var centerVerts = new[] { 0, 3 };
        data.RoomEdits.AddRange(centerVerts.Select(v => new TRRoomVertexMove
        {
            RoomIndex = roomIdx,
            VertexIndex = room.Mesh.Rectangles[5].Vertices[v],
            VertexChange = new() { Y = -256 }
        }));

        // Rotate the hook above the cabin
        level.Rooms[29].StaticMeshes[17].Angle *= -1;
        level.Rooms[29].StaticMeshes[17].X += 1024;
        data.RoomEdits.Add(new TRRoomStatic3DEdit
        {
            RoomIndex = 29,
            MeshIndex = 17,
            StaticMesh = level.Rooms[29].StaticMeshes[17],
        });
    }
}
