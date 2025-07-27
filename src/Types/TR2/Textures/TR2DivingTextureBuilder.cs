using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2DivingTextureBuilder : TextureBuilder
{
    public override string ID => "diving_textures";

    public override List<InjectionData> Build()
    {
        TR2Level level = _control2.Read($"Resources/{TR2LevelNames.DA}");
        InjectionData data = CreateBaseData();        

        data.RoomEdits.AddRange(CreateRefacings(level));
        data.RoomEdits.AddRange(CreateRotations());

        FixPassport(level, data);
        FixPushButton(data);

        return new() { data };
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR2Level level)
    {
        return new()
        {
            Reface(level, 28, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1965, 1),
            Reface(level, 30, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1916, 4),
            Reface(level, 30, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1916, 7),
            Reface(level, 77, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1946, 39),
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(23, TRMeshFaceType.TexturedQuad, 26, 2),
            Rotate(23, TRMeshFaceType.TexturedQuad, 40, 2),
            Rotate(23, TRMeshFaceType.TexturedQuad, 54, 2),
            Rotate(28, TRMeshFaceType.TexturedQuad, 10, 3),
            Rotate(28, TRMeshFaceType.TexturedQuad, 13, 1),
            Rotate(28, TRMeshFaceType.TexturedQuad, 15, 1),
            Rotate(28, TRMeshFaceType.TexturedQuad, 17, 1),
            Rotate(77, TRMeshFaceType.TexturedQuad, 39, 3),
        };
    }

    private InjectionData CreateBaseData()
    {
        TR2Level cut3 = _control2.Read($"Resources/{TR2LevelNames.DA_CUT}");
        TR2Level da = _control2.Read($"Resources/{TR2LevelNames.DA}");
        var suitStatic = cut3.StaticMeshes[TR2Type.SceneryBase + 10];

        var packer = new TR2TexturePacker(cut3);
        var regions = packer.GetMeshRegions(new[] { suitStatic.Mesh })
            .Values.SelectMany(v => v);
        var originalInfos = cut3.ObjectTextures.ToList();
        ResetLevel(cut3, 1);

        packer = new(cut3);
        packer.AddRectangles(regions);
        packer.Pack(true);

        cut3.StaticMeshes[TR2Type.SceneryBase + 9] = suitStatic;
        cut3.ObjectTextures.AddRange(regions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));
        suitStatic.Mesh.TexturedFaces.ToList()
            .ForEach(f =>
            {
                f.Texture = (ushort)cut3.ObjectTextures.IndexOf(originalInfos[f.Texture]);
            });
        GenerateImages8(cut3, da.Palette.Select(c => c.ToTR1Color()).ToList());

        foreach (int faceIdx in new[] { 6, 7 })
        {
            var suitFace = suitStatic.Mesh.TexturedRectangles[faceIdx].Clone();
            var texInfo = cut3.ObjectTextures[suitFace.Texture].Clone();
            texInfo.UVMode = TRUVMode.NE_AntiClockwise;
            cut3.ObjectTextures.Add(texInfo);
            suitFace.SwapVertices(0, 1);
            suitFace.SwapVertices(2, 3);
            suitFace.Texture = (ushort)(cut3.ObjectTextures.Count - 1);
            suitStatic.Mesh.TexturedRectangles.Add(suitFace);
        }

        var data = InjectionData.Create(cut3, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.DA);

        data.RoomEdits.Add(new TRRoomStatic3DCreate
        {
            ID = 9,
            RoomIndex = 77,
            StaticMesh = new()
            {
                X = 58880,
                Y = 5632,
                Z = 55808,
                Intensity = 4096,
            }
        });

        return data;
    }
}
