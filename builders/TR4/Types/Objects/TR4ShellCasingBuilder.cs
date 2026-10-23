using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR4.Objects;

public class TR4ShellCasingBuilder : InjectionBuilder
{
    private static readonly TR4Type[] _types = [TR4Type.Gunshell, TR4Type.Shotgunshell];

    public override List<InjectionData> Build()
    {
        var level = CreateLevel();
        var data = InjectionData.Create(level, InjectionType.General, "shell_casings");
        return [data];
    }

    private static TR4Level CreateLevel()
    {
        var karnak = _control4.Read($"Resources/TR4/{TR4LevelNames.KARNAK}");
        AddCaps(karnak, karnak.Models[TR4Type.Gunshell].Meshes[0]);
        FixShotgunShell(karnak, karnak.Models[TR4Type.Shotgunshell].Meshes[0]);

        var level = _control4.Read($"Resources/TR4/{TR4LevelNames.KARNAK}");
        ResetLevel(level, 1);
        level.Images.Objects.Images16.Add(new() { Pixels = new ushort[256 * 256] });

        var meshes = _types.SelectMany(t => karnak.Models[t].Meshes).ToList();
        var regions = new TR4TexturePacker(karnak, TRGroupPackingMode.Object)
            .GetMeshRegions(meshes).Values.SelectMany(v => v).ToList();
        var packer = new TR4TexturePacker(level, TRGroupPackingMode.Object);
        packer.AddRectangles(regions);
        packer.Pack(true);

        level.ObjectTextures.AddRange(regions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));
        meshes
            .SelectMany(m => m.TexturedFaces)
            .Distinct()
            .ToList()
            .ForEach(f => f.Texture = (ushort)level.ObjectTextures.IndexOf(karnak.ObjectTextures[f.Texture]));

        foreach (var type in _types)
        {
            level.Models[type] = karnak.Models[type];
        }
        return level;
    }

    private static void AddCaps(TR4Level level, TRMesh mesh)
    {
        // OG leaves both ends of the pistol shell open. The shotgun shell has
        // the same prism and closes it with these two triangles.
        var side = level.ObjectTextures[mesh.TexturedRectangles[0].Texture];
        var cap = side.Clone();
        cap.IsTriangle = true;
        cap.Vertices = [side.Vertices[3].Clone(), side.Vertices[0].Clone(), side.Vertices[2].Clone(), new()];
        level.ObjectTextures.Add(cap);

        var texture = (ushort)(level.ObjectTextures.Count - 1);
        mesh.TexturedTriangles.Add(new() { Type = TRFaceType.Triangle, Vertices = [2, 1, 0], Texture = texture });
        mesh.TexturedTriangles.Add(new() { Type = TRFaceType.Triangle, Vertices = [3, 5, 4], Texture = texture });
    }

    private static void FixShotgunShell(TR4Level level, TRMesh mesh)
    {
        // Turns one side round and gives each cap its own colour. The texture
        // is red with a one-pixel brass column on its right edge, which the
        // sides map to the back of the shell. OG maps one side the other way
        // round, and both caps take in a corner of the brass column.
        var side = mesh.TexturedRectangles.Find(f => f.Vertices.SequenceEqual<ushort>([0, 4, 5, 2]));
        side.Vertices = [5, 2, 0, 4];

        var back = mesh.TexturedTriangles.Find(f => f.Vertices.SequenceEqual<ushort>([2, 1, 0]));
        var front = mesh.TexturedTriangles.Find(f => f.Vertices.SequenceEqual<ushort>([3, 5, 4]));
        var cap = level.ObjectTextures[back.Texture];
        int brass = cap.Vertices[2].X;

        var brassCap = cap.Clone();
        brassCap.Vertices[0].X = brass;
        brassCap.Vertices[1].X = brass;
        level.ObjectTextures.Add(brassCap);
        back.Texture = (ushort)(level.ObjectTextures.Count - 1);

        var redCap = cap.Clone();
        redCap.Vertices[2].X = brass - 1;
        level.ObjectTextures.Add(redCap);
        front.Texture = (ushort)(level.ObjectTextures.Count - 1);
    }
}
