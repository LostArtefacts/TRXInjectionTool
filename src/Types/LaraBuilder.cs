using System.Diagnostics;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types;

public abstract class LaraBuilder : InjectionBuilder
{
    private static readonly string _extLaraPath = "Resources/lara_ext.phd";

    public abstract TRGameVersion GameVersion { get; }
    protected abstract short JumpSFX { get; }
    protected abstract short DryFeetSFX { get; }
    protected abstract short WetFeetSFX { get; }
    protected abstract short LandSFX { get; }
    protected abstract short KneesShuffleSFX { get; }
    protected abstract short ClimbOnSFX { get; }
    protected abstract short ResponsiveState { get; }

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
        Roll = 45,
        Handstand = 54,
    }

    protected enum LaraAnim
    {
        Run = 0,
        RunStart = 6,
        StandStill = 11,
        RunJumpRightStart = 16,
        RunJumpRightContinue = 17,
        RunJumpLeftStart = 18,
        RunJumpLeftContinue = 19,
        Freefall = 23,
        JumpUp = 28,
        Climb3Click = 42,
        Climb2Click = 50,
        Climb2ClickEnd = 51,
        JumpForwardEndToFreefall = 49,
        JumpBack = 75,
        JumpForward = 77,
        UnderwaterSwimForward = 86,
        UnderwaterSwimGlide = 87,
        JumpForwardToReach = 94,
        JumpForwardToReachLate = 100,
        Reach = 95,
        ReachToHang = 96,
        ClimbOnEnd = 102,
        StandIdle = 103,
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
            Dispatches = new()
            {
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
            }
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
        object stateID, object animID, object idleStateID, object idleAnimID)
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
    }

    protected void ImportCrouchTurn(TRModel lara,
        object crouchLeftStateID, object crouchLeftAnimID,
        object crouchRightStateID, object crouchRightAnimID,
        object crouchIdleStateID, object crouchIdleAnimID)
    {
        ImportCrouchTurnAnim(lara, ExtLaraAnim.CrouchTurnLeft, 
            crouchLeftStateID, crouchLeftAnimID, crouchIdleStateID, crouchIdleAnimID);
        ImportCrouchTurnAnim(lara, ExtLaraAnim.CrouchTurnRight, 
            crouchRightStateID, crouchRightAnimID, crouchIdleStateID, crouchIdleAnimID);
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
        var frames = new[] { 62, 77 };
        anim.Commands.AddRange(frames.Select(f => new TRSFXCommand
        {
            FrameNumber = (short)f,
            SoundID = (short)TR1SFX.LaraFeet,
        }));
    }

    protected static void FixHandstandSFX(TRModel lara)
    {
        var anim = lara.Animations[(int)LaraAnim.ClimbOnHandstand];
        var frames = new[] { 157, 188 };
        anim.Commands.AddRange(frames.Select(f => new TRSFXCommand
        {
            FrameNumber = (short)f,
            SoundID = (short)TR1SFX.LaraFeet,
        }));
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

            if (tr3Idx == TR3LaraAnim.HangToCrouchStart)
            {
                anim.Commands.Add(new TRSFXCommand
                {
                    SoundID = ClimbOnSFX,
                    FrameNumber = 13,
                });
            }
            else if (tr3Idx == TR3LaraAnim.CrawlDeath)
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

        AddChange(lara, LaraAnim.Run, stateMap[TR3LaraState.CrouchIdle], 0, 4, animMap[TR3LaraAnim.RunToCrouchLeftStart], 0);
        AddChange(lara, LaraAnim.Run, stateMap[TR3LaraState.CrouchIdle], 10, 14, animMap[TR3LaraAnim.RunToCrouchRightStart], 0);
        AddChange(lara, LaraAnim.StandStill, stateMap[TR3LaraState.CrouchIdle], 0, 1, animMap[TR3LaraAnim.StandToCrouch], 0);
        AddChange(lara, LaraAnim.ReachToHang, stateMap[TR3LaraState.ClimbToCrawl], 12, 22, animMap[TR3LaraAnim.HangToCrouchStart], 0);
        AddChange(lara, LaraAnim.StandIdle, stateMap[TR3LaraState.CrouchIdle], 0, 44, animMap[TR3LaraAnim.StandToCrouch], 0);
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

    protected static void AddChange
        (TRModel lara, object animIdx, object goalStateID, short low, short high, object nextAnimIdx, short nextFrame)
        => AddChange(lara.Animations[Convert.ToInt32(animIdx)], goalStateID, low, high, nextAnimIdx, nextFrame);

    protected static void AddChange
        (TRAnimation anim, object goalStateID, short low, short high, object nextAnimIdx, short nextFrame)
    {
        anim.Changes.Add(new()
        {
            StateID = Convert.ToUInt16(goalStateID),
            Dispatches = new()
            {
                new()
                {
                    Low = low,
                    High = high,
                    NextAnimation = Convert.ToInt16(nextAnimIdx),
                    NextFrame = nextFrame,
                }
            },
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
            anim.NextAnimation = Convert.ToUInt16(crouchIdleAnim);
            anim.StateID = Convert.ToUInt16(climbToCrawlState);
            lara.Animations.Add(anim);
        }

        AddChange(lara, idleLadderAnim, climbToCrawlState, 0, 48, ladderToCrouchStartAnim, 0);
    }
}
