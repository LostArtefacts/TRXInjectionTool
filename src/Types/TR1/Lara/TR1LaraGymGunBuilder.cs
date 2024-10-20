using TRDataControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Lara;

public class TR1LaraGymGunBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level gym = CreateEnhancedGymLevel();
        gym = OrganiseModels(gym);        

        InjectionData data = InjectionData.Create(gym, InjectionType.General, "lara_gym_guns");
        return new() { data };
    }

    private static TR1Level CreateEnhancedGymLevel()
    {
        TR1Level level = _control1.Read($@"Resources\{TR1LevelNames.ASSAULT}");

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

        return level;
    }

    private static TR1Level OrganiseModels(TR1Level gym)
    {
        // We need to export and reimport the models so to get rid of the other data we
        // don't need and to maintain efficient texture packing.
        // This will also import the gunflare and shotgun and magnum SFX from Caves.
        TR1Level caves = _control1.Read($@"Resources\{TR1LevelNames.CAVES}");
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

        foreach (TRMesh mesh in gym.DistinctMeshes)
        {
            foreach (TRMeshFace f in mesh.ColouredFaces.Where(f => f.Texture == 47))
            {
                f.Texture = 1;
            }
        }

        gym.SoundEffects[TR1SFX.LaraShotgun] = caves.SoundEffects[TR1SFX.LaraShotgun];
        gym.SoundEffects[TR1SFX.LaraMagnums] = caves.SoundEffects[TR1SFX.LaraMagnums];

        return gym;
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
