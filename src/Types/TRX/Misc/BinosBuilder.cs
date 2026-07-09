using System.Diagnostics;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TRX.Misc;

public class BinosBuilder : InjectionBuilder
{
    private const TR3Type _tempBinoType = (TR3Type)(int)TR4Type.BinocularsItem;
    private const TR3Type _tempGfxType = (TR3Type)(int)TR4Type.BinocularGraphics;
    public override string ID => "binoculars";

    private record Setup(int ObjectID, int GfxId, Func<TR3Level, int, int, TRLevelBase> Convert)
    {
        public int ObjectID { get; set; } = ObjectID;
        public int GfxId { get; set; } = GfxId;
        public Func<TR3Level, int, int, TRLevelBase> Convert { get; set; } = Convert;
    }

    private static readonly List<Setup> _converters =
    [
        new(294, 295, ConvertToTR1),
        new(338, 339, ConvertToTR2),
        new(431, 432, ConvertToTR3),
    ];

    public override List<InjectionData> Build()
    {
        var baseLevel = CreateBinosLevel();
        var faceEdit = CreateFaceEdit(baseLevel.Models[_tempGfxType]);
        return [.. _converters.Select(converter =>
        {
            var convLevel = converter.Convert(baseLevel, converter.ObjectID, converter.GfxId);
            var data = InjectionData.Create(convLevel, InjectionType.General, ID);
            data.Images.AddRange(baseLevel.Images16.Select(i =>
            {
                var img = new TRImage(i.Pixels);
                return new TRTexImage32 { Pixels = img.ToRGBA() };
            }));

            data.MeshEdits.Add(new()
            {
                ModelID = (uint)converter.GfxId,
                EnforcedType = TRObjectType.Game,
                FaceEffects = [.. faceEdit.FaceEffects],
            });
            return data;
        })];
    }

    private static TR3Level CreateBinosLevel()
    {
        var level4 = _control4.Read($"Resources/TR4/{TR4LevelNames.SETH}");
        level4.Models = new TRDictionary<TR4Type, TRModel>
        {
            [TR4Type.BinocularsItem] = level4.Models[TR4Type.BinocularsItem],
            [TR4Type.BinocularGraphics] = level4.Models[TR4Type.BinocularGraphics],
        };
        var packer = new TR4TexturePacker(level4, TRGroupPackingMode.Object);
        var regions = packer.GetMeshRegions(level4.Models.Values.SelectMany(m => m.Meshes))
            .Values.SelectMany(v => v);

        var level3 = _control3.Read($"Resources/TR3/{TR3LevelNames.JUNGLE}");
        ResetLevel(level3, 1);
        var packer3 = new TR3TexturePacker(level3);
        packer3.AddRectangles(regions);
        packer3.Pack(true);

        foreach (var (t, m) in level4.Models)
        {
            level3.Models[(TR3Type)(int)t] = m;
        }
        level3.ObjectTextures.AddRange(regions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));
        level3.Models.Values
            .SelectMany(m => m.Meshes)
            .SelectMany(m => m.TexturedFaces)
            .Distinct()
            .ToList()
            .ForEach(f =>
            {
                f.Texture = (ushort)level3.ObjectTextures.IndexOf(level4.ObjectTextures[f.Texture]);
                Debug.Assert(f.Texture < level3.ObjectTextures.Count);
            });
        
        GenerateImages8(level3, [.. level3.Palette.Select(c => c.ToTR1Color())]);
        return level3;
    }

    private static TRMeshEdit CreateFaceEdit(TRModel model)
    {
        Debug.Assert(model.Meshes.Count > 0);
        var mesh = model.Meshes[0];
        var edit = new TRMeshEdit();
        for (int i = 0; i < mesh.TexturedTriangles.Count; i++)
        {
            var tri = mesh.TexturedTriangles[i];
            if (tri.Effects > 0)
                edit.FaceEffects.Add(new()
                {
                    FaceIndex = (short)i,
                    Effects = tri.Effects,
                    FaceType = TRMeshFaceType.TexturedTriangle,
                });
        }
        for (int i = 0; i < mesh.TexturedRectangles.Count; i++)
        {
            var quad = mesh.TexturedRectangles[i];
            if (quad.Effects > 0)
                edit.FaceEffects.Add(new()
                {
                    FaceIndex = (short)i,
                    Effects = quad.Effects,
                    FaceType = TRMeshFaceType.TexturedQuad,
                });
        }
        return edit;
    }

    private static TR1Level ConvertToTR1(TR3Level level, int objectId, int gfxId)
    {
        var caves = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        ResetLevel(caves);
        caves.Models[(TR1Type)objectId] = level.Models[_tempBinoType];
        caves.Models[(TR1Type)gfxId] = level.Models[_tempGfxType];
        caves.ObjectTextures.AddRange(level.ObjectTextures);
        return caves;
    }

    private static TR2Level ConvertToTR2(TR3Level level, int objectId, int gfxId)
    {
        var wall = _control2.Read($"Resources/{TR2LevelNames.GW}");
        ResetLevel(wall);
        wall.Models[(TR2Type)objectId] = level.Models[_tempBinoType];
        wall.Models[(TR2Type)gfxId] = level.Models[_tempGfxType];
        wall.ObjectTextures.AddRange(level.ObjectTextures);
        return wall;
    }

    private static TR3Level ConvertToTR3(TR3Level level, int objectId, int gfxId)
    {
        var jungle = _control3.Read($"Resources/TR3/{TR3LevelNames.JUNGLE}");
        ResetLevel(jungle);
        jungle.Models[(TR3Type)objectId] = level.Models[_tempBinoType];
        jungle.Models[(TR3Type)gfxId] = level.Models[_tempGfxType];
        jungle.ObjectTextures.AddRange(level.ObjectTextures);
        return jungle;
    }
}
