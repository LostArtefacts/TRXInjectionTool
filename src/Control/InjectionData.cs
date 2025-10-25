using TRImageControl;
using TRLevelReader;
using TRLevelReader.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Applicability;
using LC = TRLevelControl;

namespace TRXInjectionTool.Control;

public class InjectionData
{
    public string Name { get; set; }
    public InjectionType InjectionType { get; set; }
    public LC.Model.TRGameVersion GameVersion { get; set; }
    public List<ApplicabilityTest> ApplicabilityTests { get; set; } = [];
    public List<LC.Model.TRTexImage32> Images { get; set; } = [];
    public List<LC.Model.TRTexImage8> Images8 { get; set; }
    public List<TRObjectTexture> ObjectTextures { get; set; } = [];
    public List<TRSpriteSequence> SpriteSequences { get; set; } = [];
    public List<TRSpriteTexture> SpriteTextures { get; set; } = [];
    public List<TRMesh> Meshes { get; set; } = [];
    public List<uint> MeshPointers { get; set; } = [];
    public List<TRStateChange> AnimChanges { get; set; } = [];
    public List<TRAnimDispatch> AnimDispatches { get; set; } = [];
    public List<TRAnimCommand> AnimCommands { get; set; } = [];
    public List<TRMeshTreeNode> MeshTrees { get; set; } = [];
    public List<TRAnimation> Animations { get; set; } = [];
    public List<ushort> AnimFrames { get; set; } = [];
    public List<TRModel> Models { get; set; } = [];
    public List<TRStaticMesh> StaticObjects { get; set; } = [];
    public List<TRColour> Palette { get; set; } = [];
    public List<TRSFXData> SFX { get; set; } = [];
    public List<TRCinematicFrame> CinematicFrames { get; set; } = [];
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

    private readonly HashSet<uint> _meshOnlyModels = [];

    private InjectionData() { }

    public void SetMeshOnlyModel(uint id)
        => _meshOnlyModels.Add(id);

    public bool IsMeshOnlyModel(uint id)
        => _meshOnlyModels.Contains(id);

    public static InjectionData Create(LC.Model.TRGameVersion version, InjectionType type, string name)
    {
        InjectionData data = new()
        {
            InjectionType = type,
            Name = name,
            GameVersion = version,
        };

        return data;
    }

    public static InjectionData Create(LC.Model.TRLevelBase controlledLevel, InjectionType type, string name, bool removeMeshData = false)
    {
        if (controlledLevel is LC.Model.TR1Level level1)
        {
            return Create(level1, type, name, removeMeshData);
        }
        else if (controlledLevel is LC.Model.TR2Level level2)
        {
            return Create(level2, type, name, removeMeshData);
        }
        throw new Exception("Only TR1 and TR2 levels supported");
    }

    public static InjectionData Create(LC.Model.TR1Level controlledLevel, InjectionType type, string name, bool removeMeshData = false)
    {
        // We convert to old-style flat level to simplify export later.
        // TODO: update old lib to support reading from memory
        new LC.TR1LevelControl().Write(controlledLevel, "temp.phd");
        TRLevel level = new TR1LevelReader().ReadLevel("temp.phd");
        File.Delete("temp.phd");

        short[] sounds = Array.FindAll(level.SoundMap, s => s != -1);
        if (type == InjectionType.LaraAnims)
        {
            ResetLaraLevel(level);
        }

        if (removeMeshData)
        {
            RemoveMeshData(level);
        }

        InjectionData data = new()
        {
            InjectionType = type,
            GameVersion = LC.Model.TRGameVersion.TR1,
            Name = name,
            Animations = [.. level.Animations],
            AnimChanges = [.. level.StateChanges],
            AnimCommands = [.. level.AnimCommands],
            AnimDispatches = [.. level.AnimDispatches],
            AnimFrames = [.. level.Frames],
            Images = Convert(level.Images8, level.Palette),
            Meshes = [.. level.Meshes],
            MeshPointers = [.. level.MeshPointers],
            MeshTrees = [.. level.MeshTrees],
            Models = [.. level.Models],
            StaticObjects = [.. level.StaticMeshes],
            ObjectTextures = [.. level.ObjectTextures],
            Palette = [.. level.Palette.Select(c =>
            {
                return new TRColour
                {
                    Red = (byte)(c.Red * 4),
                    Green = (byte)(c.Green * 4),
                    Blue = (byte)(c.Blue * 4),
                };
            })],
            SpriteSequences = [.. level.SpriteSequences],
            SpriteTextures = [.. level.SpriteTextures],
            CinematicFrames = [.. level.CinematicFrames],
        };

        for (int i = 0; i < sounds.Length; i++)
        {
            short soundID = (short)Array.IndexOf(level.SoundMap, sounds[i]);
            TRSoundDetails details = level.SoundDetails[sounds[i]];
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
                uint nextIndex = sampleIndex == level.SampleIndices.Length - 1 ? (uint)level.Samples.Length : level.SampleIndices[sampleIndex + 1];
                data.SFX[i].Data.Add(GetSample(level.SampleIndices[sampleIndex], nextIndex, level.Samples));
            }
        }

        return data;
    }

    public static InjectionData Create(LC.Model.TR2Level controlledLevel, InjectionType type, string name, bool removeMeshData = false)
    {
        // We convert to old-style flat level to simplify export later.
        // TODO: update old lib to support reading from memory
        new LC.TR2LevelControl().Write(controlledLevel, "temp.tr2");
        TR2Level level = new TR2LevelReader().ReadLevel("temp.tr2");
        File.Delete("temp.tr2");

        if (type == InjectionType.LaraAnims)
        {
            ResetLaraLevel(level);
        }

        short[] sounds = Array.FindAll(level.SoundMap, s => s != -1);
        if (removeMeshData)
        {
            RemoveMeshData(level);
        }

        InjectionData data = new()
        {
            InjectionType = type,
            GameVersion = LC.Model.TRGameVersion.TR2,
            Name = name,
            Animations = [.. level.Animations],
            AnimChanges = [.. level.StateChanges],
            AnimCommands = [.. level.AnimCommands],
            AnimDispatches = [.. level.AnimDispatches],
            AnimFrames = [.. level.Frames],
            Images = Convert(level.Images16),
            Images8 = level.NumImages == 0 ? null : [.. level.Images8.Select(i => new LC.Model.TRTexImage8 { Pixels = i.Pixels })],
            Meshes = [.. level.Meshes],
            MeshPointers = [.. level.MeshPointers],
            MeshTrees = [.. level.MeshTrees],
            Models = [.. level.Models],
            StaticObjects = [.. level.StaticMeshes],
            ObjectTextures = [.. level.ObjectTextures],
            Palette = [.. level.Palette.Select(c =>
            {
                return new TRColour
                {
                    Red = (byte)(c.Red * 4),
                    Green = (byte)(c.Green * 4),
                    Blue = (byte)(c.Blue * 4),
                };
            })],
            SpriteSequences = [.. level.SpriteSequences],
            SpriteTextures = [.. level.SpriteTextures],
            CinematicFrames = [.. level.CinematicFrames],
        };

        for (int i = 0; i < sounds.Length; i++)
        {
            short soundID = (short)Array.IndexOf(level.SoundMap, sounds[i]);
            TRSoundDetails details = level.SoundDetails[sounds[i]];
            data.SFX.Add(new()
            {
                ID = soundID,
                Chance = details.Chance,
                Characteristics = details.Characteristics,
                Volume = details.Volume,
                SampleOffset = level.SampleIndices[details.Sample],
            });
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

    private static void ResetLaraLevel(TRLevel level)
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
            TRColour c = level.Palette[i];
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

    private static void ResetLaraLevel(TR2Level level)
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
            TRColour c = level.Palette[i];
            c.Red = c.Green = c.Blue = 0;
            TRColour4 c4 = level.Palette16[i];
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

    private static void RemoveMeshData(TRLevel level)
    {
        // The game will detect that there is no mesh data associated with this injection
        // and hence retain the existing mesh data from the original level.
        level.NumMeshData = 0;
        level.Meshes = [];
        level.NumMeshPointers = 0;
        level.MeshPointers = [];
        level.NumMeshTrees = 0;
        level.MeshTrees = [];

        foreach (TRModel model in level.Models)
        {
            model.MeshTree = 0;
            model.StartingMesh = 0;
            model.NumMeshes = 0;
        }
    }

    private static void RemoveMeshData(TR2Level level)
    {
        // The game will detect that there is no mesh data associated with this injection
        // and hence retain the existing mesh data from the original level.
        level.NumMeshData = 0;
        level.Meshes = [];
        level.NumMeshPointers = 0;
        level.MeshPointers = [];
        level.NumMeshTrees = 0;
        level.MeshTrees = [];

        foreach (TRModel model in level.Models)
        {
            model.MeshTree = 0;
            model.StartingMesh = 0;
            model.NumMeshes = 0;
        }
    }

    private static List<LC.Model.TRTexImage32> Convert(TRTexImage8[] originalImages, TRColour[] originalPalette)
    {
        List<LC.Model.TRColour> palette = [.. originalPalette.Select(p => new LC.Model.TRColour
        {
            Red = (byte)(p.Red * 4),
            Blue = (byte)(p.Blue * 4),
            Green = (byte)(p.Green * 4),
        })];

        List<LC.Model.TRTexImage32> images = [];
        foreach (TRTexImage8 img8 in originalImages)
        {
            TRImage image = new(img8.Pixels, palette);
            images.Add(new()
            {
                Pixels = image.ToRGBA(),
            });
        }
        return images;
    }

    private static List<LC.Model.TRTexImage32> Convert(TRTexImage16[] originalImages)
    {
        return [.. originalImages.Select(i => new LC.Model.TRTexImage32 { Pixels = new TRImage(i.Pixels).ToRGBA() })];
    }
}
