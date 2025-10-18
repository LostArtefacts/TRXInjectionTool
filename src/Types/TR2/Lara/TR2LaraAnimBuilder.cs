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
    private static readonly Dictionary<TR3LaraAnim, InjAnim> _sprintAnimMap = new()
    {
        [TR3LaraAnim.Sprint] = InjAnim.Sprint,
        [TR3LaraAnim.RunToSprintLeft] = InjAnim.RunToSprintLeft,
        [TR3LaraAnim.RunToSprintRight] = InjAnim.RunToSprintRight,
        [TR3LaraAnim.SprintSlideStandLeft] = InjAnim.SprintSlideStandLeft,
        [TR3LaraAnim.SprintSlideStandRight] = InjAnim.SprintSlideStandRight,
        [TR3LaraAnim.SprintToRollLeft] = InjAnim.SprintToRollLeft,
        [TR3LaraAnim.SprintRollLeftToRun] = InjAnim.SprintRollLeftToRun,
        [TR3LaraAnim.SprintToRollRight] = InjAnim.SprintToRollRight,
        [TR3LaraAnim.SprintRollRightToRun] = InjAnim.SprintRollRightToRun,
        [TR3LaraAnim.SprintToRunLeft] = InjAnim.SprintToRunLeft,
        [TR3LaraAnim.SprintToRunRight] = InjAnim.SprintToRunRight,
    };

    private static readonly Dictionary<TR3LaraState, InjState> _sprintStateMap = new()
    {
        [TR3LaraState.Sprint] = InjState.Sprint,
        [TR3LaraState.SprintRoll] = InjState.SprintRoll,
    };

    public override string ID => "tr2_lara_anims";
    public override TRGameVersion GameVersion => TRGameVersion.TR2;
    protected override short JumpSFX => (short)TR2SFX.LaraJump;
    protected override short DryFeetSFX => (short)TR2SFX.LaraFeet;
    protected override short WetFeetSFX => (short)TR2SFX.LaraWetFeet;
    protected override short LandSFX => (short)TR2SFX.LaraLand;
    protected override short ResponsiveState => (short)InjState.Responsive;

    enum InjAnim : int
    {
        SlideToRun = 218,
        JumpNeutralRoll = 219,
        ControlledDrop = 220,
        ControlledDropContinue = 221,
        HangToJumpUp = 222,
        HangToJumpUpContinue = 223,
        HangToJumpBack = 224,
        HangToJumpBackContinue = 225,
        Sprint = 226,
        RunToSprintLeft = 227,
        RunToSprintRight = 228,
        SprintSlideStandLeft = 229,
        SprintSlideStandRight = 230,
        SprintToRollLeft = 231,
        SprintRollLeftToRun = 232,
        SprintToRollRight = 233,
        SprintRollRightToRun = 234,
        SprintToRunLeft = 235,
        SprintToRunRight = 236,
        PoseRightStart = 237,
        PoseRightContinue = 238,
        PoseRightEnd = 239,
        PoseLeftStart = 240,
        PoseLeftContinue = 241,
        PoseLeftEnd = 242,
    };

    enum InjState : int
    {
        Responsive = 71,
        NeutralRoll = 72,
        Sprint = 73,
        SprintRoll = 74,
        PoseStart = 75,
        PoseEnd = 76,
        PoseLeft = 77,
        PoseRight = 78,
        Controlled = 79,
    };

    public override List<InjectionData> Build()
    {
        var level = CreateLevel();
        var data = InjectionData.Create(level, InjectionType.LaraAnims, "lara_animations");

        return new() { data };
    }

    private TR2Level CreateLevel()
    {
        TR2Level wall = _control2.Read($"Resources/{TR2LevelNames.GW}");
        TRModel tr2Lara = wall.Models[TR2Type.Lara];

        ResetLevel(wall);
        wall.Models[TR2Type.Lara] = tr2Lara;
        
        ImportSlideToRun(tr2Lara);
        ImproveTwists(tr2Lara);
        ImportNeutralTwist(tr2Lara, (short)InjAnim.JumpNeutralRoll, (short)InjState.NeutralRoll);
        ImportControlledDrop(tr2Lara, (short)InjAnim.ControlledDropContinue);
        ImportHangToJump(tr2Lara, (short)InjAnim.HangToJumpUp);
        ImportSprint(tr2Lara, InjAnim.SlideToRun, _sprintAnimMap, _sprintStateMap);
        ImportIdlePose(tr2Lara, InjState.PoseStart, InjState.PoseEnd, InjState.PoseLeft, InjState.PoseRight);
        FixJumpToFreefall(tr2Lara);
        FixLadderClimbOnSFX(tr2Lara);
        FixHandstandSFX(tr2Lara);

        return wall;
    }

    public override byte[] Publish()
    {
        var level = CreateLevel();
        return ExportLaraWAD(level);
    }

    private static void ImproveTwists(TRModel lara)
    {
        var laraExt = GetLaraExtModel();
        lara.Animations[203] = laraExt.Animations[(int)ExtLaraAnim.UWRollStart];
        lara.Animations[205] = laraExt.Animations[(int)ExtLaraAnim.UWRollEnd];
        lara.Animations[203].NextAnimation = 205;
        lara.Animations[203].NextFrame = 1;
        lara.Animations[205].NextAnimation = 108;

        lara.Animations[207] = laraExt.Animations[(int)ExtLaraAnim.RunJumpRollStart];
        lara.Animations[209] = laraExt.Animations[(int)ExtLaraAnim.RunJumpRollEnd];
        lara.Animations[210] = laraExt.Animations[(int)ExtLaraAnim.JumpFwdRollStart];
        lara.Animations[211] = laraExt.Animations[(int)ExtLaraAnim.JumpFwdRollEnd];
        lara.Animations[212] = laraExt.Animations[(int)ExtLaraAnim.JumpBackRollStart];
        lara.Animations[213] = laraExt.Animations[(int)ExtLaraAnim.JumpBackRollEnd];

        lara.Animations[207].NextAnimation = 209;
        lara.Animations[209].NextAnimation = (ushort)LaraAnim.JumpBack;
        lara.Animations[209].NextFrame = 39;
        lara.Animations[210].NextAnimation = 211;
        lara.Animations[211].NextAnimation = (ushort)LaraAnim.JumpBack;
        lara.Animations[211].NextFrame = 39;
        lara.Animations[212].NextAnimation = 213;
        lara.Animations[213].NextAnimation = (ushort)LaraAnim.JumpForward;
        lara.Animations[213].NextFrame = 39;
    }

    private static byte[] ExportLaraWAD(TR2Level level)
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

        return ExportZip(level);
    }

    private static byte[] ExportZip(TR2Level level)
    {
        var stream = new MemoryStream();
        using var zip = new ZipArchive(stream, ZipArchiveMode.Create);

        {
            using var ms = new MemoryStream();
            _control2.Write(level, ms);
            byte[] phdRaw = ms.ToArray();
            var entry = zip.CreateEntry("lara.tr2", CompressionLevel.Optimal);
            using var zipStream = entry.Open();
            zipStream.Write(phdRaw, 0, phdRaw.Length);
        }

        zip.Dispose();
        stream.Flush();
        return stream.ToArray();
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
