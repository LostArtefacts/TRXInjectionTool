using TRLevelControl;
using TRLevelControl.Model;

namespace TRXInjectionTool.Actions;

public class TRSFXData
{
    public short ID { get; set; }
    public ushort Volume { get; set; }
    public ushort Chance { get; set; }
    public ushort Characteristics { get; set; }
    public List<byte[]> Data { get; set; }
    public uint SampleOffset { get; set; }

    public void Serialize(TRLevelWriter writer, TRGameVersion version)
    {
        writer.Write(ID);
        writer.Write(Volume);
        writer.Write(Chance);
        writer.Write(Characteristics);

        if (version == TRGameVersion.TR1)
        {
            foreach (byte[] data in Data)
            {
                writer.Write(data.Length);
                writer.Write(data);
            }
        }
        else
        {
            writer.Write(SampleOffset);
        }
    }

    public uint GetSampleDataSize()
    {
        return (uint)Data.Sum(data => data.Length);
    }
}
