using TRLevelControl;
using TRXInjectionTool.Actions;

namespace TRXInjectionTool.Control;

public class RoomMeta
{
    public short RoomIndex { get; set; }
    public short NumVertices { get; set; }
    public short NumQuads { get; set; }
    public short NumTriangles { get; set; }
    public short NumSprites { get; set; }
    public short NumStatic3Ds { get; set; }
    public short NumSectors { get; set; }

    public void Serialize(TRLevelWriter writer)
    {
        writer.Write(RoomIndex);
        writer.Write(NumVertices);
        writer.Write(NumQuads);
        writer.Write(NumTriangles);
        writer.Write(NumSprites);
        writer.Write(NumStatic3Ds);
        writer.Write(NumSectors);
    }

    public static List<RoomMeta> Create(InjectionData data)
    {
        var result = new List<RoomMeta>();
        // TODO: this knows too much, improve
        var edits = data.RoomEdits.Cast<IRoomMeta>()
            .Concat(data.FloorEdits.SelectMany(e => e.Fixes).Cast<IRoomMeta>());

        foreach (var edit in edits)
        {
            if (edit.RoomMetaType == ExtraRoomMeta.None)
            {
                continue;
            }

            var roomMeta = result.Find(m => m.RoomIndex == edit.RoomIndex);
            if (roomMeta == null)
            {
                result.Add(roomMeta = new()
                {
                    RoomIndex = edit.RoomIndex,
                });
            }

            switch (edit.RoomMetaType)
            {
                case ExtraRoomMeta.Vertex:
                    roomMeta.NumVertices += edit.RoomMetaUnitSize;
                    break;
                case ExtraRoomMeta.Quad:
                    roomMeta.NumQuads += edit.RoomMetaUnitSize;
                    break;
                case ExtraRoomMeta.Triangle:
                    roomMeta.NumTriangles += edit.RoomMetaUnitSize;
                    break;
                case ExtraRoomMeta.Sprite:
                    roomMeta.NumSprites += edit.RoomMetaUnitSize;
                    break;
                case ExtraRoomMeta.Static3D:
                    roomMeta.NumStatic3Ds += edit.RoomMetaUnitSize;
                    break;
                case ExtraRoomMeta.Sectors:
                    roomMeta.NumSectors += edit.RoomMetaUnitSize;
                    break;
            }
        }

        return result;
    }
}

public interface IRoomMeta
{
    public ExtraRoomMeta RoomMetaType { get; }
    public short RoomIndex { get; }
    public short RoomMetaUnitSize { get; }
}
