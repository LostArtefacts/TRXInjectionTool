using System.Diagnostics;
using System.IO.Compression;
using TRImageControl.Packing;
using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR4.Lara;

public class TR4LaraAnimBuilder : LaraBuilder
{
    public override string ID => "tr4_lara_anims";
    public override TRGameVersion GameVersion => TRGameVersion.TR4;
    protected override short JumpSFX => (short)TR4SFX.LaraJump;
    protected override short DryFeetSFX => (short)TR4SFX.LaraFeet;
    protected override short WetFeetSFX => (short)TR4SFX.LaraWetFeet;
    protected override short TreadSFX => (short)TR4SFX.LaraFloating;
    protected override short LandSFX => (short)TR4SFX.LaraLand;
    protected override short KneesShuffleSFX => (short)TR4SFX.LaraKneesShuffle;
    protected override short ClimbOnSFX => (short)TR4SFX.LaraClimb3;
    protected override short ResponsiveState => (short)InjState.Responsive;

    enum InjAnim
    {
        Kick = 445,
        JumpNeutralRoll = 446,
        ControlledDrop = 447,
        ControlledDropContinue = 448,
        HangToJumpUp = 449,
        HangToJumpUpContinue = 450,
        HangToJumpBack = 451,
        HangToJumpBackContinue = 452,
        PoseRightStart = 453,
        PoseRightContinue = 454,
        PoseRightEnd = 455,
        PoseLeftStart = 456,
        PoseLeftContinue = 457,
        PoseLeftEnd = 458,
        SwingInFast = 459,
        LadderToCrouchStart = 460,
        LadderToCrouchEnd = 461,
        FastPickup = 462, // Swapped with TR1-3
        FastPushblockPull = 463,
        FastPushblockPush = 464,
    }

    enum InjState
    {
        Responsive = 118,
        NeutralRoll = 119,
        PoseStart = 120,
        PoseEnd = 121,
        PoseLeft = 122,
        PoseRight = 123,
        CrawlJumpDown = 124,
        FastPickup = 125,
        FastPushblockPull = 126,
        FastPushblockPush = 127,
        PlinthLowPickup = 128,
        PlinthHighPickup = 129,
    }

    public override List<InjectionData> Build()
    {
        var result = new List<InjectionData>();

        {
            var level = CreateLevel();
            result.Add(InjectionData.Create(level, InjectionType.LaraAnims, "lara_animations"));
        }
        {
            // TODO: handle extra anims
        }

        return result;
    }

    public override byte[] Publish()
    {
        var level = CreateLevel(true);
        // var extraLevel = CreateExtraLevel();
        return ExportLaraWAD(level, null);
    }

    private TR4Level CreateLevel(bool useSkin = false)
    {
        var level = _control4.Read($"Resources/TR4/{TR4LevelNames.SETH}");
        if (useSkin)
        {
            level.Models[TR4Type.Lara].Meshes = level.Models[TR4Type.LaraSkin].Meshes;
        }
        ResetLevel(level);
        var tr4Lara = level.Models[TR4Type.Lara];
        PopulateMissingAnims(tr4Lara);

        SyncToTR2(tr4Lara);
        AddSlideToRun(tr4Lara);
        ImportTR1Jumping(tr4Lara);
        ImportTR1Gliding(tr4Lara);
        ImproveTwists(tr4Lara);
        ImportNeutralTwist(tr4Lara, (short)InjAnim.JumpNeutralRoll, (short)InjState.NeutralRoll);
        ImportControlledDrop(tr4Lara, (short)InjAnim.ControlledDropContinue);
        ImportHangToJump(tr4Lara, (short)InjAnim.HangToJumpUp);
        ImportIdlePose(tr4Lara, InjState.PoseStart, InjState.PoseEnd, InjState.PoseLeft, InjState.PoseRight);
        FixJumpToFreefall(tr4Lara);

        FixSprintSFX(tr4Lara, TR3LaraAnim.RunToSprintLeft, TR3LaraAnim.RunToSprintRight);
        AddCrawlJumpDown(tr4Lara);
        FixVaulting(tr4Lara);
        FixCrouchRoll(tr4Lara, TR3LaraAnim.CrouchRollForwardEnd);

        AlignJumpToReach(tr4Lara,
            TR3LaraAnim.JumpForwardStartToGrabEarly, TR3LaraAnim.JumpForwardStartToGrabLate,
            TR3LaraAnim.RunToGrabLeft, TR3LaraAnim.RunToGrabRight);
        ImportSwingInFast(tr4Lara, InjAnim.SwingInFast);
        ImportLadderToCrouch(tr4Lara, TR2LaraAnim.LadderIdle, TR3LaraAnim.CrouchIdle, TR3LaraState.ClimbToCrawl,
            InjAnim.LadderToCrouchStart, InjAnim.LadderToCrouchEnd);
        AddMinimumJumpDelay(tr4Lara);
        ImportSlowPickup(tr4Lara);
        ImportSlowPushPull(tr4Lara);
        AmendPlinthPickups(tr4Lara);
        FixClimbOnSFX(tr4Lara);
        FixLadderClimbOnSFX(tr4Lara);
        FixHangToCrouchStartSFX(tr4Lara);
        FixPoleReleaseState(tr4Lara);

        return level;
    }

    private static void FixPoleReleaseState(TRModel lara)
    {
        var anim = lara.Animations[(int)TR4LaraAnim.PoleToStand];
        anim.StateID = (ushort)TR4LaraState.PoleIdle;
        anim.Commands.Add(new TREmptyHandsCommand());
    }

    private static void PopulateMissingAnims(TRModel lara)
    {
        // OG saved space by excluding animations that weren't needed in some levels e.g. no water = no swimming anims
        var incompleteAnims = lara.Animations
            .Select((a, i) => new { Index = i, Anim = a })
            .Where(a => a.Anim.Frames.Count == 0)
            .Select(a => a.Index)
            .ToList();

        void TryReplace(TRModel altLara, int animId)
        {
            var anim = altLara.Animations[animId];
            if (anim.Frames.Count != 0)
            {
                lara.Animations[animId] = anim;
                incompleteAnims.Remove(animId);
            }
        }

        {
            // These are not present anywhere in TR4; lift from TR3, even though unused.
            var tr3Anims = new[] { 227, 229, 231, 297, 298, 299, 300 };
            var altLara = _control3.Read($"Resources/TR3/{TR3LevelNames.JUNGLE}").Models[TR3Type.Lara];
            foreach (var animId in tr3Anims)
            {
                TryReplace(altLara, animId);
            }
        }

        {
            // Unused death "magic" animation. Give it a frame.
            var anim = lara.Animations[437];
            anim.Frames.Add(lara.Animations[11].Frames[0].Clone());
            anim.FrameEnd = 1;
            incompleteAnims.Remove(437);
        }

        var animCount = lara.Animations.Count;
        foreach (var levelName in TR4LevelNames.AsList.Except(TR4LevelNames.Cambodia))
        {
            var altLara = _control4.Read($"Resources/TR4/{levelName}").Models[TR4Type.Lara];
            Debug.Assert(altLara.Animations.Count == animCount);
            for (int i = incompleteAnims.Count - 1; i >= 0; i--)
            {
                TryReplace(altLara, incompleteAnims[i]);
            }

            if (incompleteAnims.Count == 0)
            {
                break;
            }
        }
    }

    private static void SyncToTR2(TRModel lara)
    {
        var tr2Lara = _control2.Read($"Resources/{TR2LevelNames.GW}").Models[TR2Type.Lara];
        Debug.Assert(lara.Animations.Count == (int)InjAnim.Kick);
        lara.Animations.Add(tr2Lara.Animations[214]);
        AddChange(lara, 103, TR3LaraState.Kick, 0, 69, InjAnim.Kick, 0);
    }

    private void AddSlideToRun(TRModel lara)
    {
        var change = lara.Animations[(int)LaraAnim.SlideForward].Changes.Find(c => c.StateID == 1);
        change.StateID = (ushort)ResponsiveState;
        change.Dispatches[0].NextFrame = 2;

        AddChange(lara, TR3LaraAnim.SlideToRun, TR3LaraState.Sprint, 14, 14, TR3LaraAnim.RunToSprintLeft, 0);
    }

    protected new static void FixCrouchRoll<A>(TRModel lara, A crouchRollEndAnim)
    {
        var badAnim = lara.Animations[Convert.ToInt32(crouchRollEndAnim)];
        var goodAnim = GetLaraExtModel().Animations[Convert.ToInt32(ExtLaraAnim.CrouchRollEnd)];
        badAnim.Frames = goodAnim.Frames;
        badAnim.FrameRate = goodAnim.FrameRate;
        badAnim.FrameEnd = goodAnim.FrameEnd;
    }

    private static void AddCrawlJumpDown(TRModel lara)
    {
        var anim = lara.Animations[(int)TR4LaraAnim.CrawlJumpDown];
        anim.StateID = (ushort)InjState.CrawlJumpDown;

        var crawlAnim = lara.Animations[(int)TR3LaraAnim.CrawlIdle];
        crawlAnim.Changes.RemoveAll(c => c.Dispatches.Any(d => d.NextAnimation == (int)TR4LaraAnim.CrawlJumpDown));
        AddChange(crawlAnim, InjState.CrawlJumpDown, 0, 44, TR4LaraAnim.CrawlJumpDown, 0);
    }

    private static void ImportSlowPickup(TRModel lara)
    {
        var altLara = _control3.Read($"Resources/TR3/{TR3LevelNames.JUNGLE}").Models[TR3Type.Lara];
        var slowAnim = altLara.Animations[(int)LaraAnim.Pickup];
        var fastAnim = lara.Animations[(int)LaraAnim.Pickup];
        var animIdx = lara.Animations.Count;
        lara.Animations.Add(fastAnim);
        lara.Animations[(int)LaraAnim.Pickup] = slowAnim;

        foreach (var id in new[] { (int)LaraAnim.StandStill, (int)LaraAnim.StandIdle, (int)TR3LaraAnim.CrouchToStand })
        {
            var standAnim = lara.Animations[Convert.ToInt32(id)];
            var change = standAnim.Changes.First(c => c.StateID == Convert.ToInt32(LaraState.Pickup)).Clone();
            change.StateID = Convert.ToUInt16(InjState.FastPickup);
            change.Dispatches.ForEach(d => d.NextAnimation = (short)animIdx);
            standAnim.Changes.Add(change);
        }
    }

    private static void ImportSlowPushPull(TRModel lara)
    {
        // TODO: this will also require animations for the pushblocks themselves
        var altLara = _control3.Read($"Resources/TR3/{TR3LevelNames.JUNGLE}").Models[TR3Type.Lara];
        var slowPull = altLara.Animations[(int)LaraAnim.PushablePull];
        var slowPush = altLara.Animations[(int)LaraAnim.PushablePush];
        var fastPull = lara.Animations[(int)LaraAnim.PushablePull];
        var fastPush = lara.Animations[(int)LaraAnim.PushablePush];

        fastPull.NextAnimation = (ushort)InjAnim.FastPushblockPull;
        fastPush.NextAnimation = (ushort)InjAnim.FastPushblockPush;

        lara.Animations.Add(fastPull);
        lara.Animations.Add(fastPush);
        lara.Animations[(int)LaraAnim.PushablePull] = slowPull;
        lara.Animations[(int)LaraAnim.PushablePush] = slowPush;

        AddChange(lara, LaraAnim.PushableGrab, InjState.FastPushblockPull,
                19, 20, InjAnim.FastPushblockPull, 0);
        AddChange(lara, LaraAnim.PushableGrab, InjState.FastPushblockPush,
                19, 20, InjAnim.FastPushblockPush, 0);

        SplitPushableEnds(lara);
    }

    protected static void AmendPlinthPickups(TRModel lara)
    {
        var map = new Dictionary<TR4LaraAnim, InjState>
        {
            [TR4LaraAnim.PlinthLowPickup] = InjState.PlinthLowPickup,
            [TR4LaraAnim.PlinthHighPickup] = InjState.PlinthHighPickup,
        };

        foreach (var (animId, animState) in map)
        {
            var anim = lara.Animations[(int)animId];
            anim.Commands.Add(new TREmptyHandsCommand());

            foreach (var id in new[] { LaraAnim.StandStill, LaraAnim.StandIdle })
            {
                var standAnim = lara.Animations[Convert.ToInt32(id)];
                var change = standAnim.Changes.First(c => c.StateID == Convert.ToInt32(LaraState.Pickup)).Clone();
                change.StateID = (ushort)animState;
                change.Dispatches.ForEach(d => d.NextAnimation = (short)animId);
                standAnim.Changes.Add(change);
            }
        }
    }

    private static byte[] ExportLaraWAD(TR4Level level, TR4Level extraLevel)
    {
        var originalInfos = level.ObjectTextures.ToList();
        var texMap = new Dictionary<ushort, ushort>();
        level.Models[TR4Type.Lara].Meshes
            .SelectMany(m => m.TexturedFaces)
            .Select(f => f.Texture)
            .Distinct()
            .ToList().ForEach(t => texMap[t] = (ushort)texMap.Count);

        var packer = new TR4TexturePacker(level, TRGroupPackingMode.Object);
        var laraRegions = packer.GetMeshRegions(level.Models[TR4Type.Lara].Meshes)
            .Values.SelectMany(v => v);

        level.Images.Rooms = new()
        {
            Images16 = [],
            Images32 = [],
        };
        level.Images.Objects.Images32 = [new() { Pixels = new uint[TRConsts.TPageSize] }];
        level.Images.Objects.Images16 = [new() { Pixels = new ushort[TRConsts.TPageSize] }];
        level.ObjectTextures.Clear();

        packer = new(level, TRGroupPackingMode.Object);
        packer.AddRectangles(laraRegions);
        packer.Pack(true);

        level.ObjectTextures.AddRange(texMap.Keys.Select(t => originalInfos[t]));
        level.Models[TR4Type.Lara].Meshes
            .SelectMany(m => m.TexturedFaces)
            .ToList().ForEach(f => f.Texture = texMap[f.Texture]);

        return ExportZip(level, extraLevel);
    }

    private static byte[] ExportZip(TR4Level level, TR4Level extraLevel)
    {
        var stream = new MemoryStream();
        using var zip = new ZipArchive(stream, ZipArchiveMode.Create);

        {
            using var ms = new MemoryStream();
            _control4.Write(level, ms);
            byte[] phdRaw = ms.ToArray();
            var entry = zip.CreateEntry("lara.tr4", CompressionLevel.Optimal);
            using var zipStream = entry.Open();
            zipStream.Write(phdRaw, 0, phdRaw.Length);
        }

        if (extraLevel != null)
        {
            using MemoryStream ms = new();
            _control4.Write(extraLevel, ms);
            byte[] phdRaw = ms.ToArray();
            ZipArchiveEntry entry = zip.CreateEntry("lara_extra.tr4", CompressionLevel.Optimal);
            using Stream zipStream = entry.Open();
            zipStream.Write(phdRaw, 0, phdRaw.Length);
        }

        zip.Dispose();
        stream.Flush();
        return stream.ToArray();
    }

    private static void ResetLevel(TR4Level level)
    {
        level.Sprites.Clear();
        level.Rooms.Clear();
        level.StaticMeshes.Clear();
        level.Boxes.Clear();
        level.SoundEffects.Clear();
        level.Entities.Clear();
        level.Cameras.Clear();
        level.Flybys.Clear();

        level.Models = new()
        {
            [TR4Type.Lara] = level.Models[TR4Type.Lara],
        };
    }
}
