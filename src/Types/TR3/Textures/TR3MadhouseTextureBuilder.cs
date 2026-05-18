using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types.TR3.Textures;

public class TR3MadhouseTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = CreateBaseData();

        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.MADHOUSE}");
        data.RoomEdits.AddRange(CreateRefacings(level));
        data.FloorEdits.AddRange(FixMaterials(level));
        FixSpikes(data, level);

        return [data];
    }

    private static IEnumerable<TRRoomTextureReface> CreateRefacings(TR3Level level)
    {
        foreach (var face in new[] { 21, 54 })
        {
            yield return Reface(level, 97, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 2354, (short)face);
        }
        yield return Reface(level, 97, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 2213, 23);
    }

    private static void FixSpikes(InjectionData data, TR3Level level)
    {
        var mesh = level.Models[TR3Type.TeethSpikesOrBarbedWire].Meshes[0];
        var maxY = mesh.Vertices.Max(v => v.Y);

        var edit = new TRMeshEdit
        {
            ModelID = (uint)TR3Type.TeethSpikesOrBarbedWire,
            VertexEdits = [],
        };
        data.MeshEdits.Add(edit);

        for (short i = 0; i < mesh.Vertices.Count; i++)
        {
            var vertex = mesh.Vertices[i];
            if (vertex.Y < maxY)
            {
                continue;
            }

            edit.VertexEdits.Add(new()
            {
                Index = i,
                Change = new() { Y = TRConsts.Step1 / 2 },
            });
        }
    }

    private static IEnumerable<TRFloorDataEdit> FixMaterials(TR3Level level)
    {
        foreach (var roomIdx in new[] { 66, 69 })
        {
            var room = level.Rooms[roomIdx];
            for (ushort x = 1; x < room.NumXSectors - 1; x++)
            {
                for (ushort z = 1; z < room.NumZSectors - 1; z++)
                {
                    var sector = room.GetSector(x, z, TRUnit.Sector);
                    if (sector.RoomBelow != TRConsts.NoRoom || sector.Material != TRMaterial.Snow)
                    {
                        continue;
                    }

                    yield return new TRFloorDataEdit
                    {
                        RoomIndex = (short)roomIdx,
                        X = x,
                        Z = z,
                        Fixes = [new FDMaterialEdit { Material = TRMaterial.Grass }],
                    };
                }
            }
        }

        yield return new TRFloorDataEdit
        {
            RoomIndex = 133,
            X = 4,
            Z = 4,
            Fixes = [new FDMaterialEdit { Material = TRMaterial.Grass }],
        };
    }

    private static InjectionData CreateBaseData()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.MADHOUSE}");
        var lampStatic = level.StaticMeshes[TR3Type.SceneryBase + 11];
        var orbStatic = level.StaticMeshes[TR3Type.SceneryBase + 12];

        TRFaceConverter.ConvertFlatFaces(level, [.. level.Palette16.Select(c => c.ToTR1Color())], [lampStatic.Mesh]);

        var packer = new TR3TexturePacker(level);
        var regions = packer.GetMeshRegions([lampStatic.Mesh, orbStatic.Mesh])
            .Values.SelectMany(v => v);
        var originalInfos = level.ObjectTextures.ToList();
        ResetLevel(level, 1);

        packer = new(level);
        packer.AddRectangles(regions);
        packer.Pack(true);

        level.StaticMeshes[TR3Type.SceneryBase + 11] = lampStatic;
        level.StaticMeshes[TR3Type.SceneryBase + 12] = orbStatic;
        level.ObjectTextures.AddRange(regions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));
        lampStatic.Mesh.TexturedFaces.Concat(orbStatic.Mesh.TexturedFaces).ToList()
            .ForEach(f =>
            {
                f.Texture = (ushort)level.ObjectTextures.IndexOf(originalInfos[f.Texture]);
            });
        GenerateImages8(level, [.. level.Palette.Select(c => c.ToTR1Color())]);

        {
            // Fix a missing face and make the glass part double-sided
            lampStatic.Mesh.TexturedFaces.Where(f => f.Vertices.All(v => (v >= 14 && v <= 25) || (v >= 36 && v <= 47)))
                .ToList().ForEach(f => f.DoubleSided = true);
            lampStatic.Mesh.TexturedRectangles.Add(new()
            {
                Type = TRFaceType.Rectangle,
                Texture = lampStatic.Mesh.TexturedRectangles[7].Texture,
                Vertices = [14, 15, 37, 36],
            });
        }

        {
            // Fix a bad rotation and make everything double-sided
            orbStatic.Mesh.TexturedRectangles.RemoveAt(9);
            orbStatic.Mesh.TexturedRectangles[8].Rotate(2);
            orbStatic.Mesh.TexturedFaces.ToList().ForEach(f => f.DoubleSided = true);
        }

        var data = InjectionData.Create(level, InjectionType.TextureFix, "zoo_textures");
        CreateDefaultTests(data, $"TR3/{TR3LevelNames.MADHOUSE}");
        return data;
    }
}
