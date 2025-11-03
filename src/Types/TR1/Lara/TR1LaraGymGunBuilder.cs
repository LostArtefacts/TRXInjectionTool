using TRDataControl;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Lara;

public class TR1LaraGymGunBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        return [];
    }

    public static TR1Level CreateLevel()
    {
        var gym = CreateEnhancedGymLevel();
        gym = OrganiseModels(gym);
        return gym;
    }

    private static TR1Level CreateEnhancedGymLevel()
    {
        TR1Level level = _control1.Read($"Resources/{TR1LevelNames.ASSAULT}");

        List<TRMesh> lara = level.Models[TR1Type.Lara].Meshes;
        List<TRMesh> laraPistol = level.Models[TR1Type.LaraPistolAnim_H]?.Meshes;
        List<TRMesh> laraShotgun = level.Models[TR1Type.LaraShotgunAnim_H]?.Meshes;
        List<TRMesh> laraMagnums = level.Models[TR1Type.LaraMagnumAnim_H]?.Meshes;
        List<TRMesh> laraUzis = level.Models[TR1Type.LaraUziAnimation_H]?.Meshes;
        List<TRMesh> laraMisc = level.Models[TR1Type.LaraMiscAnim_H]?.Meshes;

        // Basic meshes to take from LaraMiscAnim. We don't replace Lara's gloves
        // or thighs (at this stage - handled below with gun swaps).
        int[] basicLaraIndices = new int[] { 0, 2, 3, 5, 6, 7, 8, 9, 11, 12 };
        foreach (int index in basicLaraIndices)
        {
            laraMisc[index].CopyInto(lara[index]);
        }

        // Copy the guns and holsters from the original models and paste them
        // onto each of Lara's thighs. The ranges are the specific faces we want
        // to copy from each.
        int[] thighs = new int[] { 1, 4 };
        foreach (int thigh in thighs)
        {
            // Empty holsters.
            CopyMeshParts(new()
            {
                BaseMesh = lara[thigh],
                NewMesh = laraMisc[thigh],
                ColourFaceCopies = Enumerable.Range(8, 6),
            });

            // Holstered pistols
            if (laraPistol != null)
            {
                CopyMeshParts(new()
                {
                    BaseMesh = laraPistol[thigh],
                    NewMesh = laraMisc[thigh],
                    ColourFaceCopies = Enumerable.Range(4, 6),
                    TextureFaceCopies = Enumerable.Range(5, 5)
                });
            }

            // Holstered magnums
            if (laraMagnums != null)
            {
                CopyMeshParts(new()
                {
                    BaseMesh = laraMagnums[thigh],
                    NewMesh = laraMisc[thigh],
                    ColourFaceCopies = Enumerable.Range(4, 6),
                    TextureFaceCopies = Enumerable.Range(3, 5)
                });
            }

            // Holstered uzis
            if (laraUzis != null)
            {
                CopyMeshParts(new()
                {
                    BaseMesh = laraUzis[thigh],
                    NewMesh = laraMisc[thigh],
                    ColourFaceCopies = Enumerable.Range(4, 7),
                    TextureFaceCopies = Enumerable.Range(3, 19)
                });
            }
        }

        // Don't forget the shotgun on her back
        if (laraShotgun != null)
        {
            CopyMeshParts(new()
            {
                BaseMesh = laraShotgun[7],
                NewMesh = laraMisc[7],
                TextureFaceCopies = Enumerable.Range(8, 12)
            });
        }

        // Replace gloves with normal hands
        Dictionary<ushort, ushort> map = new()
        {
            [343] = 586,
            [393] = 586,
            [347] = 590,
            [342] = 585,
            [346] = 589,
            [341] = 584,
            [348] = 591,
            [344] = 587,
            [345] = 588,
            [340] = 583,
        };
        foreach (TRMeshFace face in level.DistinctMeshes.SelectMany(m => m.TexturedFaces))
        {
            if (map.ContainsKey(face.Texture))
            {
                face.Texture = map[face.Texture];
            }
        }

        // Replace the flat quads on Lara's hands with the right colour, and rotate some faces.
        TRMesh normalHandR = level.Models[TR1Type.Lara].Meshes[10];
        TRMesh normalHandL = level.Models[TR1Type.Lara].Meshes[13];
        TRMesh pistolsHandR = level.Models[TR1Type.LaraPistolAnim_H].Meshes[10];
        TRMesh pistolsHandL = level.Models[TR1Type.LaraPistolAnim_H].Meshes[13];
        TRMesh shotgunHandR = level.Models[TR1Type.LaraShotgunAnim_H].Meshes[10];
        TRMesh shotgunHandL = level.Models[TR1Type.LaraShotgunAnim_H].Meshes[13];
        TRMesh magnumsHandR = level.Models[TR1Type.LaraMagnumAnim_H].Meshes[10];
        TRMesh magnumsHandL = level.Models[TR1Type.LaraMagnumAnim_H].Meshes[13];
        TRMesh uzisHandR = level.Models[TR1Type.LaraUziAnimation_H].Meshes[10];
        TRMesh uzisHandL = level.Models[TR1Type.LaraUziAnimation_H].Meshes[13];
        TRMesh miscHandR = level.Models[TR1Type.LaraMiscAnim_H].Meshes[10];

        normalHandR.ColouredRectangles[0].Texture = miscHandR.ColouredRectangles[0].Texture;
        normalHandL.ColouredRectangles[0].Texture = miscHandR.ColouredRectangles[0].Texture;

        shotgunHandR.ColouredRectangles[0].Texture = miscHandR.ColouredRectangles[0].Texture;
        shotgunHandL.ColouredRectangles[0].Texture = miscHandR.ColouredRectangles[0].Texture;

        shotgunHandR.TexturedRectangles.Add(shotgunHandR.ColouredRectangles[1]);
        shotgunHandR.ColouredRectangles.RemoveAt(1);
        shotgunHandR.TexturedRectangles[^1].Texture = 586;

        shotgunHandR.TexturedRectangles[^1].SwapVertices(0, 2);
        shotgunHandR.TexturedRectangles[^1].SwapVertices(1, 3);
        pistolsHandR.TexturedRectangles[3].SwapVertices(0, 2);
        pistolsHandR.TexturedRectangles[3].SwapVertices(1, 3);
        pistolsHandL.TexturedRectangles[3].SwapVertices(0, 2);
        pistolsHandL.TexturedRectangles[3].SwapVertices(1, 3);

        magnumsHandR.TexturedRectangles[3].SwapVertices(0, 2);
        magnumsHandR.TexturedRectangles[3].SwapVertices(1, 3);
        magnumsHandL.TexturedRectangles[3].Vertices = new() { 3, 7, 4, 0 };

        pistolsHandR.ColouredRectangles[0].Texture = miscHandR.ColouredRectangles[0].Texture;
        pistolsHandL.ColouredRectangles[0].Texture = miscHandR.ColouredRectangles[0].Texture;
        uzisHandR.ColouredRectangles[0].Texture = miscHandR.ColouredRectangles[0].Texture;
        uzisHandL.ColouredRectangles[0].Texture = miscHandR.ColouredRectangles[0].Texture;

        return level;
    }

    private static TR1Level OrganiseModels(TR1Level gym)
    {
        // We need to export and reimport the models so to get rid of the other data we
        // don't need and to maintain efficient texture packing.
        // This will also import the gunflare and shotgun and magnum SFX from Caves.
        TR1Level caves = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        _control1.Write(gym, "temp.phd");

        TR1DataExporter exporter = new();
        List<TR1Type> types = new()
        {
            TR1Type.Lara,
            TR1Type.LaraPistolAnim_H,
            TR1Type.LaraShotgunAnim_H,
            TR1Type.LaraMagnumAnim_H,
            TR1Type.LaraUziAnimation_H,
        };
        foreach (TR1Type type in types)
        {
            exporter.Export(gym, type);
            gym = _control1.Read("temp.phd");
        }

        File.Delete("temp.phd");

        exporter.Export(caves, TR1Type.Gunflare_H);
        types.Add(TR1Type.Gunflare_H);

        ResetLevel(gym, 1);
        TR1DataImporter importer = new()
        {
            Level = gym,
            TypesToImport = new(types),
        };
        importer.Import();

        {
            var model = gym.Models[TR1Type.LaraShotgunAnim_H];
            var oldBack = model.Meshes[7];
            oldBack.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 26));
            oldBack.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 26));
            oldBack.ColouredRectangles.RemoveAll(f => f.Vertices.All(v => v < 26));
            oldBack.Vertices.RemoveRange(0, 26);
            oldBack.Normals.RemoveRange(0, 26);
            oldBack.TexturedFaces.Concat(oldBack.ColouredFaces).ToList().ForEach(f =>
            {
                for (int i = 0; i < f.Vertices.Count; i++)
                {
                    f.Vertices[i] -= 26;
                }
            });

            model.Meshes[7] = gym.Models[TR1Type.Lara].Meshes[0];
            oldBack.Vertices.ForEach(v =>
            {
                v.Y += 204;
                v.Z += 25;
            });
            oldBack.Centre = new() { X = 42, Y = 99, Z = 70 };
            oldBack.CollRadius = 105;
            model.Meshes[14] = oldBack;

            var wall = _control2.Read($"Resources/{TR2LevelNames.GW}");
            model.Animations = wall.Models[TR2Type.LaraShotgunAnim_H].Animations;
            (model.Animations[1].Commands[0] as TRSFXCommand).SoundID = (short)TR1SFX.LaraDraw;
        }

        foreach (TRMesh mesh in gym.DistinctMeshes)
        {
            foreach (TRMeshFace f in mesh.ColouredFaces.Where(f => f.Texture == 47))
            {
                f.Texture = 1;
            }
        }

        gym.SoundEffects[TR1SFX.LaraShotgun] = caves.SoundEffects[TR1SFX.LaraShotgun];
        gym.SoundEffects[TR1SFX.LaraMagnums] = caves.SoundEffects[TR1SFX.LaraMagnums];
        gym.SoundEffects[TR1SFX.LaraUziFire] = caves.SoundEffects[TR1SFX.LaraUziFire];

        AddRicochet(gym);
        return gym;
    }

    private static void AddRicochet(TR1Level gym)
    {
        TR1Level caves = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        TRSpriteSequence ricochet = caves.Sprites[TR1Type.Ricochet_S_H];

        TR1TexturePacker packer = new(caves);
        List<TRTextileRegion> regions = packer.GetSpriteRegions(ricochet)
            .Values.SelectMany(r => r)
            .ToList();

        packer = new(gym);
        packer.AddRectangles(regions);
        packer.Pack(true);

        gym.Sprites[TR1Type.Ricochet_S_H] = ricochet;
    }

    private static void CopyMeshParts(MeshCopyData data)
    {
        TRMesh mesh = data.NewMesh.Clone();

        if (data.TextureFaceCopies != null)
        {
            foreach (int faceIndex in data.TextureFaceCopies)
            {
                TRMeshFace face = data.BaseMesh.TexturedRectangles[faceIndex];
                ushort[] vertexPointers = new ushort[4];
                for (int j = 0; j < vertexPointers.Length; j++)
                {
                    TRVertex origVertex = data.BaseMesh.Vertices[face.Vertices[j]];
                    int newVertIndex = mesh.Vertices.FindIndex(v => v.X == origVertex.X && v.Y == origVertex.Y && v.Z == origVertex.Z);
                    if (newVertIndex == -1)
                    {
                        newVertIndex = mesh.Vertices.Count;
                        mesh.Vertices.Add(origVertex);
                        if (face.Vertices[j] < data.BaseMesh.Normals.Count)
                        {
                            mesh.Normals.Add(data.BaseMesh.Normals[face.Vertices[j]]);
                        }
                    }
                    vertexPointers[j] = (ushort)newVertIndex;
                }

                mesh.TexturedRectangles.Add(new()
                {
                    Texture = face.Texture,
                    Vertices = vertexPointers.ToList()
                });
            }
        }

        if (data.ColourFaceCopies != null)
        {
            foreach (int faceIndex in data.ColourFaceCopies)
            {
                TRMeshFace face = data.BaseMesh.ColouredRectangles[faceIndex];
                ushort[] vertexPointers = new ushort[4];
                for (int j = 0; j < vertexPointers.Length; j++)
                {
                    TRVertex origVertex = data.BaseMesh.Vertices[face.Vertices[j]];
                    int newVertIndex = mesh.Vertices.FindIndex(v => v.X == origVertex.X && v.Y == origVertex.Y && v.Z == origVertex.Z);
                    if (newVertIndex == -1)
                    {
                        newVertIndex = mesh.Vertices.Count;
                        mesh.Vertices.Add(origVertex);
                        if (face.Vertices[j] < data.BaseMesh.Normals.Count)
                        {
                            mesh.Normals.Add(data.BaseMesh.Normals[face.Vertices[j]]);
                        }
                    }
                    vertexPointers[j] = (ushort)newVertIndex;
                }

                mesh.ColouredRectangles.Add(new()
                {
                    Texture = face.Texture,
                    Vertices = vertexPointers.ToList()
                });
            }
        }

        mesh.CollRadius = data.BaseMesh.CollRadius;

        mesh.CopyInto(data.BaseMesh);
    }

    public class MeshCopyData
    {
        public TRMesh BaseMesh { get; set; }
        public TRMesh NewMesh { get; set; }
        public IEnumerable<int> TextureFaceCopies { get; set; }
        public IEnumerable<int> ColourFaceCopies { get; set; }
    }
}
