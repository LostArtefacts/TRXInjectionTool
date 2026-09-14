using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.Text;

namespace TRXInjectionTool.Format;

// Structural verifier for TRXI files: checks the container, inflates the
// payload, and walks the test/chunk/block framing confirming every size
// field is exact. It does not interpret record contents - that is the
// engine reader's job - but any framing mistake the exporter could make
// fails here.
public static class InjectionVerifier
{
    public static void Verify(string path)
    {
        using var reader = new BinaryReader(File.OpenRead(path));
        var magic = Encoding.ASCII.GetString(reader.ReadBytes(4));
        Check(magic == Container.Magic, path, $"bad magic '{magic}'");
        var major = reader.ReadUInt32();
        Check(major == Container.FormatMajor, path, $"format major {major}");
        _ = reader.ReadUInt32(); // file type
        var rawLength = reader.ReadUInt32();
        var zlibLength = reader.ReadUInt32();
        var zipped = reader.ReadBytes((int)zlibLength);
        Check(zipped.Length == zlibLength, path, "truncated compressed payload");
        Check(reader.BaseStream.Position == reader.BaseStream.Length, path, "trailing bytes after payload");

        using var inflater = new InflaterInputStream(new MemoryStream(zipped));
        using var payloadStream = new MemoryStream();
        inflater.CopyTo(payloadStream);
        Check(payloadStream.Length == rawLength, path,
            $"payload inflated to {payloadStream.Length}, header claims {rawLength}");

        payloadStream.Position = 0;
        using var payload = new BinaryReader(payloadStream);

        var testCount = payload.ReadUInt32();
        for (uint i = 0; i < testCount; i++)
        {
            _ = payload.ReadUInt32(); // type
            var version = payload.ReadUInt32();
            Check(version >= 1, path, $"test {i}: version {version}");
            var size = payload.ReadUInt32();
            payload.BaseStream.Seek(size, SeekOrigin.Current);
        }

        var chunkCount = payload.ReadUInt32();
        for (uint i = 0; i < chunkCount; i++)
        {
            var chunkType = payload.ReadUInt32();
            var version = payload.ReadUInt32();
            Check(version >= 1, path, $"chunk {chunkType}: version {version}");
            var blockCount = payload.ReadUInt32();
            Check(blockCount > 0, path, $"chunk {chunkType}: empty chunk written");
            var dataLength = payload.ReadUInt32();
            var chunkEnd = payload.BaseStream.Position + dataLength;
            Check(chunkEnd <= payload.BaseStream.Length, path, $"chunk {chunkType}: overruns payload");

            for (uint b = 0; b < blockCount; b++)
            {
                var blockType = payload.ReadUInt32();
                var elementCount = payload.ReadUInt32();
                Check(elementCount > 0, path, $"block {blockType}: empty block written");
                var blockLength = payload.ReadUInt32();
                payload.BaseStream.Seek(blockLength, SeekOrigin.Current);
                Check(payload.BaseStream.Position <= chunkEnd, path, $"block {blockType}: overruns chunk {chunkType}");
            }
            Check(payload.BaseStream.Position == chunkEnd, path,
                $"chunk {chunkType}: blocks end {chunkEnd - payload.BaseStream.Position} bytes short of dataLength");
        }
        Check(payload.BaseStream.Position == payload.BaseStream.Length, path, "trailing bytes after chunks");
    }

    private static void Check(bool condition, string path, string message)
    {
        if (!condition)
        {
            throw new InvalidDataException($"{Path.GetFileName(path)}: {message}");
        }
    }
}
