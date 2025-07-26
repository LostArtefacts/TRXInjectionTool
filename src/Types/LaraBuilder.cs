using TRLevelControl.Model;

namespace TRXInjectionTool.Types;

public abstract class LaraBuilder : InjectionBuilder
{
    protected abstract short JumpSFX { get; }
    protected abstract short DryFeetSFX { get; }
    protected abstract short WetFeetSFX { get; }
    protected abstract short LandSFX { get; }

    protected void ImportNeutralTwist(TRModel lara, short animID, short stateID, short responsiveID)
    {
        var julyBeta = _control1.Read("Resources/TR1/1996-07-02.phd");
        var anim = julyBeta.Models[TR1Type.Lara].Animations[61];
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
            StateID = (ushort)responsiveID,
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
            StateID = (ushort)responsiveID,
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
        var twistLevel = _control1.Read("Resources/TR1/Lara/twist.phd");
        var twistLara = twistLevel.Models[TR1Type.Lara];
        var startAnim = twistLara.Animations[9];
        var endAnim = twistLara.Animations[10];

        lara.Animations.Add(startAnim);
        lara.Animations.Add(endAnim);
        startAnim.NextAnimation = (ushort)continueAnimID;
        endAnim.NextAnimation = 95;
    }
}
