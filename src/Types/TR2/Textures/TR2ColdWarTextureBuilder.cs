using System.Drawing;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2ColdWarTextureBuilder : InjectionBuilder
{
    public override string ID => "coldwar_textures";

    public override List<InjectionData> Build()
    {
        InjectionData data = CreateBaseData();
        CreateDefaultTests(data, TR2LevelNames.COLDWAR);

        return new() { data };
    }

    private InjectionData CreateBaseData()
    {
        TR2Level level = _control2.Read($"Resources/{TR2LevelNames.COLDWAR}");
        TRModel skybox = level.Models[TR2Type.Skybox_H];

        TR2TexturePacker packer = new(level);
        var regions = packer.GetMeshRegions(skybox.Meshes).Values.SelectMany(v => v).ToList();
        List<TRObjectTexture> originalInfos = new(level.ObjectTextures);

        List<Color> basePalette = new(level.Palette.Select(c => c.ToTR1Color()));
        ResetLevel(level, 1);
        packer = new(level);        

        TRImage flatImg = new(8, 8);
        flatImg.Fill(Color.Black);
        TRObjectTexture texture = new(0, 0, 8, 8);
        originalInfos.Add(texture);
        regions.Add(new(new()
        {
            Index = originalInfos.Count - 1,
            Texture = texture,
        }, flatImg));

        packer.AddRectangles(regions);
        packer.Pack(true);

        level.Models[TR2Type.Skybox_H] = skybox;
        level.ObjectTextures.AddRange(regions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));

        skybox.Meshes[0].TexturedFaces.ToList()
            .ForEach(f => f.Texture = (ushort)level.ObjectTextures.IndexOf(originalInfos[f.Texture]));

        skybox.Meshes[0].TexturedTriangles.AddRange(skybox.Meshes[0].ColouredTriangles);
        skybox.Meshes[0].TexturedTriangles.ForEach(f => f.Texture = (ushort)level.ObjectTextures.IndexOf(texture));
        skybox.Meshes[0].ColouredTriangles.Clear();

        GenerateImages8(level, basePalette);

        return InjectionData.Create(level, InjectionType.TextureFix, ID);
    }
}
