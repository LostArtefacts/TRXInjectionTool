using System.Diagnostics;
using System.Drawing;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl;
using TRLevelControl.Helpers;
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

    public static TRRoomTextureCreate CreateFace(short roomIdx, short sourceRoom, ushort sourceIndex,
        TRMeshFaceType faceType, ushort[] vertices)
    {
        return new()
        {
            RoomIndex = roomIdx,
            FaceType = faceType,
            SourceRoom = sourceRoom,
            SourceIndex = (short)sourceIndex,
            Vertices = new(vertices),
        };
    }

    protected static TRRoomVertexCreate CreateVertex(short roomIdx, TR1Room room, TR1RoomVertex vertex, short lighting = -1, short shift = 256)
    {
        room.Mesh.Vertices.Add(vertex.Clone() as TR1RoomVertex);
        return new()
        {
            RoomIndex = roomIdx,
            Vertex = new()
            {
                Lighting = lighting == -1 ? vertex.Lighting : lighting,
                Vertex = new()
                {
                    X = vertex.Vertex.X,
                    Y = (short)(vertex.Vertex.Y + shift),
                    Z = vertex.Vertex.Z,
                },
            },
        };
    }

    protected static TRRoomVertexCreate CreateVertex(short roomIdx, TR2Room room, TR2RoomVertex vertex, short lighting = -1, short shift = 256)
    {
        room.Mesh.Vertices.Add(vertex.Clone() as TR2RoomVertex);
        return new()
        {
            RoomIndex = roomIdx,
            Vertex = new()
            {
                Lighting = lighting == -1 ? vertex.Lighting : lighting,
                Vertex = new()
                {
                    X = vertex.Vertex.X,
                    Y = (short)(vertex.Vertex.Y + shift),
                    Z = vertex.Vertex.Z,
                },
            },
        };
    }

    protected static TRRoomVertxFlagChange SetLastVertexFlags(short roomIdx, TR2Room room)
    {
        return SetVertexFlags(roomIdx, room, (ushort)(room.Mesh.Vertices.Count - 1));
    }

    protected static TRRoomVertxFlagChange SetVertexFlags(short roomIdx, TR2Room room, ushort vtxIdx)
    {
        var vertex = room.Mesh.Vertices[vtxIdx];
        return new()
        {
            RoomIndex = roomIdx,
            VertexIndex = vtxIdx,
            Flags = vertex.Attributes,
        };
    }

    protected static TRRoomTextureMove CreateQuadShift(short roomIdx, short targetIdx, List<TRRoomVertexRemap> remap)
    {
        return new()
        {
            RoomIndex = roomIdx,
            FaceType = TRMeshFaceType.TexturedQuad,
            TargetIndex = targetIdx,
            VertexRemap = remap,
        };
    }

    protected static TRVisPortalEdit CreatePortalFix(short baseRoom, short linkRoom, ushort index, List<TRVertex> changes)
    {
        return new()
        {
            BaseRoom = baseRoom,
            LinkRoom = linkRoom,
            PortalIndex = index,
            VertexChanges = changes,
        };
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

    protected static TRImage GetImage(ushort texture, TR1Level level)
    {
        var texInfo = level.ObjectTextures[texture];
        var page = level.Images8[texInfo.Atlas];
        return new TRImage(page.Pixels, level.Palette)
            .Export(texInfo.Bounds);
    }

    protected static TRImage GetImage(ushort texture, TR2Level level)
    {
        var texInfo = level.ObjectTextures[texture];
        var page = level.Images16[texInfo.Atlas];
        return new TRImage(page.Pixels)
            .Export(texInfo.Bounds);
    }

    protected static void ImportImage(ushort texture, TRImage img, TR1Level level)
    {
        var texInfo = level.ObjectTextures[texture];
        var page = level.Images8[texInfo.Atlas];
        var image = new TRImage(page.Pixels, level.Palette);
        image.Import(img, texInfo.Position);
        level.Images8[texInfo.Atlas].Pixels = image.ToRGB(level.Palette);
    }

    protected static void ImportImage(ushort texture, TRImage img, TR2Level level)
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

    protected static void FixCatStatue(TRLevelBase level)
    {
        var khamoon = _control1.Read($"Resources/{TR1LevelNames.KHAMOON}");

        var statue = khamoon.StaticMeshes[TR1Type.SceneryBase + 31];
        var max = statue.Mesh.Vertices.Max(v => v.Y);
        statue.Mesh.Vertices.ForEach(v => v.Y -= max);
        statue.VisibilityBox.MaxX -= 64;
        statue.VisibilityBox.MinX -= 64;
        statue.CollisionBox.MaxX -= 64;
        statue.CollisionBox.MinX -= 64;

        new[] { 66, 81, 80, 62, 61, 79 }
            .ToList().ForEach(v => statue.Mesh.Vertices[v].Y = 0);
        
        var vert = statue.Mesh.Vertices[65].Clone();
        vert.Y = 0;
        statue.Mesh.Vertices[84] = vert;

        var verts = new List<List<ushort>>
        {
            new() { 65, 82, 67 },
            new() { 65, 64, 86 },
            new() { 65, 86, 82 },
            new() { 79, 84, 65 },
            new() { 65, 84, 64 },

            new() { 70, 85, 72 },
            new() { 70, 72, 71 },

            new() { 66, 65, 64 },
            new() { 65, 63, 64 },

            new() { 58, 55, 39 },
            new() { 55, 59, 39 },

            new() { 60, 56, 40 },
            new() { 56, 57, 40 },

            new() { 73, 40, 76 },
            new() { 76, 78, 73 },
        };
        statue.Mesh.TexturedTriangles.AddRange(verts.Select(v =>
            new TRMeshFace
            {
                Type = TRFaceType.Triangle,
                Texture = statue.Mesh.TexturedRectangles[55].Texture,
                Vertices = v,
            }));

        statue.Mesh.TexturedTriangles[39].Texture = statue.Mesh.TexturedTriangles[40].Texture
            = statue.Mesh.TexturedRectangles[43].Texture;
        statue.Mesh.TexturedTriangles[41].Texture = statue.Mesh.TexturedTriangles[42].Texture
            = statue.Mesh.TexturedRectangles[44].Texture;
        statue.Mesh.TexturedTriangles[43].Texture = statue.Mesh.TexturedTriangles[44].Texture
            = statue.Mesh.TexturedRectangles[52].Texture;

        statue.Mesh.TexturedTriangles[24].Vertices = new() { 39, 75, 65 };
        statue.Mesh.TexturedTriangles[27].Vertices = new() { 80, 62, 78 };
        statue.Mesh.TexturedTriangles[28].Vertices = new() { 73, 78, 62 };
        statue.Mesh.TexturedTriangles[29].Vertices = new() { 83, 85, 70 };
        statue.Mesh.TexturedRectangles[48].Vertices = new() { 63, 68, 69, 64 };
        statue.Mesh.TexturedRectangles[49].Vertices = new() { 68, 71, 72, 69 };

        new[] { 43, 44, 45, 52, 55, 58 }
            .OrderByDescending(i => i)
            .ToList().ForEach(i => statue.Mesh.TexturedRectangles.RemoveAt(i));

        var packer = new TR1TexturePacker(khamoon);
        var regions = packer.GetMeshRegions(new[] { statue.Mesh })
            .Values.SelectMany(v => v);
        var originalInfos = khamoon.ObjectTextures.ToList();

        if (level is TR1Level level1)
        {
            var levelPacker = new TR1TexturePacker(level1);
            levelPacker.AddRectangles(regions);
            levelPacker.Pack(true);
            level1.StaticMeshes[TR1Type.SceneryBase + 31] = statue;
        }
        else if (level is TR2Level level2)
        {
            var levelPacker = new TR2TexturePacker(level2);
            levelPacker.AddRectangles(regions);
            levelPacker.Pack(true);
            level2.StaticMeshes[TR2Type.SceneryBase + 37] = statue;
            GenerateImages8(level2, level2.Palette.Select(c => c.ToTR1Color()).ToList());
        }
        else
        {
            throw new Exception();
        }

        level.ObjectTextures.AddRange(regions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));
        statue.Mesh.TexturedFaces
            .ToList().ForEach(f =>
            {
                f.Texture = (ushort)level.ObjectTextures.IndexOf(originalInfos[f.Texture]);
            });
    }

    protected static void FixBigPod(InjectionData data, string levelName)
    {
        var targetType = levelName == TR1LevelNames.MINES_CUT || levelName == TR1LevelNames.ATLANTIS_CUT
            ? TR1Type.AtlanteanEgg
            : TR1Type.AdamEgg;
        var level = _control1.Read($"Resources/{TR1LevelNames.PYRAMID}");
        level.Models = new()
        {
            [targetType] = level.Models[TR1Type.AdamEgg],
        };
        foreach (var frame in level.Models[targetType].Animations.SelectMany(a => a.Frames))
        {
            frame.Bounds.MinX -= 254;
            frame.Bounds.MaxX -= 254;
            frame.OffsetX -= 254;

            frame.Bounds.MinZ += 262;
            frame.Bounds.MaxZ += 262;
            frame.OffsetZ += 262;

            frame.Bounds.MinY += 72;
            frame.Bounds.MaxY += 72;
            frame.OffsetY += 72;
        }

        data.FrameReplacements.AddRange(TRFrameReplacement.CreateFrom(level));

        var tarLevel = _control1.Read($"Resources/{levelName}");
        foreach (var item in tarLevel.Entities.Where(e => e.TypeID == targetType))
        {
            var x = (item.X & ~TRConsts.WallMask) + TRConsts.Step2;
            var z = (item.Z & ~TRConsts.WallMask) + TRConsts.Step2;
            if (x == item.X && z == item.Z)
            {
                continue;
            }
            item.X = x;
            item.Z = z;
            var idx = (short)tarLevel.Entities.IndexOf(item);
            if (idx == 47 && levelName == TR1LevelNames.HIVE)
            {
                item.X += TRConsts.Step1;
            }
            data.ItemPosEdits.Add(new()
            {
                Index = idx,
                Item = item,
            });
        }
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

    protected static void FixPushButton(InjectionData data)
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.ASSAULT}");
        level.Models = new()
        {
            [TR2Type.PushButtonSwitch] = level.Models[TR2Type.PushButtonSwitch],
        };

        foreach (var frame in level.Models[TR2Type.PushButtonSwitch].Animations.SelectMany(a => a.Frames))
        {
            frame.OffsetZ += 18;
            frame.Bounds.MinZ += 18;
            frame.Bounds.MaxZ += 18;
        }

        data.FrameReplacements.AddRange(TRFrameReplacement.CreateFrom(level));
    }

    protected static void FixWheelDoor(InjectionData data, string levelName)
    {
        var rig = _control2.Read($"Resources/{TR2LevelNames.RIG}");
        var door = rig.Models[TR2Type.LiftingDoor1];
        var knob = rig.Models[TR2Type.WheelKnob];

        door.Animations[2].Frames[^1].Rotations[0].Y++;
        door.Animations[2].Frames.Add(door.Animations[2].Frames[^1]);
        door.Animations[2].Frames.RemoveAt(0);

        knob.Animations[2].Frames[0].Rotations[0].Y = 0;
        knob.Animations[2].Frames.Insert(0, knob.Animations[2].Frames[0]);
        knob.Animations[2].Frames.RemoveAt(knob.Animations[2].Frames.Count - 1);

        knob.Animations[2].Frames[0].OffsetX--;
        knob.Animations[2].Frames[0].OffsetZ -= 2;

        short[] offsetShifts = { 1, 2, 4, 8, 8, 8, 8 };
        for (int i = 0; i < offsetShifts.Length; i++)
        {
            knob.Animations[2].Frames[^(offsetShifts.Length - i)].OffsetX -= offsetShifts[i];
        }

        knob.Animations[2].Frames[43].OffsetX = knob.Animations[2].Frames[44].OffsetX;
        knob.Animations[2].Frames[45].OffsetX = knob.Animations[2].Frames[46].OffsetX;

        foreach (var frame in door.Animations.SelectMany(a => a.Frames))
        {
            frame.OffsetY = -512;
            frame.Bounds.MinY = -1024;
            frame.Bounds.MaxY = 0;
        }

        ResetLevel(rig);
        rig.Models = new()
        {
            [levelName == TR2LevelNames.DORIA ?  TR2Type.Door2 : TR2Type.LiftingDoor1] = door,
            [TR2Type.WheelKnob] = knob,
        };

        var replacements = TRFrameReplacement.CreateFrom(rig).ToList();
        var knobReplacement = replacements.Find(r => r.ModelID == (uint)TR2Type.WheelKnob);
        knobReplacement.Frames.Remove(0);
        knobReplacement.Frames.Remove(1);
        data.FrameReplacements.AddRange(replacements);
    }

    protected static void FixSlidingOffshoreDoor(InjectionData data, string levelName)
    {
        var rig = _control2.Read($"Resources/{TR2LevelNames.RIG}");
        var door = rig.Models[TR2Type.Door5];
        var maxY = door.Meshes[1].Vertices.Max(v => v.Y);
        data.MeshEdits.Add(new()
        {
            ModelID = (uint)TR2Type.Door5,
            MeshIndex = 1,
            VertexEdits = door.Meshes[1].Vertices.Where(v => v.Y == maxY)
                .Select(v => new TRVertexEdit
                {
                    Index = (short)door.Meshes[1].Vertices.IndexOf(v),
                    Change = new() { Y = 1 },
                }).ToList(),
        });

        foreach (var frame in door.Animations.SelectMany(a => a.Frames))
        {
            frame.OffsetY = -512;
            frame.OffsetZ += 6;
            frame.Bounds.MinY = -1024;
            frame.Bounds.MaxY = 0;
            frame.Bounds.MinZ += 6;
            frame.Bounds.MaxZ += 6;
        }

        rig.Models = new()
        {
            [TR2Type.Door5] = door,
        };
        data.FrameReplacements.AddRange(TRFrameReplacement.CreateFrom(rig));

        if (levelName == TR2LevelNames.RIG)
        {
            rig.Entities[79].Z += 1024;
            data.ItemPosEdits.Add(ItemBuilder.SetAngle(rig, 79, 0));
        }
    }

    protected static void FixPlaceholderBridges(TR2Level level, string levelName,
        TRDictionary<TR2Type, TRStaticMesh> bridges)
    {
        // Get rid of bad geometry placeholder textures
        var supportBridge = bridges.Keys.First();
        var verts1 = new ushort[] { 18, 19, 26, 27 };
        var verts2 = new ushort[] { 18, 19, 22, 23 };
        bridges[supportBridge].Mesh.TexturedRectangles.RemoveAll(f =>
            f.Vertices.All(verts1.Contains) || f.Vertices.All(verts2.Contains));

        var packer = new TR2TexturePacker(level);
        var regions = packer.GetMeshRegions(bridges.Values.Select(s => s.Mesh))
            .Values.SelectMany(v => v);
        var originalInfos = level.ObjectTextures.ToList();
        ResetLevel(level, 1);

        packer = new(level);
        packer.AddRectangles(regions);
        packer.Pack(true);

        level.StaticMeshes = bridges;
        level.ObjectTextures.AddRange(regions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));
        bridges.Values.SelectMany(s => s.Mesh.TexturedFaces)
            .ToList().ForEach(f => f.Texture = (ushort)level.ObjectTextures.IndexOf(originalInfos[f.Texture]));

        foreach (var (type, bridge) in bridges)
        {
            var mesh = bridge.Mesh;
            bridge.NonCollidable = levelName == TR2LevelNames.MONASTERY || type != supportBridge;
            if (type == supportBridge && levelName == TR2LevelNames.MONASTERY)
            {
                // Fix the supports extending too far into the bridge itself
                foreach (var v in new[] { 3, 7, 11, 15 })
                {
                    mesh.Vertices[v].Y += 32;
                }
                foreach (var v in new[] { 0, 4, 8, 12 })
                {
                    mesh.Vertices[v].Y += 10;
                }
            }

            // Fix missing double-sided faces
            var singleSided = type == supportBridge
                ? mesh.TexturedRectangles.FindAll(f => f.Vertices.All(v => v > 15))
                : mesh.TexturedRectangles.ToList();
            mesh.TexturedRectangles.AddRange(singleSided.Select(f =>
                new TRMeshFace
                {
                    Type = TRFaceType.Rectangle,
                    Texture = f.Texture,
                    Vertices = level.ObjectTextures[f.Texture].Size.Height == 64
                        ? new() { f.Vertices[1], f.Vertices[0], f.Vertices[3], f.Vertices[2] }
                        : new() { f.Vertices[3], f.Vertices[2], f.Vertices[1], f.Vertices[0] }
                }));
        }
    }

    protected class TextureSource
    {
        public short Room { get; set; }
        public short Face { get; set; }
    }
}
