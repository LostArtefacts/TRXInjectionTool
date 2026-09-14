using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.Text;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Applicability;
using A = TRXInjectionTool.Applicability;
using TRXInjectionTool.Format;
using TRXInjectionTool.Util;
using W = TRXInjectionTool.Format;

namespace TRXInjectionTool.Control;

// Serializes InjectionData into the TRXI format by mapping it onto the format
// DTOs in sdk/Format and reflecting over those with FormatWriter. The
// DTOs are the format; this class only translates the in-memory model.
public class InjectionExporter : IInjectionExporter
{
    public byte[] Serialize(InjectionData data)
    {
        using var payloadStream = new MemoryStream();
        using var payload = new BinaryWriter(payloadStream);

        WriteTests(data, payload);
        WriteChunks(data, payload);

        byte[] rawData = payloadStream.ToArray();
        using MemoryStream zipStream = new();
        using (DeflaterOutputStream deflater = new(zipStream) { IsStreamOwner = false })
        {
            deflater.Write(rawData, 0, rawData.Length);
            deflater.Finish();
        }
        byte[] zippedData = zipStream.ToArray();

        using MemoryStream finalStream = new();
        using BinaryWriter final = new(finalStream);
        final.Write(Encoding.ASCII.GetBytes(W.Container.Magic));
        final.Write((uint)W.Container.FormatMajor);
        final.Write((uint)data.InjectionType);
        final.Write((uint)rawData.Length);
        final.Write((uint)zippedData.Length);
        final.Write(zippedData);
        return finalStream.ToArray();
    }

    private static FormatObjRef Ref(int id, TRObjectType type, TRGameVersion version)
    {
        if (type is TRObjectType.Static2D or TRObjectType.Static3D)
        {
            id -= version.GetSceneryBase();
        }
        return new() { Kind = (int)type, Id = id };
    }

    private static FormatVertex Vert(TRLevelReader.Model.TRVertex v)
        => new() { X = v.X, Y = v.Y, Z = v.Z };

    private static FormatVertex Vert(TRVertex v)
        => new() { X = v.X, Y = v.Y, Z = v.Z };

    // --- Applicability tests ---

    private static void WriteTests(InjectionData data, BinaryWriter writer)
    {
        List<(int Type, int Version, object Dto)> tests =
            [.. data.ApplicabilityTests.Select(t => MapTest(t, data.GameVersion))];
        if (!data.AppliesToAllGames)
        {
            tests.Insert(0, (4, 1, new W.GameVersionTest { Game = (int)data.GameVersion + 1 }));
        }

        writer.Write((uint)tests.Count);
        foreach (var (type, version, dto) in tests)
        {
            using var ms = new MemoryStream();
            using var sub = new BinaryWriter(ms);
            FormatWriter.Write(sub, dto);
            var bytes = ms.ToArray();
            writer.Write((uint)type);
            writer.Write((uint)version);
            writer.Write((uint)bytes.Length);
            writer.Write(bytes);
        }
    }

    private static (int, int, object) MapTest(ApplicabilityTest test, TRGameVersion version) => test switch
    {
        A.ItemMetaTest t => (0, 1, new W.ItemMetaTest
        {
            Index = t.Index,
            Type = Ref(t.TypeID, TRObjectType.Game, version),
            X = t.X,
            Y = t.Y,
            Z = t.Z,
            Room = t.Room,
            Angle = t.Angle,
        }),
        A.RoomCountTest t => (1, 1, new W.RoomCountTest { Count = t.RoomCount }),
        A.RoomMetaTest t => (2, 1, new W.RoomMetaTest
        {
            Index = t.Index,
            Info = new()
            {
                X = t.Info.X,
                Z = t.Info.Z,
                YBottom = t.Info.YBottom,
                YTop = t.Info.YTop,
            },
            XSize = t.XSize,
            ZSize = t.ZSize,
        }),
        A.TextureTest t => (3, 1, MapTextureTest(t, version)),
        Applicability.GameVersionTest => throw new InvalidDataException(
            "GameVersionTest is written implicitly; do not add it to ApplicabilityTests"),
        _ => throw new InvalidDataException($"unmapped applicability test {test.GetType().Name}"),
    };

    private static W.TextureSampleTest MapTextureTest(A.TextureTest t, TRGameVersion version)
    {
        var info = t.Texture.Clone();
        if (version == TRGameVersion.TR3)
        {
            A.TextureTest.DecodeTR3ObjectTextureUVs(info);
        }
        return new()
        {
            TextureIndex = t.TextureIndex,
            BlendingMode = (ushort)info.BlendingMode,
            Atlas = info.Atlas,
            Vertices = [.. info.Vertices.Select(v => new FormatUV
            {
                UCoord = (byte)(v.U & 0xFF),
                UPixel = (byte)(v.U >> 8),
                VCoord = (byte)(v.V & 0xFF),
                VPixel = (byte)(v.V >> 8),
            })],
        };
    }

    // --- Chunks ---

    private delegate int BlockBuilder(InjectionData data, BinaryWriter writer);

    private static void WriteChunks(InjectionData data, BinaryWriter writer)
    {
        var chunks = new List<byte[]>();
        foreach (var (chunkType, _, version) in ChunkBuilders())
        {
            var chunk = BuildChunk(data, chunkType, version);
            if (chunk != null)
            {
                chunks.Add(chunk);
            }
        }

        writer.Write((uint)chunks.Count);
        chunks.ForEach(writer.Write);
    }

    private static IEnumerable<(int Type, string Name, int Version)> ChunkBuilders()
        => W.Container.Chunks.Select(c => (c.Type, c.Name, c.Version));

    private static byte[] BuildChunk(InjectionData data, int chunkType, int chunkVersion)
    {
        using var ms = new MemoryStream();
        using var body = new BinaryWriter(ms);
        int blockCount = chunkType switch
        {
            W.Container.Symbols => WriteSymbols(data, body),
            W.Container.TextureData => WriteTextureData(data, body),
            W.Container.TextureInfo => WriteTextureInfo(data, body),
            W.Container.MeshData => WriteMeshData(data, body),
            W.Container.AnimationData => WriteAnimationData(data, body),
            W.Container.ObjectData => WriteObjectData(data, body),
            W.Container.SfxData => WriteSfxData(data, body),
            W.Container.CameraData => WriteCameraData(data, body),
            W.Container.DataEdits => WriteEdits(data, body),
            _ => throw new InvalidDataException($"unhandled chunk {chunkType}"),
        };

        if (blockCount == 0)
        {
            return null;
        }

        byte[] bodyBytes = ms.ToArray();
        using var outStream = new MemoryStream();
        using var final = new BinaryWriter(outStream);
        final.Write((uint)chunkType);
        final.Write((uint)chunkVersion);
        final.Write((uint)blockCount);
        final.Write((uint)bodyBytes.Length);
        final.Write(bodyBytes);
        return outStream.ToArray();
    }

    private static int WriteBlock<T>(BinaryWriter writer, int blockType, IReadOnlyList<T> dtos)
    {
        if (dtos.Count == 0)
        {
            return 0;
        }
        using var ms = new MemoryStream();
        using var sub = new BinaryWriter(ms);
        foreach (var dto in dtos)
        {
            FormatWriter.Write(sub, dto);
        }
        return WriteRawBlock(writer, blockType, dtos.Count, ms.ToArray());
    }

    private static int WriteRawBlock(BinaryWriter writer, int blockType, int elementCount, byte[] bytes)
    {
        if (elementCount == 0)
        {
            return 0;
        }
        writer.Write((uint)blockType);
        writer.Write((uint)elementCount);
        writer.Write((uint)bytes.Length);
        writer.Write(bytes);
        return 1;
    }

    // --- Symbols ---

    private static int WriteSymbols(InjectionData data, BinaryWriter writer)
        => WriteBlock(writer, 40, data.Symbols.Select(s => new W.Symbol
        {
            Context = (int)s.Context,
            Name = s.Name,
            Flags = 0,
        }).ToList());

    // --- Texture data ---

    private static int WriteTextureData(InjectionData data, BinaryWriter writer)
    {
        int blockCount = WriteBlock(writer, 0, data.Palette.Select(p => new W.PaletteEntry
        {
            Colour = new() { Red = p.Red, Green = p.Green, Blue = p.Blue },
        }).ToList());

        if (data.Images.Count > 0)
        {
            List<TRLevelControl.Model.TRTexImage8> img8s = [];
            if (data.Images8 == null)
            {
                List<TRColour> trPalette = [.. data.Palette.Select(c => new TRColour { Red = c.Red, Green = c.Green, Blue = c.Blue })];
                img8s.AddRange(data.Images.Select(i => new TRLevelControl.Model.TRTexImage8
                {
                    Pixels = new TRImageControl.TRImage(i.Pixels).ToRGB(trPalette),
                }));
            }
            else
            {
                if (data.Images8.Count != data.Images.Count)
                {
                    throw new InvalidDataException("Images8 count differs from Images count");
                }
                img8s.AddRange(data.Images8);
            }

            using var ms = new MemoryStream();
            using var sub = new BinaryWriter(ms);
            foreach (var img in data.Images)
            {
                foreach (var px in img.Pixels)
                {
                    sub.Write(px);
                }
            }
            foreach (var img in img8s)
            {
                sub.Write(img.Pixels);
            }
            blockCount += WriteRawBlock(writer, 1, data.Images.Count, ms.ToArray());
        }

        return blockCount;
    }

    // --- Texture info ---

    private static int WriteTextureInfo(InjectionData data, BinaryWriter writer)
    {
        int blockCount = WriteBlock(writer, 2,
            data.ObjectTextures.Select(t => MapObjectTexture(t, data.GameVersion)).ToList());

        blockCount += WriteBlock(writer, 3, data.SpriteTextures.Select(t => new W.SpriteTexture
        {
            Atlas = t.Atlas,
            X = t.X,
            Y = t.Y,
            Width = t.Width,
            Height = t.Height,
            Left = t.LeftSide,
            Top = t.TopSide,
            Right = t.RightSide,
            Bottom = t.BottomSide,
        }).ToList());

        List<W.SpriteSequence> sequences = [.. data.SpriteSequences.Select(s =>
            new W.SpriteSequence
            {
                SpriteID = data.IsSymbol(Model.SymbolContext.Objects, s.SpriteID)
                    ? new() { Kind = (int)TRObjectType.Symbol, Id = s.SpriteID }
                    : Ref(s.SpriteID, TRModelExtensions.GetSpriteType(s.SpriteID, data.GameVersion), data.GameVersion),
                Length = s.NegativeLength,
                StartIndex = 0,
            })];
        blockCount += WriteBlock(writer, 4, sequences);

        return blockCount;
    }

    private static W.ObjectTexture MapObjectTexture(Model.TRFlatObjectTexture t, TRGameVersion version)
    {
        // Build canonical u16 coordinate pairs; TRLevelReader reads the first
        // on-disk byte into Whole, so the u16 value is Whole | Fraction << 8.
        var verts = new FormatUV[4];
        var used = Math.Min(4, t.Vertices.Length);
        var u = new ushort[used];
        var v = new ushort[used];
        for (int i = 0; i < used; i++)
        {
            u[i] = (ushort)(t.Vertices[i].XCoordinate.Whole | (t.Vertices[i].XCoordinate.Fraction << 8));
            v[i] = (ushort)(t.Vertices[i].YCoordinate.Whole | (t.Vertices[i].YCoordinate.Fraction << 8));
        }
        if (version == TRGameVersion.TR3)
        {
            DecodeTR3UVs(u, v);
        }
        for (int i = 0; i < 4; i++)
        {
            if (i < used)
            {
                verts[i] = new()
                {
                    UCoord = (byte)(u[i] & 0xFF),
                    UPixel = (byte)(u[i] >> 8),
                    VCoord = (byte)(v[i] & 0xFF),
                    VPixel = (byte)(v[i] >> 8),
                };
            }
        }

        ComputeTextureBounds(t, u, v, used, out var originalU, out var originalV, out var w1, out var h1);
        return new()
        {
            Attribute = t.Attribute,
            TileAndFlag = t.TileAndFlag,
            NewFlags = t.NewFlags,
            Vertices = verts,
            OriginalU = originalU,
            OriginalV = originalV,
            WidthMinusOne = w1,
            HeightMinusOne = h1,
        };
    }

    private static void ComputeTextureBounds(Model.TRFlatObjectTexture t, ushort[] u, ushort[] v, int used,
        out uint originalU, out uint originalV, out uint widthMinusOne, out uint heightMinusOne)
    {
        if (t.WidthMinusOne != 0 || t.HeightMinusOne != 0 || t.OriginalU != 0 || t.OriginalV != 0)
        {
            originalU = t.OriginalU;
            originalV = t.OriginalV;
            widthMinusOne = t.WidthMinusOne;
            heightMinusOne = t.HeightMinusOne;
            return;
        }

        // Sources without the TR4 metadata: derive it from the UV extents in
        // whole pixels.
        var activeU = u.Take(used).Select(x => x >> 8).ToList();
        var activeV = v.Take(used).Select(x => x >> 8).ToList();
        if (activeU.Count == 0)
        {
            originalU = originalV = widthMinusOne = heightMinusOne = 0;
            return;
        }
        originalU = (uint)(activeU.Min() << 16);
        originalV = (uint)(activeV.Min() << 16);
        widthMinusOne = (uint)Math.Max(0, activeU.Max() - activeU.Min() - 1);
        heightMinusOne = (uint)Math.Max(0, activeV.Max() - activeV.Min() - 1);
    }

    private static void DecodeTR3UVs(ushort[] u, ushort[] v)
    {
        short[] uv = new short[u.Length * 2];
        for (int i = 0; i < u.Length; i++)
        {
            uv[i * 2] = (short)u[i];
            uv[i * 2 + 1] = (short)v[i];
        }

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
        for (int i = 0; i < u.Length; i++)
        {
            u[i] = (ushort)uv[i * 2];
            v[i] = (ushort)uv[i * 2 + 1];
        }
    }

    // --- Mesh data ---

    private static int WriteMeshData(InjectionData data, BinaryWriter writer)
    {
        // Mesh pointers are byte offsets into the mesh blob the engine seeks
        // by. InjectionData carries offsets into the legacy flat blob, so
        // remap them onto the canonical blob while serializing the meshes.
        var legacyOffsets = new Dictionary<uint, int>();
        var canonicalOffsets = new List<uint>();
        uint legacyPos = 0, canonicalPos = 0;
        using var meshStream = new MemoryStream();
        using var meshWriter = new BinaryWriter(meshStream);
        for (int i = 0; i < data.Meshes.Count; i++)
        {
            legacyOffsets[legacyPos] = i;
            canonicalOffsets.Add(canonicalPos);
            legacyPos += (uint)data.Meshes[i].Serialize(data.GameVersion).Length;
            FormatWriter.Write(meshWriter, MapMesh(data.Meshes[i]));
            canonicalPos = (uint)meshStream.Length;
        }

        int blockCount = 0;
        if (data.MeshPointers.Count > 0)
        {
            using var ms = new MemoryStream();
            using var sub = new BinaryWriter(ms);
            foreach (var pointer in data.MeshPointers)
            {
                if (!legacyOffsets.TryGetValue(pointer, out var meshIndex))
                {
                    throw new InvalidDataException(
                        $"mesh pointer {pointer} does not sit on a mesh boundary");
                }
                sub.Write(canonicalOffsets[meshIndex]);
            }
            blockCount += WriteRawBlock(writer, 6, data.MeshPointers.Count, ms.ToArray());
        }

        blockCount += WriteRawBlock(writer, 5, data.Meshes.Count, meshStream.ToArray());
        return blockCount;
    }

    private static W.Mesh MapMesh(Model.TRFlatMesh m)
        => new()
        {
            Centre = Vert(m.Centre),
            CollRadius = m.CollRadius,
            Vertices = [.. m.Vertices.Select(Vert)],
            NormalCount = m.Normals != null ? (short)m.Normals.Count : (short)-m.Lights.Count,
            Normals = m.Normals != null ? [.. m.Normals.Select(Vert)] : [],
            Lights = m.Normals != null ? [] : [.. m.Lights],
            TexturedQuads = [.. m.TexturedRectangles.Select(MapFace4)],
            TexturedTriangles = [.. m.TexturedTriangles.Select(MapFace3)],
            ColouredQuads = [.. (m.ColouredRectangles ?? []).Select(MapFace4)],
            ColouredTriangles = [.. (m.ColouredTriangles ?? []).Select(MapFace3)],
        };

    private static W.Face4 MapFace4(TRMeshFace face)
        => new()
        {
            Vertices = [.. face.Vertices.Select(x => x)],
            Texture = ComposeFaceTexture(face),
            Effects = face.Effects,
        };

    private static W.Face3 MapFace3(TRMeshFace face)
        => new()
        {
            Vertices = [.. face.Vertices.Select(x => x)],
            Texture = ComposeFaceTexture(face),
            Effects = face.Effects,
        };

    private static ushort ComposeFaceTexture(TRMeshFace face)
        => (ushort)(face.Texture | (face.DoubleSided ? 0x8000 : 0));

    // --- Animation data ---

    private static int WriteAnimationData(InjectionData data, BinaryWriter writer)
    {
        int blockCount = WriteBlock(writer, 7, data.AnimChanges.Select(c => new W.AnimChange
        {
            StateID = c.StateID,
            NumAnimDispatches = c.NumAnimDispatches,
            AnimDispatch = c.AnimDispatch,
        }).ToList());

        blockCount += WriteBlock(writer, 8, data.AnimDispatches.Select(d => new W.AnimRange
        {
            Low = d.Low,
            High = d.High,
            NextAnimation = d.NextAnimation,
            NextFrame = d.NextFrame,
        }).ToList());

        blockCount += WriteBlock(writer, 9, data.AnimCommands.Select(c => new W.AnimCommand
        {
            Value = c.Value,
        }).ToList());

        blockCount += WriteBlock(writer, 10, data.MeshTrees.Select(t => new W.AnimBone
        {
            Flags = t.Flags,
            OffsetX = t.OffsetX,
            OffsetY = t.OffsetY,
            OffsetZ = t.OffsetZ,
        }).ToList());

        var canonical = CanonicalFrames.Decode(data);
        blockCount += WriteBlock(writer, 11, canonical.Frames.Select(f => new W.AnimFrame
        {
            MinX = f.MinX,
            MaxX = f.MaxX,
            MinY = f.MinY,
            MaxY = f.MaxY,
            MinZ = f.MinZ,
            MaxZ = f.MaxZ,
            OffsetX = f.OffsetX,
            OffsetY = f.OffsetY,
            OffsetZ = f.OffsetZ,
            Rotations = [.. f.Rotations.Select(r => new W.FrameRotation
            {
                X = r.X,
                Y = r.Y,
                Z = r.Z,
            })],
        }).ToList());

        blockCount += WriteBlock(writer, 12, data.Animations.Select((a, i) => new W.Animation
        {
            FrameOffset = (uint)canonical.AnimFirstOrdinal[i],
            FrameRate = a.FrameRate,
            FrameSize = a.FrameSize,
            StateID = a.StateID,
            Speed = Fixed(a.Speed),
            Accel = Fixed(a.Accel),
            LateralSpeed = Fixed(a.LateralSpeed),
            LateralAccel = Fixed(a.LateralAccel),
            FrameStart = a.FrameStart,
            FrameEnd = a.FrameEnd,
            NextAnimation = a.NextAnimation,
            NextFrame = a.NextFrame,
            NumStateChanges = a.NumStateChanges,
            StateChangeOffset = a.StateChangeOffset,
            NumAnimCommands = a.NumAnimCommands,
            AnimCommand = a.AnimCommand,
        }).ToList());

        return blockCount;
    }

    // A model with an animation resolves its frames through it, so a stale
    // frame offset falls back to the animation's first frame; only a model
    // borrowing frames with no animation needs the offset to land exactly.
    private static uint ModelFrameOrdinal(
        CanonicalFrames.Result canonical, InjectionData data,
        TRLevelReader.Model.TRModel model)
    {
        if (canonical.OrdinalByOffset.TryGetValue(model.FrameOffset, out var ordinal))
        {
            return (uint)ordinal;
        }
        if (model.Animation != ushort.MaxValue && model.Animation < data.Animations.Count)
        {
            return FrameOrdinal(canonical, data.Animations[model.Animation].FrameOffset);
        }
        if (canonical.Frames.Count == 0)
        {
            // A file with no frames of its own: the record borrows frames at
            // the join of the level's arena, which ordinal zero denotes.
            return 0;
        }
        throw new InvalidDataException(
            $"model {model.ID}: frame offset {model.FrameOffset} sits on no frame boundary");
    }

    private static uint FrameOrdinal(CanonicalFrames.Result canonical, uint byteOffset)
    {
        if (byteOffset == uint.MaxValue)
        {
            return uint.MaxValue;
        }
        if (!canonical.OrdinalByOffset.TryGetValue(byteOffset, out var ordinal))
        {
            throw new InvalidDataException(
                $"frame offset {byteOffset} sits on no frame boundary");
        }
        return (uint)ordinal;
    }

    private static FormatFixed32 Fixed(TRLevelReader.Model.FixedFloat32 f)
        => f == null ? new() : new() { Whole = f.Whole, Fraction = f.Fraction };

    // --- Object data ---

    private static int WriteObjectData(InjectionData data, BinaryWriter writer)
    {
        var canonical = CanonicalFrames.Decode(data);
        int blockCount = WriteBlock(writer, 13, data.Models.Select(m => new W.Model
        {
            ID = Ref((int)m.ID,
                data.IsSymbol(Model.SymbolContext.Objects, (int)m.ID)
                    ? TRObjectType.Symbol : TRObjectType.Game,
                data.GameVersion),
            NumMeshes = m.NumMeshes,
            StartingMesh = m.StartingMesh,
            MeshTree = m.MeshTree,
            FrameOffset = data.IsMeshOnlyModel(m.ID)
                ? uint.MaxValue : ModelFrameOrdinal(canonical, data, m),
            Animation = m.Animation,
        }).ToList());

        blockCount += WriteBlock(writer, 29, data.StaticObjects.Select(s => new W.StaticObject
        {
            ID = s.ID,
            Mesh = s.Mesh,
            VisBox = Box(s.VisibilityBox),
            CollBox = Box(s.CollisionBox),
            Flags = s.Flags,
        }).ToList());

        return blockCount;
    }

    private static short[] Box(TRLevelReader.Model.TRBoundingBox box)
        => [box.MinX, box.MaxX, box.MinY, box.MaxY, box.MinZ, box.MaxZ];

    private static short[] Box(TRBoundingBox box)
        => [box.MinX, box.MaxX, box.MinY, box.MaxY, box.MinZ, box.MaxZ];

    // --- SFX data ---

    private static int WriteSfxData(InjectionData data, BinaryWriter writer)
    {
        var plainSFX = data.SFX.FindAll(f => !data.IsSymbol(Model.SymbolContext.Samples, f.ID));
        var namedSFX = data.SFX.FindAll(f => data.IsSymbol(Model.SymbolContext.Samples, f.ID));
        int blockCount = WriteBlock(writer, 14,
            plainSFX.Select(f => MapSfx(f, data.GameVersion)).ToList());
        blockCount += WriteBlock(writer, 41,
            namedSFX.Select(f => MapSfx(f, data.GameVersion)).ToList());
        return blockCount;
    }

    private static W.Sfx MapSfx(TRSFXData sfx, TRGameVersion version)
    {
        // TR2/TR3-style records reference the game's main.sfx; resolve to
        // inline bytes at write time (as the legacy writer did) so files stay
        // self-contained.
        if (version > TRGameVersion.TR1 && sfx.Data == null)
        {
            sfx.LoadSFX(version);
        }
        SfxSample[] samples = [.. sfx.Data.Select(d => (SfxSample)new SfxSampleInline { WavData = d })];

        return new()
        {
            ID = sfx.ID,
            Volume = sfx.Volume,
            Chance = sfx.Chance,
            Pitch = sfx.Pitch,
            Range = sfx.Range,
            Characteristics = sfx.Characteristics,
            Samples = samples,
        };
    }

    // --- Camera data ---

    private static int WriteCameraData(InjectionData data, BinaryWriter writer)
    {
        int blockCount = WriteBlock(writer, 30, data.CinematicFrames.Select(f => new W.CinematicFrame
        {
            TargetX = f.TargetX,
            TargetY = f.TargetY,
            TargetZ = f.TargetZ,
            PosZ = f.PosZ,
            PosY = f.PosY,
            PosX = f.PosX,
            FOV = f.FOV,
            Roll = f.Roll,
        }).ToList());

        blockCount += WriteBlock(writer, 38, data.FlybyCameras.Select(f => new W.FlybyCamera
        {
            X = f.X,
            Y = f.Y,
            Z = f.Z,
            DX = f.dx,
            DY = f.dy,
            DZ = f.dz,
            Sequence = f.Sequence,
            Index = f.Index,
            FOV = f.FOV,
            Roll = f.Roll,
            Timer = f.Timer,
            Speed = f.Speed,
            Flags = f.Flags,
            RoomID = f.RoomID,
        }).ToList());

        return blockCount;
    }

    // --- Data edits ---

    private static int WriteEdits(InjectionData data, BinaryWriter writer)
    {
        var version = data.GameVersion;
        int blockCount = 0;

        blockCount += WriteBlock(writer, 17, data.FloorEdits.Select(f => new W.FloorDataEdit
        {
            RoomIndex = f.RoomIndex,
            X = f.X,
            Z = f.Z,
            Fixes = [.. f.Fixes.Select(x => MapFdFix(x, version))],
        }).ToList());

        blockCount += WriteBlock(writer, 18, data.ItemPosEdits.Select(i => new W.ItemPosEdit
        {
            Index = i.Index,
            Angle = i.Item.Angle,
            X = i.Item.X,
            Y = i.Item.Y,
            Z = i.Item.Z,
            Room = i.Item.Room,
        }).ToList());

        blockCount += WriteBlock(writer, 33, data.ItemFlagEdits.Select(i => new W.ItemFlagEdit
        {
            Index = i.Index,
            Type = Ref((int)i.Item.TypeID, TRObjectType.Game, version),
            Flags = i.Item.Flags,
        }).ToList());

        blockCount += WriteBlock(writer, 37, data.ItemNameEdits.Select(i => new W.ItemNameEdit
        {
            Index = i.Index,
            Name = i.Name,
        }).ToList());

        blockCount += WriteBlock(writer, 19, data.MeshEdits.Select(m => new W.MeshEdit
        {
            ModelID = Ref((int)m.ModelID, MeshEditType(m, version), version),
            MeshIndex = m.MeshIndex,
            Centre = Vert(m.Centre),
            CollRadius = m.CollRadius,
            FaceEdits = [.. m.FaceEdits.Select(f => MapFaceEdit(f, version))],
            VertexEdits = [.. m.VertexEdits.Select(v => new W.VertexEdit
            {
                Index = v.Index,
                Change = Vert(v.Change),
            })],
            FaceEffects = [.. m.FaceEffects.Select(f => new W.FaceEffectEdit
            {
                FaceType = (uint)f.FaceType,
                FaceIndex = f.FaceIndex,
                Effects = f.Effects,
                Reflective = (byte)(f.Reflective ? 1 : 0),
            })],
        }).ToList());

        blockCount += WriteBlock(writer, 26, data.StaticMeshEdits.Select(s => new W.Object3DEdit
        {
            TypeID = s.TypeID,
            Collidable = (byte)(s.Mesh.NonCollidable ? 0 : 1),
            Visible = (byte)(s.Mesh.Visible ? 1 : 0),
            CollBox = Box(s.Mesh.CollisionBox),
            VisBox = Box(s.Mesh.VisibilityBox),
        }).ToList());

        blockCount += WriteBlock(writer, 20, data.TextureOverwrites.Select(t => new W.TextureOverwrite
        {
            Page = t.Page,
            X = t.X,
            Y = t.Y,
            Width = t.Width,
            Height = t.Height,
            Pixels = t.Data,
        }).ToList());

        blockCount += WriteBlock(writer, 35, data.AnimTextureEdits.Select(a => new W.AnimTextureEdit
        {
            Index = a.Index,
            Textures = [.. a.Textures],
        }).ToList());

        blockCount += WriteBlock(writer, 42, data.AnimTextureAdds.Select(a => new W.AnimTextureAdd
        {
            Textures = [.. a.Textures],
        }).ToList());

        var meta = RoomMeta.Create(data);
        blockCount += WriteBlock(writer, 21, meta.Select(m => new W.RoomEditMeta
        {
            RoomIndex = m.RoomIndex,
            NumVertices = m.NumVertices,
            NumQuads = m.NumQuads,
            NumTriangles = m.NumTriangles,
            NumSprites = m.NumSprites,
            NumStatic3Ds = m.NumStatic3Ds,
            NumSectors = m.NumSectors,
        }).ToList());

        blockCount += WriteBlock(writer, 22, data.RoomEdits.Select(MapRoomEdit).ToList());

        blockCount += WriteBlock(writer, 23, data.VisPortalEdits.Select(v => new W.VisPortalEdit
        {
            BaseRoom = v.BaseRoom,
            LinkRoom = v.LinkRoom,
            PortalIndex = v.PortalIndex,
            VertexChanges = [.. v.VertexChanges.Select(Vert)],
        }).ToList());

        blockCount += WriteBlock(writer, 24, data.CameraEdits.Select(c => new W.CameraEdit
        {
            Index = c.Index,
            X = c.Camera.X,
            Y = c.Camera.Y,
            Z = c.Camera.Z,
            Room = c.Camera.Room,
            Flag = c.Camera.Flag,
        }).ToList());

        blockCount += WriteBlock(writer, 25, data.FrameEdits.Select(f => new W.FrameEdit
        {
            ModelID = Ref((int)f.ModelID, TRObjectType.Game, version),
            AnimIndex = f.AnimIndex,
            PackedYZ = (short)(((f.Rotation.Y & 0x3F) << 10) | (f.Rotation.Z & 0x3FF)),
            PackedXY = (short)((f.Rotation.X << 4) | ((f.Rotation.Y & 0xFC0) >> 6)),
        }).ToList());

        blockCount += WriteBlock(writer, 32, data.FrameReplacements.Select(f => new W.FrameReplacement
        {
            ModelID = Ref((int)f.ModelID, TRObjectType.Game, version),
            Anims = [.. f.Frames.Select(kv => new W.FrameReplacementAnim
            {
                AnimID = kv.Key,
                Frames = [.. kv.Value],
            })],
        }).ToList());

        blockCount += WriteBlock(writer, 27, data.AnimCmdEdits.Select(a => new W.AnimCmdEdit
        {
            TypeID = Ref(a.TypeID, TRObjectType.Game, version),
            AnimIndex = a.AnimIndex,
            RawCount = a.RawCount,
            TotalCount = a.TotalCount,
        }).ToList());

        blockCount += WriteBlock(writer, 28, data.SpriteEdits.Select(s => new W.SpriteEdit
        {
            ID = Ref(s.ID, TRModelExtensions.GetSpriteType(s.ID, version), version),
            Left = s.Alignment.Left,
            Top = s.Alignment.Top,
            Right = s.Alignment.Right,
            Bottom = s.Alignment.Bottom,
        }).ToList());

        blockCount += WriteBlock(writer, 31, data.ObjectTypeEdits.Select(o => new W.ObjTypeEdit
        {
            BaseType = Ref(o.BaseType, TRObjectType.Game, version),
            TargetType = Ref(o.TargetType, TRObjectType.Game, version),
        }).ToList());

        blockCount += WriteBlock(writer, 34, data.AnimEdits.Select(a => new W.AnimEdit
        {
            ModelID = Ref((int)a.ModelID, TRObjectType.Game, version),
            AnimIndex = a.AnimIdx,
            Speed = new() { Whole = a.Anim.Speed.Whole, Fraction = a.Anim.Speed.Fraction },
        }).ToList());

        blockCount += WriteBlock(writer, 36, data.ObjectLinkEdits.Select(o => new W.ObjLinkEdit
        {
            BaseType = Ref(o.BaseType, TRObjectType.Game, version),
            SourceType = Ref(o.SourceType, TRObjectType.Game, version),
        }).ToList());

        blockCount += WriteBlock(writer, 39, data.PropertyEdits.Select(p => MapPropertyEdit(p, version)).ToList());

        return blockCount;
    }

    private static TRObjectType MeshEditType(TRMeshEdit edit, TRGameVersion version)
        => edit.EnforcedType
            ?? (edit.ModelID >= version.GetSceneryBase() ? TRObjectType.Static3D : TRObjectType.Game);

    private static W.FaceEdit MapFaceEdit(TRFaceTextureEdit f, TRGameVersion version)
        => new()
        {
            ModelID = Ref((int)f.ModelID, TRObjectType.Game, version),
            MeshIndex = f.MeshIndex,
            FaceType = (uint)f.FaceType,
            FaceIndex = f.FaceIndex,
            TargetFaceIndices = [.. f.TargetFaceIndices],
        };

    private static W.RoomEdit MapRoomEdit(TRRoomTextureEdit edit)
    {
        W.RoomEdit dto = edit switch
        {
            TRRoomTextureReface e => new W.RoomReface
            {
                TargetIndex = e.TargetIndex,
                SourceRoom = e.SourceRoom,
                SourceFaceType = (uint)e.SourceFaceType,
                SourceIndex = e.SourceIndex,
            },
            TRRoomTextureMove e => new W.RoomMoveFace
            {
                TargetIndex = e.TargetIndex,
                Remaps = [.. e.VertexRemap.Select(r => new W.VertexRemap
                {
                    Index = r.Index,
                    NewVertexIndex = r.NewVertexIndex,
                })],
            },
            TRRoomVertexMove e => new W.RoomMoveVertex
            {
                VertexIndex = e.VertexIndex,
                DX = e.VertexChange.X,
                DY = e.VertexChange.Y,
                DZ = e.VertexChange.Z,
                ShadeChange = e.ShadeChange,
            },
            TRRoomTextureRotate e => new W.RoomRotateFace
            {
                TargetIndex = e.TargetIndex,
                Rotations = e.Rotations,
            },
            TRRoomTextureCreate e => new W.RoomAddFace
            {
                SourceRoom = e.SourceRoom,
                SourceIndex = e.SourceIndex,
                Vertices = [.. e.Vertices],
            },
            TRRoomVertexCreate e => new W.RoomAddVertex
            {
                X = e.Vertex.Vertex.X,
                Y = e.Vertex.Vertex.Y,
                Z = e.Vertex.Vertex.Z,
                Lighting = e.Vertex.Lighting,
            },
            TRRoomSpriteCreate e => new W.RoomAddSprite
            {
                ID = e.ID,
                Vertex = e.Vertex,
                Frame = e.Frame,
            },
            TRRoomStatic3DCreate e => new W.RoomAddStatic3D
            {
                X = e.StaticMesh.X,
                Y = e.StaticMesh.Y,
                Z = e.StaticMesh.Z,
                Angle = e.StaticMesh.Angle,
                Intensity = e.StaticMesh.Intensity,
                ID = (ushort)e.ID,
            },
            TRRoomStatic3DEdit e => new W.RoomEditStatic3D
            {
                MeshIndex = e.MeshIndex,
                X = e.StaticMesh.X,
                Y = e.StaticMesh.Y,
                Z = e.StaticMesh.Z,
                Angle = e.StaticMesh.Angle,
                Intensity = e.StaticMesh.Intensity,
            },
            TRRoomVertxFlagChange e => new W.RoomSetVertexFlags
            {
                VertexIndex = e.VertexIndex,
                Flags = e.Flags,
            },
            TRRoomTextureDoubleSided e => new W.RoomSetDoubleSided
            {
                TargetIndex = e.TargetIndex,
                DoubleSided = (byte)(e.DoubleSided ? 1 : 0),
            },
            _ => throw new InvalidDataException($"unmapped room edit {edit.GetType().Name}"),
        };

        dto.RoomIndex = edit.RoomIndex;
        dto.FaceType = (uint)edit.FaceType;
        return dto;
    }

    private static W.FdFix MapFdFix(FDFix fix, TRGameVersion version) => fix switch
    {
        FDTrigParamFix f => new W.FdTrigParam
        {
            ActionType = (byte)f.ActionType,
            OldParam = f.OldParam,
            NewParam = f.NewParam,
        },
        FDMusicOneShot => new W.FdMusicOneShot(),
        FDTrigCreateFix f => new W.FdInsert { Data = [.. f.Flatten(version)] },
        FDRoomShift f => new W.FdRoomShift
        {
            XShift = f.XShift,
            ZShift = f.ZShift,
            YShift = f.YShift,
        },
        FDTrigItem f => new W.FdTrigItem
        {
            Type = Ref((int)f.Item.TypeID, TRObjectType.Game, version),
            Room = f.Item.Room,
            X = f.Item.X,
            Y = f.Item.Y,
            Z = f.Item.Z,
            Angle = f.Item.Angle,
            Intensity = f.Item.Intensity,
            Flags = f.Item.Flags,
            Name = f.EffectiveName,
        },
        FDRoomProperties f => new W.FdRoomProperties
        {
            Flags = f.CleanedFlags,
            Reverb = (byte)f.Reverb,
            FlipGroup = f.FlipGroup ?? 255,
        },
        FDTrigTypeFix f => new W.FdTrigType { NewType = (byte)f.NewType },
        FDSectorOverwrite f => new W.FdSectorOverwrite
        {
            FDIndex = f.Sector.FDIndex,
            BoxIndex = f.Sector.BoxIndex,
            RoomBelow = f.Sector.RoomBelow,
            Floor = f.Sector.Floor,
            RoomAbove = f.Sector.RoomAbove,
            Ceiling = f.Sector.Ceiling,
        },
        FDGlideCameraFix f => new W.FdGlideCamera
        {
            Timer = f.Timer,
            Glide = f.Glide,
            Shift = Vert(f.Shift),
        },
        FDZoneFix f => new W.FdZoneFix
        {
            FlipOffGround = [.. f.ZoneOverwrite.FlipOffZone.Ground.Values],
            FlipOffFly = f.ZoneOverwrite.FlipOffZone.Fly,
            FlipOnGround = [.. f.ZoneOverwrite.FlipOnZone.Ground.Values],
            FlipOnFly = f.ZoneOverwrite.FlipOnZone.Fly,
        },
        FDPortalOverwrite f => new W.FdPortalOverwrite
        {
            Wall = f.Wall,
            Sky = f.Sky,
            Pit = f.Pit,
        },
        FDClimbInsert f => new W.FdClimbInsert
        {
            Direction = (f.PosZ ? 1 : 0) | (f.PosX ? 2 : 0) | (f.NegZ ? 4 : 0) | (f.NegX ? 8 : 0),
        },
        FDTrigDelete => new W.FdTrigDelete(),
        FDTriangulation f => new W.FdTriangulation
        {
            Type = ((f.Floor?.Count ?? 0) > 0 ? 1 : 0) | ((f.Ceiling?.Count ?? 0) > 0 ? 2 : 0),
            Data = [.. f.Floor ?? [], .. f.Ceiling ?? []],
        },
        FDMineCartEdit f => new W.FdMineCart { Type = (int)f.Type },
        FDMaterialEdit f => new W.FdMaterial { Material = (byte)f.Material },
        FDRoomExtension f => new W.FdSectorExtension
        {
            AdditionalXSectors = f.AdditionalXSectors,
            AdditionalZSectors = f.AdditionalZSectors,
        },
        FDNamedTrigParamFix f => new W.FdNamedTrigParam
        {
            ActionType = (byte)f.ActionType,
            OldParam = f.OldParam,
            Symbol = new() { Index = f.Symbol },
        },
        _ => throw new InvalidDataException($"unmapped floor data fix {fix.GetType().Name}"),
    };

    private static W.PropertyEdit MapPropertyEdit(TRPropertyEdit edit, TRGameVersion version)
    {
        var properties = edit.Properties.Select(MapProperty).ToArray();
        return edit switch
        {
            TRObjectPropertyEdit e => new W.ObjectPropertyEdit
            {
                ObjectId = Ref(e.ObjectId, e.ObjectType, version),
                Properties = properties,
            },
            TRItemPropertyEdit e => new W.ItemPropertyEdit
            {
                ItemIndex = e.ItemIndex,
                Properties = properties,
            },
            _ => throw new InvalidDataException($"unmapped property edit {edit.GetType().Name}"),
        };
    }

    private static W.Property MapProperty(TRProperty property) => new()
    {
        Name = property.Name,
        Value = property switch
        {
            TRIntProperty p => new W.PropInt { Value = p.Value },
            TRFloatProperty p => new W.PropFloat { Value = p.Value },
            TRDoubleProperty p => new W.PropDouble { Value = p.Value },
            TRBoolProperty p => new W.PropBool { Value = p.Value ? 1 : 0 },
            TRXYZProperty p => new W.PropXYZ { X = p.X, Y = p.Y, Z = p.Z },
            TRRGBProperty p => new W.PropRGB
            {
                Value = new() { Red = p.Color.R, Green = p.Color.G, Blue = p.Color.B },
            },
            _ => throw new InvalidDataException($"unmapped property {property.GetType().Name}"),
        },
    };
}
