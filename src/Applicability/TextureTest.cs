using TRLevelControl;
using TRLevelControl.Model;

namespace TRXInjectionTool.Applicability;

public class TextureTest(int textureIndex, TRObjectTexture texture)
    : ApplicabilityTest
{
    public override ApplicabilityType Type => ApplicabilityType.TextureSample;
    public int TextureIndex { get; set; } = textureIndex;
    public TRObjectTexture Texture { get; set; } = texture;

    protected override void SerializeImpl(TRLevelWriter writer, TRGameVersion version)
    {
        writer.Write(TextureIndex);
        writer.Write((ushort)Texture.BlendingMode);
        writer.Write(Texture.Atlas);

        var info = Texture.Clone();
        if (version == TRGameVersion.TR3)
        {
            DecodeTR3ObjectTextureUVs(info);
        }
        writer.Write(info.Vertices);
    }

    public static void DecodeTR3ObjectTextureUVs(TRObjectTexture texture)
    {
        short[] uv = [.. texture.Vertices.SelectMany(v => new[] { (short)v.U, (short)v.V })];
        byte flags = 0;

        for (int i = 0; i < uv.Length; i++)
        {
            if ((uv[i] & 0x80) != 0)
            {
                uv[i] |= 0x00FF;
                flags |= (byte)(1 << i);
            }
            else
            {
                uv[i] &= unchecked((short)0xFF00);
            }
        }

        for (int i = 0; i < uv.Length; i++)
        {
            uv[i] += (flags & 1) != 0 ? (short)-256 : (short)256;
            flags >>= 1;
        }

        for (int i = 0; i < texture.Vertices.Count; i++)
        {
            texture.Vertices[i].U = (ushort)uv[i * 2];
            texture.Vertices[i].V = (ushort)uv[i * 2 + 1];
        }
    }
}
