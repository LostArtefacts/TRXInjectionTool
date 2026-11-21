using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR4.Objects;

public class TR4ModelReplacementBuilder : InjectionBuilder
{
    private record Replacement(TR4Type Type, string Name, string Donor, string[] Targets);

    // The Mastabas ships an older torch, which has a thin stick in Lara's
    // hand and a shorter drop animation.
    private static readonly Replacement[] _replacements =
    [
        new(TR4Type.LaraTorchAnim, "torch", TR4LevelNames.TULUN, [TR4LevelNames.MASTABAS]),
    ];

    public override List<InjectionData> Build()
    {
        var result = new List<InjectionData>();
        foreach (var replacement in _replacements)
        {
            foreach (var target in replacement.Targets)
            {
                var level = CreateLevel(replacement.Type, replacement.Donor, target);
                var data = InjectionData.Create(level, InjectionType.General, $"{_tr4NameMap[target]}_{replacement.Name}");
                CreateDefaultTests(data, $"TR4/{target}");
                result.Add(data);
            }
        }
        return result;
    }

    private static TR4Level CreateLevel(TR4Type type, string donorName, string targetName)
    {
        var donor = _control4.Read($"Resources/TR4/{donorName}");
        var model = donor.Models[type];

        var level = _control4.Read($"Resources/TR4/{targetName}");
        ResetLevel(level, 1);
        level.Images.Objects.Images16.Add(new() { Pixels = new ushort[256 * 256] });

        var regions = new TR4TexturePacker(donor, TRGroupPackingMode.Object)
            .GetMeshRegions(model.Meshes).Values.SelectMany(v => v).ToList();
        var packer = new TR4TexturePacker(level, TRGroupPackingMode.Object);
        packer.AddRectangles(regions);
        packer.Pack(true);

        level.ObjectTextures.AddRange(regions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));
        model.Meshes
            .SelectMany(m => m.TexturedFaces)
            .Distinct()
            .ToList()
            .ForEach(f => f.Texture = (ushort)level.ObjectTextures.IndexOf(donor.ObjectTextures[f.Texture]));

        level.Models[type] = model;
        return level;
    }
}
