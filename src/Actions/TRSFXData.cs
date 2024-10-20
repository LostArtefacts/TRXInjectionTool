using TRLevelControl;

namespace TRXInjectionTool.Actions;

public class TRSFXData
{
    public short ID { get; set; }
    public ushort Volume { get; set; }
    public ushort Chance { get; set; }
    public ushort Characteristics { get; set; }
    public List<byte[]> Data { get; set; }

    public void Serialize(TRLevelWriter writer)
    {
        writer.Write(ID);
        writer.Write(Volume);
        writer.Write(Chance);
        writer.Write(Characteristics);

        foreach (byte[] data in Data)
        {
            writer.Write(data.Length);
            writer.Write(data);
        }
    }

    public uint GetSampleDataSize()
    {
        return (uint)Data.Sum(data => data.Length);
    }
}
