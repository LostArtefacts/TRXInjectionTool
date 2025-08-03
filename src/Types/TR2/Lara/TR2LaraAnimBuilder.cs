using System.IO.Compression;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Lara;

public class TR2LaraAnimBuilder : LaraBuilder
{
    private static readonly string _wadZipPath = "../../Resources/Published/tr2-lara-anim-ext.zip";
    private static readonly DateTimeOffset _wadZipPlaceholderDate
        = new(new DateTime(2025, 7, 23, 19, 0, 0), new TimeSpan());

    public override string ID => "tr2-lara-anims";
    protected override short JumpSFX => (short)TR2SFX.LaraJump;
    protected override short DryFeetSFX => (short)TR2SFX.LaraFeet;
    protected override short WetFeetSFX => (short)TR2SFX.LaraWetFeet;
    protected override short LandSFX => (short)TR2SFX.LaraLand;
    protected override short ResponsiveState => (short)InjState.Responsive;

    enum InjAnim : int
    {
        SlideToRun = 218,
        JumpTwistContinue = 219,
        JumpNeutralRoll = 220,
        ControlledDrop = 221,
        ControlledDropContinue = 222,
        HangToJumpUp = 223,
        HangToJumpUpContinue = 224,
        HangToJumpBack = 225,
        HangToJumpBackContinue = 226,
    };

    enum InjState : int
    {
        Responsive = 71,
        NeutralRoll = 72,
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
        ImproveTwists(tr2Lara);
        ImportNeutralTwist(tr2Lara, (short)InjAnim.JumpNeutralRoll, (short)InjState.NeutralRoll);
        ImportControlledDrop(tr2Lara, (short)InjAnim.ControlledDropContinue);
        ImportHangToJump(tr2Lara, (short)InjAnim.HangToJumpUp);

        var data = InjectionData.Create(wall, InjectionType.LaraAnims, "lara_animations");
        ExportLaraWAD(wall);

        return new() { data };
    }

    private static void ImportSlideToRun(TRModel tr2Lara, TRModel tr3Lara)
    {
        TRAnimation anim = tr3Lara.Animations[246].Clone();
        tr2Lara.Animations.Add(anim);

        anim.Commands.RemoveAll(a => a is not TRSFXCommand);
        (anim.Commands[0] as TRSFXCommand).SoundID = (short)TR2SFX.LaraWetFeet;

        TRStateChange change = tr3Lara.Animations[70].Changes.Find(c => c.StateID == 1).Clone();
        change.StateID = (ushort)InjState.Responsive;
        change.Dispatches[0].NextAnimation = (short)InjAnim.SlideToRun;
        tr2Lara.Animations[70].Changes.Add(change);
    }

    private static void ImproveTwists(TRModel tr2Lara)
    {
        var twistLara = GetLaraExtModel();
        tr2Lara.Animations[203] = twistLara.Animations[0];
        tr2Lara.Animations[205] = twistLara.Animations[1];
        tr2Lara.Animations[203].NextAnimation = 205;
        tr2Lara.Animations[203].NextFrame = 1;
        tr2Lara.Animations[205].NextAnimation = 108;

        tr2Lara.Animations[207] = twistLara.Animations[2];
        tr2Lara.Animations[209] = twistLara.Animations[3];
        tr2Lara.Animations[210] = twistLara.Animations[4];
        tr2Lara.Animations[211] = twistLara.Animations[5];
        tr2Lara.Animations[212] = twistLara.Animations[6];
        tr2Lara.Animations[213] = twistLara.Animations[7];
        tr2Lara.Animations.Add(twistLara.Animations[8]); // JumpTwistContinue

        tr2Lara.Animations[207].NextAnimation = (ushort)InjAnim.JumpTwistContinue;
        tr2Lara.Animations[209].NextAnimation = 75;
        tr2Lara.Animations[209].NextFrame = 39;
        tr2Lara.Animations[210].NextAnimation = 211;
        tr2Lara.Animations[211].NextAnimation = 75;
        tr2Lara.Animations[211].NextFrame = 39;
        tr2Lara.Animations[212].NextAnimation = 213;
        tr2Lara.Animations[213].NextAnimation = 77;
        tr2Lara.Animations[213].NextFrame = 39;
        tr2Lara.Animations[219].NextAnimation = 209;
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
