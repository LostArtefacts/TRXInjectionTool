using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR4.Textures;

public class TR4GuardianTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR4, InjectionType.TextureFix, "guardian_textures");
        CreateDefaultTests(data, $"TR4/{TR4LevelNames.GUARDIAN}");
        var level = _control4.Read($"Resources/TR4/{TR4LevelNames.GUARDIAN}");
        data.RoomEdits.AddRange(CloseSwitchRoom(level, 62, 55, 1));
        foreach (short roomIdx in new short[] { 70, 71, 72, 73 })
        {
            data.RoomEdits.AddRange(CloseSwitchRoom(level, roomIdx, 69, 9));
        }

        return [data];
    }

    // The switch rooms have a floor and a back wall only. Adds the side walls
    // and the ceiling, so that the space behind each switch is closed. The
    // ceiling takes its texture from a ceiling face of the room outside.
    private static IEnumerable<TRRoomTextureEdit> CloseSwitchRoom(TR4Level level, short roomIdx,
        short ceilingRoom, ushort ceilingFace)
    {
        var room = level.Rooms[roomIdx];
        var mesh = room.Mesh;
        var backWall = mesh.Rectangles[1];
        var pos = mesh.Vertices.Select(v => v.Vertex).ToList();
        var centre = new double[]
        {
            pos.Average(v => (double)v.X),
            pos.Average(v => (double)v.Y),
            pos.Average(v => (double)v.Z),
        };
        var top = pos.Min(v => v.Y);
        bool backOnX = backWall.Vertices.Select(v => pos[v].X).Distinct().Count() == 1;
        int Side(int v) => backOnX ? pos[v].Z : pos[v].X;

        foreach (var side in new[] { pos.Min(v => backOnX ? v.Z : v.X), pos.Max(v => backOnX ? v.Z : v.X) })
        {
            var corners = Enumerable.Range(0, pos.Count).Where(v => Side(v) == side).ToList();
            var tops = corners.Where(v => pos[v].Y == top).ToList();
            var bottoms = corners.Where(v => pos[v].Y != top).ToList();
            var below = (int v) => bottoms.First(b => pos[b].X == pos[v].X && pos[b].Z == pos[v].Z);
            ushort[] face = [(ushort)tops[0], (ushort)tops[1], (ushort)below(tops[1]), (ushort)below(tops[0])];
            yield return CreateFace(roomIdx, roomIdx, 1, TRMeshFaceType.TexturedQuad, FaceInward(face, pos, centre));
        }

        var ceiling = Enumerable.Range(0, pos.Count).Where(v => pos[v].Y == top)
            .OrderBy(v => Math.Atan2(pos[v].Z - centre[2], pos[v].X - centre[0]))
            .Select(v => (ushort)v).ToArray();
        yield return CreateFace(roomIdx, ceilingRoom, ceilingFace, TRMeshFaceType.TexturedQuad, FaceInward(ceiling, pos, centre));
    }

    // Returns the quad in the vertex order that makes it visible from the
    // room centre.
    private static ushort[] FaceInward(ushort[] face, List<TRVertex> pos, double[] centre)
    {
        TRVertex a = pos[face[0]], b = pos[face[1]], c = pos[face[2]];
        double ux = b.X - a.X, uy = b.Y - a.Y, uz = b.Z - a.Z;
        double vx = c.X - a.X, vy = c.Y - a.Y, vz = c.Z - a.Z;
        double nx = uy * vz - uz * vy, ny = uz * vx - ux * vz, nz = ux * vy - uy * vx;
        double dx = face.Average(v => (double)pos[v].X) - centre[0];
        double dy = face.Average(v => (double)pos[v].Y) - centre[1];
        double dz = face.Average(v => (double)pos[v].Z) - centre[2];
        return nx * dx + ny * dy + nz * dz > 0 ? face : [face[1], face[0], face[3], face[2]];
    }
}
