using TRLevelControl.Model;

namespace TRXInjectionTool.Control;

// Decodes the flat packed animation frame blob into canonical frames: full
// bounds, offset and 16-bit angles per mesh, one layout for every game. The
// packing rules mirror the engine's loader, which is the code that defines
// what the blob means for each game.
public static class CanonicalFrames
{
    public class Frame
    {
        public short MinX, MaxX, MinY, MaxY, MinZ, MaxZ;
        public short OffsetX, OffsetY, OffsetZ;
        public List<(short X, short Y, short Z)> Rotations = [];
    }

    public class Result
    {
        // Frames in blob order, one run per animation. Animations that share
        // frame data in the flat blob each get their own run: the engine
        // derives an animation's frame count from the distance to the next
        // animation's frames, so every animation must sit beside its own.
        public List<Frame> Frames { get; init; }
        // Each animation's first frame ordinal, in animation order.
        public List<int> AnimFirstOrdinal { get; init; }
        // Maps a byte offset into the flat blob to a frame ordinal, for
        // records outside the animation list.
        public Dictionary<uint, int> OrdinalByOffset { get; init; }
    }

    public static Result Decode(InjectionData data)
    {
        var words = data.AnimFrames;
        var frames = new List<Frame>();
        var animFirst = new List<int>();
        var ordinals = new Dictionary<uint, int>();

        // How many mesh rotations each animation's frames carry. The counts
        // captured from the source level cover every animation, including
        // those whose model the file does not ship - Lara's, most of all.
        // Files assembled without a source level fall back to mapping their
        // own model records the way the engine does.
        int[] animOwner;
        if (data.AnimMeshCounts.Count == data.Animations.Count)
        {
            animOwner = [.. data.AnimMeshCounts];
        }
        else
        {
            animOwner = new int[data.Animations.Count];
            Array.Fill(animOwner, -1);
            foreach (var model in data.Models)
            {
                if (model.Animation != ushort.MaxValue
                    && model.Animation < animOwner.Length
                    && animOwner[model.Animation] == -1)
                {
                    animOwner[model.Animation] = model.NumMeshes;
                }
            }
            int meshCount = 0;
            for (int i = 0; i < animOwner.Length; i++)
            {
                if (animOwner[i] != -1)
                {
                    meshCount = animOwner[i];
                }
                animOwner[i] = meshCount;
            }
        }

        for (int i = 0; i < data.Animations.Count; i++)
        {
            var anim = data.Animations[i];
            int frameCount = FrameCount(data, i);
            int pos = (int)(anim.FrameOffset / 2);
            animFirst.Add(frames.Count);
            for (int j = 0; j < frameCount; j++)
            {
                ordinals.TryAdd((uint)(pos * 2), frames.Count);
                int start = pos;
                var frame = new Frame
                {
                    MinX = (short)words[pos++],
                    MaxX = (short)words[pos++],
                    MinY = (short)words[pos++],
                    MaxY = (short)words[pos++],
                    MinZ = (short)words[pos++],
                    MaxZ = (short)words[pos++],
                    OffsetX = (short)words[pos++],
                    OffsetY = (short)words[pos++],
                    OffsetZ = (short)words[pos++],
                };

                int rots = data.GameVersion == TRGameVersion.TR1
                    ? words[pos++] : animOwner[i];
                for (int r = 0; r < rots; r++)
                {
                    frame.Rotations.Add(ReadRotation(data.GameVersion, words, ref pos));
                }

                if (data.GameVersion != TRGameVersion.TR1)
                {
                    pos = start + Math.Max(pos - start, anim.FrameSize);
                }
                frames.Add(frame);
            }
        }

        return new()
        {
            Frames = frames,
            AnimFirstOrdinal = animFirst,
            OrdinalByOffset = ordinals,
        };
    }

    private static int FrameCount(InjectionData data, int animIndex)
    {
        var anim = data.Animations[animIndex];
        if (data.GameVersion is TRGameVersion.TR1 or TRGameVersion.TR4)
        {
            return (int)Math.Ceiling(
                (anim.FrameEnd - anim.FrameStart) / (double)anim.FrameRate) + 1;
        }

        uint nextOffset = animIndex == data.Animations.Count - 1
            ? (uint)(data.AnimFrames.Count * 2)
            : data.Animations[animIndex + 1].FrameOffset;
        if (anim.FrameSize == 0 || nextOffset <= anim.FrameOffset)
        {
            return 0;
        }
        return (int)((nextOffset - anim.FrameOffset) / (2u * anim.FrameSize));
    }

    private static (short, short, short) ReadRotation(
        TRGameVersion version, List<ushort> words, ref int pos)
    {
        if (version == TRGameVersion.TR1)
        {
            ushort w1 = words[pos++];
            ushort w2 = words[pos++];
            return Extract(w2, w1); // TR1 stores the pair the other way round
        }

        ushort v1 = words[pos++];
        int mode = (v1 >> 14) & 3;
        int mask = version < TRGameVersion.TR4 ? 0x3FF : 0x0FFF;
        int shift = version < TRGameVersion.TR4 ? 6 : 4;
        return mode switch
        {
            1 => ((short)((v1 & mask) << shift), (short)0, (short)0),
            2 => ((short)0, (short)((v1 & mask) << shift), (short)0),
            3 => ((short)0, (short)0, (short)((v1 & mask) << shift)),
            _ => Extract(v1, words[pos++]),
        };
    }

    private static (short, short, short) Extract(ushort v1, ushort v2)
        => ((short)((v1 & 0x3FF0) << 2),
            (short)((((v1 & 0xF) << 6) | ((v2 & 0xFC00) >> 10)) << 6),
            (short)((v2 & 0x3FF) << 6));
}
