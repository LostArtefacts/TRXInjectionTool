using TRImageControl;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types;

public abstract class OutfitBuilder : InjectionBuilder
{
    protected const int _modelBase = 302;
    protected const int _outfitCount = 26;
    protected const int _maxOutfits = 32;
    protected const int _outfitExtras = 334;
    protected const int _outfitGuns1 = 335;
    protected const int _outfitGuns2 = 336;
    protected const int _outfitGuns3 = 337;
    protected const int _outfitLegs = 338;

    public override List<InjectionData> Build()
    {
        var outfitLevel = _control2.Read("Resources/outfits.tr2");
        ModifyOutfits(outfitLevel);
        var level = CreateLevel(outfitLevel);
        level.ObjectTextures = outfitLevel.ObjectTextures;

        // Cache blend mode attribute, otherwise lost during level flattening.
        var blendModes = level.ObjectTextures.Select((o, i) => new { Mode = o.BlendingMode, Idx = i })
            .ToList();

        var data = InjectionData.Create(level, InjectionType.General, "lara_outfits");
        data.Images.AddRange(outfitLevel.Images16.Select(i =>
        {
            var img = new TRImage(i.Pixels);
            return new TRTexImage32 { Pixels = img.ToRGBA() };
        }));

        blendModes.ForEach(o => data.ObjectTextures[o.Idx].Attribute = (ushort)o.Mode);

        data.SFX.AddRange(GetBarefootSFX());

        return [data];
    }

    // Hook for game-specific builders to graft additional extra meshes into the
    // shared outfits resource before it is flattened into the injection.
    protected virtual void ModifyOutfits(TR2Level outfitLevel) { }

    protected abstract TRLevelBase CreateLevel(TR2Level outfitLevel);
    protected abstract List<TRSFXData> GetBarefootSFX();

    // Isolates the geometry a meshswap adds on top of a base mesh. TR4 hand
    // meshswaps (e.g. the crowbar) are authored as the plain hand with the held
    // item's vertices/faces appended, so everything referencing a vertex at or
    // beyond the base hand's vertex count is the item, already in hand space.
    protected static TRMesh ExtractAddedGeometry(TRMesh full, int baseVertexCount)
    {
        var result = new TRMesh
        {
            Vertices = new(full.Vertices.Skip(baseVertexCount)),
            Normals = full.Normals == null
                ? null : new(full.Normals.Skip(baseVertexCount)),
            Lights = full.Lights == null
                ? null : new(full.Lights.Skip(baseVertexCount)),
        };

        void CopyFaces(List<TRMeshFace> src, List<TRMeshFace> dst)
        {
            foreach (var face in src)
            {
                if (face.Vertices.All(v => v >= baseVertexCount))
                {
                    var clone = face.Clone();
                    clone.Vertices = new(face.Vertices.Select(
                        v => (ushort)(v - baseVertexCount)));
                    dst.Add(clone);
                }
            }
        }

        CopyFaces(full.TexturedRectangles, result.TexturedRectangles);
        CopyFaces(full.TexturedTriangles, result.TexturedTriangles);
        CopyFaces(full.ColouredRectangles, result.ColouredRectangles);
        CopyFaces(full.ColouredTriangles, result.ColouredTriangles);
        result.SelfCalculateBounds();
        return result;
    }

    // Copies a mesh from a source level's model into the outfits extras model,
    // importing the object textures it uses into the outfit's texture space.
    protected static void GraftExtraMesh(
        TR2Level outfitLevel, int extrasModelId, TRMesh sourceMesh,
        List<TRObjectTexture> sourceTextures, List<TRTexImage32> sourcePages)
    {
        var mesh = sourceMesh.Clone();
        var pageMap = new Dictionary<ushort, ushort>();
        var texRemap = new Dictionary<ushort, ushort>();

        foreach (var face in mesh.TexturedFaces)
        {
            if (texRemap.ContainsKey(face.Texture))
            {
                continue;
            }

            var tex = sourceTextures[face.Texture].Clone();
            if (!pageMap.TryGetValue(tex.Atlas, out ushort newPage))
            {
                var image = new TRImage(sourcePages[tex.Atlas].Pixels);
                outfitLevel.Images16.Add(new() { Pixels = image.ToRGB555() });
                newPage = (ushort)(outfitLevel.Images16.Count - 1);
                pageMap[tex.Atlas] = newPage;
            }

            tex.Atlas = newPage;
            texRemap[face.Texture] = (ushort)outfitLevel.ObjectTextures.Count;
            outfitLevel.ObjectTextures.Add(tex);
        }

        foreach (var face in mesh.TexturedFaces)
        {
            face.Texture = texRemap[face.Texture];
        }

        outfitLevel.Models[(TR2Type)extrasModelId].Meshes.Add(mesh);
    }
}
