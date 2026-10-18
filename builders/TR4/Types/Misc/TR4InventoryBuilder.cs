using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;
using TRXInjectionTool.Types.TR3.Misc;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types.TR4.Misc;

public class TR4InventoryBuilder : InjectionBuilder, IPublisher
{
    private static readonly Dictionary<TR3Type, TR4Type> _typeMap = new()
    {
        [TR3Type.PassportOpening_H] = (TR4Type)536,
        [TR3Type.PassportClosed_H] = (TR4Type)537,
        [TR3Type.Sunglasses_M_H] = (TR4Type)538,
        [TR3Type.CDPlayer_M_H] = (TR4Type)539,
        [TR3Type.DirectionKeys_M_H] = (TR4Type)540,
        [TR3Type.Map_H] = (TR4Type)541,
    };

    // The compass case carries its open lid as the first vertices of its mesh.
    // The lid hinges about the X axis at the origin of the case and lies on the
    // rim of the case when turned by the closed angle.
    private const int _compassLidVertexCount = 25;
    private const float _compassClosedAngle = -101.5f;

    // The original model leans the case to suit the fixed camera of the
    // original inventory. This turn lays the case level instead: the rim of
    // the case leans 11.5 degrees from the Y axis of its mesh.
    private const float _compassCasePitch = 101.5f;
    private const float _compassCaseYaw = 180;

    public override List<InjectionData> Build()
    {
        var level = CreateLevel();
        var data = InjectionData.Create(level, InjectionType.General, "inventory_models");

        var jungle = _control3.Read($"Resources/TR3/{TR3LevelNames.JUNGLE}");
        data.SFX.Add(TRSFXData.Create(TR4SFX.Unused05, jungle.SoundEffects[TR3SFX.MenuPassport]));
        return [data];
    }

    private static TR4Level CreateLevel()
    {
        var baseLevel = CreateBaseLevel();
        var level = _control4.Read($"Resources/TR4/{TR4LevelNames.SETH}");
        ResetLevel(level);

        level.ObjectTextures.AddRange(baseLevel.ObjectTextures);
        foreach (var (tr3Type, tr4Type) in _typeMap)
        {
            level.Models[tr4Type] = baseLevel.Models[tr3Type];
        }

        var images = baseLevel.Images16.Select(i => new TRImage(i.Pixels));
        level.Images.Objects.Images16.AddRange(images.Select(i => new TRTexImage16 { Pixels = i.ToRGB555() }));
        level.Images.Objects.Images32.AddRange(images.Select(i => new TRTexImage32 { Pixels = i.ToRGB32() }));

        ImportCompass(level);
        return level;
    }

    private static void ImportCompass(TR4Level level)
    {
        var seth = _control4.Read($"Resources/TR4/{TR4LevelNames.SETH}");
        var compass = seth.Models[TR4Type.CompassItem];

        var regions = new TR4TexturePacker(seth, TRGroupPackingMode.Object)
            .GetMeshRegions(compass.Meshes).Values.SelectMany(v => v).ToList();
        var packer = new TR4TexturePacker(level, TRGroupPackingMode.Object);
        packer.AddRectangles(regions);
        packer.Pack(true);

        level.ObjectTextures.AddRange(regions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));
        compass.Meshes
            .SelectMany(m => m.TexturedFaces)
            .Distinct()
            .ToList()
            .ForEach(f => f.Texture = (ushort)level.ObjectTextures.IndexOf(seth.ObjectTextures[f.Texture]));

        SplitCompassLid(compass);
        PlaceCompassCase(compass);
        AnimateCompassLid(compass);
        level.Models[TR4Type.CompassItem] = compass;
    }

    private static void SplitCompassLid(TRModel compass)
    {
        var body = compass.Meshes[0];
        var lid = body.Clone();
        Keep(lid, 0, _compassLidVertexCount);
        Keep(body, _compassLidVertexCount, body.Vertices.Count - _compassLidVertexCount);
        compass.Meshes.Add(lid);

        compass.MeshTrees[0].Flags = 2;
        compass.MeshTrees.Add(new() { Flags = 1 });
    }

    private static void PlaceCompassCase(TRModel compass)
    {
        // The ring turns and tilts an entry about its origin, so the case lies
        // level and centred on it.
        var pose = compass.Animations[0].Frames[0];
        pose.Rotations[0] = new() { X = _compassCasePitch, Y = _compassCaseYaw };
        pose.OffsetX = pose.OffsetY = pose.OffsetZ = 0;
        var bounds = TRAnimBoundsCalculator.ComputeFrameBounds(new() { Meshes = [compass.Meshes[0]] }, pose);
        pose.OffsetX = (short)-((bounds.MinX + bounds.MaxX) / 2);
        pose.OffsetY = (short)-((bounds.MinY + bounds.MaxY) / 2);
        pose.OffsetZ = (short)-((bounds.MinZ + bounds.MaxZ) / 2);
    }

    private static void Keep(TRMesh mesh, int start, int count)
    {
        bool inside(TRMeshFace f) => f.Vertices.All(v => v >= start && v < start + count);
        mesh.TexturedRectangles.RemoveAll(f => !inside(f));
        mesh.TexturedTriangles.RemoveAll(f => !inside(f));
        mesh.ColouredRectangles.RemoveAll(f => !inside(f));
        mesh.ColouredTriangles.RemoveAll(f => !inside(f));
        mesh.Vertices = mesh.Vertices.GetRange(start, count);
        mesh.Normals = mesh.Normals?.GetRange(start, count);
        mesh.Lights = mesh.Lights?.GetRange(start, count);
        foreach (var face in mesh.TexturedFaces.Concat(mesh.ColouredFaces))
        {
            for (int i = 0; i < face.Vertices.Count; i++)
            {
                face.Vertices[i] -= (ushort)start;
            }
        }
    }

    private static void AnimateCompassLid(TRModel compass)
    {
        // The lid follows the TR1 compass lid, whose widest swing stands for
        // the lid of the TR4 compass lying open.
        var caves = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        var source = caves.Models[TR1Type.Compass_M_H].Animations[0];
        var openAngle = source.Frames.Max(f => f.Rotations[2].X);

        var anim = compass.Animations[0];
        var pose = anim.Frames[0];
        anim.Frames = [.. source.Frames.Select(f =>
        {
            var frame = pose.Clone();
            frame.Rotations.Add(new()
            {
                X = _compassClosedAngle * (1 - f.Rotations[2].X / openAngle),
            });
            frame.Bounds = TRAnimBoundsCalculator.ComputeFrameBounds(compass, frame);
            return frame;
        })];
        anim.FrameRate = source.FrameRate;
        anim.FrameEnd = (short)(anim.Frames.Count - 1);
    }

    private static TR3Level CreateBaseLevel()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.JUNGLE}");
        ApplyFixes(level);
        ImportPDA(level);
        TRFaceConverter.ConvertFlatFaces(level, [.. level.Palette16.Select(c => c.ToColor())]);
        CreateModelLevel(level, [.. _typeMap.Keys]);
        return level;
    }

    private static void ApplyFixes(TR3Level level)
    {
        TR3PickupBuilder.FixCDPlayer(level);

        // Essentially InjectionBuilder.FixPassport, but applied directly
        var map = new Dictionary<TR3Type, short>
        {
            [TR3Type.PassportClosed_H] = -2,
            [TR3Type.PassportOpening_H] = -1,
        };
        foreach (var (type, shift) in map)
        {
            var mesh = level.Models[type].Meshes[0];
            foreach (var vtx in new[] { 1, 2, 5, 6 })
            {
                mesh.Vertices[vtx].X += shift;
            }
        }
    }

    private static void ImportPDA(TR3Level level)
    {
        var pdaLevel = CreatePDALevel();
        level.Models[TR3Type.Map_H] = pdaLevel.Models[TR1Type.Map_M_U];
        var texInfos = pdaLevel.ObjectTextures.ToList();

        level.Models[TR3Type.Map_H].Meshes.SelectMany(m => m.TexturedFaces)
            .ToList().ForEach(f => f.Texture += (ushort)level.ObjectTextures.Count);

        level.ObjectTextures.AddRange(texInfos);
        texInfos.ForEach(o => o.Atlas += (ushort)level.Images16.Count);

        var tile = new TRImage(TRConsts.TPageWidth, TRConsts.TPageHeight);
        tile.Import(new("Resources/TR4/pda.png"), new(0, 0));
        level.Images16.Add(new() { Pixels = tile.ToRGB555() });
        level.Images8.Add(new());
    }

    public TRLevelBase Publish()
        => CreateLevel();

    public string GetPublishedName()
        => "inventory.tr4";
}
