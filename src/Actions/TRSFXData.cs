using TRLevelControl;
using TRLevelControl.Model;

namespace TRXInjectionTool.Actions;

public class TRSFXData
{
    private static readonly Dictionary<string, List<byte[]>> _mainSFXData = [];

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

    public void LoadSFX(TRGameVersion version, bool gold = false)
    {
        var id = $"Resources/{version}/main{(gold ? "_gold" : string.Empty)}.sfx";
        if (!_mainSFXData.TryGetValue(id, out var mainSFXData))
        {
            mainSFXData = _mainSFXData[id] = ReadMainSFX(id);
        }

        int sampleCount = (Characteristics & 0xFC) >> 2;
        Data = mainSFXData.GetRange((int)SampleOffset, sampleCount);
    }

    private static List<byte[]> ReadMainSFX(string file)
    {
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

    public static TRSFXData Create(object id, TR2SoundEffect effect, bool gold = false)
    {
        var sfx = new TRSFXData
        {
            ID = Convert.ToInt16(id),
            Chance = effect.Chance,
            Characteristics = effect.GetFlags(),
            Volume = effect.Volume,
            SampleOffset = effect.SampleID,
        };
        sfx.LoadSFX(TRGameVersion.TR2, gold);
        return sfx;
    }

    public static TRSFXData Create(object id, TR3SoundEffect effect, bool gold = false)
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
        sfx.LoadSFX(TRGameVersion.TR3, gold);
        return sfx;
    }
}
