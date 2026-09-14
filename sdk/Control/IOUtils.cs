using TRImageControl;

namespace TRXInjectionTool.Control;

public static class IOUtils
{
    public static uint MakeTag(char a, char b, char c, char d)
    {
        return (uint)(a | b << 8 | c << 16 | d << 24);
    }

    public static uint[] ToRGBA(this TRImage image)
    {
        uint[] pixels = new uint[image.Width * image.Height];
        image.Read((c, x, y) =>
        {
            pixels[y * image.Width + x] = (uint)((c.A << 24) | (c.B << 16) | (c.G << 8) | c.R);
        });
        return pixels;
    }
}
