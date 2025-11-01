using System.Diagnostics;
using System.Drawing;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Lara;

public class TR1LaraBraidBuilder : InjectionBuilder, IPublisher
{
    public override List<InjectionData> Build()
    {
        var level = CreateLevel();
        var data = InjectionData.Create(level, InjectionType.Braid, "braid");
        return [data];
    }

    private static TR1Level CreateLevel()
    {
        var level = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        ImportBraid(level);
        CreateMeshEdits(level);
        return level;
    }

    private static void ImportBraid(TR1Level caves)
    {
        TR2Level wall = _control2.Read($"Resources/{TR2LevelNames.GW}");

        ResetLevel(caves, 1);

        TRModel hair = wall.Models[TR2Type.LaraPonytail_H];
        caves.Models[TR1Type.LaraPonytail_H_U] = hair;

        // This adds a band around the first mesh.
        TRImage tile = new(256, 256);
        TRImage extraHair = new("Resources/TR1/Lara/hair1.png");
        tile.Import(extraHair, new(0, 0));
        wall.Images16.Add(new() { Pixels = tile.ToRGB555() });

        wall.ObjectTextures.Add(new(new Rectangle(0, 0, extraHair.Width, extraHair.Height))
        {
            Atlas = (ushort)(wall.Images16.Count - 1),
        });

        // Set the texture on mesh 0 and rotate faces where applicable.
        for (int i = 1; i <= 4; i++)
        {
            hair.Meshes[0].TexturedRectangles[i].Texture = (ushort)(wall.ObjectTextures.Count - 1);
            if (i != 3)
            {
                Rotate(hair.Meshes[0].TexturedRectangles[i].Vertices, i % 2 == 0 ? 3 : 2);
            }
        }

        // Taper the top of mesh 0 so it blends into the bottom of Lara's head.
        for (int i = 0; i < 4; i++)
        {
            hair.Meshes[0].Vertices[i].X = (short)Math.Floor(hair.Meshes[0].Vertices[i].X / 2f);
            hair.Meshes[0].Vertices[i].Y = (short)Math.Floor(hair.Meshes[0].Vertices[i].Y / 2f);
        }

        PackTextures(caves, wall, hair, new());
    }

    public static void ImportGoldBraid(TR1Level level)
    {
        var temp = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        ImportBraid(temp);

        int goldSlot = level.Palette.FindIndex(1, c => c.Red == 0 && c.Green == 0 && c.Blue == 0);
        Debug.Assert(goldSlot != -1);
        level.Palette[goldSlot] = new()
        {
            Red = 252,
            Green = 236,
            Blue = 136,
        };

        var model = level.Models[(TR1Type)199] = temp.Models[TR1Type.LaraPonytail_H_U];
        foreach (var mesh in model.Meshes)
        {
            mesh.ColouredRectangles.AddRange(mesh.TexturedRectangles);
            mesh.ColouredTriangles.AddRange(mesh.TexturedTriangles);
            mesh.TexturedRectangles.Clear();
            mesh.TexturedTriangles.Clear();
            mesh.ColouredFaces.ToList()
                .ForEach(f => f.Texture = (ushort)goldSlot);
        }
    }

    private static void CreateMeshEdits(TR1Level level)
    {
        SetupMeshEditModel(level);

        var ponyRecId = level.Models[TR1Type.LaraPonytail_H_U].Meshes[0]
            .TexturedRectangles[0].Texture;
        var ponyTriId = level.Models[TR1Type.LaraPonytail_H_U].Meshes[5]
            .TexturedTriangles[0].Texture;

        static void ShiftHeadVerts(TRMesh mesh)
        {
            for (int i = 43; i < 46; i++)
            {
                mesh.Vertices[i].Y -= 16;
            }
        }

        var model = level.Models[(TR1Type)200];

        {
            // Lara's backback
            var mesh = model.Meshes[0];
            for (int i = 26; i < 30; i++)
            {
                mesh.Vertices[i].Z += 12;
            }
        }

        {
            // Lara's default head
            var mesh = model.Meshes[1];
            ShiftHeadVerts(mesh);

            mesh.TexturedRectangles[1].Texture = ponyRecId;
            for (int i = 66; i < 74; i++)
            {
                mesh.TexturedTriangles[i].Texture = ponyTriId;
            }
        }

        {
            // Lara's angry head
            var mesh = model.Meshes[2];
            ShiftHeadVerts(mesh);

            mesh.TexturedRectangles[6].Texture = ponyRecId;
            for (int i = 56; i < 64; i++)
            {
                mesh.TexturedTriangles[i].Texture = ponyTriId;
            }
        }

        {
            // Lara's mauled head
            var mesh = model.Meshes[3];
            ShiftHeadVerts(mesh);

            foreach (var i in new[] { 14, 16, 17 })
            {
                mesh.TexturedRectangles[i].Texture = ponyRecId;
            }

            foreach (var i in new[] { 3, 37, 38, 39, 40 })
            {
                mesh.TexturedTriangles[i].Texture = ponyTriId;
            }
        }
    }

    private static void SetupMeshEditModel(TR1Level targetLevel)
    {
        var baseLevel = _control1.Read($"Resources/{TR1LevelNames.VALLEY}");

        var model = new TRModel
        {
            Meshes =
            [
                baseLevel.Models[TR1Type.Lara].Meshes[7],
                baseLevel.Models[TR1Type.Lara].Meshes[14],
                baseLevel.Models[TR1Type.LaraUziAnimation_H].Meshes[14],
                baseLevel.Models[TR1Type.LaraMiscAnim_H].Meshes[14],
            ],
            MeshTrees =
            [
                new() { OffsetY = -198, OffsetZ = -23 },
                new() { OffsetY = -134 },
                new() { OffsetY = -134 },
            ],
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
                            Rotations = [.. Enumerable.Range(0, 4).Select(i => new TRAnimFrameRotation())],
                        },
                    ],
                }
            ],
        };

        baseLevel.Models[(TR1Type)200] = model;
        targetLevel.Models[(TR1Type)200] = model;

        foreach (var face in model.Meshes.SelectMany(m => m.ColouredFaces))
        {
            var col = baseLevel.Palette[face.Texture];
            var i = targetLevel.Palette.FindIndex(c => c.Red == col.Red && c.Green == col.Green && c.Blue == col.Blue);
            if (i == -1)
            {
                i = targetLevel.Palette.FindIndex(1, c => c.Red == 0 && c.Blue == 0 && c.Green == 0);
                Debug.Assert(i != -1);
                targetLevel.Palette[i] = col;
            }
            face.Texture = (ushort)i;
        }

        var packer = new TR1TexturePacker(baseLevel);
        var regions = packer.GetMeshRegions(model.Meshes)
            .Values.SelectMany(r => r);

        var texInfos = model.Meshes.SelectMany(m => m.TexturedFaces)
            .Select(f => baseLevel.ObjectTextures[f.Texture])
            .Distinct()
            .ToList();

        packer = new(targetLevel);
        packer.AddRectangles(regions);
        packer.Pack(true);

        targetLevel.ObjectTextures.AddRange(texInfos);
        model.Meshes.SelectMany(m => m.TexturedFaces)
            .ToList().ForEach(f => f.Texture = (ushort)targetLevel.ObjectTextures.IndexOf(baseLevel.ObjectTextures[f.Texture]));
    }

    private static void Rotate<T>(List<T> list, int count)
    {
        for (int i = 0; i < count; i++)
        {
            T first = list[0];
            list.RemoveAt(0);
            list.Add(first);
        }
    }

    public TRLevelBase Publish()
        => CreateLevel();

    public string GetPublishedName()
        => "lara_braid.phd";
}
