using TRDataControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1ToQTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level toq = _control1.Read($"Resources/{TR1LevelNames.QUALOPEC}");
        InjectionData data = CreateBaseData();

        data.RoomEdits.AddRange(CreateRefacings(toq));
        data.RoomEdits.AddRange(CreateRotations());
        data.RoomEdits.AddRange(CreateShifts(toq));

        FixWolfTransparency(toq, data);
        FixPassport(toq, data);

        return new() { data };
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR1Level toq)
    {
        return new()
        {
            new()
            {
                RoomIndex = 8,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 8,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 97,
                TargetIndex = 87,
            },
            Reface(toq, 14, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 3, 96),
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(5, TRMeshFaceType.TexturedQuad, 211, 2),
            Rotate(8, TRMeshFaceType.TexturedQuad, 87, 1),
        };
    }

    private static List<TRRoomTextureMove> CreateShifts(TR1Level toq)
    {
        return new()
        {
            new()
            {
                RoomIndex = 8,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 107,
                VertexRemap = new()
                {
                    new()
                    {
                        NewVertexIndex = toq.Rooms[8].Mesh.Rectangles[81].Vertices[3],
                    },
                    new()
                    {
                        Index = 1,
                        NewVertexIndex = toq.Rooms[8].Mesh.Rectangles[81].Vertices[2],
                    },
                }
            },

            new()
            {
                RoomIndex = 8,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 6,
                VertexRemap = new()
                {
                    new()
                    {
                        NewVertexIndex = toq.Rooms[8].Mesh.Rectangles[2].Vertices[3],
                    },
                    new()
                    {
                        Index = 1,
                        NewVertexIndex = toq.Rooms[8].Mesh.Rectangles[5].Vertices[0],
                    },
                }
            }
        };
    }

    private static InjectionData CreateBaseData()
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

        InjectionData data = InjectionData.Create(qualopec, InjectionType.TextureFix, "qualopec_textures");
        CreateDefaultTests(data, TR1LevelNames.QUALOPEC);
        return data;
    }
}
