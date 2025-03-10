using TRDataControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1EnemyTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        return new()
        {
            FixLarson(),
            FixSkateboardKid(),
            FixCowboy(),
            FixKold(),
        };
    }
    // Move these to levels
    private static InjectionData FixLarson()
    {
        // Larson's gun is silver in Sanctuary and the ToQ cutscene, but in ToQ itself
        // it's gold. We take the mesh from Sanctuary and replace it in ToQ.
        TR1Level qualopec = _control1.Read($"Resources/{TR1LevelNames.QUALOPEC}");
        TR1Level sanctuary = _control1.Read($"Resources/{TR1LevelNames.SANCTUARY}");

        CreateModelLevel(sanctuary, TR1Type.Larson);

        TRModel toqLarson = qualopec.Models[TR1Type.Larson];
        qualopec.Models[TR1Type.Pierre] = toqLarson;
        qualopec.Models.Remove(TR1Type.Larson);

        TR1DataImporter importer = new()
        {
            Level = qualopec,
            TypesToImport = new() { TR1Type.Larson },
        };
        importer.Import();

        toqLarson.Meshes[14] = qualopec.Models[TR1Type.Larson].Meshes[14];
        qualopec.Models[TR1Type.Larson] = toqLarson;

        CreateModelLevel(qualopec, TR1Type.Larson);

        qualopec.SoundEffects.Remove(TR1SFX.LarsonDeath);

        return InjectionData.Create(qualopec, InjectionType.TextureFix, "larson_textures");
    }

    private static InjectionData FixSkateboardKid()
    {
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.TextureFix, "skateboardkid_textures");

        // Fix the yellow patches on this model with skin tone.
        data.Palette.Add(new());
        data.Palette.Add(new()
        {
            Red = 204,
            Green = 132,
            Blue = 88,
        });

        short[] meshIndices = new short[] { 1, 3, 4, 7, };
        foreach (short meshIndex in meshIndices)
        {
            data.MeshEdits.Add(new()
            {
                ModelID = (uint)TR1Type.SkateboardKid,
                MeshIndex = meshIndex,
                FaceEdits = new()
                {
                    new()
                    {
                        FaceType = TRMeshFaceType.ColouredQuad,
                        MeshIndex = -1,
                        TargetFaceIndices = new() { 0 },
                    }
                },
            });
        }

        return data;
    }

    private static InjectionData FixCowboy()
    {
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.TextureFix, "cowboy_textures");

        // Fix the yellow patches on this model with skin tone.
        data.Palette.Add(new());
        data.Palette.Add(new()
        {
            Red = 204,
            Green = 132,
            Blue = 88,
        });

        short[] meshIndices = new short[] { 4, 7, };
        foreach (short meshIndex in meshIndices)
        {
            data.MeshEdits.Add(new()
            {
                ModelID = (uint)TR1Type.Cowboy,
                MeshIndex = meshIndex,
                FaceEdits = new()
                {
                    new()
                    {
                        FaceType = TRMeshFaceType.ColouredQuad,
                        MeshIndex = -1,
                        TargetFaceIndices = new() { 0 },
                    }
                },
            });
        }

        return data;
    }

    private static InjectionData FixKold()
    {
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.TextureFix, "kold_textures");

        // Fix the orange patches on this model with skin tone.
        data.Palette.Add(new());
        data.Palette.Add(new()
        {
            Red = 112,
            Green = 72,
            Blue = 16,
        });

        data.MeshEdits = new()
        {
            new()
            {
                ModelID = (uint)TR1Type.Kold,
                MeshIndex = 1,
                FaceEdits = new()
                {
                    new()
                    {
                        FaceType = TRMeshFaceType.ColouredQuad,
                        MeshIndex = -1,
                        TargetFaceIndices = GetRange(10, 3),
                    },
                    new()
                    {
                        FaceType = TRMeshFaceType.ColouredTriangle,
                        MeshIndex = -1,
                        TargetFaceIndices = GetRange(4, 4),
                    },
                },
            },

            new()
            {
                ModelID = (uint)TR1Type.Kold,
                MeshIndex = 2,
                FaceEdits = new()
                {
                    new()
                    {
                        FaceType = TRMeshFaceType.ColouredQuad,
                        MeshIndex = -1,
                        TargetFaceIndices = GetRange(0, 8),
                    },
                    new()
                    {
                        FaceType = TRMeshFaceType.ColouredTriangle,
                        MeshIndex = -1,
                        TargetFaceIndices = GetRange(0, 15),
                    },
                },
            },

            new()
            {
                ModelID = (uint)TR1Type.Kold,
                MeshIndex = 5,
                FaceEdits = new()
                {
                    new()
                    {
                        FaceType = TRMeshFaceType.ColouredQuad,
                        MeshIndex = -1,
                        TargetFaceIndices = GetRange(0, 6),
                    },
                },
            },

            new()
            {
                ModelID = (uint)TR1Type.Kold,
                MeshIndex = 8,
                FaceEdits = new()
                {
                    new()
                    {
                        FaceType = TRMeshFaceType.ColouredQuad,
                        MeshIndex = -1,
                        TargetFaceIndices = GetRange(0, 6),
                    },
                },
            },
        };

        return data;
    }
}
