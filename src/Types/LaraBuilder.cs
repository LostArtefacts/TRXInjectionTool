using TRLevelControl.Model;

namespace TRXInjectionTool.Types;

public abstract class LaraBuilder : InjectionBuilder
{
    private static readonly string _extLaraPath = "Resources/lara_ext.phd";

    protected abstract short JumpSFX { get; }
    protected abstract short DryFeetSFX { get; }
    protected abstract short WetFeetSFX { get; }
    protected abstract short LandSFX { get; }
    protected abstract short ResponsiveState { get; }

    public static TRModel GetLaraPoseModel()
        => _control1.Read(_extLaraPath).Models[TR1Type.Lara];

    public static TRModel GetLaraExtModel()
        => _control1.Read(_extLaraPath).Models[TR1Type.LaraMiscAnim_H];

    protected void ImportNeutralTwist(TRModel lara, short animID, short stateID)
    {
        var laraExt = GetLaraExtModel();
        var anim = laraExt.Animations[15];
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
        anim.Changes.Add(new()
        {
            StateID = 15,
            Dispatches = new()
            {
                new()
                {
                    Low = 33,
                    High = 37,
                    NextAnimation = 73,
                    NextFrame = 6,
                }
            },
        });
        // Arabian-to-roll
        anim.Changes.Add(new()
        {
            StateID = 45,
            Dispatches = new()
            {
                new()
                {
                    Low = 33,
                    High = 37,
                    NextAnimation = 146,
                    NextFrame = 1,
                }
            }
        });
        // Arabian-loop
        anim.Changes.Add(new()
        {
            StateID = (ushort)ResponsiveState,
            Dispatches = new()
            {
                new()
                {
                    Low = 32,
                    High = 32,
                    NextAnimation = animID,
                    NextFrame = 2,
                }
            }
        });
    }

    protected static void ImportControlledDrop(TRModel lara, short continueAnimID)
    {
        var laraExt = GetLaraExtModel();
        var startAnim = laraExt.Animations[9];
        var endAnim = laraExt.Animations[10];

        lara.Animations.Add(startAnim);
        lara.Animations.Add(endAnim);
        startAnim.NextAnimation = (ushort)continueAnimID;
        endAnim.NextAnimation = 95;
    }

    protected void ImportHangToJump(TRModel lara, short startAnimID)
    {
        var laraExt = GetLaraExtModel();
        var upStartAnim = laraExt.Animations[11];
        var upEndAnim = laraExt.Animations[12];
        var backStartAnim = laraExt.Animations[13];
        var backEndAnim = laraExt.Animations[14];        

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

        var hangAnim = lara.Animations[96];
        // Marker for engine that hanging is responsive
        hangAnim.Changes.Add(new()
        {
            StateID = (ushort)ResponsiveState,
            Dispatches = new()
            {
                new()
                {
                    NextAnimation = 96,
                    NextFrame = 21,
                    Low = 21,
                    High = 22,
                }
            },
        });
        // Hang to jump up
        hangAnim.Changes.Add(new()
        {
            StateID = 28,
            Dispatches = new()
            {
                new()
                {
                    NextAnimation = startAnimID,
                    Low = 21,
                    High = 22,
                }
            },
        });
        // Hang to jump back
        hangAnim.Changes.Add(new()
        {
            StateID = 25,
            Dispatches = new()
            {
                new()
                {
                    NextAnimation = (short)(startAnimID + 2),
                    Low = 21,
                    High = 22,
                }
            },
        });
    }
}
