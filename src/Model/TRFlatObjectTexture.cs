using TRLevelControl;
using TRLevelControl.Model;
using LR = TRLevelReader;

namespace TRXInjectionTool.Model;

public class TRFlatObjectTexture
{
    public ushort Attribute { get; set; }
    public ushort TileAndFlag { get; set; }
    public ushort NewFlags { get; set; }
    public LR.Model.TRObjectTextureVert[] Vertices { get; set; }
    public uint OriginalU { get; set; }
    public uint OriginalV { get; set; }
    public uint WidthMinusOne { get; set; }
    public uint HeightMinusOne { get; set; }

    public void Serialize(TRLevelWriter writer, TRGameVersion version)
    {
        writer.Write(Attribute);
        writer.Write(TileAndFlag);
        if (version >= TRGameVersion.TR4)
        {
            writer.Write(NewFlags);
        }

        foreach (var vert in Vertices)
        {
            writer.Write(vert.Serialize());
        }

        if (version >= TRGameVersion.TR4)
        {
            writer.Write(OriginalU);
            writer.Write(OriginalV);
            writer.Write(WidthMinusOne);
            writer.Write(HeightMinusOne);
        }
    }
}
