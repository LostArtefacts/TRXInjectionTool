using TRImageControl;
using TRLevelControl;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Applicability;
using TRXInjectionTool.Model;
using LR = TRLevelReader;

namespace TRXInjectionTool.Control;

public class InjectionData
{
    public string Name { get; set; }
    public InjectionType InjectionType { get; set; }
    public TRGameVersion GameVersion { get; set; }
    public List<ApplicabilityTest> ApplicabilityTests { get; set; } = [];
    public List<TRTexImage32> Images { get; set; } = [];
    public List<TRTexImage8> Images8 { get; set; }
    public List<TRFlatObjectTexture> ObjectTextures { get; set; } = [];
    public List<LR.Model.TRSpriteSequence> SpriteSequences { get; set; } = [];
    public List<LR.Model.TRSpriteTexture> SpriteTextures { get; set; } = [];
    public List<TRFlatMesh> Meshes { get; set; } = [];
    public List<uint> MeshPointers { get; set; } = [];
    public List<LR.Model.TRStateChange> AnimChanges { get; set; } = [];
    public List<LR.Model.TRAnimDispatch> AnimDispatches { get; set; } = [];
    public List<LR.Model.TRAnimCommand> AnimCommands { get; set; } = [];
    public List<LR.Model.TRMeshTreeNode> MeshTrees { get; set; } = [];
    public List<TRFlatAnimation> Animations { get; set; } = [];
    public List<ushort> AnimFrames { get; set; } = [];
    public List<LR.Model.TRModel> Models { get; set; } = [];
    public List<LR.Model.TRStaticMesh> StaticObjects { get; set; } = [];
    public List<LR.Model.TRColour> Palette { get; set; } = [];
    public List<TRSFXData> SFX { get; set; } = [];
    public List<LR.Model.TRCinematicFrame> CinematicFrames { get; set; } = [];
    public List<TRMeshEdit> MeshEdits { get; set; } = [];
    public List<TRStaticMeshEdit> StaticMeshEdits { get; set; } = [];
    public List<TRTextureOverwrite> TextureOverwrites { get; set; } = [];
    public List<TRFloorDataEdit> FloorEdits { get; set; } = [];
    public List<TRRoomTextureEdit> RoomEdits { get; set; } = [];
    public List<TRVisPortalEdit> VisPortalEdits { get; set; } = [];
    public List<TRItemPosEdit> ItemPosEdits { get; set; } = [];
    public List<TRItemTypeFlagEdit> ItemFlagEdits { get; set; } = [];
    public List<TRFrameRotEdit> FrameEdits { get; set; } = [];
    public List<TRFrameReplacement> FrameReplacements { get; set; } = [];
    public List<TRAnimCmdEdit> AnimCmdEdits { get; set; } = [];
    public List<TRCameraEdit> CameraEdits { get; set; } = [];
    public List<TRSpriteEdit> SpriteEdits { get; set; } = [];
    public List<TRObjectTypeEdit> ObjectTypeEdits { get; set; } = [];
    public List<TRObjectLinkEdit> ObjectLinkEdits { get; set; } = [];
    public List<TRAnimEdit> AnimEdits { get; set; } = [];
    public List<TRAnimTextureEdit> AnimTextureEdits { get; set; } = [];
    public List<TRItemNameEdit> ItemNameEdits { get; set; } = [];

    private readonly HashSet<uint> _meshOnlyModels = [];

    private InjectionData() { }

    public void SetMeshOnlyModel(uint id)
        => _meshOnlyModels.Add(id);

    public bool IsMeshOnlyModel(uint id)
        => _meshOnlyModels.Contains(id);

    public static InjectionData Create(TRGameVersion version, InjectionType type, string name)
    {
        InjectionData data = new()
        {
            InjectionType = type,
            Name = name,
            GameVersion = version,
        };

        return data;
    }

    public static InjectionData Create(TRLevelBase controlledLevel, InjectionType type, string name, bool removeMeshData = false)
    {
        if (controlledLevel is TR1Level level1)
        {
            return Create(level1, type, name, removeMeshData);
        }
        else if (controlledLevel is TR2Level level2)
        {
            return Create(level2, type, name, removeMeshData);
        }
        else if (controlledLevel is TR3Level level3)
        {
            return Create(level3, type, name, removeMeshData);
        }
        else if (controlledLevel is TR4Level level4)
        {
            return Create(level4, type, name, removeMeshData);
        }
        throw new Exception("Only TR1-4 levels supported");
    }

    public static InjectionData Create(TR1Level controlledLevel, InjectionType type, string name, bool removeMeshData = false)
    {
        // We convert to old-style flat level to simplify export later.
        new TR1LevelControl().Write(controlledLevel, "temp.phd");
        var flatLevel = new LR.TR1LevelReader().ReadLevel("temp.phd");
        File.Delete("temp.phd");

        short[] sounds = Array.FindAll(flatLevel.SoundMap, s => s != -1);
        if (type == InjectionType.LaraAnims)
        {
            ResetLaraLevel(flatLevel);
        }

        if (removeMeshData)
        {
            RemoveMeshData(flatLevel);
        }

        InjectionData data = new()
        {
            InjectionType = type,
            GameVersion = TRGameVersion.TR1,
            Name = name,
            Animations = Convert(flatLevel.Animations),
            AnimChanges = [.. flatLevel.StateChanges],
            AnimCommands = [.. flatLevel.AnimCommands],
            AnimDispatches = [.. flatLevel.AnimDispatches],
            AnimFrames = [.. flatLevel.Frames],
            Images = Convert(flatLevel.Images8, flatLevel.Palette),
            Meshes = Convert(flatLevel.Meshes, TRGameVersion.TR1),
            MeshPointers = [.. flatLevel.MeshPointers],
            MeshTrees = [.. flatLevel.MeshTrees],
            Models = [.. flatLevel.Models],
            StaticObjects = [.. flatLevel.StaticMeshes],
            ObjectTextures = Convert(flatLevel.ObjectTextures),
            Palette = [.. flatLevel.Palette.Select(c =>
            {
                return new LR.Model.TRColour
                {
                    Red = (byte)(c.Red * 4),
                    Green = (byte)(c.Green * 4),
                    Blue = (byte)(c.Blue * 4),
                };
            })],
            SpriteSequences = [.. flatLevel.SpriteSequences],
            SpriteTextures = [.. flatLevel.SpriteTextures],
            CinematicFrames = [.. flatLevel.CinematicFrames],
        };

        for (int i = 0; i < sounds.Length; i++)
        {
            var soundID = (short)Array.IndexOf(flatLevel.SoundMap, sounds[i]);
            var details = flatLevel.SoundDetails[sounds[i]];
            data.SFX.Add(new()
            {
                ID = soundID,
                Chance = details.Chance,
                Characteristics = details.Characteristics,
                Volume = details.Volume,
                Data = [],
            });

            for (int j = 0; j < details.NumSounds; j++)
            {
                ushort sampleIndex = (ushort)(details.Sample + j);
                uint nextIndex = sampleIndex == flatLevel.SampleIndices.Length - 1 ? (uint)flatLevel.Samples.Length : flatLevel.SampleIndices[sampleIndex + 1];
                data.SFX[i].Data.Add(GetSample(flatLevel.SampleIndices[sampleIndex], nextIndex, flatLevel.Samples));
            }
        }

        return data;
    }

    public static InjectionData Create(TR2Level controlledLevel, InjectionType type, string name, bool removeMeshData = false)
    {
        new TR2LevelControl().Write(controlledLevel, "temp.tr2");
        var flatLevel = new LR.TR2LevelReader().ReadLevel("temp.tr2");
        File.Delete("temp.tr2");

        if (type == InjectionType.LaraAnims)
        {
            ResetLaraLevel(flatLevel);
        }

        short[] sounds = Array.FindAll(flatLevel.SoundMap, s => s != -1);
        if (removeMeshData)
        {
            RemoveMeshData(flatLevel);
        }

        InjectionData data = new()
        {
            InjectionType = type,
            GameVersion = TRGameVersion.TR2,
            Name = name,
            Animations = Convert(flatLevel.Animations),
            AnimChanges = [.. flatLevel.StateChanges],
            AnimCommands = [.. flatLevel.AnimCommands],
            AnimDispatches = [.. flatLevel.AnimDispatches],
            AnimFrames = [.. flatLevel.Frames],
            Images = Convert(flatLevel.Images16),
            Images8 = flatLevel.NumImages == 0 ? null : [.. flatLevel.Images8.Select(i => new TRTexImage8 { Pixels = i.Pixels })],
            Meshes = Convert(flatLevel.Meshes, TRGameVersion.TR2),
            MeshPointers = [.. flatLevel.MeshPointers],
            MeshTrees = [.. flatLevel.MeshTrees],
            Models = [.. flatLevel.Models],
            StaticObjects = [.. flatLevel.StaticMeshes],
            ObjectTextures = Convert(flatLevel.ObjectTextures),
            Palette = [.. flatLevel.Palette.Select(c =>
            {
                return new LR.Model.TRColour
                {
                    Red = (byte)(c.Red * 4),
                    Green = (byte)(c.Green * 4),
                    Blue = (byte)(c.Blue * 4),
                };
            })],
            SpriteSequences = [.. flatLevel.SpriteSequences],
            SpriteTextures = [.. flatLevel.SpriteTextures],
            CinematicFrames = [.. flatLevel.CinematicFrames],
        };

        for (int i = 0; i < sounds.Length; i++)
        {
            short soundID = (short)Array.IndexOf(flatLevel.SoundMap, sounds[i]);
            var details = flatLevel.SoundDetails[sounds[i]];
            data.SFX.Add(new()
            {
                ID = soundID,
                Chance = details.Chance,
                Characteristics = details.Characteristics,
                Volume = details.Volume,
                SampleOffset = flatLevel.SampleIndices[details.Sample],
            });
        }

        return data;
    }

    public static InjectionData Create(TR3Level controlledLevel, InjectionType type, string name, bool removeMeshData = false)
    {
        new TR3LevelControl().Write(controlledLevel, "temp.tr2");
        var flatLevel = new LR.TR3LevelReader().ReadLevel("temp.tr2");
        File.Delete("temp.tr2");

        if (type == InjectionType.LaraAnims)
        {
            ResetLaraLevel(flatLevel);
        }

        short[] sounds = Array.FindAll(flatLevel.SoundMap, s => s != -1);
        if (removeMeshData)
        {
            RemoveMeshData(flatLevel);
        }

        InjectionData data = new()
        {
            InjectionType = type,
            GameVersion = TRGameVersion.TR3,
            Name = name,
            Animations = Convert(flatLevel.Animations),
            AnimChanges = [.. flatLevel.StateChanges],
            AnimCommands = [.. flatLevel.AnimCommands],
            AnimDispatches = [.. flatLevel.AnimDispatches],
            AnimFrames = [.. flatLevel.Frames],
            Images = Convert(flatLevel.Images16),
            Images8 = flatLevel.NumImages == 0 ? null : [.. flatLevel.Images8.Select(i => new TRTexImage8 { Pixels = i.Pixels })],
            Meshes = Convert(flatLevel.Meshes, TRGameVersion.TR3),
            MeshPointers = [.. flatLevel.MeshPointers],
            MeshTrees = [.. flatLevel.MeshTrees],
            Models = [.. flatLevel.Models],
            StaticObjects = [.. flatLevel.StaticMeshes],
            ObjectTextures = Convert(flatLevel.ObjectTextures),
            Palette = [.. flatLevel.Palette.Select(c =>
            {
                return new LR.Model.TRColour
                {
                    Red = (byte)(c.Red * 4),
                    Green = (byte)(c.Green * 4),
                    Blue = (byte)(c.Blue * 4),
                };
            })],
            SpriteSequences = [.. flatLevel.SpriteSequences],
            SpriteTextures = [.. flatLevel.SpriteTextures],
            CinematicFrames = [.. flatLevel.CinematicFrames],
        };

        for (int i = 0; i < sounds.Length; i++)
        {
            short soundID = (short)Array.IndexOf(flatLevel.SoundMap, sounds[i]);
            var details = flatLevel.SoundDetails[sounds[i]];
            data.SFX.Add(new()
            {
                ID = soundID,
                Chance = details.Chance,
                Characteristics = (ushort)details.Characteristics,
                Volume = (ushort)(details.Volume << 7),
                Pitch = details.Pitch,
                Range = details.Range,
                SampleOffset = flatLevel.SampleIndices[details.Sample],
            });
        }

        return data;
    }

    public static InjectionData Create(TR4Level controlledLevel, InjectionType type, string name, bool removeMeshData = false)
    {
        new TR4LevelControl().Write(controlledLevel, "temp.tr4");
        var flatLevel = new LR.TR4LevelReader().ReadLevel("temp.tr4");
        File.Delete("temp.tr4");

        if (type == InjectionType.LaraAnims)
        {
            ResetLaraLevel(flatLevel);
        }

        var sounds = Array.FindAll(flatLevel.LevelDataChunk.SoundMap, s => s != -1);
        if (removeMeshData)
        {
            RemoveMeshData(flatLevel);
        }

        InjectionData data = new()
        {
            InjectionType = type,
            GameVersion = TRGameVersion.TR4,
            Name = name,
            Animations = Convert(flatLevel.LevelDataChunk.Animations),
            AnimChanges = [.. flatLevel.LevelDataChunk.StateChanges],
            AnimCommands = [.. flatLevel.LevelDataChunk.AnimCommands],
            AnimDispatches = [.. flatLevel.LevelDataChunk.AnimDispatches],
            AnimFrames = [.. flatLevel.LevelDataChunk.Frames],
            Images = Convert(flatLevel.Texture32Chunk.Textiles),
            Images8 = flatLevel.Texture32Chunk.Textiles.Length == 0 ? null : [.. flatLevel.Texture32Chunk.Textiles.Select(i => new TRTexImage8 { Pixels = new byte[256 * 256] })],
            Meshes = Convert(flatLevel.LevelDataChunk.Meshes),
            MeshPointers = [.. flatLevel.LevelDataChunk.MeshPointers],
            MeshTrees = [.. flatLevel.LevelDataChunk.MeshTrees],
            Models = [.. flatLevel.LevelDataChunk.Models],
            Palette = [.. Enumerable.Repeat(0, 256).Select(i => new LR.Model.TRColour())],
            StaticObjects = [.. flatLevel.LevelDataChunk.StaticMeshes],
            ObjectTextures = Convert(flatLevel.LevelDataChunk.ObjectTextures),
            SpriteSequences = [.. flatLevel.LevelDataChunk.SpriteSequences],
            SpriteTextures = [.. flatLevel.LevelDataChunk.SpriteTextures],
        };

        for (int i = 0; i < sounds.Length; i++)
        {
            var soundID = (short)Array.IndexOf(flatLevel.LevelDataChunk.SoundMap, sounds[i]);
            var details = flatLevel.LevelDataChunk.SoundDetails[sounds[i]];
            data.SFX.Add(new()
            {
                ID = soundID,
                Chance = details.Chance,
                Characteristics = (ushort)details.Characteristics,
                Volume = (ushort)(details.Volume << 7),
                Pitch = details.Pitch,
                Range = details.Range,
                Data = [],
            });

            var rawSamples = flatLevel.Samples.SelectMany(s => s.SoundData).ToArray();
            for (int j = 0; j < details.NumSounds; j++)
            {
                ushort sampleIndex = (ushort)(details.Sample + j);
                uint nextIndex = sampleIndex == flatLevel.LevelDataChunk.SampleIndices.Length - 1
                    ? (uint)flatLevel.Samples.Length
                    : flatLevel.LevelDataChunk.SampleIndices[sampleIndex + 1];
                data.SFX[i].Data.Add(GetSample(flatLevel.LevelDataChunk.SampleIndices[sampleIndex], nextIndex, rawSamples));
            }
        }

        return data;
    }

    static byte[] GetSample(uint offset, uint endOffset, byte[] wavSamples)
    {
        List<byte> data = [];
        for (uint i = offset; i < endOffset; i++)
        {
            data.Add(wavSamples[i]);
        }
        return [.. data];
    }

    private static void ResetLaraLevel(LR.Model.TRLevel level)
    {
        level.NumMeshData = 0;
        level.Meshes = [];
        level.NumMeshPointers = 0;
        level.MeshPointers = [];
        level.NumMeshTrees = 0;
        level.MeshTrees = [];
        level.Images8 = [];
        level.NumImages = 0;
        level.ObjectTextures = [];
        level.NumObjectTextures = 0;
        for (int i = 0; i < 256; i++)
        {
            var c = level.Palette[i];
            c.Red = c.Green = c.Blue = 0;
        }
        level.NumMeshData = 0;
        level.Meshes = [];
        level.NumMeshPointers = 0;
        level.MeshPointers = [];
        level.NumMeshTrees = 0;
        level.MeshTrees = [];
        level.Models[0].MeshTree = 0;
        level.Models[0].StartingMesh = 0;
        level.Models[0].NumMeshes = 0;
    }

    private static void ResetLaraLevel(LR.Model.TR2Level level)
    {
        level.NumMeshData = 0;
        level.Meshes = [];
        level.NumMeshPointers = 0;
        level.MeshPointers = [];
        level.NumMeshTrees = 0;
        level.MeshTrees = [];
        level.Images8 = [];
        level.Images16 = [];
        level.NumImages = 0;
        level.ObjectTextures = [];
        level.NumObjectTextures = 0;
        for (int i = 0; i < 256; i++)
        {
            var c = level.Palette[i];
            c.Red = c.Green = c.Blue = 0;
            var c4 = level.Palette16[i];
            c4.Unused = c4.Red = c4.Green = c4.Blue = 0;
        }
        level.NumMeshData = 0;
        level.Meshes = [];
        level.NumMeshPointers = 0;
        level.MeshPointers = [];
        level.NumMeshTrees = 0;
        level.MeshTrees = [];
        level.Models[0].MeshTree = 0;
        level.Models[0].StartingMesh = 0;
        level.Models[0].NumMeshes = 0;
    }

    private static void ResetLaraLevel(LR.Model.TR3Level level)
    {
        level.NumMeshData = 0;
        level.Meshes = [];
        level.NumMeshPointers = 0;
        level.MeshPointers = [];
        level.NumMeshTrees = 0;
        level.MeshTrees = [];
        level.Images8 = [];
        level.Images16 = [];
        level.NumImages = 0;
        level.ObjectTextures = [];
        level.NumObjectTextures = 0;
        for (int i = 0; i < 256; i++)
        {
            var c = level.Palette[i];
            c.Red = c.Green = c.Blue = 0;
            var c4 = level.Palette16[i];
            c4.Unused = c4.Red = c4.Green = c4.Blue = 0;
        }
        level.NumMeshData = 0;
        level.Meshes = [];
        level.NumMeshPointers = 0;
        level.MeshPointers = [];
        level.NumMeshTrees = 0;
        level.MeshTrees = [];
        level.Models[0].MeshTree = 0;
        level.Models[0].StartingMesh = 0;
        level.Models[0].NumMeshes = 0;
    }

    private static void ResetLaraLevel(LR.Model.TR4Level level)
    {
        level.LevelDataChunk.NumMeshData = 0;
        level.LevelDataChunk.Meshes = [];
        level.LevelDataChunk.NumMeshPointers = 0;
        level.LevelDataChunk.MeshPointers = [];
        level.LevelDataChunk.NumMeshTrees = 0;
        level.LevelDataChunk.MeshTrees = [];
        level.Texture32Chunk.Textiles = [];
        level.LevelDataChunk.ObjectTextures = [];
        level.LevelDataChunk.NumObjectTextures = 0;
        level.LevelDataChunk.NumMeshData = 0;
        level.LevelDataChunk.Meshes = [];
        level.LevelDataChunk.NumMeshPointers = 0;
        level.LevelDataChunk.MeshPointers = [];
        level.LevelDataChunk.NumMeshTrees = 0;
        level.LevelDataChunk.MeshTrees = [];
        level.LevelDataChunk.Models[0].MeshTree = 0;
        level.LevelDataChunk.Models[0].StartingMesh = 0;
        level.LevelDataChunk.Models[0].NumMeshes = 0;
    }

    private static void RemoveMeshData(LR.Model.TRLevel level)
    {
        // The game will detect that there is no mesh data associated with this injection
        // and hence retain the existing mesh data from the original level.
        level.NumMeshData = 0;
        level.Meshes = [];
        level.NumMeshPointers = 0;
        level.MeshPointers = [];
        level.NumMeshTrees = 0;
        level.MeshTrees = [];

        foreach (var model in level.Models)
        {
            model.MeshTree = 0;
            model.StartingMesh = 0;
            model.NumMeshes = 0;
        }
    }

    private static void RemoveMeshData(LR.Model.TR2Level level)
    {
        // The game will detect that there is no mesh data associated with this injection
        // and hence retain the existing mesh data from the original level.
        level.NumMeshData = 0;
        level.Meshes = [];
        level.NumMeshPointers = 0;
        level.MeshPointers = [];
        level.NumMeshTrees = 0;
        level.MeshTrees = [];

        foreach (var model in level.Models)
        {
            model.MeshTree = 0;
            model.StartingMesh = 0;
            model.NumMeshes = 0;
        }
    }

    private static void RemoveMeshData(LR.Model.TR3Level level)
    {
        // The game will detect that there is no mesh data associated with this injection
        // and hence retain the existing mesh data from the original level.
        level.NumMeshData = 0;
        level.Meshes = [];
        level.NumMeshPointers = 0;
        level.MeshPointers = [];
        level.NumMeshTrees = 0;
        level.MeshTrees = [];

        foreach (var model in level.Models)
        {
            model.MeshTree = 0;
            model.StartingMesh = 0;
            model.NumMeshes = 0;
        }
    }

    private static void RemoveMeshData(LR.Model.TR4Level level)
    {
        level.LevelDataChunk.NumMeshData = 0;
        level.LevelDataChunk.Meshes = [];
        level.LevelDataChunk.NumMeshPointers = 0;
        level.LevelDataChunk.MeshPointers = [];
        level.LevelDataChunk.NumMeshTrees = 0;
        level.LevelDataChunk.MeshTrees = [];

        foreach (var model in level.LevelDataChunk.Models)
        {
            model.MeshTree = 0;
            model.StartingMesh = 0;
            model.NumMeshes = 0;
        }
    }

    private static List<TRTexImage32> Convert(LR.Model.TRTexImage8[] originalImages, LR.Model.TRColour[] originalPalette)
    {
        List<TRColour> palette = [.. originalPalette.Select(p => new TRColour
        {
            Red = (byte)(p.Red * 4),
            Blue = (byte)(p.Blue * 4),
            Green = (byte)(p.Green * 4),
        })];

        List<TRTexImage32> images = [];
        foreach (var img8 in originalImages)
        {
            TRImage image = new(img8.Pixels, palette);
            images.Add(new()
            {
                Pixels = image.ToRGBA(),
            });
        }
        return images;
    }

    private static List<TRTexImage32> Convert(LR.Model.TRTexImage16[] originalImages)
    {
        return [.. originalImages.Select(i => new TRTexImage32 { Pixels = new TRImage(i.Pixels).ToRGBA() })];
    }

    private static List<TRTexImage32> Convert(LR.Model.TR4TexImage32[] originalImages)
    {
        return [.. originalImages.Select(i => new TRTexImage32 { Pixels = new TRImage(i.Tile).ToRGBA() })];
    }

    private static List<TRFlatAnimation> Convert(LR.Model.TRAnimation[] animations)
    {
        return [.. animations.Select(a => new TRFlatAnimation
        {
            Accel = a.Accel,
            AnimCommand = a.AnimCommand,
            FrameEnd = a.FrameEnd,
            FrameStart = a.FrameStart,
            FrameOffset = a.FrameOffset,
            FrameRate = a.FrameRate,
            FrameSize = a.FrameSize,
            NextAnimation = a.NextAnimation,
            NextFrame = a.NextFrame,
            NumAnimCommands = a.NumAnimCommands,
            NumStateChanges = a.NumStateChanges,
            Speed = a.Speed,
            StateChangeOffset = a.StateChangeOffset,
            StateID = a.StateID,
        })];
    }

    private static List<TRFlatAnimation> Convert(LR.Model.TR4Animation[] animations)
    {
        return [.. animations.Select(a => new TRFlatAnimation
        {
            Accel = a.Accel,
            AnimCommand = a.AnimCommand,
            FrameEnd = a.FrameEnd,
            FrameStart = a.FrameStart,
            FrameOffset = a.FrameOffset,
            FrameRate = a.FrameRate,
            FrameSize = a.FrameSize,
            LateralAccel = a.AccelLateral,
            LateralSpeed = a.SpeedLateral,
            NextAnimation = a.NextAnimation,
            NextFrame = a.NextFrame,
            NumAnimCommands = a.NumAnimCommands,
            NumStateChanges = a.NumStateChanges,
            Speed = a.Speed,
            StateChangeOffset = a.StateChangeOffset,
            StateID = a.StateID,
        })];
    }

    private static List<TRFlatMesh> Convert(LR.Model.TRMesh[] meshes, TRGameVersion version)
    {
        return [.. meshes.Select(m => new TRFlatMesh
        {
            Centre = m.Centre,
            CollRadius = m.CollRadius,
            ColouredRectangles = Convert(m.ColouredRectangles, version),
            ColouredTriangles = Convert(m.ColouredTriangles, version),
            Lights = m.NumNormals <= 0 ? [.. m.Lights] : null,
            Normals = m.NumNormals > 0 ? [.. m.Normals] : null,
            TexturedRectangles = Convert(m.TexturedRectangles, version),
            TexturedTriangles = Convert(m.TexturedTriangles, version),
            Vertices = [.. m.Vertices],
        })];
    }

    private static List<TRFlatMesh> Convert(LR.Model.TR4Mesh[] meshes)
    {
        return [.. meshes.Select(m => new TRFlatMesh
        {
            Centre = m.Centre,
            CollRadius = m.CollRadius,
            Lights = m.NumNormals <= 0 ? [.. m.Lights] : null,
            Normals = m.NumNormals > 0 ? [.. m.Normals] : null,
            TexturedRectangles = Convert(m.TexturedRectangles),
            TexturedTriangles = Convert(m.TexturedTriangles),
            Vertices = [.. m.Vertices],
        })];
    }

    private static List<TRMeshFace> Convert(LR.Model.TRFace4[] faces, TRGameVersion version)
    {
        var result = faces.Select(f => new TRMeshFace
        {
            Texture = f.Texture,
            Vertices = [.. f.Vertices],
            Type = TRFaceType.Rectangle,
        }).ToList();
        AdjustFaces(result, version);
        return [.. result];
    }

    private static List<TRMeshFace> Convert(LR.Model.TRFace3[] faces, TRGameVersion version)
    {
        var result = faces.Select(f => new TRMeshFace
        {
            Texture = f.Texture,
            Vertices = [.. f.Vertices],
            Type = TRFaceType.Triangle,
        }).ToList();
        AdjustFaces(result, version);
        return [.. result];
    }

    private static List<TRMeshFace> Convert(LR.Model.TR4MeshFace4[] faces)
    {
        var result = faces.Select(f => new TRMeshFace
        {
            Texture = f.Texture,
            Vertices = [.. f.Vertices],
            Type = TRFaceType.Rectangle,
            Effects = f.Effects,
        }).ToList();
        AdjustFaces(result, TRGameVersion.TR4);
        return [.. result];
    }

    private static List<TRMeshFace> Convert(LR.Model.TR4MeshFace3[] faces)
    {
        var result = faces.Select(f => new TRMeshFace
        {
            Texture = f.Texture,
            Vertices = [.. f.Vertices],
            Type = TRFaceType.Triangle,
            Effects = f.Effects,
        }).ToList();
        AdjustFaces(result, TRGameVersion.TR4);
        return [.. result];
    }

    private static void AdjustFaces(List<TRMeshFace> faces, TRGameVersion version)
    {
        if (version < TRGameVersion.TR3)
        {
            return;
        }

        foreach (var face in faces)
        {
            var texture = face.Texture;
            face.Texture = (ushort)(texture & (version == TRGameVersion.TR5 ? 0x3FFF : 0x7FFF));
            face.DoubleSided = (texture & 0x8000) > 0;
            if (version == TRGameVersion.TR5)
            {
                face.UnknownFlag = (texture & 0x4000) > 0;
            }
        }
    }

    private static List<TRFlatObjectTexture> Convert(LR.Model.TRObjectTexture[] textures)
    {
        return [.. textures.Select(t => new TRFlatObjectTexture
        {
            Attribute = t.Attribute,
            TileAndFlag = t.AtlasAndFlag,
            Vertices = t.Vertices,
        })];
    }

    private static List<TRFlatObjectTexture> Convert(LR.Model.TR4ObjectTexture[] textures)
    {
        return [.. textures.Select(t => new TRFlatObjectTexture
        {
            Attribute = t.Attribute,
            TileAndFlag = t.TileAndFlag,
            Vertices = t.Vertices,
            HeightMinusOne = t.HeightMinusOne,
            NewFlags = t.NewFlags,
            OriginalU = t.OriginalU,
            OriginalV = t.OriginalV,
            WidthMinusOne = t.WidthMinusOne,
        })];
    }
}
