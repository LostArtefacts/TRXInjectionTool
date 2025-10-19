using TRImageControl.Packing;
using TRImageControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;
using System.Drawing;

namespace TRXInjectionTool.Types.TR1.Misc;

public class TR2PSXCrystalBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        var caves = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        var crystal = caves.Models[TR1Type.SavegameCrystal_P];

        var wall = _control2.Read($"Resources/{TR2LevelNames.GW}");
        ResetLevel(wall, 1);
        wall.Models[(TR2Type)269] = crystal;
        TR2TexturePacker packer = new(wall);
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
        wall.ObjectTextures.Add(region.Segments.First().Texture as TRObjectTexture);

        InjectionData data = InjectionData.Create(wall, InjectionType.PSCrystal, "purple_crystal");
        return new() { data };
    }
}
