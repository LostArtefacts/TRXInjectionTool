using System.Diagnostics;
using System.Drawing;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

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

    protected static TRRoomTextureReface Reface(TRLevelBase level, short roomIndex, TRMeshFaceType targetType,
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

    protected static TextureSource GetSource(TRLevelBase level, TRMeshFaceType type, ushort textureIndex)
    {
        List<List<TRFace>> meshes;
        if (level is TR1Level level1)
        {
            meshes = level1.Rooms
                .Select(r => type == TRMeshFaceType.TexturedQuad ? r.Mesh.Rectangles : r.Mesh.Triangles)
                .ToList();
        }
        else if (level is TR2Level level2)
        {
            meshes = level2.Rooms
                .Select(r => type == TRMeshFaceType.TexturedQuad ? r.Mesh.Rectangles : r.Mesh.Triangles)
                .ToList();
        }
        else
        {
            throw new Exception();
        }

        for (short i = 0; i < meshes.Count; i++)
        {
            List<TRFace> faces = meshes[i];
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

    protected static TR1Level CreateAtlantisContinuityLevel(TR1Type startSceneryIdx)
    {
        TR1Level pyramid = _control1.Read($"Resources/{TR1LevelNames.PYRAMID}");

        TRMesh lightningBoxMesh = pyramid.Models[TR1Type.ThorLightning].Meshes[0];
        lightningBoxMesh.Vertices.ForEach(v => v.Y += 52);

        TRMesh doorMesh = pyramid.Models[TR1Type.Door2].Meshes[0];
        new[] { 1, 2, 5, 6 }.Select(i => doorMesh.Vertices[i])
            .ToList().ForEach(v => v.X -= 26);

        var statics = new[]
        {
            new TRStaticMesh
            {
                Mesh = lightningBoxMesh,
                CollisionBox = lightningBoxMesh.GetBounds(),
                VisibilityBox = lightningBoxMesh.GetBounds(),
                Visible = true,
            },
            new TRStaticMesh
            {
                Mesh = doorMesh,
                CollisionBox = doorMesh.GetBounds(),
                VisibilityBox = doorMesh.GetBounds(),
                Visible = true,
            },
        };

        var packer = new TR1TexturePacker(pyramid);
        var regions = packer.GetMeshRegions(statics.Select(s => s.Mesh))
            .Values.SelectMany(v => v);
        var originalInfos = pyramid.ObjectTextures.ToList();
        ResetLevel(pyramid, 1);

        packer = new(pyramid);
        packer.AddRectangles(regions);
        packer.Pack(true);

        for (int i = 0; i < statics.Length; i++)
        {
            pyramid.StaticMeshes[(TR1Type)((int)startSceneryIdx + i)] = statics[i];
        }
        pyramid.ObjectTextures.AddRange(regions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));
        statics.Select(s => s.Mesh)
            .SelectMany(m => m.TexturedFaces)
            .ToList()
            .ForEach(f =>
            {
                f.Texture = (ushort)pyramid.ObjectTextures.IndexOf(originalInfos[f.Texture]);
            });

        return pyramid;
    }

    protected static void FixHomeWindows(TR2Level baseLevel, string baseName)
    {
        var venice = _control2.Read($"Resources/{TR2LevelNames.VENICE}");
        CreateModelLevel(venice, TR2Type.BreakableWindow1);

        var home = _control2.Read($"Resources/{baseName}");
        var ogWindow = home.Models[TR2Type.BreakableWindow1];
        var normalImg = GetImage(ogWindow.Meshes[0].TexturedFaces.First().Texture, home);
        var smashedImg = GetImage(ogWindow.Meshes[8].TexturedFaces.First().Texture, home);

        var window = venice.Models[TR2Type.BreakableWindow1];
        window.Animations = ogWindow.Animations;
        ImportImage(window.Meshes[0].TexturedFaces.First().Texture, normalImg, venice);
        ImportImage(window.Meshes[8].TexturedFaces.First().Texture, smashedImg, venice);

        for (int i = 0; i < window.Meshes.Count; i++)
        {
            window.Meshes[i].Normals = ogWindow.Meshes[i].Normals;
            window.Meshes[i].Lights = ogWindow.Meshes[i].Lights;
        }

        // Replace the wooden bit to match the house windows: off-brown => off-purple
        var paneImg = GetImage(window.Meshes[1].TexturedFaces.First().Texture, venice);
        paneImg.Write((c, x, y) =>
        {
            if (c.A == 0 || c.R >= 200)
            {
                return c;
            }
            int g = (int)(c.R * 0.95f);
            int b = (int)(g * 1.3f);
            return Color.FromArgb(c.R, g, b);
        });
        ImportImage(window.Meshes[1].TexturedFaces.First().Texture, paneImg, venice);

        var packer = new TR2TexturePacker(venice);
        var regions = packer.GetMeshRegions(window.Meshes)
            .Values.SelectMany(v => v);
        packer = new(baseLevel);
        packer.AddRectangles(regions);
        packer.Pack(true);

        baseLevel.Models[TR2Type.BreakableWindow1] = window;
        window.Meshes.SelectMany(m => m.TexturedFaces)
            .ToList().ForEach(f => f.Texture += (ushort)baseLevel.ObjectTextures.Count);
        baseLevel.ObjectTextures.AddRange(venice.ObjectTextures);
        GenerateImages8(baseLevel, baseLevel.Palette.Select(c => c.ToTR1Color()).ToList());
    }

    private static TRImage GetImage(ushort texture, TR2Level level)
    {
        var texInfo = level.ObjectTextures[texture];
        var page = level.Images16[texInfo.Atlas];
        return new TRImage(page.Pixels)
            .Export(texInfo.Bounds);
    }

    private static void ImportImage(ushort texture, TRImage img, TR2Level level)
    {
        var texInfo = level.ObjectTextures[texture];
        var page = level.Images16[texInfo.Atlas];
        var image = new TRImage(page.Pixels);
        image.Import(img, texInfo.Position);
        level.Images16[texInfo.Atlas].Pixels = image.ToRGB555();
    }

    protected static void FixToilets(TR2Level level, string levelName)
    {
        TR2Level gym = _control2.Read($"Resources/{TR2LevelNames.ASSAULT}");
        var statics = new List<TRStaticMesh>
        {
            gym.StaticMeshes[TR2Type.SceneryBase + 4],
        };
        if (levelName == TR2LevelNames.HOME)
        {
            statics.Add(gym.StaticMeshes[TR2Type.SceneryBase + 45]);
        }

        var packer = new TR2TexturePacker(gym);
        var regions = packer.GetMeshRegions(statics.Select(s => s.Mesh))
            .Values.SelectMany(v => v);
        var originalInfos = gym.ObjectTextures.ToList();

        packer = new(level);
        packer.AddRectangles(regions);
        packer.Pack(true);

        level.StaticMeshes[TR2Type.SceneryBase + 4] = statics[0];
        if (statics.Count > 1)
        {
            level.StaticMeshes[TR2Type.SceneryBase + 26] = statics[1];
        }
        level.ObjectTextures.AddRange(regions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));
        statics.Select(s => s.Mesh)
            .SelectMany(m => m.TexturedFaces)
            .ToList()
            .ForEach(f =>
            {
                f.Texture = (ushort)level.ObjectTextures.IndexOf(originalInfos[f.Texture]);
            });

        GenerateImages8(level, level.Palette.Select(c => c.ToTR1Color()).ToList());
    }

    protected static void FixHomeStatues(TR2Level level)
    {
        TR2Level gym = _control2.Read($"Resources/{TR2LevelNames.ASSAULT}");
        var statues = new List<TRStaticMesh>
        {
            gym.StaticMeshes[TR2Type.SceneryBase + 16],
            gym.StaticMeshes[TR2Type.SceneryBase + 38],
        };

        var packer = new TR2TexturePacker(gym);
        var regions = packer.GetMeshRegions(statues.Select(s => s.Mesh))
            .Values.SelectMany(v => v).ToList();
        var originalInfos = gym.ObjectTextures.ToList();

        var img = new TRImage(8, 8);
        img.Fill(Color.FromArgb(40, 24, 24));

        var texInfo = new TRObjectTexture(0, 0, 8, 8);
        regions.Add(new()
        {
            Image = img,
            Bounds = texInfo.Bounds,
            Segments = new()
            {
                new()
                {
                    Index = regions.Count,
                    Texture = texInfo,
                }
            }
        });

        packer = new(level);
        packer.AddRectangles(regions);
        packer.Pack(true);

        level.StaticMeshes[TR2Type.SceneryBase + 16] = statues[0];
        level.StaticMeshes[TR2Type.SceneryBase + 38] = statues[1];
        level.ObjectTextures.AddRange(regions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));
        
        statues.SelectMany(s => s.Mesh.TexturedFaces)
            .ToList().ForEach(f =>
            {
                f.Texture = (ushort)level.ObjectTextures.IndexOf(originalInfos[f.Texture]);
            });
        statues[0].Mesh.ColouredRectangles.ForEach(f => f.Texture = (ushort)(level.ObjectTextures.Count - 1));
        statues[0].Mesh.TexturedRectangles.AddRange(statues[0].Mesh.ColouredRectangles);
        statues[0].Mesh.ColouredRectangles.Clear();

        statues[1].Mesh.TexturedRectangles.Add(new()
        {
            Type = TRFaceType.Rectangle,
            Texture = statues[1].Mesh.TexturedRectangles[11].Texture,
            Vertices = new() { 20, 23, 22, 21 }
        });

        GenerateImages8(level, level.Palette.Select(c => c.ToTR1Color()).ToList());
    }

    protected static void FixPassport(TRLevelBase level, InjectionData data)
    {
        Dictionary<uint, short> typeMap;
        if (level is TR1Level level1)
        {
            typeMap = new()
            {
                [(uint)TR1Type.PassportClosed_M_H] = -2,
                [(uint)TR1Type.PassportOpen_M_H] = -1,
            };

            var texInfos = new[]
            {
                level1.ObjectTextures[level1.Models[TR1Type.PassportClosed_M_H].Meshes[0].TexturedRectangles[^1].Texture],
                level1.ObjectTextures[level1.Models[TR1Type.PassportOpen_M_H].Meshes[1].TexturedRectangles[^1].Texture],
            }.Distinct();

            foreach (var texInfo in texInfos)
            {
                var texFixBounds = new Rectangle(texInfo.Bounds.X + 1, texInfo.Bounds.Y, 1, texInfo.Size.Height);
                var img = new TRImage(level1.Images8[texInfo.Atlas].Pixels, level1.Palette)
                    .Export(texFixBounds);
                data.TextureOverwrites.Add(new()
                {
                    Page = texInfo.Atlas,
                    X = (byte)texInfo.Bounds.X,
                    Y = (byte)texInfo.Bounds.Y,
                    Width = (ushort)img.Width,
                    Height = (ushort)img.Height,
                    Data = img.ToRGBA(),
                });
            }
        }
        else if (level is TR2Level)
        {
            typeMap = new()
            {
                [(uint)TR2Type.PassportClosed_M_H] = -2,
                [(uint)TR2Type.PassportOpen_M_H] = -1,
            };
        }
        else
        {
            throw new Exception();
        }

        foreach (var (type, shift) in typeMap)
        {
            data.MeshEdits.Add(new()
            {
                ModelID = type,
                VertexEdits = new[] { 1, 2, 5, 6 }.Select(i =>
                {
                    return new TRVertexEdit
                    {
                        Index = (short)i,
                        Change = new() { X = shift },
                    };
                }).ToList(),
            });
        }
    }

    protected class TextureSource
    {
        public short Room { get; set; }
        public short Face { get; set; }
    }
}
