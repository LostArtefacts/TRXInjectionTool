using System.Diagnostics;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types;

public abstract class LaraBuilder : InjectionBuilder
{
    private static readonly string _extLaraPath = "Resources/lara_ext.phd";
    private const int _tr1AnimCount = 160;

    public abstract TRGameVersion GameVersion { get; }
    protected abstract short JumpSFX { get; }
    protected abstract short DryFeetSFX { get; }
    protected abstract short WetFeetSFX { get; }
    protected abstract short TreadSFX { get; }
    protected abstract short LandSFX { get; }
    protected abstract short KneesShuffleSFX { get; }
    protected abstract short PoleLoopSFX { get; }
    protected abstract short ClimbOnSFX { get; }
    protected abstract short ResponsiveState { get; }

    private static TRModel _completeTR4Lara;

    protected enum LaraState
    {
        Stop = 2,
        JumpForward = 3,
        Pose = 4,
        Death = 8,
        Freefall = 9,
        Reach = 11,
        Land = 14,
        Glide = 18,
        PullUp = 19,
        ShimmyLeft = 30,
        ShimmyRight = 31,
        PushblockPush = 36,
        PushblockPull = 37,
        Pickup = 39,
        Roll = 45,
        Handstand = 54,
        FastPickup = 99,
        FastPushblockPull = 100,
        FastPushblockPush = 101,
        PlinthLowPickup = 102,
        PlinthHighPickup = 103,
    }

    protected enum LaraAnim
    {
        Run = 0,
        RunStart = 6,
        StandStill = 11,
        TurnRightSlow = 12,
        TurnLeftSlow = 13,
        RunJumpRightStart = 16,
        RunJumpRightContinue = 17,
        RunJumpLeftStart = 18,
        RunJumpLeftContinue = 19,
        Freefall = 23,
        JumpUp = 28,
        Climb3Click = 42,
        TurnRight = 44,
        Climb2Click = 50,
        Climb2ClickEnd = 51,
        JumpForwardEndToFreefall = 49,
        TurnLeft = 69,
        SlideForward = 70,
        JumpBack = 75,
        JumpForward = 77,
        UnderwaterSwimForward = 86,
        UnderwaterSwimGlide = 87,
        JumpForwardToReach = 94,
        Reach = 95,
        ReachToHang = 96,
        ClimbOn = 97,
        JumpForwardToReachLate = 100,
        ClimbOnEnd = 102,
        StandIdle = 103,
        PushableGrab = 120,
        PushablePull = 122,
        PushablePush = 123,
        Pickup = 135,
        ShimmyLeft = 136,
        ShimmyRight = 137,
        StandDeath = 138,
        RollStart = 146,
        ClimbOnHandstand = 159,
    }

    protected enum TR2LaraAnim
    {
        StandToLadder = 160,
        LadderUp = 161,
        LadderUpStopRight = 162,
        LadderUpStopLeft = 163,
        LadderIdle = 164,
        LadderUpStart = 165,
        LadderDownStopLeft = 166,
        LadderDownStopRight = 167,
        LadderDown = 168,
        LadderDownStart = 169,
        LadderRight = 170,
        LadderLeft = 171,
        LadderHang = 172,
        LadderHangToIdle = 173,
        LadderClimbOn = 174,
        LadderBackFlipStart = 182,
        LadderBackFlipContinue = 183,
        LadderUpHanging = 187,
        LadderDownHanging = 188,
        LadderToHangDown = 194,
        LadderToHangRight = 201,
        LadderToHangLeft = 202,
    }

    protected enum TR2LaraState
    {
        ClimbStance = 56,
        Climbing = 57,
        ClimbLeft = 58,
        ClimbEnd = 59,
        ClimbRight = 60,
        ClimbDown = 61,
    }

    protected enum TR3LaraAnim
    {
        SwingInSlow = 150,
        Sprint = 223,
        RunToSprintLeft = 224,
        RunToSprintRight = 225,
        SprintSlideStandLeft = 228,
        SprintSlideStandRight = 226,
        SprintToRollLeft = 230,
        SprintRollLeftToRun = 232,
        SprintToRollRight = 308,
        SprintRollRightToRun = 309,
        SprintToRunLeft = 243,
        SprintToRunRight = 244,
        SlideToRun = 246,
        JumpForwardStartToGrabEarly = 248,
        JumpForwardStartToGrabLate = 249,
        RunToGrabRight = 250,
        RunToGrabLeft = 251,

        StandToCrouch = 217,
        CrouchRollForwardStart = 218,
        CrouchRollForwardContinue = 219,
        CrouchRollForwardEnd = 220,
        CrouchToStand = 221,
        CrouchIdle = 222,
        StandToCrouchEnd = 245,
        CrouchRollForwardStartAlternate = 247,
        CrouchToCrawlStart = 258,        
        CrouchToCrawlEnd = 264,
        CrouchToCrawlContinue = 273,        
        HangToCrouchStart = 287,
        HangToCrouchEnd = 288,
        CrouchPickup = 291,
        CrouchHitFront = 293,
        CrouchHitBack = 294,
        CrouchHitRight = 295,
        CrouchHitLeft = 296,
        StandToCrouchAbort = 303,
        RunToCrouchLeftStart = 304,
        RunToCrouchRightStart = 305,
        RunToCrouchLeftEnd = 306,
        RunToCrouchRightEnd = 307,
        SprintToCrouchLeft = 310,
        SprintToCrouchRight = 311,
        CrouchPickupFlare = 312,

        CrawlToCrouchStart = 259,
        CrawlForward = 260,
        CrawlIdleToForward = 261,
        CrawlForwardToIdleStartRight = 262,
        CrawlIdle = 263,
        CrawlToCrouchlEndUnused = 265,
        CrawlForwardToIdleEndRight = 266,
        CrawlForwardToIdleStartLeft = 267,
        CrawlForwardToIdleEndLeft = 268,
        CrawlTurnLeft = 269,
        CrawlTurnRight = 270,
        CrawlToCrouchContinue = 274,
        CrawlIdleToBackward = 275,
        CrawlBackward = 276,
        CrawlBackwardToIdleStartRight = 277,
        CrawlBackwardToIdleEndRight = 278,
        CrawlBackwardToIdleStartLeft = 279,
        CrawlBackwardToIdleEndLeft = 280,
        CrawlTurnLeftEarlyEnd = 281,
        CrawlTurnRightEarlyEnd = 282,
        CrawlToHangStart = 289,
        CrawlToHangContinue = 290,
        CrawlPickup = 292,
        CrawlHitFront = 297,
        CrawlHitBack = 298,
        CrawlHitRight = 299,
        CrawlHitLeft = 300,
        CrawlDeath = 301,
        CrawlToHangEnd = 302,

        MonkeyIdle = 234,
        MonkeyFall = 235,
        MonkeyGrab = 233,
        MonkeyForward = 236,
        MonkeyStopLeft = 237,
        MonkeyStopRight = 238,
        MonkeyIdleToForwardLeft = 239,
        MonkeyIdleToForwardRight = 252,
        MonkeyShimmyLeft = 253,
        MonkeyShimmyLeftEnd = 254,
        MonkeyShimmyRight = 255,
        MonkeyShimmyRightEnd = 256,
        MonkeyTurnAround = 257,
        MonkeyTurnLeft = 271,
        MonkeyTurnRight = 272,
        MonkeyTurnLeftEarlyEnd = 283,
        MonkeyTurnLeftLateEnd = 284,
        MonkeyTurnRightEarlyEnd = 285,
        MonkeyTurnRightLateEnd = 286,
    }

    protected enum TR3LaraState
    {
        Kick = 69,
        CrouchIdle = 71,
        CrouchRoll = 72,
        Sprint = 73,
        SprintRoll = 74,
        CrawlIdle = 80,
        CrawlForward = 81,
        CrawlTurnLeft = 84,
        CrawlTurnRight = 85,
        CrawlBackward = 86,
        ClimbToCrawl = 87,
        CrawlToClimb = 88,
        MonkeyIdle = 75,
        MonkeyForward = 76,
        MonkeyLeft = 77,
        MonkeyRight = 78,
        MonkeyRoll = 79,
        MonkeyTurnLeft = 82,
        MonkeyTurnRight = 83,
    }

    protected enum TR4LaraAnim
    {
        DoorOpenForward = 313,
        DoorOpenBack = 314,
        DoorKick = 315,
        GiantButtonPush = 316,
        FloorTrapdoorOpen = 317,
        CeilingTrapdoorOpen = 318,
        TurnswitchGrabLeft = 319,
        TurnswitchGrabRight = 320,
        CogwheelPull = 321,
        CogwheelGrab = 322,
        CogwheelUngrab = 323,
        LeverswitchPush = 324,
        HoleGrab = 325,
        StandToPoleGrab = 326,
        PoleJump = 327,
        PoleIdle = 328,
        PoleClimbUp = 329,
        PoleFall = 330,
        ReachToPoleGrab = 331,
        PoleTurnLeftStart = 332,
        PoleTurnRightStart = 333,
        PoleIdleToClimbDown = 334,
        PoleClimbDown = 335,
        PoleClimbDownToIdle = 336,
        JumpUpToPoleGrab = 337,
        PoleClimbUpEnd = 338,
        PulleyGrab = 339,
        PulleyPull = 340,
        PulleyUngrab = 341,
        PoleToStand = 342,
        PoleTurnLeftContinueUnused = 343,
        PoleTurnLeftEnd = 344,
        PoleTurnRightContinueUnused = 345,
        PoleTurnRightEnd = 346,
        TurnswitchPushRightStart = 347,
        TurnswitchPushRightContinue = 348,
        TurnswitchPushRightEnd = 349,
        TurnswitchPushLeftStart = 350,
        TurnswitchPushLeftContinue = 351,
        TurnswitchPushLeftEnd = 352,
        HangCornerLeftOuterStart = 355,
        HangCornerLeftOuterEnd = 356,
        HangCornerRightOuterStart = 357,
        HangCornerRightOuterEnd = 358,
        HangCornerLeftInnerStart = 359,
        HangCornerLeftInnerEnd = 360,
        HangCornerRightInnerStart = 361,
        HangCornerRightInnerEnd = 362,
        LadderCornerLeftOuterStart = 363,
        LadderCornerLeftOuterEnd = 364,
        LadderCornerRightOuterStart = 365,
        LadderCornerRightOuterEnd = 366,
        LadderCornerLeftInnerStart = 367,
        LadderCornerLeftInnerEnd = 368,
        LadderCornerRightInnerStart = 369,
        LadderCornerRightInnerEnd = 370,
        JumpUpToRopeStart = 371,
        TrainDeath = 372,
        JumpUpToRopeEnd = 373,
        RopeIdle = 374,
        RopeDownStart = 375,
        RopeUp = 376,
        RopeIdleToRopeHangUnused = 377,
        RopeGrabToFallUnused = 378,
        RopeJumpToGrab = 379,
        RopeIdleToBackflipUnused = 380,
        RopeSwingToFallSemifrontUnused = 381,
        RopeSwingToFallMiddlUnused = 382,
        RopeSwingToFallBackUnused = 383,
        RopeDown = 384,
        RopeDownEnd = 385,
        RopeSwingToReach = 386,
        RopeIdleToSwing = 387,
        RopeIdleToSwingSemimiddleUnused = 388,
        RopeIdleToSwingHalfmiddleUnused = 389,
        RopeSwingToFallFrontUnused = 390,
        RopeGrabToFallAlternateUnused = 391,
        RopeTurnLeft = 392,
        RopeTurnRight = 393,
        RopeSwing = 394,
        LadderToHandsDownAlternateUnused = 395,
        RopeSwingBackContinueUnused = 396,
        RopeSwingBackEndUnused = 397,
        RopeSwingBackStartUnused = 398,
        RopeSwingForwardSoftUnused = 399,
        PourWaterskinLow = 400,
        FillWaterskin = 401,
        PourWaterskinHigh = 402,
        PryDoor = 403,
        RopeSwingForwardHardUnused = 404,
        RopeChangeRopeUnused = 405,
        RopeSwingToReachFront = 406,
        RopeSwingToReachMiddle = 407,
        RopeSwingBlockUnused = 408,
        RopeSwingToReachSemimiddle = 409,
        RopeSwingToReachHangFront = 410,
        RopeSwingToReachFront2 = 411,
        DoubledoorsPush = 412,
        BigButtonPush = 413,
        Jumpswitch = 414,
        UnderwaterPulley = 415,
        UnderwaterDoorOpen = 416,
        FastPushblockPushStop = 417,
        FastPushblockPullStop = 418,
        CrowbarUseOnWall = 419,
        CrowbarUseOnFloor = 420,
        CrawlJumpDown = 421,        
        HarpPlay = 422,
        PutTrident = 423,
        PlinthHighPickup = 424,
        PlinthLowPickup = 425,
        RotateSenet = 426,
        TorchLight1 = 427,
        TorchLight2 = 428,
        TorchLight3 = 429,
        TorchLight4 = 430,
        TorchLight5 = 431,
        DetonatorUse = 432,
        CorrectPositionFrontUnused = 433,
        CorrectPositionLeftUnused = 434,
        CorrectPositionRightUnused = 435,
        CrowbarUseOnFloorFailUnused = 436,
        DeathMagicUnused = 437,
        DeathBlowup = 438,
        PickupSarcophagus = 439,
        DragBody = 440,
        Binoculars = 441,
        DeathBigScorpion = 442,
        DeathSeth = 443,
        BeetlePut = 444,
    }

    protected enum TR4LaraState
    {
        RopeLeft = 90,
        RopeRight = 91,
        BlockSwitch = 92,
        LiftTrapdoor = 93,
        PullTrapdoor = 94,
        TurnSwitch = 95,
        CogSwitch = 96,
        RailSwitch = 97,
        HiddenPickup = 98,
        PoleIdle = 99,
        PoleUp = 100,
        PoleDown = 101,
        PoleLeft = 102,
        PoleRight = 103,
        Pulley = 104,
        CrouchTurnLeft = 105,
        CrouchTurnRight = 106,
        ShimmyOuterLeft = 107,
        ShimmyOuterRight = 108,
        ShimmyInnerLeft = 109,
        ShimmyInnerRight = 110,
        RopeIdle = 111,
        RopeClimb = 112,
        RopeSlide = 113,
        RopeForward = 114,
        RopeBack = 115,
        RopeForwardSoftUnused = 116,
        PushDoors = 117,
    }

    protected enum ExtLaraAnim
    {
        UWRollStart = 0,
        UWRollEnd = 1,
        RunJumpRollStart = 2,
        RunJumpRollEnd = 3,
        JumpFwdRollStart = 4,
        JumpFwdRollEnd = 5,
        JumpBackRollStart = 6,
        JumpBackRollEnd = 7,
        ControlledDrop = 8,
        ControlledDropContinue = 9,
        HangToJumpUp = 10,
        HangToJumpUpContinue = 11,
        HangToJumpBack = 12,
        HangToJumpBackContinue = 13,
        JumpNeutralRoll = 14,
        PoseRightStart = 15,
        PoseRightContinue = 16,
        PoseRightEnd = 17,
        PoseLeftStart = 18,
        PoseLeftContinue = 19,
        PoseLeftEnd = 20,
        JailWakeUp = 21,
        CrawlJumpDown = 22,
        CrouchTurnLeft = 23,
        CrouchTurnRight = 24,
        LadderToCrouchStart = 25,
        LadderToCrouchEnd = 26,
        CrouchRollEnd = 27,
        FastPickup = 28,
        FastPushblockPull = 29,
        FastPushblockPush = 30,
        FastPushblockPullStop = 31,
        FastPushblockPushStop = 32,
        PlinthLowPickup = 33,
        PlinthHighPickup = 34,
    }

    protected enum LaraExtraState
    {
        Breath       = 0,
        TrexKill     = 1,
        ScionPickup1 = 2,
        UseMidas     = 3,
        MidasKill    = 4,
        ScionPickup2 = 5,
        TorsoKill    = 6,
        Plunger      = 7,
        StartAnim    = 8,
        Airlock      = 9,
        SharkKill    = 10,
        YetiKill     = 11,
        GongBong     = 12,
        GuardKill    = 13,
        PullDagger   = 14,
        StartHouse   = 15,
        EndHouse     = 16,
        ShivaKill    = 17,
        RapidsDrown  = 18,
        TrainKill    = 19,
        JailWakeUp   = 20,
        WillardKill  = 21,
    }

    private static readonly List<LaraExtraState> _extraDeathStates =
    [
        LaraExtraState.TrexKill, LaraExtraState.MidasKill, LaraExtraState.TorsoKill,
        LaraExtraState.SharkKill, LaraExtraState.YetiKill, LaraExtraState.GuardKill,
        LaraExtraState.ShivaKill, LaraExtraState.TrainKill, LaraExtraState.WillardKill,
        LaraExtraState.RapidsDrown,
    ];

    private class ExtraAnimData(string levelName, LaraExtraState state, int animIdx)
    {
        public string LevelName { get; set; } = levelName;
        public LaraExtraState State { get; set; } = state;
        public int AnimIdx { get; set; } = animIdx;
    }

    private static readonly List<ExtraAnimData> _extraData =
    [
        new(TR1LevelNames.VALLEY, LaraExtraState.TrexKill, 1),
        new(TR1LevelNames.QUALOPEC, LaraExtraState.ScionPickup1, 0),
        new(TR1LevelNames.MIDAS, LaraExtraState.UseMidas, 0),
        new(TR1LevelNames.MIDAS, LaraExtraState.MidasKill, 1),
        new(TR1LevelNames.ATLANTIS, LaraExtraState.ScionPickup2, 0),
        new(TR1LevelNames.PYRAMID, LaraExtraState.TorsoKill, 0),
        new(TR2LevelNames.BARTOLI, LaraExtraState.Plunger, 1),
        new(TR2LevelNames.RIG, LaraExtraState.StartAnim, 3),
        new(TR2LevelNames.RIG, LaraExtraState.Airlock, 2),
        new(TR2LevelNames.FATHOMS, LaraExtraState.SharkKill, 1),
        new(TR2LevelNames.COT, LaraExtraState.YetiKill, 1),
        new(TR2LevelNames.CHICKEN, LaraExtraState.GongBong, 2),
        new(TR2LevelNames.FLOATER, LaraExtraState.GuardKill, 1),
        new(TR2LevelNames.LAIR, LaraExtraState.PullDagger, 2),
        new(TR2LevelNames.HOME, LaraExtraState.StartHouse, 1),
        new(TR2LevelNames.HOME, LaraExtraState.EndHouse, 2),
        new(TR3LevelNames.RUINS, LaraExtraState.ShivaKill, 1),
        new(TR3LevelNames.MADUBU, LaraExtraState.RapidsDrown, 25),
        new(TR3LevelNames.ALDWYCH, LaraExtraState.TrainKill, 0),
        new(TR3LevelNames.HSC, LaraExtraState.JailWakeUp, (int)ExtLaraAnim.JailWakeUp),
        new(TR3LevelNames.WILLIE, LaraExtraState.WillardKill, 1),
    ];

    public static TRModel GetLaraPoseModel()
        => _control1.Read(_extLaraPath).Models[TR1Type.Lara];

    public static TRModel GetLaraExtModel()
        => _control1.Read(_extLaraPath).Models[TR1Type.LaraMiscAnim_H];

    public abstract byte[] Publish();

    protected static TRModel GetCompleteTR4Lara()
    {
        if (_completeTR4Lara != null)
        {
            return _completeTR4Lara;
        }

        // OG saved space by excluding animations that weren't needed in some levels e.g. no water = no swimming anims
        var lara = _control4.Read($"Resources/TR4/{TR4LevelNames.SETH}").Models[TR4Type.Lara];
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

        FixPoleReleaseState(lara);
        FixInteractiveDoorHandsFree(lara);
        _completeTR4Lara = lara;
        return _completeTR4Lara;
    }

    protected void ImportSlideToRun(TRModel lara)
    {
        var tr3Lara = _control3.Read($"Resources/{TR3LevelNames.JUNGLE}").Models[TR3Type.Lara];
        var anim = tr3Lara.Animations[246].Clone();
        lara.Animations.Add(anim);

        anim.Commands.RemoveAll(a => a is not TRSFXCommand);
        (anim.Commands[0] as TRSFXCommand).SoundID = WetFeetSFX;

        var change = tr3Lara.Animations[70].Changes.Find(c => c.StateID == 1).Clone();
        change.StateID = (ushort)ResponsiveState;
        change.Dispatches[0].NextAnimation = (short)(lara.Animations.Count - 1);
        change.Dispatches[0].NextFrame = 2;
        lara.Animations[70].Changes.Add(change);
    }

    protected void ImportTR1Jumping(TRModel lara)
    {
        var runAnim = lara.Animations[(int)LaraAnim.Run];
        var jumpChange = runAnim.Changes.FirstOrDefault(c => c.StateID == (ushort)LaraState.JumpForward);
        var responsiveChange = jumpChange.Clone();
        runAnim.Changes.Add(responsiveChange);
        responsiveChange.StateID = (ushort)ResponsiveState;

        foreach (var dispatch in jumpChange.Dispatches)
        {
            if (dispatch.NextAnimation == (short)LaraAnim.RunJumpRightStart)
            {
                dispatch.Low = 14;
                dispatch.High = 15;
            }
            else
            {
                dispatch.Low = 3;
                dispatch.High = 4;
            }
        }
    }

    protected void ImportTR1Gliding(TRModel lara)
    {
        var swimAnim = lara.Animations[(int)LaraAnim.UnderwaterSwimForward];
        var glideChange = swimAnim.Changes.FirstOrDefault(c => c.StateID == (ushort)LaraState.Glide);
        var dispatches = glideChange.Dispatches.Select(d => d.Clone()).ToList();
        glideChange.Dispatches.RemoveAll(d => d.NextAnimation != (short)LaraAnim.UnderwaterSwimGlide);
        glideChange.Dispatches.FirstOrDefault(d => d.Low == 0).High = 2;

        swimAnim.Changes.Add(new()
        {
            StateID = (ushort)ResponsiveState,
            Dispatches = dispatches,
        });

        dispatches.Sort((d1, d2) => d1.Low.CompareTo(d2.Low));
    }

    protected static void ImproveTwists(TRModel lara)
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

    protected void ImportNeutralTwist(TRModel lara, short animID, short stateID)
    {
        var laraExt = GetLaraExtModel();
        var anim = laraExt.Animations[(int)ExtLaraAnim.JumpNeutralRoll];
        lara.Animations.Add(anim);
        anim.NextAnimation = 11;
        anim.StateID = (ushort)stateID;

        anim.Commands.Add(new TRFXCommand { FrameNumber = 23 });
        anim.Commands.Add(new TRSFXCommand
        {
            FrameNumber = 10,
            SoundID = JumpSFX,
        });
        anim.Commands.Add(new TRSFXCommand
        {
            FrameNumber = 7,
            SoundID = DryFeetSFX,
            Environment = TRSFXEnvironment.Land,
        });
        anim.Commands.Add(new TRSFXCommand
        {
            FrameNumber = 7,
            SoundID = WetFeetSFX,
            Environment = TRSFXEnvironment.Water,
        });
        anim.Commands.Add(new TRSFXCommand
        {
            FrameNumber = 32,
            SoundID = LandSFX,
            Environment = TRSFXEnvironment.Land,
        });
        anim.Commands.Add(new TRSFXCommand
        {
            FrameNumber = 32,
            SoundID = WetFeetSFX,
            Environment = TRSFXEnvironment.Water,
        });

        // Compress-to-arabian
        lara.Animations[73].Changes.Add(new()
        {
            StateID = (ushort)ResponsiveState,
            Dispatches =
            [
                new()
                {
                    Low = 0,
                    High = 2,
                    NextFrame = 1,
                    NextAnimation = animID,
                },
                new()
                {
                    Low = 3,
                    High = 4,
                    NextFrame = 4,
                    NextAnimation = animID,
                },
                new()
                {
                    Low = 5,
                    High = 7,
                    NextFrame = 5,
                    NextAnimation = animID,
                }
            ]
        });

        // Arabian-to-jump
        AddChange(anim, 15, 33, 37, 73, 6);
        // Arabian-to-roll
        AddChange(anim, 45, 33, 37, 146, 1);
        // Arabian-loop
        AddChange(anim, ResponsiveState, 32, 32, animID, 2);
    }

    protected static void ImportControlledDrop(TRModel lara, short continueAnimID)
    {
        var laraExt = GetLaraExtModel();
        var startAnim = laraExt.Animations[(int)ExtLaraAnim.ControlledDrop];
        var endAnim = laraExt.Animations[(int)ExtLaraAnim.ControlledDropContinue];

        lara.Animations.Add(startAnim);
        lara.Animations.Add(endAnim);
        startAnim.NextAnimation = (ushort)continueAnimID;
        endAnim.NextAnimation = 95;
    }

    protected void ImportHangToJump(TRModel lara, short startAnimID)
    {
        var laraExt = GetLaraExtModel();
        var upStartAnim = laraExt.Animations[(int)ExtLaraAnim.HangToJumpUp];
        var upEndAnim = laraExt.Animations[(int)ExtLaraAnim.HangToJumpUpContinue];
        var backStartAnim = laraExt.Animations[(int)ExtLaraAnim.HangToJumpBack];
        var backEndAnim = laraExt.Animations[(int)ExtLaraAnim.HangToJumpBackContinue];

        lara.Animations.Add(upStartAnim);
        lara.Animations.Add(upEndAnim);
        upStartAnim.StateID = 28;
        upStartAnim.NextAnimation = (ushort)(lara.Animations.Count - 1);
        upEndAnim.NextAnimation = 28;

        lara.Animations.Add(backStartAnim);
        lara.Animations.Add(backEndAnim);
        backStartAnim.NextAnimation = (ushort)(lara.Animations.Count - 1);
        backEndAnim.NextAnimation = 76;

        upStartAnim.Commands.Add(new TREmptyHandsCommand());
        backStartAnim.Commands.Add(new TREmptyHandsCommand());

        // Marker for engine that hanging is responsive
        AddChange(lara, 96, ResponsiveState, 21, 22, 96, 21);
        // Hang to jump up
        AddChange(lara, 96, 28, 21, 22, startAnimID, 0);
        // Hang to jump back
        AddChange(lara, 96, 25, 21, 22, startAnimID + 2, 0);
    }

    protected static void ImportCrawlJumpDown(TRModel lara, 
        object crawlJumpStateID, object crawlJumpAnimID, object crawlIdleAnimID)
    {
        var laraExt = GetLaraExtModel();
        var anim = laraExt.Animations[(int)ExtLaraAnim.CrawlJumpDown].Clone();
        anim.NextAnimation = (ushort)LaraAnim.Freefall;
        anim.StateID = Convert.ToUInt16(crawlJumpStateID);
        lara.Animations.Add(anim);

        var crawlAnim = lara.Animations[Convert.ToInt32(crawlIdleAnimID)];
        AddChange(crawlAnim, crawlJumpStateID, 0, 44, crawlJumpAnimID, 0);
    }

    private void ImportCrouchTurnAnim(TRModel lara, ExtLaraAnim extAnimID,
        object stateID, object animID, object idleStateID, object idleAnimID,
        object standAnimID, object rollStateID, object rollAnimID,
        object crawlStateID, object crawlAnimID)
    {
        var laraExt = GetLaraExtModel();
        var anim = laraExt.Animations[Convert.ToInt32(extAnimID)].Clone();
        anim.StateID = Convert.ToUInt16(stateID);
        anim.NextAnimation = Convert.ToUInt16(animID);
        anim.Commands.OfType<TRSFXCommand>().Where(s => s.SoundID == 24)
            .ToList().ForEach(s => s.SoundID = KneesShuffleSFX);
        lara.Animations.Add(anim);

        var idleAnim = lara.Animations[Convert.ToInt32(idleAnimID)];
        AddChange(idleAnim, stateID, 0, 44, animID, 0);
        AddChange(anim, idleStateID, 24, 25, idleAnimID, 0);
        AddChange(anim, LaraState.Stop, 0, 24, standAnimID, 0);
        AddChange(anim, rollStateID, 0, 24, rollAnimID, 0);
        AddChange(anim, crawlStateID, 0, 24, crawlAnimID, 0);
    }

    protected void ImportCrouchTurn(TRModel lara,
        object crouchLeftStateID, object crouchLeftAnimID,
        object crouchRightStateID, object crouchRightAnimID,
        object crouchIdleStateID, object crouchIdleAnimID,
        object standAnimID, object rollStateID, object rollAnimID,
        object crawlStateID, object crawlAnimID)
    {
        ImportCrouchTurnAnim(lara, ExtLaraAnim.CrouchTurnLeft, 
            crouchLeftStateID, crouchLeftAnimID, crouchIdleStateID, crouchIdleAnimID,
            standAnimID, rollStateID, rollAnimID, crawlStateID, crawlAnimID);
        ImportCrouchTurnAnim(lara, ExtLaraAnim.CrouchTurnRight, 
            crouchRightStateID, crouchRightAnimID, crouchIdleStateID, crouchIdleAnimID,
            standAnimID, rollStateID, rollAnimID, crawlStateID, crawlAnimID);
    }

    protected void ImportSprint<A, S>(TRModel lara, object slideToRunAnim,
        Dictionary<TR3LaraAnim, A> sprintAnimMap, Dictionary<TR3LaraState, S> sprintStateMap,
        Dictionary<TR3LaraAnim, A> crawlAnimMap, Dictionary<TR3LaraState, S> crawlStateMap)
        where A : Enum
        where S : Enum
    {
        var tr3Lara = _control3.Read($"Resources/{TR3LevelNames.JUNGLE}").Models[TR3Type.Lara];
        foreach (var (tr3Idx, newIdx) in sprintAnimMap)
        {
            var anim = tr3Lara.Animations[(int)tr3Idx].Clone();
            var animIdx = Convert.ToInt16(newIdx);
            Debug.Assert(lara.Animations.Count == animIdx);
            lara.Animations.Add(anim);

            anim.Commands.RemoveAll(a => a is not TRSFXCommand);
            anim.Commands.OfType<TRSFXCommand>().Where(s => s.SoundID == 17)
                .ToList().ForEach(s => s.SoundID = WetFeetSFX);

            if (Enum.IsDefined(typeof(TR3LaraState), (int)anim.StateID))
            {
                anim.StateID = Convert.ToUInt16(sprintStateMap[(TR3LaraState)anim.StateID]);
            }
            if (Enum.IsDefined(typeof(TR3LaraAnim), (int)anim.NextAnimation))
            {
                anim.NextAnimation = Convert.ToUInt16(sprintAnimMap[(TR3LaraAnim)anim.NextAnimation]);
            }

            anim.Changes.RemoveAll(c => c.Dispatches.Any(d => !Enum.IsDefined(typeof(TR3LaraAnim), (int)d.NextAnimation)));
            foreach (var change in anim.Changes)
            {
                if (Enum.IsDefined(typeof(TR3LaraState), (int)change.StateID))
                {
                    var state = (TR3LaraState)change.StateID;
                    if (sprintStateMap.TryGetValue(state, out var s))
                        change.StateID = Convert.ToUInt16(s);
                    else if (crawlStateMap.TryGetValue(state, out var c))
                        change.StateID = Convert.ToUInt16(c);
                    else
                        throw new Exception($"Cannot find mapped state for {state}");
                }
                foreach (var dispatch in change.Dispatches)
                {
                    if (!Enum.IsDefined(typeof(TR3LaraAnim), (int)dispatch.NextAnimation))
                        continue;
                    var nextAnim = (TR3LaraAnim)dispatch.NextAnimation;
                    if (sprintAnimMap.TryGetValue(nextAnim, out var s))
                        dispatch.NextAnimation = Convert.ToInt16(s);
                    else if (crawlAnimMap.TryGetValue(nextAnim, out var c))
                        dispatch.NextAnimation = Convert.ToInt16(c);
                    else
                        throw new Exception($"Cannot find mapped next animation for {nextAnim}");
                }
            }
        }

        AddChange(lara, 0, sprintStateMap[TR3LaraState.Sprint], 0, 1, sprintAnimMap[TR3LaraAnim.RunToSprintLeft], 0);
        AddChange(lara, 0, sprintStateMap[TR3LaraState.Sprint], 11, 12, sprintAnimMap[TR3LaraAnim.RunToSprintRight], 0);
        AddChange(lara, slideToRunAnim, sprintStateMap[TR3LaraState.Sprint], 14, 14, sprintAnimMap[TR3LaraAnim.RunToSprintLeft], 0);
    }

    protected static void ImportIdlePose<S>(TRModel lara, S startState, S endState, S leftState, S rightState)
        where S : Enum
    {
        var laraExt = GetLaraExtModel();
        var states = new[] { rightState, leftState };
        for (int i = 0; i < states.Length; i++)
        {
            var poseAnims = laraExt.Animations.GetRange((int)ExtLaraAnim.PoseRightStart + i * 3, 3);
            poseAnims[0].StateID = Convert.ToUInt16(startState);
            poseAnims[1].StateID = (ushort)LaraState.Pose;
            poseAnims[2].StateID = Convert.ToUInt16(endState);

            lara.Animations.AddRange(poseAnims);
            poseAnims[0].NextAnimation = (ushort)(lara.Animations.Count - 2);
            poseAnims[1].NextAnimation = (ushort)(lara.Animations.Count - 2);
            poseAnims[2].NextAnimation = (ushort)LaraAnim.StandIdle;

            AddChange(lara, LaraAnim.StandIdle, states[i], 0, 69, lara.Animations.Count - 3, 0);
            AddChange(poseAnims[1], LaraState.Stop, 0, 42, lara.Animations.Count - 1, 0);
            AddChange(poseAnims[1], LaraState.Death, 0, 42, LaraAnim.StandDeath, 0);
            AddChange(poseAnims[1], LaraState.Roll, 0, 42, LaraAnim.RollStart, 0);
        }
    }

    protected static void FixJumpToFreefall(TRModel lara)
    {
        lara.Animations[(int)LaraAnim.JumpForward].Changes
            .Find(c => c.StateID == (ushort)LaraState.Freefall)
            .Dispatches.First().NextAnimation = (short)LaraAnim.JumpForwardEndToFreefall;
    }

    protected static void FixLadderClimbOnSFX(TRModel lara)
    {
        var anim = lara.Animations[(int)TR2LaraAnim.LadderClimbOn];
        var frames = new[] { 62, 77, 81 };
        anim.Commands.RemoveAll(c => c is TRSFXCommand s && frames.Contains(s.FrameNumber));
        anim.Commands.AddRange(frames.Select(f => new TRSFXCommand
        {
            FrameNumber = (short)f,
            SoundID = (short)TR1SFX.LaraFeet,
        }));
    }

    protected static void FixLadderUpSFX(TRModel lara)
    {
        var anim = lara.Animations[(int)TR2LaraAnim.LadderUp];
        anim.Commands.OfType<TRSFXCommand>().ToList()
            .ForEach(s => s.Environment = TRSFXEnvironment.Any);
    }

    protected static void FixHandstandSFX(TRModel lara)
    {
        var anim = lara.Animations[(int)LaraAnim.ClimbOnHandstand];
        var frames = new[] { 157, 188, 192 };
        anim.Commands.AddRange(frames.Select(f => new TRSFXCommand
        {
            FrameNumber = (short)f,
            SoundID = (short)TR1SFX.LaraFeet,
        }));
    }

    protected static void FixClimbOnSFX(TRModel lara)
    {
        var anim = lara.Animations[(int)LaraAnim.ClimbOn];
        anim.Commands.Add(new TRSFXCommand
        {
            FrameNumber = 36,
            SoundID = (short)TR3SFX.LaraKneesShuffle,
        });
    }

    protected void FixHangToCrouchStartSFX(TRModel lara)
    {
        var anim = lara.Animations[(int)TR3LaraAnim.HangToCrouchStart];
        anim.Commands.Add(new TRSFXCommand
        {
            SoundID = ClimbOnSFX,
            FrameNumber = 13,
        });
        anim.Commands.Add(new TRSFXCommand
        {
            SoundID = (short)TR1SFX.LaraFeet,
            FrameNumber = 56,
        });
    }

    protected void FixWadeTurnSFX(TRModel lara)
    {
        foreach (var animId in new[] { LaraAnim.TurnRightSlow, LaraAnim.TurnLeftSlow })
        {
            var anim = lara.Animations[(int)animId];
            if (!anim.Commands.Any(c => c is TRSFXCommand s && s.SoundID == TreadSFX))
            {
                anim.Commands.Add(new TRSFXCommand
                {
                    FrameNumber = 3,
                    SoundID = TreadSFX,
                    Environment = TRSFXEnvironment.Water,
                });
            }
        }

        foreach (var animId in new[] { LaraAnim.TurnRight, LaraAnim.TurnLeft })
        {
            var anim = lara.Animations[(int)animId];
            anim.Commands.RemoveAll(c => c is TRSFXCommand s
                && (s.SoundID == (short)TR1SFX.LaraSwim || s.SoundID == TreadSFX));

            anim.Commands.Add(new TRSFXCommand
            {
                FrameNumber = (short)(animId == LaraAnim.TurnRight ? 9 : 12),
                SoundID = (short)TR1SFX.LaraFloating,
                Environment = TRSFXEnvironment.Water,
            });

            anim.Commands.OfType<TRSFXCommand>()
                .Where(s => s.SoundID == 0)
                .ToList().ForEach(s => s.Environment = TRSFXEnvironment.Land);
        }
    }

    protected static void FixSprintSFX(TRModel lara, object runToLeftIdx, object runToRightIdx)
    {
        var cmdMap = new Dictionary<int, int>
        {
            [Convert.ToInt32(runToLeftIdx)] = 4,
            [Convert.ToInt32(runToRightIdx)] = 3,
        };
        foreach (var (animIdx, frameIdx) in cmdMap)
        {
            var anim = lara.Animations[animIdx];
            var cmds = anim.Commands.FindAll(
                c => (c is TRSFXCommand s && s.FrameNumber == frameIdx) || (c is TRFootprintCommand f && f.FrameNumber == frameIdx))
                .Select(c => c.Clone());
            foreach (var cmd in cmds)
            {
                if (cmd is TRSFXCommand s)
                {
                    s.FrameNumber = 23;
                }
                else if (cmd is TRFootprintCommand f)
                {
                    f.FrameNumber = 23;
                }
                anim.Commands.Add(cmd);
            }
        }
    }

    protected void ImportCrawling<A, S>(TRModel lara,
        Dictionary<TR3LaraAnim, A> animMap, Dictionary<TR3LaraState, S> stateMap)
        where A : Enum
        where S : Enum
    {
        var tr3Lara = _control3.Read($"Resources/{TR3LevelNames.JUNGLE}").Models[TR3Type.Lara];
        FixHangToCrouchStartSFX(tr3Lara);
        foreach (var (tr3Idx, newIdx) in animMap)
        {
            var anim = tr3Lara.Animations[(int)tr3Idx].Clone();
            var animIdx = Convert.ToInt16(newIdx);
            Debug.Assert(lara.Animations.Count == animIdx);
            lara.Animations.Add(anim);

            anim.Commands.RemoveAll(a => a is TRFootprintCommand);
            anim.Commands.OfType<TRSFXCommand>().Where(s => s.SoundID == 17)
                .ToList().ForEach(s => s.SoundID = WetFeetSFX);
            anim.Commands.OfType<TRSFXCommand>().Where(s => s.SoundID == 24)
                .ToList().ForEach(s => s.SoundID = KneesShuffleSFX);

            if (tr3Idx == TR3LaraAnim.CrawlDeath)
            {
                anim.Commands.OfType<TRSFXCommand>().Where(s => s.SoundID != (short)TR3SFX.LaraKneesDeath)
                    .ToList().ForEach(s => s.SoundID = KneesShuffleSFX);
            }

            if (Enum.IsDefined(typeof(TR3LaraState), (int)anim.StateID))
            {
                anim.StateID = Convert.ToUInt16(stateMap[(TR3LaraState)anim.StateID]);
            }
            if (Enum.IsDefined(typeof(TR3LaraAnim), (int)anim.NextAnimation))
            {
                anim.NextAnimation = Convert.ToUInt16(animMap[(TR3LaraAnim)anim.NextAnimation]);
            }

            foreach (var change in anim.Changes)
            {
                if (Enum.IsDefined(typeof(TR3LaraState), (int)change.StateID))
                {
                    change.StateID = Convert.ToUInt16(stateMap[(TR3LaraState)change.StateID]);
                }
                foreach (var dispatch in change.Dispatches)
                {
                    if (Enum.IsDefined(typeof(TR3LaraAnim), (int)dispatch.NextAnimation))
                    {
                        dispatch.NextAnimation = Convert.ToInt16(animMap[(TR3LaraAnim)dispatch.NextAnimation]);
                    }
                }
            }
        }

        FixCrouchRoll(lara, animMap[TR3LaraAnim.CrouchRollForwardEnd]);
        AddChange(lara, LaraAnim.Run, stateMap[TR3LaraState.CrouchIdle], 0, 4, animMap[TR3LaraAnim.RunToCrouchLeftStart], 0);
        AddChange(lara, LaraAnim.Run, stateMap[TR3LaraState.CrouchIdle], 10, 14, animMap[TR3LaraAnim.RunToCrouchRightStart], 0);
        AddChange(lara, LaraAnim.StandStill, stateMap[TR3LaraState.CrouchIdle], 0, 1, animMap[TR3LaraAnim.StandToCrouch], 0);
        AddChange(lara, LaraAnim.ReachToHang, stateMap[TR3LaraState.ClimbToCrawl], 12, 22, animMap[TR3LaraAnim.HangToCrouchStart], 0);
        AddChange(lara, LaraAnim.StandIdle, stateMap[TR3LaraState.CrouchIdle], 0, 44, animMap[TR3LaraAnim.StandToCrouch], 0);
    }

    protected static void FixCrouchRoll<A>(TRModel lara, A crouchRollEndAnim)
    {
        // Manually adjusted to stop Lara's right arm raising to her knee then immediately going
        // to the floor on the next animation.
        var badAnim = lara.Animations[Convert.ToInt32(crouchRollEndAnim)];
        var goodAnim = GetLaraExtModel().Animations[Convert.ToInt32(ExtLaraAnim.CrouchRollEnd)];
        Debug.Assert(badAnim.Frames.Count == goodAnim.Frames.Count);
        for (int i = 0; i < badAnim.Frames.Count; i++)
        {
            for (int bone = 8; bone < 11; bone++)
            {
                badAnim.Frames[i].Rotations[bone] = goodAnim.Frames[i].Rotations[bone];
            }
        }
    }

    protected void ImportKneesShuffle(InjectionData data)
    {
        var jungle = _control3.Read($"Resources/{TR3LevelNames.JUNGLE}");
        var shuffle = jungle.SoundEffects[TR3SFX.LaraKneesShuffle];
        shuffle.Range = 10;
        shuffle.Volume *= 2;
        if (data.GameVersion == TRGameVersion.TR1)
        {
            shuffle.Mode = (TR3SFXMode)TR1SFXMode.Restart;
        }
        data.SFX.Add(TRSFXData.Create(KneesShuffleSFX, shuffle));
    }

    protected void ImportPoleLoopSFX(InjectionData data)
    {
        var level = _control4.Read($"Resources/TR4/{TR4LevelNames.LAKE}");
        var loop = level.SoundEffects[TR4SFX.LaraPoleLoop];
        if (data.GameVersion == TRGameVersion.TR1)
        {
            loop.Mode = (TR3SFXMode)TR1SFXMode.Wait;
        }
        data.SFX.Add(TRSFXData.Create(PoleLoopSFX, loop));
    }

    protected static void AddChange
        (TRModel lara, object animIdx, object goalStateID, short low, short high, object nextAnimIdx, short nextFrame)
        => AddChange(lara.Animations[Convert.ToInt32(animIdx)], goalStateID, low, high, nextAnimIdx, nextFrame);

    protected static void AddChange
        (TRAnimation anim, object goalStateID, short low, short high, object nextAnimIdx, short nextFrame)
    {
        anim.Changes.Add(new()
        {
            StateID = Convert.ToUInt16(goalStateID),
            Dispatches =
            [
                new()
                {
                    Low = low,
                    High = high,
                    NextAnimation = Convert.ToInt16(nextAnimIdx),
                    NextFrame = nextFrame,
                }
            ],
        });
    }

    protected static TRAnimation GetBreathAnim(TRModel lara)
    {
        var breathAnim = lara.Animations[(int)LaraAnim.StandStill].Clone();
        breathAnim.Changes.Clear();
        breathAnim.StateID = 0;
        breathAnim.NextAnimation = 0;
        return breathAnim;
    }

    protected static void ImportExtraAnims<T>(TRDictionary<T, TRModel> models, T extraType)
        where T : Enum
    {
        var hips = models[default].Meshes[0];
        models[extraType] = new()
        {
            Meshes = [.. Enumerable.Repeat(0, 15).Select(m => hips)],
            MeshTrees = [.. models[default].MeshTrees.Select(t => t.Clone())],
            Animations = [GetBreathAnim(models[default])],
        };

        var model = models[extraType];
        var breath = model.Animations[0];

        foreach (var data in _extraData)
        {
            TRModel srcModel;
            if (TR1LevelNames.AsList.Contains(data.LevelName))
            {
                srcModel = _control1.Read($"Resources/{data.LevelName}").Models[TR1Type.LaraMiscAnim_H];
            }
            else if (TR2LevelNames.AsList.Contains(data.LevelName))
            {
                srcModel = _control2.Read($"Resources/{data.LevelName}").Models[TR2Type.LaraMiscAnim_H];
            }
            else if (data.State == LaraExtraState.JailWakeUp)
            {
                // Restored from PSX, original in retail HSC is a dupe of Rig start anim
                srcModel = GetLaraExtModel();
            }
            else
            {
                srcModel = _control3.Read($"Resources/TR3/{data.LevelName}").Models
                    [data.State == LaraExtraState.RapidsDrown ? TR3Type.LaraVehicleAnimation_H : TR3Type.LaraExtraAnimation_H];
            }

            var anim = srcModel.Animations[data.AnimIdx];
            anim.Commands.RemoveAll(c => c is TRFXCommand f && f.EffectID == (short)TR1FX.LaraNormal);
            anim.StateID = (ushort)data.State;
            anim.NextAnimation = (ushort)(_extraDeathStates.Contains(data.State) ? data.State : 0);
            model.Animations.Add(anim);
            breath.Changes.Add(new()
            {
                StateID = (ushort)data.State,
                Dispatches = [new()
                {
                    High = 1,
                    NextAnimation = (short)data.State,
                }],
            });

            if (data.State == LaraExtraState.EndHouse)
            {
                for (int i = 0; i < 120; i++)
                {
                    anim.Frames.Add(anim.Frames[^1]);
                    anim.FrameEnd++;
                }
            }
            else if (data.State == LaraExtraState.JailWakeUp)
            {
                // Lara needs to rotate 90deg so she is properly aligned on the bed. At the end
                // of the animation rotate another 180deg to face away from the bed when the
                // player regains control.
                anim.Commands.Add(new TRFXCommand
                {
                    EffectID = 62, // Turn 90deg
                    FrameNumber = 0,
                });
                anim.Commands.Add(new TRFXCommand
                {
                    EffectID = 0, // Turn 180deg
                    FrameNumber = 900,
                });
            }
        }
    }

    protected static void FixVaulting(TRModel lara)
    {
        {
            var anim = lara.Animations[(int)LaraAnim.Climb3Click];
            anim.StateID = (ushort)LaraState.PullUp;
            anim.Commands.OfType<TRSetPositionCommand>().First().Z = 0;
            anim.NextAnimation = (ushort)LaraAnim.ClimbOnEnd;
        }

        {
            var anim = lara.Animations[(int)LaraAnim.Climb2Click];
            anim.StateID = (ushort)LaraState.PullUp;
        }

        {
            var anim = lara.Animations[(int)LaraAnim.Climb2ClickEnd];
            anim.StateID = (ushort)LaraState.PullUp;
            anim.Commands.Add(new TRSetPositionCommand
            {
                Z = 72,
            });
        }
    }

    protected static void ImportResponsiveReach<A>(TRModel lara, Dictionary<TR3LaraAnim, A> animMap)
        where A : Enum
    {
        var tr3Lara = _control3.Read($"Resources/{TR3LevelNames.JUNGLE}").Models[TR3Type.Lara];
        foreach (var (tr3Idx, newIdx) in animMap)
        {
            var anim = tr3Lara.Animations[(int)tr3Idx].Clone();
            var animIdx = Convert.ToInt16(newIdx);
            Debug.Assert(lara.Animations.Count == animIdx);
            lara.Animations.Add(anim);
        }

        AlignJumpToReach(lara,
            animMap[TR3LaraAnim.JumpForwardStartToGrabEarly], animMap[TR3LaraAnim.JumpForwardStartToGrabLate],
            animMap[TR3LaraAnim.RunToGrabLeft], animMap[TR3LaraAnim.RunToGrabRight]);
    }

    protected static void AlignJumpToReach<A>(TRModel lara,
        A jumpStartToGrabEarly, A jumpStartToGrabLate,
        A runToGrabLeftAnim, A runToGrabRightAnim)
        where A : Enum
    {
        // Import TR3 early jump-grab state changes. This also restores the abilty to grab
        // cancel in TR3 (JumpForwardToReachLate) - broken state change in OG.

        {
            var anim = lara.Animations[(int)LaraAnim.JumpForward];
            anim.Changes.RemoveAll(c => c.StateID == (ushort)LaraState.Reach);
            AddChange(anim, LaraState.Reach, 5, 6, jumpStartToGrabEarly, 0);
            AddChange(anim, LaraState.Reach, 9, 10, jumpStartToGrabLate, 0);
            AddChange(anim, LaraState.Reach, 17, 28, LaraAnim.JumpForwardToReach, 0);
            AddChange(anim, LaraState.Reach, 28, 40, LaraAnim.JumpForwardToReachLate, 0);
            SortChanges(anim);
        }

        {
            var anim = lara.Animations[(int)LaraAnim.RunJumpRightContinue];
            anim.Changes.RemoveAll(c => c.StateID == (ushort)LaraState.Reach);
            AddChange(anim, LaraState.Reach, 5, 6, runToGrabLeftAnim, 0); // Left is correct
            SortChanges(anim);
        }

        {
            var anim = lara.Animations[(int)LaraAnim.RunJumpLeftContinue];
            anim.Changes.RemoveAll(c => c.StateID == (ushort)LaraState.Reach);
            AddChange(anim, LaraState.Reach, 5, 6, runToGrabRightAnim, 0); // Right is correct
            SortChanges(anim);
        }
    }

    private static void SortChanges(TRAnimation anim)
        => anim.Changes.Sort((c1, c2) => c1.StateID.CompareTo(c2.StateID));

    private static TRAnimation ImportSwingIn<A>(TRModel destLara, TRModel srcLara, A swingInAnim)
        where A : Enum
    {
        var anim = srcLara.Animations[(int)TR3LaraAnim.SwingInSlow].Clone();
        var animIdx = Convert.ToInt16(swingInAnim);
        Debug.Assert(destLara.Animations.Count == animIdx);
        destLara.Animations.Add(anim);
        return anim;
    }

    protected static void ImportSwingInFast<A>(TRModel lara, A swingInAnim)
        where A : Enum
    {
        var tr2Lara = _control2.Read($"Resources/{TR2LevelNames.GW}").Models[TR2Type.Lara];
        ImportSwingIn(lara, tr2Lara, swingInAnim);
    }

    protected static void ImportSwingInSlow<A, S>(TRModel lara, A swingInAnim,
        A monkeyIdleAnim, A monkeyFallAnim, S monkeyIdleState,
        A climbToCrawlAnim, S climbToCrawlState)
        where A : Enum
        where S : Enum
    {
        var tr3Lara = _control3.Read($"Resources/{TR3LevelNames.JUNGLE}").Models[TR3Type.Lara];
        {
            var anim = ImportSwingIn(lara, tr3Lara, swingInAnim);
            anim.StateID = Convert.ToUInt16(monkeyIdleState);
            anim.NextAnimation = Convert.ToUInt16(monkeyIdleAnim);
            
            anim.Changes.RemoveAll(c => c.StateID != (ushort)LaraState.PullUp && c.StateID != (ushort)LaraState.Handstand);
            AddChange(anim, climbToCrawlState, 54, 60, climbToCrawlAnim, 0);
            AddChange(anim, climbToCrawlState, 78, 84, climbToCrawlAnim, 0);
            AddChange(anim, climbToCrawlState, 54, 60, climbToCrawlAnim, 0);
            SortChanges(anim);
        }

        {
            var anim = tr3Lara.Animations[(int)TR3LaraAnim.MonkeyIdle].Clone();
            var animIdx = Convert.ToUInt16(monkeyIdleAnim);
            Debug.Assert(lara.Animations.Count == animIdx);
            lara.Animations.Add(anim);
            anim.StateID = Convert.ToUInt16(monkeyIdleState);
            anim.NextAnimation = animIdx;

            var defaultChanges = new[] { LaraState.PullUp, LaraState.ShimmyLeft, LaraState.ShimmyRight, LaraState.Handstand };
            anim.Changes.RemoveAll(c => !defaultChanges.Contains((LaraState)c.StateID));
            AddChange(anim, LaraState.Land, 0, 48, monkeyFallAnim, 0);
            AddChange(anim, climbToCrawlState, 0, 48, climbToCrawlAnim, 0);
            SortChanges(anim);
        }

        {
            var anim = tr3Lara.Animations[(int)TR3LaraAnim.MonkeyFall].Clone();
            var animIdx = Convert.ToUInt16(monkeyFallAnim);
            Debug.Assert(lara.Animations.Count == animIdx);
            lara.Animations.Add(anim);
            anim.StateID = Convert.ToUInt16(monkeyIdleState);
        }
    }

    protected void ImportMonkeySwing<A, S>(TRModel lara,
        Dictionary<TR3LaraAnim, A> animMap, Dictionary<TR3LaraState, S> stateMap)
        where A : Enum
        where S : Enum
    {
        var tr3Lara = _control3.Read($"Resources/{TR3LevelNames.JUNGLE}").Models[TR3Type.Lara];
        var skipAnims = new[] { TR3LaraAnim.SwingInSlow, TR3LaraAnim.MonkeyIdle, TR3LaraAnim.MonkeyFall };

        foreach (var (tr3Idx, newIdx) in animMap)
        {
            if (skipAnims.Contains(tr3Idx))
            {
                // Handled in ImportSwingInSlow, but mapping needed for other anim changes
                continue;
            }

            var anim = tr3Lara.Animations[(int)tr3Idx].Clone();
            var animIdx = Convert.ToInt16(newIdx);
            Debug.Assert(lara.Animations.Count == animIdx);
            lara.Animations.Add(anim);

            anim.Commands.RemoveAll(a => a is TRFootprintCommand);
            anim.Commands.OfType<TRSFXCommand>().Where(s => s.SoundID == 17)
                .ToList().ForEach(s => s.SoundID = WetFeetSFX);
            anim.Commands.OfType<TRSFXCommand>().Where(s => s.SoundID == 24)
                .ToList().ForEach(s => s.SoundID = KneesShuffleSFX);

            if (Enum.IsDefined(typeof(TR3LaraState), (int)anim.StateID))
            {
                anim.StateID = Convert.ToUInt16(stateMap[(TR3LaraState)anim.StateID]);
            }
            if (Enum.IsDefined(typeof(TR3LaraAnim), (int)anim.NextAnimation))
            {
                anim.NextAnimation = Convert.ToUInt16(animMap[(TR3LaraAnim)anim.NextAnimation]);
            }

            foreach (var change in anim.Changes)
            {
                if (Enum.IsDefined(typeof(TR3LaraState), (int)change.StateID))
                {
                    change.StateID = Convert.ToUInt16(stateMap[(TR3LaraState)change.StateID]);
                }
                foreach (var dispatch in change.Dispatches)
                {
                    if (Enum.IsDefined(typeof(TR3LaraAnim), (int)dispatch.NextAnimation))
                    {
                        dispatch.NextAnimation = Convert.ToInt16(animMap[(TR3LaraAnim)dispatch.NextAnimation]);
                    }
                }
            }
        }

        AddChange(lara, LaraAnim.JumpUp, stateMap[TR3LaraState.MonkeyIdle], 0, 11, animMap[TR3LaraAnim.MonkeyGrab], 0);
        AddChange(lara, LaraAnim.Reach, stateMap[TR3LaraState.MonkeyIdle], 0, 1, animMap[TR3LaraAnim.SwingInSlow], 0);

        // Other changes not handled in ImportSwingInSlow
        foreach (var idx in new[] { TR3LaraAnim.SwingInSlow, TR3LaraAnim.MonkeyIdle })
        {
            var swingInSrc = tr3Lara.Animations[(int)idx].Clone();
            var swingInDst = lara.Animations[Convert.ToInt32(animMap[idx])];
            foreach (var change in swingInSrc.Changes)
            {
                if (!stateMap.TryGetValue((TR3LaraState)change.StateID, out var stateID))
                    continue;

                swingInDst.Changes.Add(change);
                change.StateID = Convert.ToUInt16(stateID);
                foreach (var dispatch in change.Dispatches)
                {
                    dispatch.NextAnimation = Convert.ToInt16(animMap[(TR3LaraAnim)dispatch.NextAnimation]);
                }
            }
            SortChanges(swingInDst);
        }
    }

    protected static void SyncToTR3<A, S>(TRModel lara,
        Dictionary<int, A> animMap, Dictionary<int, S> stateMap,
        A sprintRollLeftToRun)
        where A : Enum
        where S : Enum
    {
        var tr3Lara = _control3.Read($"Resources/{TR3LevelNames.JUNGLE}").Models[TR3Type.Lara];
        foreach (var (tr3Idx, newIdx) in animMap)
        {
            var anim = tr3Lara.Animations[tr3Idx].Clone();
            var animIdx = Convert.ToInt16(newIdx);
            Debug.Assert(lara.Animations.Count == animIdx);
            lara.Animations.Add(anim);

            if (stateMap.TryGetValue(anim.StateID, out var state))
            {
                anim.StateID = Convert.ToUInt16(state);
            }
            if (animMap.TryGetValue(anim.NextAnimation, out var nextAnim))
            {
                anim.NextAnimation = Convert.ToUInt16(nextAnim);
            }
            else if (anim.NextAnimation == 232)
            {
                anim.NextAnimation = Convert.ToUInt16(sprintRollLeftToRun);
            }
        }
    }

    protected void AddMinimumJumpDelay(TRModel lara)
    {
        var anim = lara.Animations[(int)LaraAnim.RunStart];
        AddChange(anim, ResponsiveState, 13, 13, LaraAnim.RunJumpRightStart, 1);
    }

    protected void ImportLadderToCrouch<A, B, C, S>(TRModel lara, A idleLadderAnim, B crouchIdleAnim, S climbToCrawlState,
        C ladderToCrouchStartAnim, C ladderToCrouchEndAnim)
        where A : Enum
        where B : Enum
        where C : Enum
        where S : Enum
    {
        var laraExt = GetLaraExtModel();

        {
            var anim = laraExt.Animations[(int)ExtLaraAnim.LadderToCrouchStart].Clone();
            var animIdx = Convert.ToInt16(ladderToCrouchStartAnim);
            Debug.Assert(lara.Animations.Count == animIdx);
            anim.NextAnimation = Convert.ToUInt16(ladderToCrouchEndAnim);
            anim.StateID = Convert.ToUInt16(climbToCrawlState);
            lara.Animations.Add(anim);

            anim.Commands.RemoveAll(c => c is TREmptyHandsCommand);

            anim.Commands.Add(new TRSFXCommand
            {
                SoundID = 5,
                FrameNumber = 18,
            });
            anim.Commands.Add(new TRSFXCommand
            {
                SoundID = KneesShuffleSFX,
                FrameNumber = 32,
            });
            anim.Commands.Add(new TRSFXCommand
            {
                SoundID = KneesShuffleSFX,
                FrameNumber = 44,
            });
        }

        {
            var anim = laraExt.Animations[(int)ExtLaraAnim.LadderToCrouchEnd].Clone();
            var animIdx = Convert.ToInt16(ladderToCrouchEndAnim);
            Debug.Assert(lara.Animations.Count == animIdx);
            Debug.Assert(anim.Commands.Any(c => c is TREmptyHandsCommand));
            anim.NextAnimation = Convert.ToUInt16(crouchIdleAnim);
            anim.StateID = Convert.ToUInt16(climbToCrawlState);
            lara.Animations.Add(anim);
        }

        AddChange(lara, idleLadderAnim, climbToCrawlState, 0, 48, ladderToCrouchStartAnim, 0);
    }

    protected static void ImportCornerShimmy<A, S>(TRModel lara,
        Dictionary<TR4LaraAnim, A> animMap, Dictionary<TR4LaraState, S> stateMap,
        object ladderIdleAnim)
        where A : Enum
        where S : Enum
    {
        var tr4Lara = GetCompleteTR4Lara();

        short RemapAnim(int tr4Idx)
        {
            if (Enum.IsDefined(typeof(TR4LaraAnim), tr4Idx)
                && animMap.TryGetValue((TR4LaraAnim)tr4Idx, out var mapped))
            {
                return Convert.ToInt16(mapped);
            }
            if (tr4Idx == (int)LaraAnim.ReachToHang)
            {
                return (short)LaraAnim.ReachToHang;
            }
            Debug.Assert(tr4Idx == (int)TR2LaraAnim.LadderIdle);
            return Convert.ToInt16(ladderIdleAnim);
        }

        ushort RemapState(int tr4State)
        {
            if (Enum.IsDefined(typeof(TR4LaraState), tr4State)
                && stateMap.TryGetValue((TR4LaraState)tr4State, out var mapped))
            {
                return Convert.ToUInt16(mapped);
            }
            return (ushort)tr4State;
        }

        foreach (var (tr4Idx, newIdx) in animMap)
        {
            var anim = tr4Lara.Animations[(int)tr4Idx].Clone();
            var animIdx = Convert.ToInt16(newIdx);
            Debug.Assert(lara.Animations.Count == animIdx);
            lara.Animations.Add(anim);

            anim.Commands.RemoveAll(c => c is TRFootprintCommand);
            anim.StateID = RemapState(anim.StateID);
            anim.NextAnimation = (ushort)RemapAnim(anim.NextAnimation);

            foreach (var change in anim.Changes)
            {
                change.StateID = RemapState(change.StateID);
                foreach (var dispatch in change.Dispatches)
                {
                    dispatch.NextAnimation = RemapAnim(dispatch.NextAnimation);
                }
            }
        }

        // The engine initiates hang corner turns by setting the corner goal
        // states while on the hang-idle animation, so import TR4's state
        // changes for those states onto the target's animation. Ladder
        // corner turns need no changes as the engine switches to their
        // animations directly.
        var grabLedge = tr4Lara.Animations[(int)LaraAnim.ReachToHang].Clone();
        var grabLedgeDst = lara.Animations[(int)LaraAnim.ReachToHang];
        foreach (var change in grabLedge.Changes)
        {
            if (!Enum.IsDefined(typeof(TR4LaraState), (int)change.StateID)
                || !stateMap.ContainsKey((TR4LaraState)change.StateID))
            {
                continue;
            }

            grabLedgeDst.Changes.Add(change);
            change.StateID = RemapState(change.StateID);
            foreach (var dispatch in change.Dispatches)
            {
                dispatch.NextAnimation = RemapAnim(dispatch.NextAnimation);
            }
        }
        SortChanges(grabLedgeDst);
    }

    protected static void ImportFastPickup(TRModel lara)
    {
        var laraExt = GetLaraExtModel();
        var anim = laraExt.Animations[(int)ExtLaraAnim.FastPickup].Clone();
        var animIdx = lara.Animations.Count;
        lara.Animations.Add(anim);
        anim.NextAnimation = Convert.ToUInt16(LaraAnim.StandStill);
        anim.StateID = Convert.ToUInt16(LaraState.Pickup);

        // Retain "aha" if present
        var cmd = lara.Animations[(int)LaraAnim.Pickup].Commands.FirstOrDefault(c => c is TRSFXCommand);
        if (cmd != null)
        {
            var sfxCmd = cmd.Clone() as TRSFXCommand;
            sfxCmd.FrameNumber = 10;
            anim.Commands.Add(sfxCmd);
        }

        foreach (var id in new[] { LaraAnim.StandStill, LaraAnim.StandIdle })
        {
            var standAnim = lara.Animations[Convert.ToInt32(id)];
            var change = standAnim.Changes.First(c => c.StateID == Convert.ToInt32(LaraState.Pickup)).Clone();
            change.StateID = Convert.ToUInt16(LaraState.FastPickup);
            change.Dispatches.ForEach(d => d.NextAnimation = (short)animIdx);
            standAnim.Changes.Add(change);
        }
    }

    protected void ImportFastPushPull(TRModel lara, bool enableFootprints = false)
    {
        var animMap = new[] {
            ExtLaraAnim.FastPushblockPull, ExtLaraAnim.FastPushblockPush,
            ExtLaraAnim.FastPushblockPullStop, ExtLaraAnim.FastPushblockPushStop }
        .ToDictionary(a => a, _ => 0);

        var laraExt = GetLaraExtModel();
        foreach (var animId in animMap.Keys.ToList())
        {
            var anim = laraExt.Animations[(int)animId].Clone();
            animMap[animId] = lara.Animations.Count;
            lara.Animations.Add(anim);

            anim.Commands.OfType<TRSFXCommand>().Where(f => f.SoundID == 17)
                .ToList().ForEach(f => f.SoundID = WetFeetSFX);
            if (!enableFootprints)
            {
                anim.Commands.RemoveAll(c => c is TRFXCommand);
            }
        }

        {
            var anim = lara.Animations[animMap[ExtLaraAnim.FastPushblockPull]];
            anim.StateID = (ushort)LaraState.PushblockPull;
            anim.NextAnimation = (ushort)animMap[ExtLaraAnim.FastPushblockPull];
            anim.NextFrame = 35;
            AddChange(anim, LaraState.Stop, 183, 184, animMap[ExtLaraAnim.FastPushblockPullStop], 0);
            AddChange(lara, LaraAnim.PushableGrab, LaraState.FastPushblockPull,
                19, 20, animMap[ExtLaraAnim.FastPushblockPull], 0);

            var cmd = anim.Commands.OfType<TRSFXCommand>().First(s => s.SoundID == (short)TR1SFX.LaraBlockPush2);
            cmd.SoundID = (short)TR1SFX.LaraBlockPull;
            cmd.FrameNumber = 36;

            anim.Commands.AddRange(new[] { 44, 87, 136, 152 }.Select(f => new TRSFXCommand
            {
                FrameNumber = (short)f,
                SoundID = (short)TR1SFX.BlockSound,
            }));
        }

        {
            var anim = lara.Animations[animMap[ExtLaraAnim.FastPushblockPush]];
            anim.StateID = (ushort)LaraState.PushblockPush;
            anim.NextAnimation = (ushort)animMap[ExtLaraAnim.FastPushblockPush];
            anim.NextFrame = 30;
            AddChange(anim, LaraState.Stop, 169, 170, animMap[ExtLaraAnim.FastPushblockPushStop], 0);
            AddChange(lara, LaraAnim.PushableGrab, LaraState.FastPushblockPush,
                19, 20, animMap[ExtLaraAnim.FastPushblockPush], 0);

            var cmd = anim.Commands.OfType<TRSFXCommand>().First(s => s.SoundID == (short)TR1SFX.LaraBlockPush2);
            cmd.SoundID = (short)TR1SFX.LaraBlockPush1;
            cmd.FrameNumber = 30;

            anim.Commands.AddRange(new[] { 38, 92, 136 }.Select(f => new TRSFXCommand
            {
                FrameNumber = (short)f,
                SoundID = (short)TR1SFX.BlockSound,
            }));
        }

        foreach (var animId in new[] { ExtLaraAnim.FastPushblockPullStop, ExtLaraAnim.FastPushblockPushStop })
        {
            var anim = lara.Animations[animMap[animId]];
            anim.Commands.Add(new TREmptyHandsCommand());
            anim.StateID = (ushort)(animId == ExtLaraAnim.FastPushblockPullStop 
                ? LaraState.PushblockPull : LaraState.PushblockPush);
            anim.NextAnimation = (ushort)LaraAnim.StandStill;
            anim.NextFrame = 0;
        }
    }

    protected void ImportPlinthPickups(TRModel lara, bool enableFootprints = false)
    {
        var map = new Dictionary<ExtLaraAnim, LaraState>
        {
            [ExtLaraAnim.PlinthLowPickup] = LaraState.PlinthLowPickup,
            [ExtLaraAnim.PlinthHighPickup] = LaraState.PlinthHighPickup,
        };

        var laraExt = GetLaraExtModel();
        foreach (var (animType, animState) in map)
        {
            var anim = laraExt.Animations[(int)animType].Clone();
            var animIdx = lara.Animations.Count;
            lara.Animations.Add(anim);

            anim.StateID = (ushort)LaraState.Pickup;
            anim.Commands.OfType<TRSFXCommand>().Where(f => f.SoundID == 17)
                .ToList().ForEach(f => f.SoundID = WetFeetSFX);
            if (!enableFootprints)
            {
                anim.Commands.RemoveAll(c => c is TRFXCommand);
            }

            anim.NextAnimation = (ushort)LaraAnim.StandStill;
            anim.NextFrame = 0;

            foreach (var id in new[] { LaraAnim.StandStill, LaraAnim.StandIdle })
            {
                var standAnim = lara.Animations[Convert.ToInt32(id)];
                var change = standAnim.Changes.First(c => c.StateID == Convert.ToInt32(LaraState.Pickup)).Clone();
                change.StateID = (ushort)animState;
                change.Dispatches.ForEach(d => d.NextAnimation = (short)animIdx);
                standAnim.Changes.Add(change);
            }
        }
    }

    protected static void SplitPushableEnds(TRModel lara)
    {
        var level = _control1.Read($"Resources/{TR1LevelNames.VILCABAMBA}");
        var block = level.Models[TR1Type.PushBlock1];
        var map = new Dictionary<LaraAnim, int>
        {
            [LaraAnim.PushablePull] = block.Animations[2].FrameEnd,
            [LaraAnim.PushablePush] = block.Animations[1].FrameEnd,
        };

        foreach (var (animId, blockFrameEnd) in map)
        {
            var baseAnim = lara.Animations[(int)animId];
            var endAnim = baseAnim.Clone();
            baseAnim.NextAnimation = (ushort)lara.Animations.Count;
            lara.Animations.Add(endAnim);

            var keyFrameEnd = blockFrameEnd / baseAnim.FrameRate;
            baseAnim.FrameEnd = (short)(keyFrameEnd * baseAnim.FrameRate);

            endAnim.Frames.RemoveRange(0, keyFrameEnd);
            endAnim.Commands.Clear();
            endAnim.FrameEnd -= baseAnim.FrameEnd;

            var shift = baseAnim.Commands
                .OfType<TRSetPositionCommand>()
                .First();
            foreach (var frame in endAnim.Frames)
            {
                frame.OffsetZ -= shift.Z;
                frame.Bounds.MinZ -= shift.Z;
                frame.Bounds.MaxZ -= shift.Z;
            }            

            baseAnim.Commands.RemoveAll(c => c is TREmptyHandsCommand);
            endAnim.Commands.Add(new TREmptyHandsCommand());
        }
    }

    protected static void FixPoleReleaseState(TRModel lara)
    {
        var anim = lara.Animations[(int)TR4LaraAnim.PoleToStand];
        anim.StateID = (ushort)TR4LaraState.PoleIdle;
        anim.Commands.Add(new TREmptyHandsCommand());

        anim = lara.Animations[(int)TR4LaraAnim.PoleFall];
        anim.Commands.Add(new TREmptyHandsCommand());
    }

    protected static void FixInteractiveDoorHandsFree(TRModel lara)
    {
        // The interactive door open animations leave Lara's hands busy on
        // completion, so she never returns to an armless state (stuck unable to
        // draw weapons and, for the knob doors, jittering against the door as
        // the interaction never fully releases). The stock crowbar door
        // (PryDoor) already carries the empty-hands command; give the rest the
        // same treatment.
        TR4LaraAnim[] doorAnims =
        [
            TR4LaraAnim.DoorOpenForward,
            TR4LaraAnim.DoorOpenBack,
            TR4LaraAnim.DoorKick,
            TR4LaraAnim.FloorTrapdoorOpen,
            TR4LaraAnim.CeilingTrapdoorOpen,
            TR4LaraAnim.DoubledoorsPush,
            TR4LaraAnim.UnderwaterDoorOpen,
        ];

        foreach (var animId in doorAnims)
        {
            var anim = lara.Animations[(int)animId];
            if (!anim.Commands.Any(c => c is TREmptyHandsCommand))
            {
                anim.Commands.Add(new TREmptyHandsCommand());
            }
        }
    }

    protected void SyncToTR4(TRModel lara, bool enableFootprints = false)
    {
        // TR1-3 at this stage are aligned so we can use a common id mapping instead of defining
        // maps elsewhere.
        var tr4Lara = GetCompleteTR4Lara();
        foreach (var (tr4Idx, newIdx) in _tr4AnimSyncMap)
        {
            var anim = tr4Lara.Animations[(int)tr4Idx].Clone();
            var animIdx = Convert.ToInt16(newIdx);
            Debug.Assert(lara.Animations.Count == animIdx);
            lara.Animations.Add(anim);

            anim.Commands.OfType<TRSFXCommand>().Where(f => f.SoundID == (short)TR4SFX.LaraWetFeet)
                .ToList().ForEach(f => f.SoundID = WetFeetSFX);
            anim.Commands.OfType<TRSFXCommand>().Where(f => f.SoundID == (short)TR4SFX.LaraPoleClimb)
                .ToList().ForEach(f => f.SoundID = KneesShuffleSFX);
            if (!enableFootprints)
            {
                anim.Commands.RemoveAll(c => c is TRFootprintCommand);
            }

            if (Enum.IsDefined(typeof(TR4LaraState), (int)anim.StateID))
            {
                anim.StateID = Convert.ToUInt16(_tr4StateSyncMap[(TR4LaraState)anim.StateID]);
            }
            if (Enum.IsDefined(typeof(TR4LaraAnim), (int)anim.NextAnimation))
            {
                anim.NextAnimation = Convert.ToUInt16(_tr4AnimSyncMap[(TR4LaraAnim)anim.NextAnimation]);
            }

            foreach (var change in anim.Changes)
            {
                if (Enum.IsDefined(typeof(TR4LaraState), (int)change.StateID))
                {
                    var state = (TR4LaraState)change.StateID;
                    if (_tr4StateSyncMap.TryGetValue(state, out var s))
                        change.StateID = Convert.ToUInt16(s);
                    else
                        throw new Exception($"Cannot find mapped state for {state}");
                }
                foreach (var dispatch in change.Dispatches)
                {
                    if (!Enum.IsDefined(typeof(TR4LaraAnim), (int)dispatch.NextAnimation))
                        continue;
                    var nextAnim = (TR4LaraAnim)dispatch.NextAnimation;
                    if (_tr4AnimSyncMap.TryGetValue(nextAnim, out var s))
                        dispatch.NextAnimation = Convert.ToInt16(s);
                    else
                        throw new Exception($"Cannot find mapped next animation for {nextAnim}");
                }
            }            
        }

        for (int i = 0; i < tr4Lara.Animations.Count; i++)
        {
            if (_tr4AnimSyncMap.ContainsKey((TR4LaraAnim)i))
                continue;

            var anim = tr4Lara.Animations[i];
            foreach (var change in anim.Changes)
            {
                foreach (var dispatch in change.Dispatches)
                {
                    if (!_tr4AnimSyncMap.TryGetValue((TR4LaraAnim)dispatch.NextAnimation, out var nextAnim))
                    {
                        continue;
                    }

                    Debug.Assert(i < _tr1AnimCount); // If anything above this has a state change, we would need a mapping per game.
                    var baseAnim = lara.Animations[i];
                    var goal = _tr4StateSyncMap[(TR4LaraState)change.StateID];
                    AddChange(baseAnim, goal, dispatch.Low, dispatch.High, nextAnim, dispatch.NextFrame);
                }
            }
        }
    }

    private static readonly Dictionary<TR4LaraAnim, int> _tr4AnimSyncMap = new()
    {
        [TR4LaraAnim.DoorOpenForward] = 358,
        [TR4LaraAnim.DoorOpenBack] = 359,
        [TR4LaraAnim.DoorKick] = 360,
        [TR4LaraAnim.GiantButtonPush] = 361,
        [TR4LaraAnim.FloorTrapdoorOpen] = 362,
        [TR4LaraAnim.CeilingTrapdoorOpen] = 363,
        [TR4LaraAnim.TurnswitchGrabLeft] = 364,
        [TR4LaraAnim.TurnswitchGrabRight] = 365,
        [TR4LaraAnim.CogwheelPull] = 366,
        [TR4LaraAnim.CogwheelGrab] = 367,
        [TR4LaraAnim.CogwheelUngrab] = 368,
        [TR4LaraAnim.LeverswitchPush] = 369,
        [TR4LaraAnim.HoleGrab] = 370,
        [TR4LaraAnim.StandToPoleGrab] = 371,
        [TR4LaraAnim.PoleJump] = 372,
        [TR4LaraAnim.PoleIdle] = 373,
        [TR4LaraAnim.PoleClimbUp] = 374,
        [TR4LaraAnim.PoleFall] = 375,
        [TR4LaraAnim.ReachToPoleGrab] = 376,
        [TR4LaraAnim.PoleTurnLeftStart] = 377,
        [TR4LaraAnim.PoleTurnRightStart] = 378,
        [TR4LaraAnim.PoleIdleToClimbDown] = 379,
        [TR4LaraAnim.PoleClimbDown] = 380,
        [TR4LaraAnim.PoleClimbDownToIdle] = 381,
        [TR4LaraAnim.JumpUpToPoleGrab] = 382,
        [TR4LaraAnim.PoleClimbUpEnd] = 383,
        [TR4LaraAnim.PulleyGrab] = 384,
        [TR4LaraAnim.PulleyPull] = 385,
        [TR4LaraAnim.PulleyUngrab] = 386,
        [TR4LaraAnim.PoleToStand] = 387,
        [TR4LaraAnim.PoleTurnLeftContinueUnused] = 388,
        [TR4LaraAnim.PoleTurnLeftEnd] = 389,
        [TR4LaraAnim.PoleTurnRightContinueUnused] = 390,
        [TR4LaraAnim.PoleTurnRightEnd] = 391,
        [TR4LaraAnim.TurnswitchPushRightStart] = 392,
        [TR4LaraAnim.TurnswitchPushRightContinue] = 393,
        [TR4LaraAnim.TurnswitchPushRightEnd] = 394,
        [TR4LaraAnim.TurnswitchPushLeftStart] = 395,
        [TR4LaraAnim.TurnswitchPushLeftContinue] = 396,
        [TR4LaraAnim.TurnswitchPushLeftEnd] = 397,
        [TR4LaraAnim.JumpUpToRopeStart] = 398,
        [TR4LaraAnim.TrainDeath] = 399,
        [TR4LaraAnim.JumpUpToRopeEnd] = 400,
        [TR4LaraAnim.RopeIdle] = 401,
        [TR4LaraAnim.RopeDownStart] = 402,
        [TR4LaraAnim.RopeUp] = 403,
        [TR4LaraAnim.RopeIdleToRopeHangUnused] = 404,
        [TR4LaraAnim.RopeGrabToFallUnused] = 405,
        [TR4LaraAnim.RopeJumpToGrab] = 406,
        [TR4LaraAnim.RopeIdleToBackflipUnused] = 407,
        [TR4LaraAnim.RopeSwingToFallSemifrontUnused] = 408,
        [TR4LaraAnim.RopeSwingToFallMiddlUnused] = 409,
        [TR4LaraAnim.RopeSwingToFallBackUnused] = 410,
        [TR4LaraAnim.RopeDown] = 411,
        [TR4LaraAnim.RopeDownEnd] = 412,
        [TR4LaraAnim.RopeSwingToReach] = 413,
        [TR4LaraAnim.RopeIdleToSwing] = 414,
        [TR4LaraAnim.RopeIdleToSwingSemimiddleUnused] = 415,
        [TR4LaraAnim.RopeIdleToSwingHalfmiddleUnused] = 416,
        [TR4LaraAnim.RopeSwingToFallFrontUnused] = 417,
        [TR4LaraAnim.RopeGrabToFallAlternateUnused] = 418,
        [TR4LaraAnim.RopeTurnLeft] = 419,
        [TR4LaraAnim.RopeTurnRight] = 420,
        [TR4LaraAnim.RopeSwing] = 421,
        [TR4LaraAnim.LadderToHandsDownAlternateUnused] = 422,
        [TR4LaraAnim.RopeSwingBackContinueUnused] = 423,
        [TR4LaraAnim.RopeSwingBackEndUnused] = 424,
        [TR4LaraAnim.RopeSwingBackStartUnused] = 425,
        [TR4LaraAnim.RopeSwingForwardSoftUnused] = 426,
        [TR4LaraAnim.PourWaterskinLow] = 427,
        [TR4LaraAnim.FillWaterskin] = 428,
        [TR4LaraAnim.PourWaterskinHigh] = 429,
        [TR4LaraAnim.PryDoor] = 430,
        [TR4LaraAnim.RopeSwingForwardHardUnused] = 431,
        [TR4LaraAnim.RopeChangeRopeUnused] = 432,
        [TR4LaraAnim.RopeSwingToReachFront] = 433,
        [TR4LaraAnim.RopeSwingToReachMiddle] = 434,
        [TR4LaraAnim.RopeSwingBlockUnused] = 435,
        [TR4LaraAnim.RopeSwingToReachSemimiddle] = 436,
        [TR4LaraAnim.RopeSwingToReachHangFront] = 437,
        [TR4LaraAnim.RopeSwingToReachFront2] = 438,
        [TR4LaraAnim.DoubledoorsPush] = 439,
        [TR4LaraAnim.BigButtonPush] = 440,
        [TR4LaraAnim.Jumpswitch] = 441,
        [TR4LaraAnim.UnderwaterPulley] = 442,
        [TR4LaraAnim.UnderwaterDoorOpen] = 443,
        [TR4LaraAnim.CrowbarUseOnWall] = 444,
        [TR4LaraAnim.CrowbarUseOnFloor] = 445,
        [TR4LaraAnim.HarpPlay] = 446,
        [TR4LaraAnim.PutTrident] = 447,
        [TR4LaraAnim.RotateSenet] = 448,
        [TR4LaraAnim.TorchLight1] = 449,
        [TR4LaraAnim.TorchLight2] = 450,
        [TR4LaraAnim.TorchLight3] = 451,
        [TR4LaraAnim.TorchLight4] = 452,
        [TR4LaraAnim.TorchLight5] = 453,
        [TR4LaraAnim.DetonatorUse] = 454,
        [TR4LaraAnim.CorrectPositionFrontUnused] = 455,
        [TR4LaraAnim.CorrectPositionLeftUnused] = 456,
        [TR4LaraAnim.CorrectPositionRightUnused] = 457,
        [TR4LaraAnim.CrowbarUseOnFloorFailUnused] = 458,
        [TR4LaraAnim.DeathMagicUnused] = 459,
        [TR4LaraAnim.DeathBlowup] = 460,
        [TR4LaraAnim.PickupSarcophagus] = 461,
        [TR4LaraAnim.DragBody] = 462,
        [TR4LaraAnim.Binoculars] = 463,
        [TR4LaraAnim.DeathBigScorpion] = 464,
        [TR4LaraAnim.DeathSeth] = 465,
        [TR4LaraAnim.BeetlePut] = 466,
    };

    private static readonly Dictionary<TR4LaraState, int> _tr4StateSyncMap = new()
    {
        [TR4LaraState.RopeLeft] = 108,
        [TR4LaraState.RopeRight] = 109,
        [TR4LaraState.BlockSwitch] = 110,
        [TR4LaraState.LiftTrapdoor] = 111,
        [TR4LaraState.PullTrapdoor] = 112,
        [TR4LaraState.TurnSwitch] = 113,
        [TR4LaraState.CogSwitch] = 114,
        [TR4LaraState.RailSwitch] = 115,
        [TR4LaraState.HiddenPickup] = 116,
        [TR4LaraState.PoleIdle] = 117,
        [TR4LaraState.PoleUp] = 118,
        [TR4LaraState.PoleDown] = 119,
        [TR4LaraState.PoleLeft] = 120,
        [TR4LaraState.PoleRight] = 121,
        [TR4LaraState.Pulley] = 122,
        [TR4LaraState.RopeIdle] = 123,
        [TR4LaraState.RopeClimb] = 124,
        [TR4LaraState.RopeSlide] = 125,
        [TR4LaraState.RopeForward] = 126,
        [TR4LaraState.RopeBack] = 127,
        [TR4LaraState.RopeForwardSoftUnused] = 128,
        [TR4LaraState.PushDoors] = 129,
    };
}
