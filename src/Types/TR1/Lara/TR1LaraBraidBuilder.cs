using System.Diagnostics;
using System.Drawing;
using TRImageControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Lara;

public class TR1LaraBraidBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        List<InjectionData> dataGroup = new();

        {
            TR1Level caves = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
            ImportBraid(caves);

            InjectionData data = InjectionData.Create(caves, InjectionType.Braid, "braid");
            AddDefaultHeadEdits(data);
            AddBackpackEdits(data);

            dataGroup.Add(data);
        }

        {
            InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.Braid, "braid_valley");
            AddValleyHeadEdits(data);

            dataGroup.Add(data);
        }

        return dataGroup;
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

    private static void AddDefaultHeadEdits(InjectionData data)
    {
        TRMeshEdit headEdit = new()
        {
            ModelID = (uint)TR1Type.Lara,
            MeshIndex = 14,
            FaceEdits = new()
            {
                new()
                {
                    ModelID = (uint)TR1Type.LaraPonytail_H_U,
                    MeshIndex = 0,
                    FaceType = TRMeshFaceType.TexturedQuad,
                    FaceIndex = 0,
                    TargetFaceIndices = new() { 1 }
                },
                new()
                {
                    ModelID = (uint)TR1Type.LaraPonytail_H_U,
                    MeshIndex = 5,
                    FaceType = TRMeshFaceType.TexturedTriangle,
                    FaceIndex = 0,
                    TargetFaceIndices = new() { 66, 67, 68, 69, 70, 71, 72, 73 }
                }
            },
            VertexEdits = new()
            {
                new()
                {
                    Index = 45,
                    Change = new()
                    {
                        Y = -16
                    }
                },
                new()
                {
                    Index = 44,
                    Change = new()
                    {
                        Y = -16
                    }
                },
                new()
                {
                    Index = 43,
                    Change = new()
                    {
                        Y = -16
                    }
                }
            }
        };

        TRMeshEdit uziHeadEdit = new()
        {
            ModelID = (uint)TR1Type.LaraUziAnimation_H,
            MeshIndex = 14,
            FaceEdits = new()
            {
                new()
                {
                    ModelID = (uint)TR1Type.LaraPonytail_H_U,
                    MeshIndex = 0,
                    FaceType = TRMeshFaceType.TexturedQuad,
                    FaceIndex = 0,
                    TargetFaceIndices = new() { 6 }
                },
                new()
                {
                    ModelID = (uint)TR1Type.LaraPonytail_H_U,
                    MeshIndex = 5,
                    FaceType = TRMeshFaceType.TexturedTriangle,
                    FaceIndex = 0,
                    TargetFaceIndices = new() { 56, 57, 58, 59, 60, 61, 62, 63 }
                }
            },
            VertexEdits = new()
            {
                new()
                {
                    Index = 45,
                    Change = new()
                    {
                        Y = -16
                    }
                },
                new()
                {
                    Index = 44,
                    Change = new()
                    {
                        Y = -16
                    }
                },
                new()
                {
                    Index = 43,
                    Change = new()
                    {
                        Y = -16
                    }
                }
            }
        };

        data.MeshEdits.Add(headEdit);
        data.MeshEdits.Add(uziHeadEdit);
    }

    private static void AddValleyHeadEdits(InjectionData data)
    {
        TRMeshEdit miscHeadEdit = new()
        {
            ModelID = (uint)(TR1Type)195,
            EnforcedType = TRObjectType.Game,
            MeshIndex = 14,
            FaceEdits = new()
            {
                new()
                {
                    ModelID = (uint)TR1Type.LaraPonytail_H_U,
                    MeshIndex = 0,
                    FaceType = TRMeshFaceType.TexturedQuad,
                    FaceIndex = 0,
                    TargetFaceIndices = new() { 14, 16, 17, }
                },
                new()
                {
                    ModelID = (uint)TR1Type.LaraPonytail_H_U,
                    MeshIndex = 5,
                    FaceType = TRMeshFaceType.TexturedTriangle,
                    FaceIndex = 0,
                    TargetFaceIndices = new() { 3, 37, 38, 39, 40 }
                }
            },
            VertexEdits = new()
            {
                new()
                {
                    Index = 45,
                    Change = new()
                    {
                        Y = -16
                    }
                },
                new()
                {
                    Index = 44,
                    Change = new()
                    {
                        Y = -16
                    }
                },
                new()
                {
                    Index = 43,
                    Change = new()
                    {
                        Y = -16
                    }
                }
            }
        };

        TRMeshEdit miscShotgunEdit = new()
        {
            ModelID = (uint)TR1Type.LaraMiscAnim_H,
            MeshIndex = 7,
            FaceEdits = new(),
            VertexEdits = new()
            {
                new()
                {
                    Index = 26,
                    Change = new()
                    {
                        Z = 12
                    }
                },
                new()
                {
                    Index = 27,
                    Change = new()
                    {
                        Z = 12
                    }
                },
                new()
                {
                    Index = 28,
                    Change = new()
                    {
                        Z = 12
                    }
                },
                new()
                {
                    Index = 29,
                    Change = new()
                    {
                        Z = 12
                    }
                },
            }
        };

        data.MeshEdits.Add(miscHeadEdit);
        data.MeshEdits.Add(miscShotgunEdit);
    }

    private static void AddBackpackEdits(InjectionData data)
    {
        TRMeshEdit backpackEdit = new()
        {
            ModelID = (uint)TR1Type.Lara,
            MeshIndex = 7,
            FaceEdits = new(),
            VertexEdits = new()
            {
                new()
                {
                    Index = 26,
                    Change = new()
                    {
                        Z = 12
                    }
                },
                new()
                {
                    Index = 27,
                    Change = new()
                    {
                        Z = 12
                    }
                },
                new()
                {
                    Index = 28,
                    Change = new()
                    {
                        Z = 12
                    }
                },
                new()
                {
                    Index = 29,
                    Change = new()
                    {
                        Z = 12
                    }
                },
            }
        };

        TRMeshEdit backpackShotgunEdit = new()
        {
            ModelID = (uint)TR1Type.LaraShotgunAnim_H,
            MeshIndex = 7,
            FaceEdits = new(),
            VertexEdits = new()
            {
                new()
                {
                    Index = 26,
                    Change = new()
                    {
                        Z = 12
                    }
                },
                new()
                {
                    Index = 27,
                    Change = new()
                    {
                        Z = 12
                    }
                },
                new()
                {
                    Index = 28,
                    Change = new()
                    {
                        Z = 12
                    }
                },
                new()
                {
                    Index = 29,
                    Change = new()
                    {
                        Z = 12
                    }
                },
            }
        };

        data.MeshEdits.Add(backpackEdit);
        data.MeshEdits.Add(backpackShotgunEdit);
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
}
