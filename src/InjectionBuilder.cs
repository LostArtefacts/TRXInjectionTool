using Newtonsoft.Json;
using System.Drawing;
using TRDataControl;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Applicability;
using TRXInjectionTool.Control;

namespace TRXInjectionTool;

public abstract class InjectionBuilder
{
    protected static readonly TR1LevelControl _control1 = new();
    protected static readonly TR2LevelControl _control2 = new();
    protected static readonly TR3LevelControl _control3 = new();

    public virtual string ID { get; } = string.Empty;

    public abstract List<InjectionData> Build();

    protected static void CreateDefaultTests(InjectionData data, string levelName)
    {
        int roomCount = -1;
        TRRoom room = null;

        if (data.GameVersion == TRGameVersion.TR1)
        {
            TR1Level level = _control1.Read($"Resources/{levelName}");
            roomCount = level.Rooms.Count;

            if (level.Entities.Count > 0)
            {
                TR1Entity item = level.Entities.Find(e => e.TypeID == TR1Type.Lara) ?? level.Entities[0];
                data.ApplicabilityTests.Add(new ItemMetaTest
                {
                    Index = level.Entities.IndexOf(item),
                    TypeID = (int)item.TypeID,
                    X = item.X,
                    Y = item.Y,
                    Z = item.Z,
                    Room = item.Room,
                    Angle = item.Angle,
                });
            }

            if (level.Rooms.Count > 0)
            {
                room = level.Rooms[0];
            }
        }
        else if (data.GameVersion == TRGameVersion.TR2)
        {
            TR2Level level = _control2.Read($"Resources/{levelName}");
            roomCount = level.Rooms.Count;

            if (level.Entities.Count > 0)
            {
                TR2Entity item = level.Entities.Find(e => e.TypeID == TR2Type.Lara) ?? level.Entities[0];
                data.ApplicabilityTests.Add(new ItemMetaTest
                {
                    Index = level.Entities.IndexOf(item),
                    TypeID = (int)item.TypeID,
                    X = item.X,
                    Y = item.Y,
                    Z = item.Z,
                    Room = item.Room,
                    Angle = item.Angle,
                });
            }

            if (level.Rooms.Count > 0)
            {
                room = level.Rooms[0];
            }
        }

        if (roomCount != -1)
        {
            data.ApplicabilityTests.Add(new RoomCountTest
            {
                RoomCount = roomCount,
            });
        }
        if (room != null)
        {
            data.ApplicabilityTests.Add(new RoomMetaTest
            {
                Index = 0,
                Info = room.Info,
                XSize = room.NumXSectors,
                ZSize = room.NumZSectors,
            });
        }
    }

    public static void ResetLevel(TR1Level level, uint texturePageCount = 0)
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
        level.CinematicFrames.Clear();

        for (int i = 0; i < 256; i++)
        {
            TRColour c = level.Palette[i];
            c.Red = c.Green = c.Blue = 0;
        }
    }

    public static void ResetLevel(TR2Level level, uint texturePageCount = 0)
    {
        level.Images8.Clear();
        level.Images16.Clear();
        for (int i = 0; i < texturePageCount; i++)
        {
            level.Images8.Add(new() { Pixels = new byte[256 * 256] });
            level.Images16.Add(new() { Pixels = new ushort[256 * 256] });
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
        level.CinematicFrames.Clear();

        for (int i = 0; i < 256; i++)
        {
            TRColour c8 = level.Palette[i];
            c8.Red = c8.Green = c8.Blue = 0;
            TRColour4 c16 = level.Palette16[i];
            c16.Red = c16.Green = c16.Blue = 0;
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

    protected static void CreateModelLevel(TR2Level level, params TR2Type[] types)
    {
        var models = new TRDictionary<TR2Type, TRModel>();
        foreach (TR2Type type in types)
        {
            models[type] = level.Models[type];
        }

        TR2DataProvider tr2Data = new();
        List<TR2SFX> soundIDs = new(types.SelectMany(t => tr2Data.GetHardcodedSounds(t)));
        
        soundIDs.AddRange(models.Values.SelectMany(m => m.Animations)
            .SelectMany(a => a.Commands.Where(c => c is TRSFXCommand))
            .Select(s => (TR2SFX)((TRSFXCommand)s).SoundID));

        var idsToPack = soundIDs
            .Where(s => level.SoundEffects.ContainsKey(s))
            .Distinct()
            .ToList();
        TRDictionary<TR2SFX, TR2SoundEffect> effects = new();
        idsToPack.ForEach(s => effects[s] = level.SoundEffects[s]);

        var packer = new TR2TexturePacker(level);
        var regions = packer.GetMeshRegions(models.Values.SelectMany(m => m.Meshes))
            .Values.SelectMany(v => v);
        var originalInfos = level.ObjectTextures.ToList();

        var basePalette = level.Palette.Select(c => c.ToTR1Color()).ToList();
        ResetLevel(level, 1);

        packer = new(level);
        packer.AddRectangles(regions);
        packer.Pack(true);

        level.Models = models;
        level.SoundEffects = effects;
        level.ObjectTextures.AddRange(regions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));
        models.Values
            .SelectMany(m => m.Meshes)
            .SelectMany(m => m.TexturedFaces)
            .Distinct()
            .ToList()
            .ForEach(f =>
            {
                f.Texture = (ushort)level.ObjectTextures.IndexOf(originalInfos[f.Texture]);
            });

        GenerateImages8(level, basePalette);
    }

    protected static TR1Level CreatePDALevel()
    {
        const TR1Type betaMapID = TR1Type.PushBlock4;
        var caves = _control1.Read($"Resources/TR1/1996-07-02.phd");
        CreateModelLevel(caves, betaMapID);
        caves.Models.ChangeKey(betaMapID, TR1Type.Map_M_U);

        // Repeat the frames in reverse so the map can close on exit. The open frame is also
        // slightly misaligned, so fix that.
        var anim = caves.Models[TR1Type.Map_M_U].Animations[0];
        anim.Frames[^1].Rotations[1].Z++;

        for (int i = anim.Frames.Count - 2; i >= 0; i--)
        {
            anim.Frames.Add(anim.Frames[i].Clone());
            anim.FrameEnd++;
        }
        return caves;
    }

    protected static TR2Level CreateWinstonLevel(string levelName)
    {
        TR2Level level = _control2.Read($"Resources/{levelName}");
        CreateModelLevel(level, TR2Type.Winston);

        // Fix Winston's nose
        var model = level.Models[TR2Type.Winston];
        model.Meshes[25].TexturedTriangles.Add(new()
        {
            Type = TRFaceType.Triangle,
            Vertices = new() { 24, 22, 25 },
            Texture = model.Meshes[25].TexturedRectangles[9].Texture,
        });

        return level;
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

    protected static void GenerateImages8(TR2Level level, List<Color> palette)
    {
        List<TRImage> imgs = level.Images16.Select(i => new TRImage(i.Pixels)).ToList();
        imgs.ForEach(i => i.Write((c, x, y) => AddColourToPalette(c, palette)));
        while (palette.Count < 256)
        {
            palette.Add(Color.Black);
        }

        level.Palette = palette.Select(c => c.ToTRColour()).ToList();
        level.Images8 = imgs.Select(i => new TRTexImage8 { Pixels = i.ToRGB(level.Palette) }).ToList();
    }

    protected static void GenerateImages8(TR3Level level, List<Color> palette)
    {
        List<TRImage> imgs = level.Images16.Select(i => new TRImage(i.Pixels)).ToList();
        imgs.ForEach(i => i.Write((c, x, y) => AddColourToPalette(c, palette)));
        while (palette.Count < 256)
        {
            palette.Add(Color.Black);
        }

        level.Palette = palette.Select(c => c.ToTRColour()).ToList();
        level.Images8 = imgs.Select(i => new TRTexImage8 { Pixels = i.ToRGB(level.Palette) }).ToList();
    }

    protected static Color AddColourToPalette(Color c, List<Color> palette, int limit = 256)
    {
        if (c.A == 0)
        {
            return c;
        }

        Color snapped = Color.FromArgb(c.R & ~3, c.G & ~3, c.B & ~3);
        if (!palette.Contains(snapped))
        {
            if (palette.Count < limit)
            {
                palette.Add(snapped);
            }
            else
            {
                snapped = palette[palette.FindClosest(snapped, 1)];
            }
        }

        return snapped;
    }

    public static TRAnimCmdEdit CreateAnimCmdEdit(TR2Level level, TR2Type type, int animIdx)
    {
        TRAnimation anim = level.Models[type].Animations[animIdx];
        int rawCount = 0;
        foreach (TRAnimCommand cmd in anim.Commands)
        {
            rawCount++;
            switch (cmd.Type)
            {
                case TRAnimCommandType.SetPosition:
                    rawCount += 3;
                    break;
                case TRAnimCommandType.JumpDistance:
                case TRAnimCommandType.FlipEffect:
                case TRAnimCommandType.PlaySound:
                    rawCount += 2;
                    break;
            }
        }

        return new()
        {
            AnimIndex = animIdx,
            TypeID = (short)type,
            RawCount = rawCount,
            TotalCount = anim.Commands.Count,
        };
    }

    protected static T DeserializeFile<T>(string path)
    {
        return Deserialize<T>(File.ReadAllText(path));
    }

    protected static T Deserialize<T>(string data)
    {
        return JsonConvert.DeserializeObject<T>(data);
    }

    public static string MakeOutputPath(InjectionData data)
    {
        return MakeOutputPath(data.GameVersion, $"{data.Name}.bin");
    }

    public static string MakeOutputPath(TRGameVersion version, string path)
    {
        string fullPath = $"Output/{version}/{path}";
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
        return fullPath;
    }

    protected static readonly Dictionary<string, string> _tr2NameMap = new()
    {
        [TR2LevelNames.ASSAULT] = "gym",
        [TR2LevelNames.GW] = "wall",
        [TR2LevelNames.VENICE] = "venice",
        [TR2LevelNames.BARTOLI] = "bartoli",
        [TR2LevelNames.OPERA] = "opera",
        [TR2LevelNames.RIG] = "rig",
        [TR2LevelNames.DA] = "diving",
        [TR2LevelNames.FATHOMS] = "fathoms",
        [TR2LevelNames.DORIA] = "wreck",
        [TR2LevelNames.LQ] = "living",
        [TR2LevelNames.DECK] = "deck",
        [TR2LevelNames.TIBET] = "tibet",
        [TR2LevelNames.MONASTERY] = "barkhang",
        [TR2LevelNames.COT] = "catacombs",
        [TR2LevelNames.CHICKEN] = "palace",
        [TR2LevelNames.XIAN] = "xian",
        [TR2LevelNames.FLOATER] = "floating",
        [TR2LevelNames.LAIR] = "lair",
        [TR2LevelNames.HOME] = "house",
        [TR2LevelNames.COLDWAR] = "coldwar",
        [TR2LevelNames.FOOLGOLD] = "fools",
        [TR2LevelNames.FURNACE] = "furnace",
        [TR2LevelNames.KINGDOM] = "kingdom",
        [TR2LevelNames.VEGAS] = "vegas",
    };
}
