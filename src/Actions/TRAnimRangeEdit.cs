using TRLevelControl;

namespace TRXInjectionTool.Actions;

public class TRAnimRangeEdit
{
    public uint ModelID { get; set; }
    public ushort AnimIndex { get; set; }
    public List<TRAnimRange> Ranges { get; set; }

    public void Serialize(TRLevelWriter writer)
    {
        writer.Write(ModelID);
        writer.Write(AnimIndex);
        writer.Write((uint)Ranges.Count);
        Ranges.ForEach(r => r.Serialize(writer));
    }
}

public class TRAnimRange
{
    public short ChangeOffset { get; set; }
    public short RangeOffset { get; set; }
    public short Low { get; set; }
    public short High { get; set; }

    public void Serialize(TRLevelWriter writer)
    {
        writer.Write(ChangeOffset);
        writer.Write(RangeOffset);
        writer.Write(Low);
        writer.Write(High);
    }
}
