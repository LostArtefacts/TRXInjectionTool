using System.IO.Compression;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Lara;

public class TR2LaraAnimBuilder : InjectionBuilder
{
    private static readonly string _wadZipPath = "../../Resources/Published/tr2-lara-anim-ext.zip";
    private static readonly DateTimeOffset _wadZipPlaceholderDate
        = new(new DateTime(2025, 7, 23, 19, 0, 0), new TimeSpan());

    public override string ID => "tr2-lara-anims";

    enum InjAnim : int
    {
        SlideToRun = 218,
    };

    enum InjState : int
    {
        Responsive = 71,
    };

    public override List<InjectionData> Build()
    {
        TR2Level wall = _control2.Read($"Resources/{TR2LevelNames.GW}");
        TR3Level jungle = _control3.Read($"Resources/{TR3LevelNames.JUNGLE}");
        TRModel tr2Lara = wall.Models[TR2Type.Lara];
        TRModel tr3Lara = jungle.Models[TR3Type.Lara];

        ResetLevel(wall);
        wall.Models[TR2Type.Lara] = tr2Lara;
        
        ImportSlideToRun(tr2Lara, tr3Lara);

        var data = InjectionData.Create(wall, InjectionType.LaraAnims, "lara_animations");
        ExportLaraWAD(wall);

        return new() { data };
    }

    private static void ImportSlideToRun(TRModel tr1Lara, TRModel tr3Lara)
    {
        TRAnimation anim = tr3Lara.Animations[246].Clone();
        tr1Lara.Animations.Add(anim);

        anim.Commands.RemoveAll(a => a is not TRSFXCommand);
        (anim.Commands[0] as TRSFXCommand).SoundID = (short)TR2SFX.LaraWetFeet;

        TRStateChange change = tr3Lara.Animations[70].Changes.Find(c => c.StateID == 1).Clone();
        change.StateID = (ushort)InjState.Responsive;
        change.Dispatches[0].NextAnimation = (short)InjAnim.SlideToRun;
        tr1Lara.Animations[70].Changes.Add(change);
    }

    private static void ExportLaraWAD(TR2Level level)
    {
        // Generate the injection's effect on a regular level to allow TRLE builders to utilise
        // the new animations while also being able to edit the defaults. This is a stripped back
        // level file that can be opened in WADTool.
        var originalInfos = level.ObjectTextures.ToList();
        var texMap = new Dictionary<ushort, ushort>();
        level.Models[TR2Type.Lara].Meshes
            .SelectMany(m => m.TexturedFaces)
            .Select(f => f.Texture)
            .Distinct()
            .ToList().ForEach(t => texMap[t] = (ushort)texMap.Count);

        var packer = new TR2TexturePacker(level);
        var laraRegions = packer.GetMeshRegions(level.Models[TR2Type.Lara].Meshes)
            .Values.SelectMany(v => v);

        level.Images16 = new() { new() { Pixels = new ushort[TRConsts.TPageSize] } };
        level.ObjectTextures.Clear();

        packer = new(level);
        packer.AddRectangles(laraRegions);
        packer.Pack(true);

        level.ObjectTextures.AddRange(texMap.Keys.Select(t => originalInfos[t]));
        level.Models[TR2Type.Lara].Meshes
            .SelectMany(m => m.TexturedFaces)
            .ToList().ForEach(f => f.Texture = texMap[f.Texture]);

        GenerateImages8(level, level.Palette.Select(c => c.ToTR1Color()).ToList());

        ExportZip(level);
        using var archive = ZipFile.Open(_wadZipPath, ZipArchiveMode.Update);
        foreach (var entry in archive.Entries)
        {
            // Prevent the zip changing despite the contents having not. C# provides no way to do this on create.
            entry.LastWriteTime = _wadZipPlaceholderDate;
        }
    }

    private static void ExportZip(TR2Level level)
    {
        using var stream = new FileStream(_wadZipPath, FileMode.Create);
        using var zip = new ZipArchive(stream, ZipArchiveMode.Create);

        {
            using var ms = new MemoryStream();
            _control2.Write(level, ms);
            byte[] phdRaw = ms.ToArray();
            var entry = zip.CreateEntry("lara_anim_ext.tr2", CompressionLevel.Optimal);
            using var zipStream = entry.Open();
            zipStream.Write(phdRaw, 0, phdRaw.Length);
        }
    }

    private static void ResetLevel(TR2Level level)
    {
        level.Sprites.Clear();
        level.Rooms.Clear();
        level.StaticMeshes.Clear();
        level.Boxes.Clear();
        level.SoundEffects.Clear();
        level.Entities.Clear();
        level.Cameras.Clear();

        level.Models = new()
        {
            [TR2Type.Lara] = level.Models[TR2Type.Lara],
        };
    }
}
