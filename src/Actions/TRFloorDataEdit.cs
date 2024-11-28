using TRLevelControl;
using TRLevelControl.Build;
using TRLevelControl.Model;

namespace TRXInjectionTool.Actions;

public class TRFloorDataEdit
{
    public short RoomIndex { get; set; }
    public ushort X { get; set; }
    public ushort Z { get; set; }
    public List<FDFix> Fixes { get; set; }

    public void Serialize(TRLevelWriter writer, TRGameVersion version)
    {
        writer.Write(RoomIndex);
        writer.Write(X);
        writer.Write(Z);
        writer.Write((uint)Fixes.Count);
        Fixes.ForEach(f => f.Serialize(writer, version));
    }
}

public enum FDFixType
{
    TrigParam,
    MusicOneShot,
    TrigCreate,
    RoomShift,
    TrigItem,
    RoomProperties,
}

public abstract class FDFix
{
    public abstract FDFixType FixType { get; }

    public void Serialize(TRLevelWriter writer, TRGameVersion version)
    {
        writer.Write((uint)FixType);
        SerializeImpl(writer, version);
    }

    protected abstract void SerializeImpl(TRLevelWriter writer, TRGameVersion version);
}

public class FDTrigItem : FDFix
{
    public override FDFixType FixType => FDFixType.TrigItem;
    public TR1Entity Item { get; set; }

    protected override void SerializeImpl(TRLevelWriter writer, TRGameVersion version)
    {
        writer.Write(Item);
    }
}

public class FDTrigParamFix : FDFix
{
    public override FDFixType FixType => FDFixType.TrigParam;
    public FDTrigAction ActionType { get; set; }
    public short OldParam { get; set; }
    public short NewParam { get; set; }

    protected override void SerializeImpl(TRLevelWriter writer, TRGameVersion version)
    {
        writer.Write((byte)ActionType);
        writer.Write(OldParam);
        writer.Write(NewParam);
    }
}

public class FDTrigCreateFix : FDFix
{
    public override FDFixType FixType => FDFixType.TrigCreate;
    public List<FDEntry> Entries { get; set; }

    private List<ushort> Flatten(TRGameVersion version)
    {
        TRFDBuilder builder = new(version);
        return builder.Flatten(Entries);
    }

    protected override void SerializeImpl(TRLevelWriter writer, TRGameVersion version)
    {
        List<ushort> data = Flatten(version);
        writer.Write((uint)data.Count);
        writer.Write(data);
    }
}

public class FDMusicOneShot : FDFix
{
    public override FDFixType FixType => FDFixType.MusicOneShot;

    protected override void SerializeImpl(TRLevelWriter writer, TRGameVersion version) { }
}

public class FDRoomShift : FDFix
{
    public override FDFixType FixType => FDFixType.RoomShift;
    public uint XShift { get; set; }
    public uint ZShift { get; set; }
    public int YShift { get; set; }

    protected override void SerializeImpl(TRLevelWriter writer, TRGameVersion version)
    {
        writer.Write(XShift);
        writer.Write(ZShift);
        writer.Write(YShift);
    }
}

public class FDRoomProperties : FDFix
{
    public override FDFixType FixType => FDFixType.RoomProperties;
    public TRRoomFlag Flags { get; set; }

    protected override void SerializeImpl(TRLevelWriter writer, TRGameVersion version)
    {
        writer.Write((ushort)Flags);
    }
}
