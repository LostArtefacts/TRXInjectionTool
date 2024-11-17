﻿using TRLevelControl;
using TRLevelReader.Model;
using TRXInjectionTool.Actions;

using LC = TRLevelControl;

namespace TRXInjectionTool.Control;

public static class InjectionIO
{
    private static readonly Dictionary<LC.Model.TRGameVersion, InjectionVersion> _versions = new()
    {
        [LC.Model.TRGameVersion.TR1] = new()
        {
            Magic = MakeTag('T', '1', 'M', 'J'),
            Iteration = 8,
        },
        [LC.Model.TRGameVersion.TR2] = new()
        {
            Magic = MakeTag('T', '2', 'X', 'J'),
            Iteration = 1,
        },
    };

    public static void Export(InjectionData data, string file)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(file)));
        File.WriteAllBytes(file, Serialize(data));
    }

    private static uint MakeTag(char a, char b, char c, char d)
    {
        return (uint)(a | b << 8 | c << 16 | d << 24);
    }

    public static byte[] Serialize(InjectionData data)
    {
        if (!_versions.ContainsKey(data.GameVersion))
        {
            throw new NotSupportedException();
        }

        using MemoryStream stream = new();
        using TRLevelWriter writer = new(stream);

        switch (data.GameVersion)
        {
            case LC.Model.TRGameVersion.TR1:
                WriteTR1Data(data, writer);
                break;
            case LC.Model.TRGameVersion.TR2:
                WriteTR2Data(data, writer);
                break;
        }

        return stream.ToArray();
    }

    private static void WriteTR1Data(InjectionData data, TRLevelWriter writer)
    {
        List<byte> meshData = new();
        foreach (TRMesh mesh in data.Meshes)
            meshData.AddRange(mesh.Serialize());

        {
            // Header
            WriteVersionAndType(data, writer);
            writer.Write((uint)data.Images8.Count);
            writer.Write((uint)data.ObjectTextures.Count);
            writer.Write((uint)data.SpriteTextures.Count);
            writer.Write((uint)data.SpriteSequences.Count);
            writer.Write((uint)(meshData.Count / 2));
            writer.Write((uint)data.MeshPointers.Count);
            writer.Write((uint)data.AnimChanges.Count);
            writer.Write((uint)data.AnimDispatches.Count);
            writer.Write((uint)data.AnimCommands.Count);
            writer.Write((uint)(data.MeshTrees.Count * 4));
            writer.Write((uint)data.AnimFrames.Count);
            writer.Write((uint)data.Animations.Count);
            writer.Write((uint)data.Models.Count);
            writer.Write((uint)data.SFX.Count);
            uint sampleDataSize = 0;
            uint sampleCount = 0;
            foreach (TRSFXData sfx in data.SFX)
            {
                sampleDataSize += sfx.GetSampleDataSize();
                sampleCount += (uint)((sfx.Characteristics & 0xFC) >> 2);
            }
            writer.Write(sampleDataSize);
            writer.Write(sampleCount);
            writer.Write((uint)data.MeshEdits.Count);
            writer.Write((uint)data.TextureOverwrites.Count);
            writer.Write((uint)data.FloorEdits.Count);

            WriteRoomMeshData(writer, data); // Summary of room mesh size changes
            writer.Write((uint)data.RoomEdits.Count); // Actual fixes
            writer.Write((uint)data.VisPortalEdits.Count);
            writer.Write((uint)data.AnimRangeEdits.Count);
            writer.Write((uint)data.ItemEdits.Count);
        }

        {
            // Regular TR structures
            data.Palette.ForEach(p => writer.Write(SquashColour(p).Serialize()));
            data.Images8.ForEach(i => writer.Write(i.Serialize()));
            data.ObjectTextures.ForEach(o => writer.Write(o.Serialize()));
            data.SpriteTextures.ForEach(s => writer.Write(s.Serialize()));
            data.SpriteSequences.ForEach(s => writer.Write(s.Serialize()));

            writer.Write(meshData);
            writer.Write(data.MeshPointers);

            data.AnimChanges.ForEach(a => writer.Write(a.Serialize()));
            data.AnimDispatches.ForEach(a => writer.Write(a.Serialize()));
            data.AnimCommands.ForEach(a => writer.Write(a.Serialize()));
            data.MeshTrees.ForEach(m => writer.Write(m.Serialize()));
            writer.Write(data.AnimFrames);
            data.Animations.ForEach(a => writer.Write(a.Serialize()));
            data.Models.ForEach(m => writer.Write(m.Serialize()));
        }

        {
            // Injection edits
            data.SFX.ForEach(s => s.Serialize(writer));
            data.MeshEdits.ForEach(m => m.Serialize(writer));
            data.TextureOverwrites.ForEach(t => t.Serialize(writer));
            data.FloorEdits.ForEach(f => f.Serialize(writer));
            data.RoomEdits.ForEach(r => r.Serialize(writer));
            data.VisPortalEdits.ForEach(v => v.Serialize(writer));
            data.AnimRangeEdits.ForEach(a => a.Serialize(writer));
            data.ItemEdits.ForEach(i => i.Serialize(writer));
        }
    }

    private static void WriteTR2Data(InjectionData data, TRLevelWriter writer)
    {
        {
            // Header
            WriteVersionAndType(data, writer);
            writer.Write((uint)data.FloorEdits.Count);
        }

        {
            // Injection edits
            data.FloorEdits.ForEach(f => f.Serialize(writer));
        }
    }

    private static void WriteVersionAndType(InjectionData data, TRLevelWriter writer)
    {
        InjectionVersion version = _versions[data.GameVersion];
        writer.Write(version.Magic);
        writer.Write(version.Iteration);
        writer.Write((uint)data.InjectionType);
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

    private static void WriteRoomMeshData(BinaryWriter writer, InjectionData data)
    {
        Dictionary<short, uint> sizes = new();
        foreach (TRRoomTextureEdit edit in data.RoomEdits)
        {
            uint size = edit.AdditionalMeshLength;
            if (size == 0)
                continue;

            if (!sizes.ContainsKey(edit.RoomIndex))
                sizes[edit.RoomIndex] = size;
            else
                sizes[edit.RoomIndex] += size;
        }

        writer.Write((uint)sizes.Count);
        foreach (short room in sizes.Keys)
        {
            writer.Write(room);
            writer.Write(sizes[room]);
        }
    }
}

public class InjectionVersion
{
    public uint Magic { get; set; }
    public uint Iteration { get; set; }
}
