using TRLevelControl;
using TRLevelControl.Build;
using TRLevelControl.Model;

namespace TRXInjectionTool.Actions;

public class TRFloorDataEdit
{
    public short RoomIndex { get; set; }
    public ushort X { get; set; }
    public ushort Z { get; set; }
    public uint NewFDLength => (uint)Fixes.Sum(f => f.NewFDLength);
    public List<FDFix> Fixes { get; set; }

    public void Serialize(TRLevelWriter writer)
    {
        writer.Write(RoomIndex);
        writer.Write(X);
        writer.Write(Z);
        writer.Write((uint)Fixes.Count);
        Fixes.ForEach(f => f.Serialize(writer));
    }
}

public enum FDFixType
{
    TrigParam,
    MusicOneShot,
    TrigCreate,
    RoomShift,
    TrigItem,
}

public abstract class FDFix
{
    public abstract FDFixType FixType { get; }
    public abstract uint NewFDLength { get; }

    public void Serialize(TRLevelWriter writer)
    {
        writer.Write((uint)FixType);
        SerializeImpl(writer);
    }

    protected abstract void SerializeImpl(TRLevelWriter writer);
}

public class FDTrigItem : FDFix
{
    public override FDFixType FixType => FDFixType.TrigItem;
    public override uint NewFDLength => 0;
    public TR1Entity Item { get; set; }

    protected override void SerializeImpl(TRLevelWriter writer)
    {
        writer.Write(Item);
    }
}

public class FDTrigParamFix : FDFix
{
    public override FDFixType FixType => FDFixType.TrigParam;
    public override uint NewFDLength => 0;
    public FDTrigAction ActionType { get; set; }
    public short OldParam { get; set; }
    public short NewParam { get; set; }

    protected override void SerializeImpl(TRLevelWriter writer)
    {
        writer.Write((byte)ActionType);
        writer.Write(OldParam);
        writer.Write(NewParam);
    }
}

public class FDTrigCreateFix : FDFix
{
    public override FDFixType FixType => FDFixType.TrigCreate;
    public override uint NewFDLength => (uint)Flatten().Count;
    public List<FDEntry> Entries { get; set; }

    private List<ushort> Flatten()
    {
        TRFDBuilder builder = new(TRGameVersion.TR1);
        return builder.Flatten(Entries);
    }

    protected override void SerializeImpl(TRLevelWriter writer)
    {
        List<ushort> data = Flatten();
        writer.Write((uint)data.Count);
        writer.Write(data);
    }
}

public class FDMusicOneShot : FDFix
{
    public override FDFixType FixType => FDFixType.MusicOneShot;
    public override uint NewFDLength => 0;

    protected override void SerializeImpl(TRLevelWriter writer) { }
}

public class FDRoomShift : FDFix
{
    public override FDFixType FixType => FDFixType.RoomShift;
    public override uint NewFDLength => 0;
    public uint XShift { get; set; }
    public uint ZShift { get; set; }
    public int YShift { get; set; }

    protected override void SerializeImpl(TRLevelWriter writer)
    {
        writer.Write(XShift);
        writer.Write(ZShift);
        writer.Write(YShift);
    }
}
