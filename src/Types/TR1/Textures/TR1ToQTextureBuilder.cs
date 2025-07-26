using TRDataControl;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1ToQTextureBuilder : TextureBuilder
{
    public override string ID => "qualopec_textures";

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
            Rotate(20, TRMeshFaceType.TexturedQuad, 185, 2),
            Rotate(46, TRMeshFaceType.TexturedQuad, 188, 2),
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

    private InjectionData CreateBaseData()
    {
        // Larson's gun is silver in Sanctuary and the ToQ cutscene, but in ToQ itself
        // it's gold. We take the mesh from Sanctuary and replace it in ToQ.
        TR1Level qualopec = _control1.Read($"Resources/{TR1LevelNames.QUALOPEC}");
        TR1Level sanctuary = _control1.Read($"Resources/{TR1LevelNames.SANCTUARY}");

        CreateModelLevel(sanctuary, TR1Type.Larson);

        var statue = qualopec.StaticMeshes[TR1Type.SceneryBase + 30];
        var packer = new TR1TexturePacker(qualopec);
        var regions = packer.GetMeshRegions(new[] { statue.Mesh })
            .Values.SelectMany(v => v);
        var originalInfos = qualopec.ObjectTextures.ToList();

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


        packer = new(qualopec);
        packer.AddRectangles(regions);
        packer.Pack(true);

        qualopec.StaticMeshes[TR1Type.SceneryBase + 30] = statue;
        qualopec.ObjectTextures.AddRange(regions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));
        statue.Mesh.TexturedFaces.ToList()
            .ForEach(f =>
            {
                f.Texture = (ushort)qualopec.ObjectTextures.IndexOf(originalInfos[f.Texture]);
            });

        var backTexInfo = qualopec.ObjectTextures[statue.Mesh.TexturedRectangles[0].Texture].Clone();
        backTexInfo.Vertices[0].Y += 124;
        backTexInfo.Vertices[1].Y += 124;
        qualopec.ObjectTextures.Add(backTexInfo);

        var verts = new List<List<ushort>>
        {
            new() { 20, 23, 22, 21 },
            new() { 16, 14, 10, 19 },
            new() { 15, 17, 18, 11 },
        };
        statue.Mesh.TexturedRectangles.AddRange(verts.Select(v =>
            new TRMeshFace
            {
                Type = TRFaceType.Rectangle,
                Texture = statue.Mesh.TexturedRectangles[9].Texture,
                Vertices = v,
            }));

        verts = new()
        {
            new() { 7, 28, 29, 6 },
            new() { 11, 7, 6, 10 },
            new() { 23, 11, 10, 22 },
        };
        statue.Mesh.TexturedRectangles.AddRange(verts.Select(v =>
            new TRMeshFace
            {
                Type = TRFaceType.Rectangle,
                Texture = (ushort)(qualopec.ObjectTextures.Count - 1),
                Vertices = v,
            }));

        statue.CollisionBox.MaxX -= 131;
        statue.CollisionBox.MinX += 131;

        var data = InjectionData.Create(qualopec, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR1LevelNames.QUALOPEC);
        return data;
    }
}
