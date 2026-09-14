using System.IO.Compression;
using TRLevelControl;
using TRLevelControl.Model;

namespace TRXInjectionTool.Types;

public static class AssetPublisher
{
    private static readonly string _wadZipPath
        = (Directory.Exists("../../Resources/Published") ? "../../" : "")
            + "Resources/Published/{0}-ext.zip";

    private static readonly DateTimeOffset _wadZipPlaceholderDate
        = new(new DateTime(2025, 8, 11, 14, 0, 0), new TimeSpan());

    // Zip entry order affects the published bytes, so publishers register
    // with an explicit order key rather than relying on discovery order.
    private static readonly Dictionary<TRGameVersion, SortedList<int, IPublisher>> _publishers = new();
    private static readonly Dictionary<TRGameVersion, LaraBuilder> _laraBuilders = new();
    private static readonly Dictionary<TRGameVersion, bool> _runFlags = new();

    public static void Register(TRGameVersion version, int order, IPublisher publisher)
    {
        if (!_publishers.TryGetValue(version, out var publishers))
        {
            _publishers[version] = publishers = new();
        }
        publishers.Add(order, publisher);
        _runFlags.TryAdd(version, false);
    }

    public static void RegisterLara(TRGameVersion version, LaraBuilder builder)
    {
        _laraBuilders[version] = builder;
        _runFlags.TryAdd(version, false);
    }

    public static void OnBuilderRun(InjectionBuilder builder)
    {
        if (builder is LaraBuilder laraBuilder)
        {
            _runFlags[laraBuilder.GameVersion] = true;
            return;
        }

        foreach (var (version, publishers) in _publishers)
        {
            if (publishers.Values.Any(p => p.GetType() == builder.GetType()))
            {
                _runFlags[version] = true;
                break;
            }
        }
    }

    public static void Publish()
    {
        foreach (var (version, laraBuilder) in _laraBuilders.OrderBy(kv => kv.Key))
        {
            Publish(version, laraBuilder);
        }
    }

    private static void Publish(TRGameVersion version, LaraBuilder laraBuilder)
    {
        if (!_runFlags[version])
        {
            return;
        }

        Console.WriteLine($"\tUpdating {version} published assets");
        var laraWad = laraBuilder.Publish();
        using var laraStream = new MemoryStream(laraWad);
        using var outStream = new MemoryStream();
        laraStream.CopyTo(outStream);

        var archive = new ZipArchive(outStream, ZipArchiveMode.Update);
        foreach (var p in _publishers[version].Values)
        {
            var level = p.Publish();
            var rawOutput = SerializeLevel(level);
            var entry = archive.CreateEntry(p.GetPublishedName(), CompressionLevel.Optimal);
            using var zipStream = entry.Open();
            zipStream.Write(rawOutput, 0, rawOutput.Length);
        }

        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            // Prevent the zip changing despite the contents having not. C# provides no way to do this on create.
            entry.LastWriteTime = _wadZipPlaceholderDate;
        }

        archive.Dispose();
        File.WriteAllBytes(string.Format(_wadZipPath, version.ToString().ToLower()), outStream.ToArray());

        _runFlags[version] = false;
    }

    private static byte[] SerializeLevel(TRLevelBase level)
    {
        using var ms = new MemoryStream();
        if (level is TR1Level level1)
        {
            var control = new TR1LevelControl();
            control.Write(level1, ms);
        }
        else if (level is TR2Level level2)
        {
            var control = new TR2LevelControl();
            control.Write(level2, ms);
        }
        else if (level is TR3Level level3)
        {
            var control = new TR3LevelControl();
            control.Write(level3, ms);
        }
        else if (level is TR4Level level4)
        {
            var control = new TR4LevelControl();
            control.Write(level4, ms);
        }
        else
        {
            throw new ArgumentException("Only TR1-4 levels supported.");
        }

        return ms.ToArray();
    }
}

public interface IPublisher
{
    TRLevelBase Publish();
    string GetPublishedName();
}

// A builder pack implements this to register its publishers; the host runs
// every manifest it discovers after loading builder assemblies.
public interface IBuilderPackManifest
{
    void Register();
}
