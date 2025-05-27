using System.Diagnostics;
using System.Drawing;
using TRImageControl;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

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

    protected static TRMeshEdit FixStaticMeshPosition<T>(TRDictionary<T, TRStaticMesh> meshes, T id, TRVertex change)
        where T : Enum
    {
        return new()
        {
            ModelID = (uint)(object)id,
            VertexEdits = meshes[id].Mesh.Vertices.Select((v, i) => new TRVertexEdit
            {
                Index = (short)i,
                Change = change,
            }).ToList(),
        };
    }

    protected static void FixWolfTransparency(TRLevelBase level, InjectionData data)
    {
        TRModel model;
        if (level is TR1Level level1)
        {
            model = level1.Models[TR1Type.Wolf];
        }
        else if (level is TR2Level level2)
        {
            model = level2.Models[TR2Type.Spider];
        }
        else
        {
            throw new Exception("Unsupported level type");
        }

        List<ushort> eyeVerts = new() { 20, 13, 12, 22 };
        TRMeshFace eyeFace = model.Meshes[3]
            .TexturedRectangles.Find(t => t.Vertices.All(eyeVerts.Contains));

        FixTransparentPixels(level, data, eyeFace, Color.Black);
    }

    protected static void FixBatTransparency(TR1Level level, InjectionData data)
    {
        List<ushort> eyeVerts = new() { 0, 1, 3 };
        TRMeshFace eyeFace = level.Models[TR1Type.Bat].Meshes[4]
            .TexturedTriangles.Find(t => t.Vertices.All(eyeVerts.Contains));

        FixTransparentPixels(level, data, eyeFace, Color.Black);
    }

    protected static void FixLaraTransparency(TR2Level level, InjectionData data)
    {
        List<ushort> eyeVerts = new() { 12, 13, 34 };
        TRMeshFace eyeFace = level.Models[TR2Type.Lara].Meshes[14]
            .TexturedTriangles.Find(t => t.Vertices.All(eyeVerts.Contains));

        FixTransparentPixels(level, data, eyeFace, Color.FromArgb(249, 236, 217));
    }

    protected static void FixTransparentPixels(TRLevelBase level, InjectionData data, TRFace face, Color fillColour)
    {
        Debug.Assert(face != null);

        TRObjectTexture texInfo = level.ObjectTextures[face.Texture];
        TRImage tile;
        if (level is TR1Level level1)
        {
            tile = new(level1.Images8[texInfo.Atlas].Pixels, level1.Palette);
        }
        else if (level is TR2Level level2)
        {
            tile = new(level2.Images16[texInfo.Atlas].Pixels);
        }
        else
        {
            throw new Exception("Unsupported level type");
        }

        TRImage img = tile.Export(texInfo.Bounds);
        img.Write((c, x, y) => c.A == 0 ? fillColour : c);

        data.TextureOverwrites.Add(new()
        {
            Page = texInfo.Atlas,
            X = (byte)texInfo.Bounds.X,
            Y = (byte)texInfo.Bounds.Y,
            Width = (ushort)texInfo.Size.Width,
            Height = (ushort)texInfo.Size.Height,
            Data = img.ToRGBA(),
        });
    }

    protected class TextureSource
    {
        public short Room { get; set; }
        public short Face { get; set; }
    }
}
