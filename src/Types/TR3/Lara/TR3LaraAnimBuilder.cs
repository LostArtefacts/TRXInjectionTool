using System.Diagnostics;
using System.IO.Compression;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Lara;

public class TR3LaraAnimBuilder : LaraBuilder
{
    public override string ID => "tr3_lara_anims";
    public override TRGameVersion GameVersion => TRGameVersion.TR3;
    protected override short JumpSFX => (short)TR3SFX.LaraJump;
    protected override short DryFeetSFX => (short)TR3SFX.LaraFeet;
    protected override short WetFeetSFX => (short)TR3SFX.LaraWetFeet;
    protected override short TreadSFX => (short)TR3SFX.LaraFloating;
    protected override short LandSFX => (short)TR3SFX.LaraLand;
    protected override short KneesShuffleSFX => (short)TR3SFX.LaraKneesShuffle;
    protected override short ClimbOnSFX => (short)TR3SFX.LaraClimb3;
    protected override short ResponsiveState => (short)InjState.Responsive;

    enum InjAnim : int
    {
        Kick = 313,
        JumpNeutralRoll = 314,
        ControlledDrop = 315,
        ControlledDropContinue = 316,
        HangToJumpUp = 317,
        HangToJumpUpContinue = 318,
        HangToJumpBack = 319,
        HangToJumpBackContinue = 320,
        PoseRightStart = 321,
        PoseRightContinue = 322,
        PoseRightEnd = 323,
        PoseLeftStart = 324,
        PoseLeftContinue = 325,
        PoseLeftEnd = 326,
        CrawlJumpDown = 327,
        CrouchTurnLeft = 328,
        CrouchTurnRight = 329,
        SwingInFast = 330,
        LadderToCrouchStart = 331,
        LadderToCrouchEnd = 332,
        HangCornerLeftOuterStart = 342,
        HangCornerLeftOuterEnd = 343,
        HangCornerRightOuterStart = 344,
        HangCornerRightOuterEnd = 345,
        HangCornerLeftInnerStart = 346,
        HangCornerLeftInnerEnd = 347,
        HangCornerRightInnerStart = 348,
        HangCornerRightInnerEnd = 349,
        LadderCornerLeftOuterStart = 350,
        LadderCornerLeftOuterEnd = 351,
        LadderCornerRightOuterStart = 352,
        LadderCornerRightOuterEnd = 353,
        LadderCornerLeftInnerStart = 354,
        LadderCornerLeftInnerEnd = 355,
        LadderCornerRightInnerStart = 356,
        LadderCornerRightInnerEnd = 357,

    };

    enum InjState : int
    {
        Controlled = 89,
        Responsive = 90,
        NeutralRoll = 91,
        PoseStart = 92,
        PoseEnd = 93,
        PoseLeft = 94,
        PoseRight = 95,
        CrawlJumpDown = 96,
        CrouchTurnLeft = 97,
        CrouchTurnRight = 98,
        ShimmyOuterLeft = 104,
        ShimmyOuterRight = 105,
        ShimmyInnerLeft = 106,
        ShimmyInnerRight = 107,

    };


    private static readonly Dictionary<TR4LaraAnim, InjAnim> _cornerAnimMap = new()
    {
        [TR4LaraAnim.HangCornerLeftOuterStart] = InjAnim.HangCornerLeftOuterStart,
        [TR4LaraAnim.HangCornerLeftOuterEnd] = InjAnim.HangCornerLeftOuterEnd,
        [TR4LaraAnim.HangCornerRightOuterStart] = InjAnim.HangCornerRightOuterStart,
        [TR4LaraAnim.HangCornerRightOuterEnd] = InjAnim.HangCornerRightOuterEnd,
        [TR4LaraAnim.HangCornerLeftInnerStart] = InjAnim.HangCornerLeftInnerStart,
        [TR4LaraAnim.HangCornerLeftInnerEnd] = InjAnim.HangCornerLeftInnerEnd,
        [TR4LaraAnim.HangCornerRightInnerStart] = InjAnim.HangCornerRightInnerStart,
        [TR4LaraAnim.HangCornerRightInnerEnd] = InjAnim.HangCornerRightInnerEnd,
        [TR4LaraAnim.LadderCornerLeftOuterStart] = InjAnim.LadderCornerLeftOuterStart,
        [TR4LaraAnim.LadderCornerLeftOuterEnd] = InjAnim.LadderCornerLeftOuterEnd,
        [TR4LaraAnim.LadderCornerRightOuterStart] = InjAnim.LadderCornerRightOuterStart,
        [TR4LaraAnim.LadderCornerRightOuterEnd] = InjAnim.LadderCornerRightOuterEnd,
        [TR4LaraAnim.LadderCornerLeftInnerStart] = InjAnim.LadderCornerLeftInnerStart,
        [TR4LaraAnim.LadderCornerLeftInnerEnd] = InjAnim.LadderCornerLeftInnerEnd,
        [TR4LaraAnim.LadderCornerRightInnerStart] = InjAnim.LadderCornerRightInnerStart,
        [TR4LaraAnim.LadderCornerRightInnerEnd] = InjAnim.LadderCornerRightInnerEnd,
    };

    private static readonly Dictionary<TR4LaraState, InjState> _cornerStateMap = new()
    {
        [TR4LaraState.ShimmyOuterLeft] = InjState.ShimmyOuterLeft,
        [TR4LaraState.ShimmyOuterRight] = InjState.ShimmyOuterRight,
        [TR4LaraState.ShimmyInnerLeft] = InjState.ShimmyInnerLeft,
        [TR4LaraState.ShimmyInnerRight] = InjState.ShimmyInnerRight,
    };

    public override List<InjectionData> Build()
    {
        var result = new List<InjectionData>();

        {
            var level = CreateLevel();
            result.Add(InjectionData.Create(level, InjectionType.LaraAnims, "lara_animations"));
        }
        {
            var level = CreateExtraLevel();
            result.Add(InjectionData.Create(level, InjectionType.General, "lara_extra"));
            result.Add(CreateAirlockEdit(level, TR3LevelNames.COASTAL, "coastal_airlock"));
            result.Add(CreateAirlockEdit(level, TR3LevelNames.ANTARC, "antarc_airlock"));
        }

        return result;
    }

    public override byte[] Publish()
    {
        var level = CreateLevel(true);
        var extraLevel = CreateExtraLevel();
        return ExportLaraWAD(level, extraLevel);
    }

    private TR3Level CreateLevel(bool useSkin = false)
    {
        var jungle = _control3.Read($"Resources/{TR3LevelNames.JUNGLE}");
        if (useSkin)
        {
            jungle.Models[TR3Type.Lara].Meshes = jungle.Models[TR3Type.LaraSkin_H].Meshes;
        }
        ResetLevel(jungle);
        var tr3Lara = jungle.Models[TR3Type.Lara];
        
        SyncToTR2(tr3Lara);
        ImportTR1Jumping(tr3Lara);
        ImportTR1Gliding(tr3Lara);
        ImproveTwists(tr3Lara);
        ImportNeutralTwist(tr3Lara, (short)InjAnim.JumpNeutralRoll, (short)InjState.NeutralRoll);
        ImportControlledDrop(tr3Lara, (short)InjAnim.ControlledDropContinue);
        ImportHangToJump(tr3Lara, (short)InjAnim.HangToJumpUp);
        ImportIdlePose(tr3Lara, InjState.PoseStart, InjState.PoseEnd, InjState.PoseLeft, InjState.PoseRight);
        FixJumpToFreefall(tr3Lara);
        FixSprintSFX(tr3Lara, TR3LaraAnim.RunToSprintLeft, TR3LaraAnim.RunToSprintRight);
        AddChange(tr3Lara, TR3LaraAnim.SlideToRun, TR3LaraState.Sprint, 14, 14, TR3LaraAnim.RunToSprintLeft, 0);
        ImportCrawlJumpDown(tr3Lara, InjState.CrawlJumpDown, InjAnim.CrawlJumpDown, TR3LaraAnim.CrawlIdle);
        ImportCrouchTurn(tr3Lara, InjState.CrouchTurnLeft, InjAnim.CrouchTurnLeft, 
            InjState.CrouchTurnRight, InjAnim.CrouchTurnRight,
            TR3LaraState.CrouchIdle, TR3LaraAnim.CrouchIdle,
            TR3LaraAnim.CrouchToStand, TR3LaraState.CrouchRoll, TR3LaraAnim.CrouchRollForwardStart,
            TR3LaraState.CrawlIdle, TR3LaraAnim.CrouchToCrawlStart);
        FixVaulting(tr3Lara);
        FixCrouchRoll(tr3Lara, TR3LaraAnim.CrouchRollForwardEnd);

        AlignJumpToReach(tr3Lara,
            TR3LaraAnim.JumpForwardStartToGrabEarly, TR3LaraAnim.JumpForwardStartToGrabLate,
            TR3LaraAnim.RunToGrabLeft, TR3LaraAnim.RunToGrabRight);
        ImportSwingInFast(tr3Lara, InjAnim.SwingInFast);
        ImportLadderToCrouch(tr3Lara, TR2LaraAnim.LadderIdle, TR3LaraAnim.CrouchIdle, TR3LaraState.ClimbToCrawl,
            InjAnim.LadderToCrouchStart, InjAnim.LadderToCrouchEnd);
        AddMinimumJumpDelay(tr3Lara);
        ImportFastPickup(tr3Lara);
        ImportFastPushPull(tr3Lara, true);
        ImportPlinthPickups(tr3Lara, true);
        FixHandstandSFX(tr3Lara);
        FixClimbOnSFX(tr3Lara);
        FixLadderClimbOnSFX(tr3Lara);
        FixHangToCrouchStartSFX(tr3Lara);
        FixShimmySFX(tr3Lara);
        FixWadeTurnSFX(tr3Lara);
        FixTreadSFX(tr3Lara);
        SplitPushableEnds(tr3Lara);
        ImportCornerShimmy(tr3Lara, _cornerAnimMap, _cornerStateMap, TR2LaraAnim.LadderIdle);

        return jungle;
    }

    private static TR3Level CreateExtraLevel()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.RUINS}");
        level.Models[TR3Type.Lara].Meshes[0].TexturedRectangles.Clear();
        level.Models[TR3Type.Lara].Meshes[0].TexturedTriangles.Clear();
        level.Models[TR3Type.Lara].Meshes[0].ColouredRectangles.Clear();
        level.Models[TR3Type.Lara].Meshes[0].ColouredTriangles.Clear();
        level.Models[TR3Type.Lara].Meshes = [.. Enumerable.Repeat(0, 15).Select(m => level.Models[TR3Type.Lara].Meshes[0])];
        CreateModelLevel(level, TR3Type.Lara);

        ImportExtraAnims(level.Models, TR3Type.LaraExtraAnimation_H);

        level.Models.Remove(TR3Type.Lara);
        level.SoundEffects.Clear();

        return level;
    }

    private void SyncToTR2(TRModel lara)
    {
        // Unused kick from TR2 was removed in TR3. Re-add so that counts align in all three games.
        var tr2Lara = _control2.Read($"Resources/{TR2LevelNames.GW}").Models[TR2Type.Lara];
        Debug.Assert(lara.Animations.Count == (int)InjAnim.Kick);
        lara.Animations.Add(tr2Lara.Animations[214]);
        AddChange(lara, 103, TR3LaraState.Kick, 0, 69, InjAnim.Kick, 0);

        // Alter slide to run
        var change = lara.Animations[70].Changes.Find(c => c.StateID == 1);
        change.StateID = (ushort)ResponsiveState;
        change.Dispatches[0].NextFrame = 2;
    }

    private static void FixShimmySFX(TRModel lara)
    {
        foreach (var animId in new[] { LaraAnim.ShimmyLeft, LaraAnim.ShimmyRight })
        {
            var anim = lara.Animations[(int)animId];
            anim.Commands.OfType<TRSFXCommand>()
                .Where(s => s.SoundID == (short)TR2SFX.LaraShimmy)
                .ToList()
                .ForEach(s => s.SoundID = (short)TR3SFX.LaraGrabhand);
        }
    }

    private void FixTreadSFX(TRModel lara)
    {
        // ID 20 was tread in TR2 but was repurposed in TR3 for the ticket booth (Aldwych only).
        // Stale references remained in animations 1, 21, 40, 57, 58, 63, 64, 73. Using TR3's "tread" is
        // overwhelming, so adding missing wet feet sounds is sufficient, similar to TR1.
        foreach (var anim in lara.Animations)
        {
            var removed = anim.Commands.RemoveAll(c => c is TRSFXCommand s && s.SoundID == (short)TR2SFX.LaraTread);
            if (removed == 0)
            {
                continue;
            }
            
            var landFeetSfx = anim.Commands.OfType<TRSFXCommand>()
                .Where(s => s.SoundID == (short)TR3SFX.LaraFeet)
                .ToList();
            if (landFeetSfx.Count == 0 || anim.Commands.Any(c => c is TRSFXCommand s && s.SoundID == WetFeetSFX))
            {
                continue;
            }

            anim.Commands.AddRange(landFeetSfx.Select(f => new TRSFXCommand
            {
                FrameNumber = f.FrameNumber,
                SoundID = WetFeetSFX,
                Environment = TRSFXEnvironment.Water,
            }));
        }
    }

    private static byte[] ExportLaraWAD(TR3Level level, TR3Level extraLevel)
    {
        // Generate the injection's effect on a regular level to allow TRLE builders to utilise
        // the new animations while also being able to edit the defaults. This is a stripped back
        // level file that can be opened in WADTool.
        var originalInfos = level.ObjectTextures.ToList();
        var texMap = new Dictionary<ushort, ushort>();
        level.Models[TR3Type.Lara].Meshes
            .SelectMany(m => m.TexturedFaces)
            .Select(f => f.Texture)
            .Distinct()
            .ToList().ForEach(t => texMap[t] = (ushort)texMap.Count);

        var packer = new TR3TexturePacker(level);
        var laraRegions = packer.GetMeshRegions(level.Models[TR3Type.Lara].Meshes)
            .Values.SelectMany(v => v);

        level.Images16 = new() { new() { Pixels = new ushort[TRConsts.TPageSize] } };
        level.ObjectTextures.Clear();

        packer = new(level);
        packer.AddRectangles(laraRegions);
        packer.Pack(true);

        level.ObjectTextures.AddRange(texMap.Keys.Select(t => originalInfos[t]));
        level.Models[TR3Type.Lara].Meshes
            .SelectMany(m => m.TexturedFaces)
            .ToList().ForEach(f => f.Texture = texMap[f.Texture]);

        GenerateImages8(level, level.Palette.Select(c => c.ToTR1Color()).ToList());

        return ExportZip(level, extraLevel);
    }

    private static byte[] ExportZip(TR3Level level, TR3Level extraLevel)
    {
        var stream = new MemoryStream();
        using var zip = new ZipArchive(stream, ZipArchiveMode.Create);

        {
            using var ms = new MemoryStream();
            _control3.Write(level, ms);
            byte[] phdRaw = ms.ToArray();
            var entry = zip.CreateEntry("lara.tr2", CompressionLevel.Optimal);
            using var zipStream = entry.Open();
            zipStream.Write(phdRaw, 0, phdRaw.Length);
        }

        {
            using MemoryStream ms = new();
            _control3.Write(extraLevel, ms);
            byte[] phdRaw = ms.ToArray();
            ZipArchiveEntry entry = zip.CreateEntry("lara_extra.tr2", CompressionLevel.Optimal);
            using Stream zipStream = entry.Open();
            zipStream.Write(phdRaw, 0, phdRaw.Length);
        }

        zip.Dispose();
        stream.Flush();
        return stream.ToArray();
    }

    private static void ResetLevel(TR3Level level)
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
            [TR3Type.Lara] = level.Models[TR3Type.Lara],
        };
    }

    private static InjectionData CreateAirlockEdit(TR3Level level, string targetLevel, string binName)
    {
        // Different SFX required for the wheel door animation
        var model = level.Models[TR3Type.LaraExtraAnimation_H];
        InjectionBuilder.ResetLevel(level);
        level.Models[TR3Type.LaraExtraAnimation_H] = model;

        var shore = _control3.Read($"Resources/TR3/{targetLevel}");
        var switchAnim = model.Animations[(int)LaraExtraState.Airlock];
        model.Animations.ForEach(a => a.Commands.Clear());
        switchAnim.Commands.AddRange(shore.Models[TR3Type.LaraExtraAnimation_H].Animations[2].Commands);

        var data = InjectionData.Create(level, InjectionType.General, binName, true);
        data.Animations.Clear();
        data.AnimFrames.Clear();
        data.AnimChanges.Clear();
        data.AnimDispatches.Clear();
        data.Models.Clear();

        data.AnimCmdEdits.Add(new()
        {
            TypeID = (int)TR3Type.LaraExtraAnimation_H,
            AnimIndex = (int)LaraExtraState.Airlock,
            RawCount = data.AnimCommands.Count,
            TotalCount = switchAnim.Commands.Count,
        });

        return data;
    }
}
