using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR4.Textures;

public class TR4TitleTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = CreateBaseData();
        data.RoomEdits.Add(FixRoom30());

        return [data];
    }

    private static TRRoomTextureReface FixRoom30()
    {
        var level = _control4.Read($"Resources/TR4/title.tr4");
        var tex = level.Rooms[30].Mesh.Rectangles[9].Texture;
        return Reface(level, 30, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, tex, 14);
    }

    private static InjectionData CreateBaseData()
    {
        var level = _control4.Read($"Resources/TR4/{TR4LevelNames.COASTAL}");
        var horizon = level.Models[TR4Type.Horizon];
        var skyPage = new TRImage(level.Images.Sky.Pixels);

        var packer = new TR4TexturePacker(level, TRGroupPackingMode.Object);
        var horizonRegions = packer.GetMeshRegions(horizon.Meshes)
            .Values.SelectMany(v => v);
        var originalInfos = level.ObjectTextures.ToList();

        var jungle = _control3.Read($"Resources/TR3/{TR3LevelNames.JUNGLE}");
        ResetLevel(jungle, 1);
        var packer3 = new TR3TexturePacker(jungle);
        packer3.AddRectangles(horizonRegions);
        packer3.Pack(true);

        jungle.ObjectTextures.AddRange(horizonRegions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));
        horizon.Meshes
            .SelectMany(m => m.TexturedFaces)
            .Distinct()
            .ToList()
            .ForEach(f =>
            {
                f.Texture = (ushort)jungle.ObjectTextures.IndexOf(originalInfos[f.Texture]);
            });

        ResetLevel(level);
        level.Models[TR4Type.Horizon] = horizon;
        level.ObjectTextures.AddRange(horizonRegions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));

        var image = new TRImage(jungle.Images16[0].Pixels);
        level.Images.Objects.Images16.Add(new() { Pixels = image.ToRGB555() });
        level.Images.Objects.Images32.Add(new() { Pixels = image.ToRGB32() });

        var data = InjectionData.Create(level, InjectionType.TextureFix, "title_textures");
        CreateDefaultTests(data, "TR4/title.tr4");

        // Replace the title's existing sky sprites
        data.TextureOverwrites.Add(new()
        {
            Page = 13,
            Width = TRConsts.TPageWidth,
            Height = TRConsts.TPageHeight,
            Data = skyPage.ToRGBA(),
        });

        data.MeshEdits.Add(new()
        {
            ModelID = (uint)(TR4Type.SceneryBase + 32),
            VertexEdits = [.. Enumerable.Range(32, 7).Select(i => new TRVertexEdit
            {
                Index = (short)i,
                Change = new() { Y = -128 },
            })],
        });

        var title = _control4.Read("Resources/TR4/title.tr4");
        var noSkyRooms = Enumerable.Range(0, title.Rooms.Count)
            .Where(i => title.Rooms[i].Flags.HasFlag(TRRoomFlag.Skybox))
            .Select(i => (short)i)
            .ToList();
        noSkyRooms.Remove(24);
        data.FloorEdits.AddRange(FDBuilder.RemoveRoomFlags(noSkyRooms, TRRoomFlag.Skybox, title.Rooms));

        return data;
    }
}
