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
    public List<ApplicabilityTest> ApplicabilityTests { get; set; } = new();
    public List<LC.Model.TRTexImage32> Images { get; set; } = new();
    public List<LC.Model.TRTexImage8> Images8 { get; set; }
    public List<TRObjectTexture> ObjectTextures { get; set; } = new();
    public List<TRSpriteSequence> SpriteSequences { get; set; } = new();
    public List<TRSpriteTexture> SpriteTextures { get; set; } = new();
    public List<TRMesh> Meshes { get; set; } = new();
    public List<uint> MeshPointers { get; set; } = new();
    public List<TRStateChange> AnimChanges { get; set; } = new();
    public List<TRAnimDispatch> AnimDispatches { get; set; } = new();
    public List<TRAnimCommand> AnimCommands { get; set; } = new();
    public List<TRMeshTreeNode> MeshTrees { get; set; } = new();
    public List<TRAnimation> Animations { get; set; } = new();
    public List<ushort> AnimFrames { get; set; } = new();
    public List<TRModel> Models { get; set; } = new();
    public List<TRColour> Palette { get; set; } = new();
    public List<TRSFXData> SFX { get; set; } = new();
    public List<TRMeshEdit> MeshEdits { get; set; } = new();
    public List<TRTextureOverwrite> TextureOverwrites { get; set; } = new();
    public List<TRFloorDataEdit> FloorEdits { get; set; } = new();
    public List<TRRoomTextureEdit> RoomEdits { get; set; } = new();
    public List<TRVisPortalEdit> VisPortalEdits { get; set; } = new();
    public List<TRItemEdit> ItemEdits { get; set; } = new();
    public List<TRFrameRotEdit> FrameEdits { get; set; } = new();
    public List<TRCameraEdit> CameraEdits { get; set; } = new();

    private InjectionData() { }

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
            Animations = level.Animations.ToList(),
            AnimChanges = level.StateChanges.ToList(),
            AnimCommands = level.AnimCommands.ToList(),
            AnimDispatches = level.AnimDispatches.ToList(),
            AnimFrames = level.Frames.ToList(),
            Images = Convert(level.Images8, level.Palette),
            Meshes = level.Meshes.ToList(),
            MeshPointers = level.MeshPointers.ToList(),
            MeshTrees = level.MeshTrees.ToList(),
            Models = level.Models.ToList(),
            ObjectTextures = level.ObjectTextures.ToList(),
            Palette = level.Palette.Select(c =>
            {
                return new TRColour
                {
                    Red = (byte)(c.Red * 4),
                    Green = (byte)(c.Green * 4),
                    Blue = (byte)(c.Blue * 4),
                };
            }).ToList(),
            SpriteSequences = level.SpriteSequences.ToList(),
            SpriteTextures = level.SpriteTextures.ToList(),
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
                Data = new(),
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
            Animations = level.Animations.ToList(),
            AnimChanges = level.StateChanges.ToList(),
            AnimCommands = level.AnimCommands.ToList(),
            AnimDispatches = level.AnimDispatches.ToList(),
            AnimFrames = level.Frames.ToList(),
            Images = Convert(level.Images16),
            Images8 = level.NumImages == 0 ? null : level.Images8.Select(i => new LC.Model.TRTexImage8 { Pixels = i.Pixels }).ToList(),
            Meshes = level.Meshes.ToList(),
            MeshPointers = level.MeshPointers.ToList(),
            MeshTrees = level.MeshTrees.ToList(),
            Models = level.Models.ToList(),
            ObjectTextures = level.ObjectTextures.ToList(),
            Palette = level.Palette.Select(c =>
            {
                return new TRColour
                {
                    Red = (byte)(c.Red * 4),
                    Green = (byte)(c.Green * 4),
                    Blue = (byte)(c.Blue * 4),
                };
            }).ToList(),
            SpriteSequences = level.SpriteSequences.ToList(),
            SpriteTextures = level.SpriteTextures.ToList(),
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
        List<byte> data = new();
        for (uint i = offset; i < endOffset; i++)
        {
            data.Add(wavSamples[i]);
        }
        return data.ToArray();
    }

    private static void ResetLaraLevel(TRLevel level)
    {
        level.NumMeshData = 0;
        level.Meshes = Array.Empty<TRMesh>();
        level.NumMeshPointers = 0;
        level.MeshPointers = Array.Empty<uint>();
        level.NumMeshTrees = 0;
        level.MeshTrees = Array.Empty<TRMeshTreeNode>();
        level.Images8 = Array.Empty<TRTexImage8>();
        level.NumImages = 0;
        level.ObjectTextures = Array.Empty<TRObjectTexture>();
        level.NumObjectTextures = 0;
        for (int i = 0; i < 256; i++)
        {
            TRColour c = level.Palette[i];
            c.Red = c.Green = c.Blue = 0;
        }
        level.NumMeshData = 0;
        level.Meshes = Array.Empty<TRMesh>();
        level.NumMeshPointers = 0;
        level.MeshPointers = Array.Empty<uint>();
        level.NumMeshTrees = 0;
        level.MeshTrees = Array.Empty<TRMeshTreeNode>();
        level.Models[0].MeshTree = 0;
        level.Models[0].StartingMesh = 0;
        level.Models[0].NumMeshes = 0;
    }

    private static void RemoveMeshData(TRLevel level)
    {
        // The game will detect that there is no mesh data associated with this injection
        // and hence retain the existing mesh data from the original level.
        level.NumMeshData = 0;
        level.Meshes = Array.Empty<TRMesh>();
        level.NumMeshPointers = 0;
        level.MeshPointers = Array.Empty<uint>();
        level.NumMeshTrees = 0;
        level.MeshTrees = Array.Empty<TRMeshTreeNode>();

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
        level.Meshes = Array.Empty<TRMesh>();
        level.NumMeshPointers = 0;
        level.MeshPointers = Array.Empty<uint>();
        level.NumMeshTrees = 0;
        level.MeshTrees = Array.Empty<TRMeshTreeNode>();

        foreach (TRModel model in level.Models)
        {
            model.MeshTree = 0;
            model.StartingMesh = 0;
            model.NumMeshes = 0;
        }
    }

    private static List<LC.Model.TRTexImage32> Convert(TRTexImage8[] originalImages, TRColour[] originalPalette)
    {
        List<LC.Model.TRColour> palette = originalPalette.Select(p => new LC.Model.TRColour
        {
            Red = (byte)(p.Red * 4),
            Blue = (byte)(p.Blue * 4),
            Green = (byte)(p.Green * 4),
        }).ToList();

        List<LC.Model.TRTexImage32> images = new();
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
        return originalImages
            .Select(i => new LC.Model.TRTexImage32 { Pixels = new TRImage(i.Pixels).ToRGBA() })
            .ToList();
    }
}
