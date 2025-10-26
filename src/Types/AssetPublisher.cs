using System.IO.Compression;
using TRLevelControl;
using TRLevelControl.Model;
using TRXInjectionTool.Types.TR1.Lara;
using TRXInjectionTool.Types.TR1.Misc;
using TRXInjectionTool.Types.TR2.Lara;
using TRXInjectionTool.Types.TR2.Misc;

namespace TRXInjectionTool.Types;

public static class AssetPublisher
{
    private static readonly string _wadZipPath
        = (Directory.Exists("../../Resources/Published") ? "../../" : "")
            + "Resources/Published/{0}-ext.zip";

    private static readonly DateTimeOffset _wadZipPlaceholderDate
        = new(new DateTime(2025, 8, 11, 14, 0, 0), new TimeSpan());

    private static readonly Dictionary<TRGameVersion, List<IPublisher>> _publishers = new()
    {
        [TRGameVersion.TR1] = new()
        {
            new TR1FontBuilder(),
            new TR1PDABuilder(),
            new TR1LaraFlareBuilder(),
            new TR1LaraGunBuilder(),
        },
        [TRGameVersion.TR2] = new()
        {
            new TR2FontBuilder(),
            new TR2PDABuilder(),
        },
    };

    private static readonly Dictionary<TRGameVersion, bool> _runFlags = new()
    {
        [TRGameVersion.TR1] = false,
        [TRGameVersion.TR2] = false,
    };

    public static void OnBuilderRun(InjectionBuilder builder)
    {
        if (builder is LaraBuilder laraBuilder)
        {
            _runFlags[laraBuilder.GameVersion] = true;
            return;
        }

        foreach (var (version, publishers) in _publishers)
        {
            if (publishers.Any(p => p.GetType() == builder.GetType()))
            {
                _runFlags[version] = true;
                break;
            }
        }
    }

    public static void Publish()
    {
        Publish(TRGameVersion.TR1, new TR1LaraAnimBuilder());
        Publish(TRGameVersion.TR2, new TR2LaraAnimBuilder());
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
        _publishers[version].ForEach(p =>
        {
            var level = p.Publish();
            var rawOutput = SerializeLevel(level);
            var entry = archive.CreateEntry(p.GetPublishedName(), CompressionLevel.Optimal);
            using var zipStream = entry.Open();
            zipStream.Write(rawOutput, 0, rawOutput.Length);
        });

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
        else
        {
            throw new ArgumentException("Only TR1 and TR2 levels supported.");
        }

        return ms.ToArray();
    }
}

public interface IPublisher
{
    TRLevelBase Publish();
    string GetPublishedName();
}
