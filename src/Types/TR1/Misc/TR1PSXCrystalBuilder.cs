using TRImageControl.Packing;
using TRImageControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;
using System.Drawing;

namespace TRXInjectionTool.Types.TR1.Misc;

public class TR1PSXCrystalBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level caves = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        TRModel crystal = caves.Models[TR1Type.SavegameCrystal_P];
        ResetLevel(caves, 1);

        caves.Models[TR1Type.SavegameCrystal_P] = crystal;

        TR1TexturePacker packer = new(caves);
        TRImage img = new(16, 16);
        img.Write((c, x, y) => Color.FromArgb(64, 64, 252));
        TRTextileRegion region = new(new()
        {
            Texture = new TRObjectTexture(0, 0, 16, 16),
        }, img);
        packer.AddRectangle(region);
        packer.Pack(true);

        crystal.Meshes[0].TexturedTriangles.AddRange(crystal.Meshes[0].ColouredTriangles);
        crystal.Meshes[0].ColouredTriangles.Clear();
        crystal.Meshes[0].TexturedTriangles.ToList()
            .ForEach(f => f.Texture = 0);
        caves.ObjectTextures.Add(region.Segments.First().Texture as TRObjectTexture);

        InjectionData data = InjectionData.Create(caves, InjectionType.PSCrystal, "purple_crystal");
        return new() { data };
    }
}
