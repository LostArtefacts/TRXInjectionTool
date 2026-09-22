using System.Diagnostics;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool;
using TRXInjectionTool.Control;

namespace TRXBuilders.TR4.Types.Misc;

public class TR4GuideGunBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        var jungle = MakeBaseLevel();
        TransferGun(jungle);

        return [ExportData(jungle)];
    }

    private static TR3Level MakeBaseLevel()
    {
        var valley = _control4.Read($"Resources/TR4/{TR4LevelNames.VALLEY}");
        var packer4 = new TR4TexturePacker(valley, TRGroupPackingMode.Object);
        var regions = packer4.GetMeshRegions(valley.Models[TR4Type.Guide].Meshes)
            .Values.SelectMany(v => v);

        var jungle = _control3.Read($"Resources/TR3/{TR3LevelNames.JUNGLE}");
        jungle.Models[TR3Type.Lara].Meshes = jungle.Models[TR3Type.LaraPistolAnimation_H].Meshes;
        CreateModelLevel(jungle, TR3Type.Lara);

        var packer3 = new TR3TexturePacker(jungle);
        packer3.AddRectangles(regions);
        packer3.Pack(true);

        jungle.ObjectTextures.AddRange(regions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));
        jungle.Models[TR3Type.Animating1] = valley.Models[TR4Type.Guide];
        jungle.Models[TR3Type.Animating1].Meshes
            .SelectMany(m => m.TexturedFaces)
            .Distinct()
            .ToList()
            .ForEach(f =>
            {
                f.Texture = (ushort)jungle.ObjectTextures.IndexOf(valley.ObjectTextures[f.Texture]);
                Debug.Assert(f.Texture < jungle.ObjectTextures.Count);
            });

        GenerateImages8(jungle, [.. jungle.Palette.Select(c => c.ToTR1Color())]);
        return jungle;
    }

    private static void TransferGun(TR3Level level)
    {
        // Strip away the torch from the guide's hand, and stitch on Lara's pistol
        // instead. OG hard-coded a mesh swap using O_MESH_SWAP_2 but in Valley of
        // the Kings that object is not present, so it defaults to using the default
        // mesh of the level, which just so happens to be Lara's pistol hand.

        var laraHand = level.Models[TR3Type.Lara].Meshes[10];
        laraHand.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 8));
        laraHand.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 8));
        laraHand.Vertices = laraHand.Vertices.GetRange(8, laraHand.Vertices.Count - 8);
        laraHand.Normals = laraHand.Normals.GetRange(8, laraHand.Normals.Count - 8);
        laraHand.TexturedFaces.ToList().ForEach(f =>
        {
            for (int i = 0; i < f.Vertices.Count; i++)
            {
                f.Vertices[i] -= 8;
            }
        });

        var guideHand = level.Models[TR3Type.Animating1].Meshes[18];
        guideHand.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v > 9));
        guideHand.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v > 9));
        guideHand.Vertices = guideHand.Vertices.GetRange(0, 10);
        guideHand.Normals = guideHand.Normals.GetRange(0, 10);

        var vtxBase = (ushort)guideHand.Vertices.Count;
        foreach (var face in laraHand.TexturedFaces.Select(f => f.Clone()))
        {
            for (int i = 0; i < face.Vertices.Count; i++)
            {
                face.Vertices[i] += vtxBase;
            }
            if (face.Type == TRFaceType.Triangle)
            {
                guideHand.TexturedTriangles.Add(face);
            }
            else
            {
                guideHand.TexturedRectangles.Add(face);
            }
        }

        guideHand.Vertices.AddRange(laraHand.Vertices);
        guideHand.Normals.AddRange(laraHand.Normals);

        CreateModelLevel(level, TR3Type.Animating1);
    }

    private static InjectionData ExportData(TR3Level level)
    {
        var valley = _control4.Read($"Resources/TR4/{TR4LevelNames.VALLEY}");
        ResetLevel(valley);
        valley.Models[TR4Type.Guide] = level.Models[TR3Type.Animating1];
        valley.ObjectTextures.AddRange(level.ObjectTextures);

        var data = InjectionData.Create(valley, InjectionType.General, "guide_gun");
        data.Images.AddRange(level.Images16.Select(i =>
        {
            var img = new TRImage(i.Pixels);
            return new TRTexImage32 { Pixels = img.ToRGBA() };
        }));

        return data;
    }
}
