using TRLevelControl;
using TRXInjectionTool.Actions;

namespace TRXInjectionTool.Control;

public class RoomMeshMeta
{
    public short RoomIndex { get; set; }
    public short NumVertices { get; set; }
    public short NumQuads { get; set; }
    public short NumTriangles { get; set; }
    public short NumSprites { get; set; }

    public void Serialize(TRLevelWriter writer)
    {
        writer.Write(RoomIndex);
        writer.Write(NumVertices);
        writer.Write(NumQuads);
        writer.Write(NumTriangles);
        writer.Write(NumSprites);
    }

    public static List<RoomMeshMeta> Create(InjectionData data)
    {
        List<RoomMeshMeta> result = new();
        foreach (TRRoomTextureEdit edit in data.RoomEdits)
        {
            if (edit.Meta == ExtraMeshMeta.None)
            {
                continue;
            }

            RoomMeshMeta roomMeta = result.Find(m => m.RoomIndex == edit.RoomIndex);
            if (roomMeta == null)
            {
                result.Add(roomMeta = new()
                {
                    RoomIndex = edit.RoomIndex,
                });
            }

            switch (edit.Meta)
            {
                case ExtraMeshMeta.Vertex:
                    roomMeta.NumVertices++;
                    break;
                case ExtraMeshMeta.Quad:
                    roomMeta.NumQuads++;
                    break;
                case ExtraMeshMeta.Triangle:
                    roomMeta.NumTriangles++;
                    break;
                case ExtraMeshMeta.Sprite:
                    roomMeta.NumSprites++;
                    break;
            }
        }

        return result;
    }
}
