using System.Drawing;
using TRDataControl;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool;

public abstract class InjectionBuilder
{
    protected static readonly TR1LevelControl _control1 = new();
    protected static readonly TR2LevelControl _control2 = new();
    protected static readonly TR3LevelControl _control3 = new();

    public abstract List<InjectionData> Build();

    protected static void ResetLevel(TR1Level level, uint texturePageCount = 0)
    {
        level.Images8.Clear();
        for (int i = 0; i < texturePageCount; i++)
        {
            level.Images8.Add(new() { Pixels = new byte[256 * 256] });
        }

        level.AnimatedTextures.Clear();
        level.ObjectTextures.Clear();
        level.Sprites.Clear();
        level.Models.Clear();
        level.SoundEffects.Clear();
        level.SoundSources.Clear();
        level.Rooms.Clear();
        level.StaticMeshes.Clear();
        level.Boxes.Clear();
        level.Entities.Clear();
        level.Cameras.Clear();

        for (int i = 0; i < 256; i++)
        {
            TRColour c = level.Palette[i];
            c.Red = c.Green = c.Blue = 0;
        }
    }

    protected static void CreateModelLevel(TR1Level level, params TR1Type[] types)
    {
        // Remove everything from the level bar the data related to the provided types.
        // Slight hack to export what we want, then re-import it into an empty level.
        TR1DataExporter exporter = new();
        foreach (TR1Type type in types)
        {
            exporter.Export(level, type);
        }

        ResetLevel(level, 1);
        TR1DataImporter importer = new()
        {
            Level = level,
            TypesToImport = new(types),
        };
        importer.Import();
    }

    protected static void PackTextures(TR1Level dataLevel, TRLevelBase sourceLevel, TRModel sky, Dictionary<string, string> regionMap)
    {
        TRTexturePacker sourcePacker;
        if (sourceLevel is TR2Level level2)
        {
            sourcePacker = new TR2TexturePacker(level2);
        }
        else if (sourceLevel is TR3Level level3)
        {
            sourcePacker = new TR3TexturePacker(level3);
        }
        else
        {
            throw new ArgumentException("Source level must be TR2 or TR3.");
        }

        TR1TexturePacker packer1 = new(dataLevel);
        IEnumerable<TRTextileRegion> regions = sourcePacker.GetMeshRegions(sky.Meshes)
            .Values.SelectMany(r => r);

        foreach (TRTextileRegion region in regions)
        {
            region.GenerateID();
            if (regionMap.ContainsKey(region.ID))
            {
                region.Image = new(regionMap[region.ID]);
            }
        }

        packer1.AddRectangles(regions);
        packer1.Pack(true);

        foreach (TRTextileSegment segment in regions.SelectMany(r => r.Segments))
        {
            ushort newTextureRef = (ushort)dataLevel.ObjectTextures.Count;
            dataLevel.ObjectTextures.Add(segment.Texture as TRObjectTexture);
            sky.Meshes.SelectMany(m => m.TexturedFaces.Where(f => f.Texture == (ushort)segment.Index))
                .ToList()
                .ForEach(f => f.Texture = newTextureRef);
        }
    }

    protected static List<TRColour> InitialisePalette8(TRModel model, List<TRColour4> palette16)
    {
        List<Color> palette = new();
        foreach (TRMeshFace face in model.Meshes.SelectMany(m => m.ColouredFaces))
        {
            Color c = palette16[face.Texture >> 8].ToColor();
            int i = palette.IndexOf(c);
            if (i == -1)
            {
                palette.Add(c);
            }
            face.Texture = (ushort)palette.Count;
        }

        palette.Insert(0, Color.Black);
        while (palette.Count < 256)
        {
            palette.Add(Color.Black);
        }

        return new(palette.Select(c => c.ToTRColour()));
    }
}
