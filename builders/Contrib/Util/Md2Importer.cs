using System.Drawing;
using TRImageControl;
using TRLevelControl.Model;

namespace TRXInjectionTool.Types.Contrib.Util;

// Reads geometry and skins out of Quake II models held in a PAK archive, for
// use as modelling reference. The archive is a local input: nothing it holds
// is redistributable, so only meshes traced by hand from what this produces
// belong in a shipped injection.
public static class Md2Importer
{
    public record Md2Model(TRMesh Mesh, List<PointF[]> TriangleUVs, string SkinName);

    public static byte[] ReadPakEntry(string pakPath, string entryName)
    {
        using var stream = File.OpenRead(pakPath);
        using var reader = new BinaryReader(stream);

        if (new string(reader.ReadChars(4)) != "PACK")
        {
            throw new InvalidDataException($"{pakPath} is not a PAK archive");
        }

        var dirOffset = reader.ReadInt32();
        var dirLength = reader.ReadInt32();

        stream.Position = dirOffset;
        for (int i = 0; i < dirLength / 64; i++)
        {
            var name = new string(reader.ReadChars(56)).TrimEnd('\0');
            var offset = reader.ReadInt32();
            var length = reader.ReadInt32();
            if (!name.Equals(entryName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            stream.Position = offset;
            return reader.ReadBytes(length);
        }

        throw new FileNotFoundException($"{entryName} not found in {pakPath}");
    }

    // Converts frame 0 to a TR mesh, scaled so the barrel spans targetLength
    // and laid out with the barrel along +Y, as TR hand meshes are.
    public static Md2Model Import(byte[] md2, int targetLength)
    {
        using var stream = new MemoryStream(md2);
        using var reader = new BinaryReader(stream);

        if (new string(reader.ReadChars(4)) != "IDP2")
        {
            throw new InvalidDataException("not an MD2 model");
        }
        if (reader.ReadInt32() != 8)
        {
            throw new InvalidDataException("unsupported MD2 version");
        }

        var skinWidth = reader.ReadInt32();
        var skinHeight = reader.ReadInt32();
        reader.ReadInt32();                     // frame size
        var skinCount = reader.ReadInt32();
        var vertexCount = reader.ReadInt32();
        reader.ReadInt32();                     // texture coordinate count
        var triangleCount = reader.ReadInt32();
        reader.ReadInt32();                     // GL command count
        var frameCount = reader.ReadInt32();
        var skinOffset = reader.ReadInt32();
        var stOffset = reader.ReadInt32();
        var triangleOffset = reader.ReadInt32();
        var frameOffset = reader.ReadInt32();

        if (frameCount < 1)
        {
            throw new InvalidDataException("model holds no frames");
        }

        var skinName = string.Empty;
        if (skinCount > 0)
        {
            stream.Position = skinOffset;
            // The field is padded to 64 bytes and what follows the first NUL
            // is whatever was in memory, so the name ends at that NUL.
            skinName = new string(reader.ReadChars(64)).Split('\0')[0];
        }

        stream.Position = stOffset;
        var st = new List<PointF>();
        for (int i = 0; i < triangleCount * 3; i++)
        {
            if (stream.Position + 4 > stream.Length)
            {
                break;
            }
            var s = reader.ReadInt16();
            var t = reader.ReadInt16();
            st.Add(new(s / (float)skinWidth, t / (float)skinHeight));
        }

        stream.Position = frameOffset;
        var scale = new[] { reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle() };
        var translate = new[] { reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle() };
        reader.ReadBytes(16);                   // frame name

        var points = new List<float[]>(vertexCount);
        for (int i = 0; i < vertexCount; i++)
        {
            var x = reader.ReadByte() * scale[0] + translate[0];
            var y = reader.ReadByte() * scale[1] + translate[1];
            var z = reader.ReadByte() * scale[2] + translate[2];
            reader.ReadByte();                  // light normal index

            // Quake II lays a weapon along +X with Z up. TR hand meshes run
            // along +Y away from the grip, so the barrel axis becomes +Y.
            points.Add([-y, -x, -z]);
        }

        var span = points.Max(p => p[1]) - points.Min(p => p[1]);
        var factor = span > 0 ? targetLength / span : 1;
        var baseY = points.Min(p => p[1]);

        var mesh = new TRMesh { Normals = [] };
        foreach (var p in points)
        {
            mesh.Vertices.Add(new()
            {
                X = (short)Math.Round(p[0] * factor),
                Y = (short)Math.Round((p[1] - baseY) * factor),
                Z = (short)Math.Round(p[2] * factor),
            });
        }

        stream.Position = triangleOffset;
        var uvs = new List<PointF[]>();
        for (int i = 0; i < triangleCount; i++)
        {
            var xyz = new[] { reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16() };
            var stIdx = new[] { reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16() };

            mesh.TexturedTriangles.Add(new()
            {
                Type = TRFaceType.Triangle,
                Vertices = [xyz[0], xyz[1], xyz[2]],
            });
            uvs.Add([.. stIdx.Select(s => s < st.Count ? st[s] : new PointF())]);
        }

        foreach (var v in mesh.Vertices)
        {
            var len = Math.Max(1, Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z));
            mesh.Normals.Add(new()
            {
                X = (short)(v.X / len * 16300),
                Y = (short)(v.Y / len * 16300),
                Z = (short)(v.Z / len * 16300),
            });
        }

        mesh.Lights = null;
        mesh.SelfCalculateBounds();
        Console.WriteLine($"MD2: {vertexCount} vertices, {triangleCount} triangles, skin {skinName}");
        return new(mesh, uvs, skinName);
    }

    // Decodes a run-length encoded 8-bit PCX and scales it to one TR page.
    // Quake II skins are lit by the engine and read very dark under TR's own
    // lighting, so brightness and saturation are raised on the way in.
    public static TRImage ImportSkin(
        byte[] pcx, int pageSize, float brightness = 1.0f, float saturation = 1.0f)
    {
        if (pcx[0] != 0x0A || pcx[2] != 1 || pcx[3] != 8)
        {
            throw new InvalidDataException("unsupported PCX format");
        }

        var xMin = BitConverter.ToUInt16(pcx, 4);
        var yMin = BitConverter.ToUInt16(pcx, 6);
        var xMax = BitConverter.ToUInt16(pcx, 8);
        var yMax = BitConverter.ToUInt16(pcx, 10);
        var bytesPerLine = BitConverter.ToUInt16(pcx, 66);
        var width = xMax - xMin + 1;
        var height = yMax - yMin + 1;

        var palette = new List<TRColour>();
        var paletteOffset = pcx.Length - 768;
        for (int i = 0; i < 256; i++)
        {
            palette.Add(new()
            {
                Red = pcx[paletteOffset + i * 3],
                Green = pcx[paletteOffset + i * 3 + 1],
                Blue = pcx[paletteOffset + i * 3 + 2],
            });
        }

        var indices = new byte[width * height];
        var pos = 128;
        for (int y = 0; y < height; y++)
        {
            var x = 0;
            while (x < bytesPerLine && pos < paletteOffset)
            {
                var b = pcx[pos++];
                var run = 1;
                if ((b & 0xC0) == 0xC0)
                {
                    run = b & 0x3F;
                    b = pcx[pos++];
                }
                for (int i = 0; i < run && x < bytesPerLine; i++, x++)
                {
                    if (x < width)
                    {
                        indices[y * width + x] = b;
                    }
                }
            }
        }

        var source = new TRImage(new Size(width, height), indices, palette);
        var page = new TRImage(new Size(pageSize, pageSize));
        page.Write((c, x, y) => Adjust(
            source.GetPixel(x * width / pageSize, y * height / pageSize),
            brightness, saturation));
        return page;
    }

    private static Color Adjust(Color color, float brightness, float saturation)
    {
        var grey = color.R * 0.299f + color.G * 0.587f + color.B * 0.114f;
        return Color.FromArgb(
            color.A,
            Channel(color.R, grey, brightness, saturation),
            Channel(color.G, grey, brightness, saturation),
            Channel(color.B, grey, brightness, saturation));
    }

    private static int Channel(byte value, float grey, float brightness, float saturation)
    {
        var saturated = grey + (value - grey) * saturation;
        return Math.Clamp((int)Math.Round(saturated * brightness), 0, 255);
    }
}
