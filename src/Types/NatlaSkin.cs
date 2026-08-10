using System.Drawing;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;

namespace TRXInjectionTool.Types;

// Natla's cutscene actor rebuilt on Lara's skeleton. Her legs carry a knee
// joint Lara does not have, whose mesh bakes onto the thigh above it.
//
// She is longer in the limb than Lara, so her body and her arms are each
// brought down to the reach the animations assume: her feet to the floor they
// stand her on, her hands to the ledges they reach for. Everything she is made
// of goes down with one or the other, her head filling back out afterwards to
// the width Lara wears hers at.
//
// Each part shrinks whole rather than lengthways. Squeezing only the length
// leaves the limbs as wide as they were and she comes out stubby, and scaling
// mesh by mesh sets neighbours far enough apart that their rings no longer
// overlap and every joint splits open. Whole parts meet only at the hips, the
// shoulders and the neck, where the skirt, the sleeves and her hair cover the
// step in girth.
public static class NatlaSkin
{
    private static readonly TR1LevelControl _control = new();

    private const int _pageWidth = 256;
    private const int _pageSize = _pageWidth * _pageWidth;
    private const int _swatchSize = 8;

    private const int _kneeL = 10;
    private const int _kneeR = 14;

    // Natla's mesh for each of Lara's fifteen.
    private static readonly int[] _meshMap =
    [
        0,  // hips
        9,  // thigh L
        11, // calf L
        12, // foot L
        13, // thigh R
        15, // calf R
        16, // foot R
        1,  // torso
        5,  // upper arm R
        6,  // lower arm R
        7,  // hand R
        2,  // upper arm L
        3,  // lower arm L
        4,  // hand L
        8,  // head
    ];

    // Natla's bone nodes for each of Lara's, root aside. The merged knees mean
    // the thigh bones span two.
    private static readonly int[][] _boneMap =
    [
        [8], [9, 10], [11],   // left leg
        [12], [13, 14], [15], // right leg
        [0],                  // torso
        [4], [5], [6],        // right arm
        [1], [2], [3],        // left arm
        [7],                  // head
    ];

    // A part of her that shrinks together: the meshes, the bones hanging off
    // them, and the bones measured against Lara's to find by how much. Whatever
    // no part claims keeps the size it was drawn.
    private record Part(int[] Meshes, int[] Bones, int[] Span);

    private static readonly Part[] _parts =
    [
        // Her body, down to where the animations stand her feet on the floor.
        new([0, 1, 2, 3, 4, 5, 6, 7, 14], [1, 2, 3, 4, 5, 6, 7, 8, 11, 14],
            [1, 2, 3, 4, 5, 6]),
        // Her arms, down to where they reach the ledges the animations grab.
        new([8, 9, 10, 11, 12, 13], [9, 10, 12, 13], [9, 10, 12, 13]),
    ];

    private const int _head = 14;

    public static TRModel Build(TR2Level outfitLevel, TRModel skeleton)
    {
        var cut = _control.Read($"Resources/{TR1LevelNames.MINES_CUT}");
        var natla = cut.Models[TR1Type.CutsceneActor2];

        var meshes = _meshMap.Select(i => natla.Meshes[i]).ToList();
        MergeKneecap(meshes[1], natla.Meshes[_kneeL], natla.MeshTrees[_kneeL - 1]);
        MergeKneecap(meshes[4], natla.Meshes[_kneeR], natla.MeshTrees[_kneeR - 1]);

        var crown = CrownScale(skeleton, meshes[_head]);
        var scales = _parts
            .Select(p => ReachScale(skeleton, natla, p.Span))
            .ToArray();
        foreach (var (part, scale) in _parts.Zip(scales))
        {
            foreach (var i in part.Meshes)
            {
                Scale(meshes[i], scale);
            }
        }
        ScaleCrown(meshes[_head], crown);

        ReplaceColouredFaces(cut, meshes);
        PackTextures(cut, outfitLevel, meshes);

        return new()
        {
            Meshes = meshes,
            MeshTrees = [.. skeleton.MeshTrees.Select((n, i) =>
            {
                var (x, y, z) = BoneOffset(natla, i + 1);
                var part = Array.FindIndex(_parts, p => p.Bones.Contains(i + 1));
                var scale = part == -1 ? 1.0 : scales[part];
                return new TRMeshTreeNode
                {
                    Flags = n.Flags,
                    OffsetX = (short)Math.Round(x * scale),
                    OffsetY = (short)Math.Round(y * scale),
                    OffsetZ = (short)Math.Round(z * scale),
                };
            })],
            Animations =
            [
                new()
                {
                    FrameRate = 1,
                    Accel = new(),
                    Speed = new(),
                    Frames =
                    [
                        new()
                        {
                            Bounds = new(),
                            Rotations = [.. Enumerable.Range(0, _meshMap.Length)
                                .Select(i => new TRAnimFrameRotation())],
                        },
                    ],
                },
            ],
        };
    }

    private static void MergeKneecap(TRMesh thigh, TRMesh knee, TRMeshTreeNode node)
    {
        var vertexBase = thigh.Vertices.Count;
        thigh.Vertices.AddRange(knee.Vertices.Select(v => new TRVertex
        {
            X = (short)(v.X + node.OffsetX),
            Y = (short)(v.Y + node.OffsetY),
            Z = (short)(v.Z + node.OffsetZ),
        }));
        thigh.Normals.AddRange(knee.Normals);

        foreach (var face in knee.TexturedFaces.Concat(knee.ColouredFaces))
        {
            for (int i = 0; i < face.Vertices.Count; i++)
            {
                face.Vertices[i] += (ushort)vertexBase;
            }
        }

        thigh.TexturedRectangles.AddRange(knee.TexturedRectangles);
        thigh.TexturedTriangles.AddRange(knee.TexturedTriangles);
        thigh.ColouredRectangles.AddRange(knee.ColouredRectangles);
        thigh.ColouredTriangles.AddRange(knee.ColouredTriangles);
    }

    private static double ReachScale(TRModel skeleton, TRModel natla, int[] span)
    {
        double lara = 0, hers = 0;
        foreach (var bone in span)
        {
            lara += skeleton.MeshTrees[bone - 1].OffsetY;
            hers += BoneOffset(natla, bone).Y;
        }

        return lara / hers;
    }

    // How much wider Lara wears her head than Natla does, taken before the body
    // is scaled so that it stays a proportion rather than a size: her head goes
    // down with the rest of her and only then fills out to Lara's build.
    //
    // Measured across rather than up, since Lara's head mesh carries her hair
    // down to her shoulders and matching its height draws Natla's bob out into
    // a long box.
    private static double CrownScale(TRModel skeleton, TRMesh head)
    {
        var lara = skeleton.Meshes[_head];
        return (lara.Vertices.Max(v => v.X) - lara.Vertices.Min(v => v.X))
            / (double)(head.Vertices.Max(v => v.X) - head.Vertices.Min(v => v.X));
    }

    private static (int X, int Y, int Z) BoneOffset(TRModel natla, int bone)
    {
        int x = 0, y = 0, z = 0;
        foreach (var node in _boneMap[bone - 1].Select(i => natla.MeshTrees[i]))
        {
            x += node.OffsetX;
            y += node.OffsetY;
            z += node.OffsetZ;
        }

        return (x, y, z);
    }

    private static void Scale(TRMesh mesh, double scale)
    {
        foreach (var vertex in mesh.Vertices)
        {
            vertex.X = (short)Math.Round(vertex.X * scale);
            vertex.Y = (short)Math.Round(vertex.Y * scale);
            vertex.Z = (short)Math.Round(vertex.Z * scale);
        }

        mesh.CollRadius = (short)Math.Round(mesh.CollRadius * scale);
    }

    // Her head is small next to Lara's, and enlarging it whole would widen the
    // ring where it meets the neck and open the seam. The growth is instead
    // graded from nothing at the neck to the full amount at the crown, and the
    // stretch upwards is anchored on the neck, so that ring keeps its size.
    private static void ScaleCrown(TRMesh head, double scale)
    {
        var neck = head.Vertices.Max(v => v.Y);
        var crown = head.Vertices.Min(v => v.Y);
        foreach (var vertex in head.Vertices)
        {
            var spread = 1 + (scale - 1) * (neck - vertex.Y) / (double)(neck - crown);
            vertex.X = (short)Math.Round(vertex.X * spread);
            vertex.Z = (short)Math.Round(vertex.Z * spread);
            vertex.Y = (short)Math.Round(neck + (vertex.Y - neck) * scale);
        }
    }

    // The outfits are carried in TR4 injections too, where the palette that
    // coloured faces index does not exist, so each colour becomes a swatch on
    // a page of its own and the faces that used it become textured.
    private static void ReplaceColouredFaces(TR1Level level, List<TRMesh> meshes)
    {
        var colours = meshes.SelectMany(m => m.ColouredFaces)
            .Select(f => f.Texture)
            .Distinct()
            .ToList();

        var page = new byte[_pageSize];
        var swatches = new Dictionary<ushort, ushort>();
        for (int i = 0; i < colours.Count; i++)
        {
            var x = i * _swatchSize;
            for (int y = 0; y < _swatchSize; y++)
            {
                Array.Fill(page, (byte)colours[i], y * _pageWidth + x, _swatchSize);
            }

            swatches[colours[i]] = (ushort)level.ObjectTextures.Count;
            level.ObjectTextures.Add(new(new Rectangle(x, 0, _swatchSize, _swatchSize))
            {
                Atlas = (ushort)level.Images8.Count,
            });
        }
        level.Images8.Add(new() { Pixels = page });

        foreach (var mesh in meshes)
        {
            foreach (var face in mesh.ColouredFaces)
            {
                face.Texture = swatches[face.Texture];
            }

            mesh.TexturedRectangles.AddRange(mesh.ColouredRectangles);
            mesh.TexturedTriangles.AddRange(mesh.ColouredTriangles);
            mesh.ColouredRectangles.Clear();
            mesh.ColouredTriangles.Clear();
        }
    }

    private static void PackTextures(TR1Level cut, TR2Level outfitLevel, List<TRMesh> meshes)
    {
        var packer = new TR1TexturePacker(cut);
        var regions = packer.GetMeshRegions(meshes).Values.SelectMany(r => r).ToList();
        var originalInfos = cut.ObjectTextures.ToList();
        var palette = cut.Palette.Select(c => new TRColour
        {
            Red = c.Red,
            Green = c.Green,
            Blue = c.Blue,
        }).ToList();

        InjectionBuilder.ResetLevel(cut, 1);
        cut.Palette = palette;

        packer = new(cut);
        packer.AddRectangles(regions);
        packer.Pack(true);

        var pageBase = outfitLevel.Images16.Count;
        outfitLevel.Images16.AddRange(cut.Images8.Select(i => new TRTexImage16
        {
            Pixels = new TRImage(i.Pixels, cut.Palette).ToRGB555(),
        }));

        var textureMap = new Dictionary<TRObjectTexture, ushort>();
        foreach (var segment in regions.SelectMany(r => r.Segments))
        {
            var texture = segment.Texture as TRObjectTexture;
            texture.Atlas += (ushort)pageBase;
            textureMap[texture] = (ushort)outfitLevel.ObjectTextures.Count;
            outfitLevel.ObjectTextures.Add(texture);
        }

        foreach (var face in meshes.SelectMany(m => m.TexturedFaces))
        {
            face.Texture = textureMap[originalInfos[face.Texture]];
        }
    }
}
