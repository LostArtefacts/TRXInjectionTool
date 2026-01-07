using TRLevelControl;
using TRLevelControl.Model;

namespace TRXInjectionTool.Actions;

public class TRSFXData
{
    private static readonly Dictionary<TRGameVersion, List<byte[]>> _mainSFXData = [];

    public short ID { get; set; }
    public ushort Volume { get; set; }
    public ushort Chance { get; set; }
    public ushort Characteristics { get; set; }
    public byte Pitch { get; set; }
    public byte Range { get; set; } = 10;
    public List<byte[]> Data { get; set; }
    public uint SampleOffset { get; set; }

    public void Serialize(TRLevelWriter writer, TRGameVersion version)
    {
        writer.Write(ID);
        writer.Write(Volume);
        writer.Write(Chance);
        writer.Write(Characteristics);
        writer.Write(Range * TRConsts.Step4);
        writer.Write(Pitch);

        if (version > TRGameVersion.TR1 && Data == null)
        {
            LoadSFX(version);
        }

        Data.ForEach(wav =>
        {
            writer.Write(wav.Length);
            writer.Write(wav);
        });
    }

    public uint GetSampleDataSize()
    {
        return (uint)Data.Sum(data => data.Length);
    }

    public void LoadSFX(TRGameVersion version)
    {
        if (!_mainSFXData.TryGetValue(version, out var mainSFXData))
        {
            mainSFXData = _mainSFXData[version] = ReadMainSFX(version);
        }

        int sampleCount = (Characteristics & 0xFC) >> 2;
        Data = mainSFXData.GetRange((int)SampleOffset, sampleCount);
    }

    private static List<byte[]> ReadMainSFX(TRGameVersion version)
    {
        var file = $"Resources/{version}/main.sfx";
        var result = new List<byte[]>();

        using var reader = new TRLevelControl.TRLevelReader(File.Open(file, FileMode.Open));
        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            using var stream = new MemoryStream();
            using var writer = new TRLevelWriter(stream);

            var header = reader.ReadUInt32s(11);
            var data = reader.ReadUInt8s(header[10]);
            writer.Write(header);
            writer.Write(data);

            result.Add(stream.ToArray());
        }

        return result;
    }

    public static TRSFXData Create(object id, TR1SoundEffect effect)
    {
        return new()
        {
            ID = Convert.ToInt16(id),
            Chance = effect.Chance,
            Characteristics = effect.GetFlags(),
            Volume = effect.Volume,
            Data = effect.Samples,
        };
    }

    public static TRSFXData Create(object id, TR2SoundEffect effect)
    {
        var sfx = new TRSFXData
        {
            ID = Convert.ToInt16(id),
            Chance = effect.Chance,
            Characteristics = effect.GetFlags(),
            Volume = effect.Volume,
            SampleOffset = effect.SampleID,
        };
        sfx.LoadSFX(TRGameVersion.TR2);
        return sfx;
    }

    public static TRSFXData Create(object id, TR3SoundEffect effect)
    {
        var sfx = new TRSFXData
        {
            ID = Convert.ToInt16(id),
            Chance = effect.Chance,
            Characteristics = effect.GetFlags(),
            Volume = (ushort)(effect.Volume << 7),
            Pitch = effect.Pitch,
            Range = effect.Range,
            SampleOffset = effect.SampleID,
        };
        sfx.LoadSFX(TRGameVersion.TR3);
        return sfx;
    }
}
