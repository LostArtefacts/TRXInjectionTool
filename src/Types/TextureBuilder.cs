using TRLevelControl.Model;
using TRXInjectionTool.Actions;

namespace TRXInjectionTool.Types;

public abstract class TextureBuilder : InjectionBuilder
{
    protected static List<short> GetRange(int start, int count)
    {
        return Enumerable.Range(start, count).Select(i => (short)i).ToList();
    }

    protected static TRRoomTextureRotate Rotate(short roomIndex, TRMeshFaceType type, short targetIndex, byte rotations)
    {
        return new()
        {
            RoomIndex = roomIndex,
            FaceType = type,
            TargetIndex = targetIndex,
            Rotations = rotations,
        };
    }

    protected static TRRoomTextureReface Reface(TR1Level level, short roomIndex, TRMeshFaceType targetType,
        TRMeshFaceType sourceType, ushort texture, short targetIndex)
    {
        TextureSource source = GetSource(level, sourceType, texture);
        return new()
        {
            RoomIndex = roomIndex,
            FaceType = targetType,
            SourceRoom = source.Room,
            SourceFaceType = sourceType,
            SourceIndex = source.Face,
            TargetIndex = targetIndex,
        };
    }

    protected static TextureSource GetSource(TR1Level level, TRMeshFaceType type, ushort textureIndex)
    {
        for (short i = 0; i < level.Rooms.Count; i++)
        {
            TR1Room room = level.Rooms[i];
            List<TRFace> faces = type == TRMeshFaceType.TexturedQuad ? room.Mesh.Rectangles : room.Mesh.Triangles;
            int match = faces.FindIndex(f => f.Texture == textureIndex);
            if (match != -1)
            {
                return new()
                {
                    Room = i,
                    Face = (short)match,
                };
            }
        }

        return null;
    }

    protected class TextureSource
    {
        public short Room { get; set; }
        public short Face { get; set; }
    }
}
