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
        Pose = 4,
        Death = 8,
        Freefall = 9,
        Roll = 45,
    }

    protected enum LaraAnim
    {
        JumpForwardEndToFreefall = 49,
        JumpBack = 75,
        JumpForward = 77,
        StandIdle = 103,
        StandDeath = 138,
        RollStart = 146,
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
        lara.Animations[70].Changes.Add(change);
    }

    protected void ImportNeutralTwist(TRModel lara, short animID, short stateID)
    {
        var laraExt = GetLaraExtModel();
        var anim = laraExt.Animations[(int)ExtLaraAnim.JumpNeutralRoll];
        lara.Animations.Add(anim);
        anim.NextAnimation = 11;
        anim.StateID = (ushort)stateID;

        anim.Commands.Add(new TRFXCommand { FrameNumber = 22 });
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

    protected void ImportSprint<A, S>(TRModel lara,
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
}
