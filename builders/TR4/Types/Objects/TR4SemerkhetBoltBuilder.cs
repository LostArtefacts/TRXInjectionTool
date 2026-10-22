using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR4.Objects;

public class TR4SemerkhetBoltBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        var level = CreateLevel();
        var data = InjectionData.Create(level, InjectionType.General, "semerkhet_bolt");
        CreateDefaultTests(data, $"TR4/{TR4LevelNames.SEMERKHET}");
        return [data];
    }

    private static TR4Level CreateLevel()
    {
        // Tomb of Semerkhet has crossbow ammo but no model for the bolts that
        // the crossbow fires, so the model comes from Guardian of Semerkhet.
        var guardian = _control4.Read($"Resources/TR4/{TR4LevelNames.GUARDIAN}");
        var bolt = guardian.Models[TR4Type.CrossbowBolt];

        var level = _control4.Read($"Resources/TR4/{TR4LevelNames.SEMERKHET}");
        ResetLevel(level, 1);
        level.Images.Objects.Images16.Add(new() { Pixels = new ushort[256 * 256] });

        var regions = new TR4TexturePacker(guardian, TRGroupPackingMode.Object)
            .GetMeshRegions(bolt.Meshes).Values.SelectMany(v => v).ToList();
        var packer = new TR4TexturePacker(level, TRGroupPackingMode.Object);
        packer.AddRectangles(regions);
        packer.Pack(true);

        level.ObjectTextures.AddRange(regions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));
        bolt.Meshes
            .SelectMany(m => m.TexturedFaces)
            .Distinct()
            .ToList()
            .ForEach(f => f.Texture = (ushort)level.ObjectTextures.IndexOf(guardian.ObjectTextures[f.Texture]));

        level.Models[TR4Type.CrossbowBolt] = bolt;
        return level;
    }
}
