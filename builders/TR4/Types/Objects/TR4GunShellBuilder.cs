using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR4.Objects;

public class TR4GunShellBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        var level = CreateLevel();
        var data = InjectionData.Create(level, InjectionType.General, "gun_shell");
        return [data];
    }

    private static TR4Level CreateLevel()
    {
        var karnak = _control4.Read($"Resources/TR4/{TR4LevelNames.KARNAK}");
        var shell = karnak.Models[TR4Type.Gunshell];
        AddCaps(karnak, shell.Meshes[0]);

        var level = _control4.Read($"Resources/TR4/{TR4LevelNames.KARNAK}");
        ResetLevel(level, 1);
        level.Images.Objects.Images16.Add(new() { Pixels = new ushort[256 * 256] });

        var regions = new TR4TexturePacker(karnak, TRGroupPackingMode.Object)
            .GetMeshRegions(shell.Meshes).Values.SelectMany(v => v).ToList();
        var packer = new TR4TexturePacker(level, TRGroupPackingMode.Object);
        packer.AddRectangles(regions);
        packer.Pack(true);

        level.ObjectTextures.AddRange(regions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));
        shell.Meshes
            .SelectMany(m => m.TexturedFaces)
            .Distinct()
            .ToList()
            .ForEach(f => f.Texture = (ushort)level.ObjectTextures.IndexOf(karnak.ObjectTextures[f.Texture]));

        level.Models[TR4Type.Gunshell] = shell;
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
}
