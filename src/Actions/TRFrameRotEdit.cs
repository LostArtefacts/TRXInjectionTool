using TRLevelControl;
using TRLevelControl.Model;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Actions;

public class TRFrameRotEdit
{
    // Temporary measure until further frame re-handling engine side i.e. to target
    // specific frame and rot indices.

    public uint ModelID { get; set; }
    public int AnimIndex { get; set; }
    public TRVertex Rotation { get; set; }

    public void Serialize(TRLevelWriter writer, TRGameVersion version)
    {
        writer.Write((int)ModelID, TRObjectType.Game, version);
        writer.Write(AnimIndex);        
        writer.Write(PackYZRotation(Rotation.Y, Rotation.Z));
        writer.Write(PackXYRotation(Rotation.X, Rotation.Y));
    }

    private static short PackXYRotation(int x, int y)
    {
        return (short)((x << 4) | ((y & 0x0FC0) >> 6));
    }

    private static short PackYZRotation(int y, int z)
    {
        return (short)(((y & 0x003F) << 10) | (z & 0x03FF));
    }
}

public class TRFrameReplacement
{
    public uint ModelID { get; set; }
    public Dictionary<int, List<ushort>> Frames { get; set; } = new();

    public void Serialize(TRLevelWriter writer, TRGameVersion version)
    {
        writer.Write((int)ModelID, TRObjectType.Game, version);
        writer.Write(Frames.Count);
        foreach (var (animID, frames) in Frames)
        {
            writer.Write(animID);
            writer.Write(frames.Count);
            writer.Write(frames);
        }
    }

    public static IEnumerable<TRFrameReplacement> CreateFrom(TR1Level level)
    {
        var tempData = InjectionData.Create(level, InjectionType.General, string.Empty);
        return level.Models.Select(kvp => Create(tempData, kvp.Value, (uint)kvp.Key));
    }

    public static IEnumerable<TRFrameReplacement> CreateFrom(TR2Level level)
    {
        var tempData = InjectionData.Create(level, InjectionType.General, string.Empty);
        return level.Models.Select(kvp => Create(tempData, kvp.Value, (uint)kvp.Key));
    }

    private static TRFrameReplacement Create(InjectionData tempData, TRModel model, uint type)
    {
        var replacer = new TRFrameReplacement
        {
            ModelID = (uint)type,
        };
        var flatModel = tempData.Models.Find(m => m.ID == replacer.ModelID);
        var idx = tempData.Models.IndexOf(flatModel);
        var nextFramesetOffset = idx == tempData.Models.Count - 1
            ? tempData.AnimFrames.Count
            : (int)(tempData.Models[idx + 1].FrameOffset / 2);
        for (int i = 0; i < model.Animations.Count; i++)
        {
            var animIdx = flatModel.Animation + i;
            var offset = (int)(tempData.Animations[animIdx].FrameOffset / 2);
            var nextOffset = i == model.Animations.Count - 1
                ? nextFramesetOffset
                : (int)(tempData.Animations[animIdx + 1].FrameOffset / 2);
            replacer.Frames[i] = tempData.AnimFrames.GetRange(offset, nextOffset - offset);
        }
        return replacer;
    }
}
