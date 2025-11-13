using System.Diagnostics;
using TRLevelControl.Helpers;
using TRLevelControl.Model;

namespace TRXInjectionTool.Types;

public abstract class LaraBuilder : InjectionBuilder
{
    private static readonly string _extLaraPath = "Resources/lara_ext.phd";

    public abstract TRGameVersion GameVersion { get; }
    protected abstract short JumpSFX { get; }
    protected abstract short DryFeetSFX { get; }
    protected abstract short WetFeetSFX { get; }
    protected abstract short LandSFX { get; }
    protected abstract short ResponsiveState { get; }

    protected enum LaraState
    {
        Stop = 2,
        JumpForward = 3,
        Pose = 4,
        Death = 8,
        Freefall = 9,
        Glide = 18,
        Roll = 45,
    }

    protected enum LaraAnim
    {
        Run = 0,
        StandStill = 11,
        RunJumpRightStart = 16,
        RunJumpLeftStart = 18,
        JumpForwardEndToFreefall = 49,
        JumpBack = 75,
        JumpForward = 77,
        UnderwaterSwimForward = 86,
        UnderwaterSwimGlide = 87,
        ReachToHang = 96,
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
    }

    protected enum TR3LaraState
    {
        Kick = 69,
        Sprint = 73,
        SprintRoll = 74
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
    }

    protected enum LaraExtraState
    {
        Breath,
        TrexKill,
        ScionPickup1,
        UseMidas,
        MidasKill,
        ScionPickup2,
        TorsoKill,
        Plunger,
        StartAnim,
        Airlock,
        SharkKill,
        YetiKill,
        GongBong,
        GuardKill,
        PullDagger,
        StartHouse,
        EndHouse,
    }

    private static readonly List<LaraExtraState> _extraDeathStates =
    [
        LaraExtraState.TrexKill, LaraExtraState.MidasKill, LaraExtraState.TorsoKill,
        LaraExtraState.SharkKill, LaraExtraState.YetiKill, LaraExtraState.GuardKill,
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

    protected void ImportSprint<A, S>(TRModel lara, object slideToRunAnim,
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

            anim.Commands.RemoveAll(a => a is not TRSFXCommand);
            anim.Commands.OfType<TRSFXCommand>().Where(s => s.SoundID == 17)
                .ToList().ForEach(s => s.SoundID = WetFeetSFX);

            if (Enum.IsDefined(typeof(TR3LaraState), (int)anim.StateID))
            {
                anim.StateID = Convert.ToUInt16(stateMap[(TR3LaraState)anim.StateID]);
            }
            if (Enum.IsDefined(typeof(TR3LaraAnim), (int)anim.NextAnimation))
            {
                anim.NextAnimation = Convert.ToUInt16(animMap[(TR3LaraAnim)anim.NextAnimation]);
            }

            anim.Changes.RemoveAll(c => c.Dispatches.Any(d => !Enum.IsDefined(typeof(TR3LaraAnim), (int)d.NextAnimation)));
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

        AddChange(lara, 0, stateMap[TR3LaraState.Sprint], 0, 1, animMap[TR3LaraAnim.RunToSprintLeft], 0);
        AddChange(lara, 0, stateMap[TR3LaraState.Sprint], 11, 12, animMap[TR3LaraAnim.RunToSprintRight], 0);
        AddChange(lara, slideToRunAnim, stateMap[TR3LaraState.Sprint], 14, 14, animMap[TR3LaraAnim.RunToSprintLeft], 0);
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
            else
            {
                srcModel = _control2.Read($"Resources/{data.LevelName}").Models[TR2Type.LaraMiscAnim_H];
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
        }
    }
}
