using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using TRImageControl;
using TRLevelControl;
using TRLevelReader.Model;
using TRXInjectionTool.Util;
using LC = TRLevelControl.Model;

namespace TRXInjectionTool.Control;

public static class InjectionIO
{
    private static readonly InjectionVersion _version = new()
    {
        Magic = IOUtils.MakeTag('T', 'R', 'X', 'J'),
        Iteration = 5,
    };

    public static void Export(InjectionData data, string file)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(file)));
        File.WriteAllBytes(file, Serialize(data));
    }

    public static byte[] Serialize(InjectionData data)
    {
        using MemoryStream stream = new();
        using TRLevelWriter writer = new(stream);

        WriteData(data, writer);

        byte[] exportedData = stream.ToArray();
        using MemoryStream outStream = new();
        using DeflaterOutputStream deflater = new(outStream);
        using MemoryStream inStream = new(exportedData);

        inStream.CopyTo(deflater);
        deflater.Finish();

        byte[] zippedData = outStream.ToArray();

        using MemoryStream finalStream = new();
        using TRLevelWriter finalWriter = new(finalStream);

        finalWriter.Write(_version.Magic);
        finalWriter.Write(_version.Iteration);
        finalWriter.Write((uint)data.InjectionType);

        finalWriter.Write(exportedData.Length);
        finalWriter.Write(zippedData.Length);
        finalWriter.Write(zippedData);

        return finalStream.ToArray();
    }

    private static void WriteData(InjectionData data, TRLevelWriter writer)
    {
        WriteApplicabilityTests(data, writer);

        List<Chunk> chunks =
        [
            CreateChunk(ChunkType.TextureData, data, WriteTextureData),
            CreateChunk(ChunkType.TextureInfo, data, WriteTextureInfo),
            CreateChunk(ChunkType.MeshData, data, WriteMeshData),
            CreateChunk(ChunkType.AnimationData, data, WriteAnimationData),
            CreateChunk(ChunkType.ObjectData, data, WriteObjectData),
            CreateChunk(ChunkType.SFXData, data, WriteSFXData),
            CreateChunk(ChunkType.CameraData, data, WriteCameraData),
            CreateChunk(ChunkType.DataEdits, data, WriteEdits),
        ];

        chunks.RemoveAll(b => b.BlockCount == 0);

        writer.Write(chunks.Count);
        chunks.ForEach(b => b.Serialize(writer));
    }

    private static void WriteApplicabilityTests(InjectionData data, TRLevelWriter writer)
    {
        using MemoryStream ms = new();
        using TRLevelWriter testWriter = new(ms);
        data.ApplicabilityTests.ForEach(t => t.Serialize(testWriter, data.GameVersion));

        byte[] testData = ms.ToArray();
        writer.Write(data.ApplicabilityTests.Count);
        writer.Write(testData.Length);
        writer.Write(testData);
    }

    private static Chunk CreateChunk(ChunkType type, InjectionData data, Func<InjectionData, TRLevelWriter, int> process)
    {
        using MemoryStream ms = new();
        using TRLevelWriter blockSetWriter = new(ms);
        int blockCount = process(data, blockSetWriter);

        return new()
        {
            Type = type,
            BlockCount = blockCount,
            Data = ms.ToArray(),
        };
    }

    private static int WriteTextureData(InjectionData data, TRLevelWriter writer)
    {
        int blockCount = 0;

        blockCount += WriteBlock(BlockType.Palette, data.Palette.Count, writer,
            s => data.Palette.ForEach(p => s.Write(SquashColour(p).Serialize())));

        if (data.Images.Count > 0)
        {
            List<LC.TRTexImage8> img8s = [];
            if (data.Images8 == null)
            {
                List<LC.TRColour> trPalette = [.. data.Palette.Select(c => new LC.TRColour { Red = c.Red, Green = c.Green, Blue = c.Blue })];
                img8s.AddRange(data.Images.Select(i => new LC.TRTexImage8 { Pixels = new TRImage(i.Pixels).ToRGB(trPalette) }));
            }
            else
            {
                if (data.Images8.Count != data.Images.Count)
                {
                    throw new Exception();
                }
                img8s.AddRange(data.Images8);
            }

            blockCount += WriteBlock(BlockType.Images, data.Images.Count, writer,
                s =>
                {
                    data.Images.ForEach(i => s.Write(i.Pixels));
                    img8s.ForEach(i => s.Write(i.Pixels));
                });
        }

        return blockCount;
    }

    private static int WriteTextureInfo(InjectionData data, TRLevelWriter writer)
    {
        int blockCount = 0;

        blockCount += WriteBlock(BlockType.ObjectTextures, data.ObjectTextures.Count, writer,
            s => data.ObjectTextures.ForEach(t => s.Write(t.Serialize())));

        blockCount += WriteBlock(BlockType.SpriteTextures, data.SpriteTextures.Count, writer,
            s => data.SpriteTextures.ForEach(t => s.Write(t.Serialize())));

        blockCount += WriteBlock(BlockType.SpriteSequences, data.SpriteSequences.Count, writer,
            s => data.SpriteSequences.ForEach(t => t.Serialize(s, data.GameVersion)));

        return blockCount;
    }

    private static int WriteMeshData(InjectionData data, TRLevelWriter writer)
    {
        int blockCount = 0;

        blockCount += WriteBlock(BlockType.MeshPointers, data.MeshPointers.Count, writer,
            s => s.Write(data.MeshPointers));

        List<byte> meshData = [.. data.Meshes.SelectMany(m => m.Serialize())];
        blockCount += WriteBlock(BlockType.ObjectMeshes, meshData.Count / 2, writer,
            s => s.Write(meshData));

        return blockCount;
    }

    private static int WriteAnimationData(InjectionData data, TRLevelWriter writer)
    {
        int blockCount = 0;

        blockCount += WriteBlock(BlockType.AnimChanges, data.AnimChanges.Count, writer,
            s => data.AnimChanges.ForEach(a => s.Write(a.Serialize())));

        blockCount += WriteBlock(BlockType.AnimDispatches, data.AnimDispatches.Count, writer,
            s => data.AnimDispatches.ForEach(d => s.Write(d.Serialize())));

        blockCount += WriteBlock(BlockType.AnimCommands, data.AnimCommands.Count, writer,
            s => data.AnimCommands.ForEach(c => s.Write(c.Serialize())));

        blockCount += WriteBlock(BlockType.MeshTrees, data.MeshTrees.Count, writer,
            s => data.MeshTrees.ForEach(m => s.Write(m.Serialize())));

        blockCount += WriteBlock(BlockType.AnimFrames, data.AnimFrames.Count, writer,
            s => s.Write(data.AnimFrames));

        blockCount += WriteBlock(BlockType.Animations, data.Animations.Count, writer,
            s => data.Animations.ForEach(a => s.Write(a.Serialize())));

        return blockCount;
    }

    private static int WriteObjectData(InjectionData data, TRLevelWriter writer)
    {
        int blockCount = 0;

        blockCount += WriteBlock(BlockType.Objects, data.Models.Count, writer,
            s => data.Models.ForEach(m => m.Serialize(s, data.GameVersion, data.IsMeshOnlyModel(m.ID))));

        blockCount += WriteBlock(BlockType.StaticObjects, data.StaticObjects.Count, writer,
            s => data.StaticObjects.ForEach(m => s.Write(m.Serialize())));

        return blockCount;
    }

    private static int WriteSFXData(InjectionData data, TRLevelWriter writer)
    {
        return WriteBlock(BlockType.SampleInfos, data.SFX.Count, writer,
            s => data.SFX.ForEach(f => f.Serialize(s, data.GameVersion)));
    }

    private static int WriteCameraData(InjectionData data, TRLevelWriter writer)
    {
        return WriteBlock(BlockType.CinematicFrames, data.CinematicFrames.Count, writer,
            s => data.CinematicFrames.ForEach(f => s.Write(f.Serialize())));
    }

    private static int WriteEdits(InjectionData data, TRLevelWriter writer)
    {
        int blockCount = 0;

        blockCount += WriteBlock(BlockType.FloorEdits, data.FloorEdits.Count, writer,
            s => data.FloorEdits.ForEach(f => f.Serialize(s, data.GameVersion)));

        blockCount += WriteBlock(BlockType.ItemPosEdits, data.ItemPosEdits.Count, writer,
            s => data.ItemPosEdits.ForEach(i => i.Serialize(s)));

        blockCount += WriteBlock(BlockType.ItemFlagEdits, data.ItemFlagEdits.Count, writer,
            s => data.ItemFlagEdits.ForEach(i => i.Serialize(s, data.GameVersion)));

        blockCount += WriteBlock(BlockType.MeshEdits, data.MeshEdits.Count, writer,
            s => data.MeshEdits.ForEach(m => m.Serialize(s, data.GameVersion)));

        blockCount += WriteBlock(BlockType.StaticEdits, data.StaticMeshEdits.Count, writer,
            s => data.StaticMeshEdits.ForEach(m => m.Serialize(s)));

        blockCount += WriteBlock(BlockType.TextureEdits, data.TextureOverwrites.Count, writer,
            s => data.TextureOverwrites.ForEach(t => t.Serialize(s)));

        {
            // Summary data
            List<RoomMeshMeta> meta = RoomMeshMeta.Create(data);
            blockCount += WriteBlock(BlockType.RoomEditSummary, meta.Count, writer,
                s => meta.ForEach(m => m.Serialize(s)));

            blockCount += WriteBlock(BlockType.RoomEdits, data.RoomEdits.Count, writer,
                s => data.RoomEdits.ForEach(r => r.Serialize(s)));
        }

        blockCount += WriteBlock(BlockType.VisPortalEdits, data.VisPortalEdits.Count, writer,
            s => data.VisPortalEdits.ForEach(v => v.Serialize(s)));

        blockCount += WriteBlock(BlockType.CameraEdits, data.CameraEdits.Count, writer,
            s => data.CameraEdits.ForEach(c => c.Serialize(s)));

        blockCount += WriteBlock(BlockType.FrameEdits, data.FrameEdits.Count, writer,
            s => data.FrameEdits.ForEach(f => f.Serialize(s, data.GameVersion)));

        blockCount += WriteBlock(BlockType.FrameReplace, data.FrameReplacements.Count, writer,
            s => data.FrameReplacements.ForEach(f => f.Serialize(s, data.GameVersion)));

        blockCount += WriteBlock(BlockType.AnimCmdEdits, data.AnimCmdEdits.Count, writer,
            s => data.AnimCmdEdits.ForEach(c => c.Serialize(s, data.GameVersion)));

        blockCount += WriteBlock(BlockType.SpriteEdit, data.SpriteEdits.Count, writer,
            s => data.SpriteEdits.ForEach(c => c.Serialize(s, data.GameVersion)));

        blockCount += WriteBlock(BlockType.ObjectTypeEdits, data.ObjectTypeEdits.Count, writer,
            s => data.ObjectTypeEdits.ForEach(o => o.Serialize(s, data.GameVersion)));

        return blockCount;
    }

    private static int WriteBlock(BlockType type, int elementCount, TRLevelWriter writer, Action<TRLevelWriter> subCallback)
    {
        if (elementCount == 0)
        {
            return 0;
        }

        using MemoryStream ms = new();
        using TRLevelWriter subWriter = new(ms);
        subCallback(subWriter);
        subWriter.Flush();

        byte[] data = ms.ToArray();
        writer.Write((int)type);
        writer.Write(elementCount);
        writer.Write(data.Length);
        writer.Write(data);

        return 1;
    }

    private static TRColour SquashColour(TRColour colour)
    {
        return new()
        {
            Red = (byte)(colour.Red / 4),
            Green = (byte)(colour.Green / 4),
            Blue = (byte)(colour.Blue / 4),
        };
    }
}
