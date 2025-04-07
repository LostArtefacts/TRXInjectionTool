using System.Diagnostics;
using System.Drawing;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Misc;

public class TR1BubblesBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level caves = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        TRSpriteSequence bubbles = caves.Sprites[TR1Type.Bubbles1_S_H];

        TR1TexturePacker packer = new(caves);
        IEnumerable<TRTextileRegion> regions = packer.GetSpriteRegions(bubbles)
            .Values.SelectMany(r => r);

        List<string> ids = regions.Select(r => r.Image.GenerateID()).ToList();
        Debug.Assert(ids.All(i => i == ids[0]));       
        
        TRImage image = regions.First().Image;
        TRImage fixedImage = new(image.Size);
        fixedImage.Import(image.Export(new(2, 0, image.Width - 2, image.Height)), new(0, 0));

        TRImage edge = new(1, 8);
        edge.Fill(Color.FromArgb(228, 228, 228));
        fixedImage.Import(edge, new(54, 24));

        foreach (var region in regions)
        {
            region.Image = fixedImage;
        }

        ResetLevel(caves, 1);

        packer = new(caves);
        packer.AddRectangles(regions);
        packer.Pack(true);

        caves.Sprites[TR1Type.Bubbles1_S_H] = bubbles;

        InjectionData data = InjectionData.Create(caves, InjectionType.TextureFix, "bubbles");
        return new() { data };
    }
}
