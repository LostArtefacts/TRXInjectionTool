using TRLevelControl;
using TRLevelControl.Model;
using LR = TRLevelReader;

namespace TRXInjectionTool.Model;

public class TRFlatMesh
{
    public LR.Model.TRVertex Centre { get; set; }
    public int CollRadius { get; set; }
    public List<LR.Model.TRVertex> Vertices { get; set; }

    public short NumNormals { get; set; }
    public List<LR.Model.TRVertex> Normals { get; set; }
    public List<short> Lights { get; set; }

    public List<TRMeshFace> TexturedRectangles { get; set; }
    public List<TRMeshFace> TexturedTriangles { get; set; }
    public List<TRMeshFace> ColouredRectangles { get; set; }
    public List<TRMeshFace> ColouredTriangles { get; set; }

    public byte[] Serialize(TRGameVersion version)
    {
        using var ms = new MemoryStream();
        using var writer = new TRLevelWriter(ms);
        writer.Write(Centre.Serialize());
        writer.Write(CollRadius);
        writer.Write((short)Vertices.Count);
        foreach (var vert in Vertices)
        {
            writer.Write(vert.Serialize());
        }

        if (Normals != null)
        {
            writer.Write((short)Normals.Count);
            foreach (var normal in Normals)
            {
                writer.Write(normal.Serialize());
            }
        }
        else
        {
            writer.Write((short)-Lights.Count);
            writer.Write(Lights);
        }

        writer.Write((short)TexturedRectangles.Count);
        writer.Write(TexturedRectangles, version);
        writer.Write((short)TexturedTriangles.Count);
        writer.Write(TexturedTriangles, version);

        if (version < TRGameVersion.TR4)
        {
            writer.Write((short)ColouredRectangles.Count);
            writer.Write(ColouredRectangles, version);
            writer.Write((short)ColouredTriangles.Count);
            writer.Write(ColouredTriangles, version);
        }

        var pad = writer.BaseStream.Position % 4;
        for (int i = 0; i < pad; i++)
        {
            writer.Write((byte)0);
        }

        return ms.ToArray();
    }
}
