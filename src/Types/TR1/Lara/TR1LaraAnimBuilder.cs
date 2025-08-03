using System.IO.Compression;
using System.Text;
using System.Xml;
using TRImageControl.Packing;
using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Lara;

public class TR1LaraAnimBuilder : LaraBuilder
{
    private static readonly string _wadZipPath = "../../Resources/Published/tr1-lara-anim-ext.zip";
    private static readonly DateTimeOffset _wadZipPlaceholderDate
        = new(new DateTime(2025, 5, 15, 11, 0, 0), new TimeSpan());

    public override string ID => "tr1-lara-anims";
    protected override short JumpSFX => (short)TR1SFX.LaraJump;
    protected override short DryFeetSFX => (short)TR1SFX.LaraFeet;
    protected override short WetFeetSFX => (short)TR1SFX.LaraWetFeet;
    protected override short LandSFX => (short)TR1SFX.LaraLand;
    protected override short ResponsiveState => (short)InjState.Responsive;

    enum InjAnim : int
    {
        RunJumpRollStart = 160,
        Somersault = 161,
        RunJumpRollEnd = 162,
        JumpFwdRollStart = 163,
        JumpFwdRollEnd = 164,
        JumpBackRollStart = 165,
        JumpBackRollEnd = 166,
        UWRollStart = 167,
        UWRollEnd = 168,
        SurfClimbMedium = 169,
        Wade = 170,
        RunToWadeLeft = 171,
        RunToWadeRight = 172,
        WadeToRunLeft = 173,
        WadeToRunRight = 174,
        WadeToStandRight = 175,
        WadeToStandLeft = 176,
        StandToWade = 177,
        SurfToWade = 178,
        SurfToWadeLow = 179,
        UWToStand = 180,
        SwimToHuddle = 181,
        SwimToSprawl = 182,
        SwimToMedium = 183,
        SlideToRun = 184,
        JumpTwistContinue = 185,
        JumpNeutralRoll = 186,
        ControlledDrop = 187,
        ControlledDropContinue = 188,
        HangToJumpUp = 189,
        HangToJumpUpContinue = 190,
        HangToJumpBack = 191,
        HangToJumpBackContinue = 192,
    };

    enum InjState : int
    {
        Twist = 57,
        UWRoll = 58,
        Wade = 59,
        Responsive = 60,
        NeutralRoll = 61,
    };

    public override List<InjectionData> Build()
    {
        List<InjectionData> dataGroup = new();

        TR1Level caves = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        TR2Level wall = _control2.Read($"Resources/{TR2LevelNames.GW}");
        ResetLevel(caves);

        TRModel tr1Lara = caves.Models[TR1Type.Lara];
        TRModel tr2Lara = wall.Models[TR2Type.Lara];
        ImportTR2Jumping(tr1Lara, tr2Lara);
        ImportJumpTwist(tr1Lara, tr2Lara);
        ImportUWRoll(tr1Lara);
        ImportWading(tr1Lara, tr2Lara);
        ImportWetFeet(tr1Lara, caves);
        ImportTR2Gliding(tr1Lara, tr2Lara);
        ImportSlideToRun(tr1Lara);
        ImproveTwists(tr1Lara);
        ImportNeutralTwist(tr1Lara, (short)InjAnim.JumpNeutralRoll, (short)InjState.NeutralRoll);
        ImportControlledDrop(tr1Lara, (short)InjAnim.ControlledDropContinue);
        ImportHangToJump(tr1Lara, (short)InjAnim.HangToJumpUp);

        InjectionData data = InjectionData.Create(caves, InjectionType.LaraAnims, "lara_animations");
        dataGroup.Add(data);

        ExportLaraWAD(caves);

        return dataGroup;
    }

    static void ImportJumpTwist(TRModel tr1Lara, TRModel tr2Lara)
    {
        Dictionary<int, InjAnim> map = new()
        {
            [207] = InjAnim.RunJumpRollStart,
            [208] = InjAnim.Somersault,
            [209] = InjAnim.RunJumpRollEnd,
            [210] = InjAnim.JumpFwdRollStart,
            [211] = InjAnim.JumpFwdRollEnd,
            [212] = InjAnim.JumpBackRollStart,
            [213] = InjAnim.JumpBackRollEnd,
        };

        foreach (int tr2Idx in map.Keys)
        {
            TRAnimation anim = tr2Lara.Animations[tr2Idx];
            tr1Lara.Animations.Add(anim);
            if (map.ContainsKey(anim.NextAnimation))
            {
                anim.NextAnimation = (ushort)map[anim.NextAnimation];
            }
            foreach (TRAnimDispatch dispatch in anim.Changes.SelectMany(c => c.Dispatches))
            {
                if (map.ContainsKey(dispatch.NextAnimation))
                {
                    dispatch.NextAnimation = (short)map[dispatch.NextAnimation];
                }
            }
        }

        // Running jump right forward
        AddChange(tr1Lara, 17, InjState.Twist, 0, 4, InjAnim.RunJumpRollStart, 0);
        // Running jump left forward
        AddChange(tr1Lara, 19, InjState.Twist, 0, 4, InjAnim.RunJumpRollStart, 0);
        // Standing jump back
        AddChange(tr1Lara, 75, InjState.Twist, 0, 4, InjAnim.JumpBackRollStart, 0);
        // Standing jump forward
        AddChange(tr1Lara, 77, InjState.Twist, 0, 4, InjAnim.JumpFwdRollStart, 0);
        // Freefall to somersault
        AddChange(tr1Lara, 153, InjState.Twist, 0, 9, InjAnim.Somersault, 0);
    }

    static void ImportUWRoll(TRModel tr1Lara)
    {
        // See PR 1272/1276
        TRModel tr2Lara = GetLaraExtModel();

        TRAnimation uwRollStart = tr2Lara.Animations[0];
        TRAnimation uwRollEnd = tr2Lara.Animations[1];

        uwRollStart.NextAnimation = (ushort)InjAnim.UWRollEnd;
        uwRollStart.NextFrame = 1;
        uwRollEnd.NextAnimation = 108;
        tr1Lara.Animations.Add(uwRollStart);
        tr1Lara.Animations.Add(uwRollEnd);

        uwRollStart.StateID = (ushort)InjState.UWRoll;
        uwRollEnd.StateID = (ushort)InjState.UWRoll;

        (uwRollStart.Commands.Find(c => c is TRSFXCommand) as TRSFXCommand).Environment = TRSFXEnvironment.Any;

        // Swimming, gliding and UW idle
        foreach (int changeAnim in new[] { 86, 87, 108 })
        {
            AddChange(tr1Lara, changeAnim, InjState.UWRoll, 0,
                (short)(tr1Lara.Animations[changeAnim].FrameEnd + 1), InjAnim.UWRollStart, 0);
        }
    }

    private static void ImproveTwists(TRModel tr2Lara)
    {
        var twistLara = GetLaraExtModel();

        tr2Lara.Animations[(int)InjAnim.RunJumpRollStart] = twistLara.Animations[2];
        tr2Lara.Animations[(int)InjAnim.RunJumpRollEnd] = twistLara.Animations[3];
        tr2Lara.Animations[(int)InjAnim.JumpFwdRollStart] = twistLara.Animations[4];
        tr2Lara.Animations[(int)InjAnim.JumpFwdRollEnd] = twistLara.Animations[5];
        tr2Lara.Animations[(int)InjAnim.JumpBackRollStart] = twistLara.Animations[6];
        tr2Lara.Animations[(int)InjAnim.JumpBackRollEnd] = twistLara.Animations[7];
        tr2Lara.Animations.Add(twistLara.Animations[8]); // JumpTwistContinue

        tr2Lara.Animations[(int)InjAnim.RunJumpRollStart].NextAnimation = (ushort)InjAnim.JumpTwistContinue;
        tr2Lara.Animations[(int)InjAnim.RunJumpRollEnd].NextAnimation = 75;
        tr2Lara.Animations[(int)InjAnim.RunJumpRollEnd].NextFrame = 39;
        tr2Lara.Animations[(int)InjAnim.JumpFwdRollStart].NextAnimation = (ushort)InjAnim.JumpFwdRollEnd;
        tr2Lara.Animations[(int)InjAnim.JumpFwdRollEnd].NextAnimation = 75;
        tr2Lara.Animations[(int)InjAnim.JumpFwdRollEnd].NextFrame = 39;
        tr2Lara.Animations[(int)InjAnim.JumpBackRollStart].NextAnimation = (ushort)InjAnim.JumpBackRollEnd;
        tr2Lara.Animations[(int)InjAnim.JumpBackRollEnd].NextAnimation = 77;
        tr2Lara.Animations[(int)InjAnim.JumpBackRollEnd].NextFrame = 39;
        tr2Lara.Animations[(int)InjAnim.JumpTwistContinue].NextAnimation = (int)InjAnim.RunJumpRollEnd;
    }

    static void ImportWading(TRModel tr1Lara, TRModel tr2Lara)
    {
        Dictionary<int, InjAnim> map = new()
        {
            [191] = InjAnim.SurfClimbMedium,
            [177] = InjAnim.Wade,
            [178] = InjAnim.RunToWadeLeft,
            [179] = InjAnim.RunToWadeRight,
            [180] = InjAnim.WadeToRunLeft,
            [181] = InjAnim.WadeToRunRight,
            [184] = InjAnim.WadeToStandRight,
            [185] = InjAnim.WadeToStandLeft,
            [186] = InjAnim.StandToWade,
            [190] = InjAnim.SurfToWade,
            [193] = InjAnim.SurfToWadeLow,
            [192] = InjAnim.UWToStand,
        };

        for (int i = 0; i < tr1Lara.Animations.Count; i++)
        {
            TRAnimation anim1 = tr1Lara.Animations[i];
            TRAnimation anim2 = tr2Lara.Animations[i];
            foreach (TRStateChange change in anim2.Changes)
            {
                if (change.Dispatches.All(d => map.ContainsKey(d.NextAnimation)))
                {
                    anim1.Changes.Add(change);
                    if (change.StateID == 65)
                    {
                        change.StateID = (ushort)InjState.Wade;
                    }
                    change.Dispatches.ForEach(d => d.NextAnimation = (short)map[d.NextAnimation]);
                }
            }
        }

        foreach (int tr2Idx in map.Keys)
        {
            TRAnimation anim = tr2Lara.Animations[tr2Idx];
            tr1Lara.Animations.Add(anim);
            if (anim.StateID == 65)
            {
                anim.StateID = (ushort)InjState.Wade;
            }

            if (map.ContainsKey(anim.NextAnimation))
            {
                anim.NextAnimation = (ushort)map[anim.NextAnimation];
            }
            foreach (TRAnimDispatch dispatch in anim.Changes.SelectMany(c => c.Dispatches))
            {
                if (map.ContainsKey(dispatch.NextAnimation))
                {
                    dispatch.NextAnimation = (short)map[dispatch.NextAnimation];
                }
            }

            anim.Commands.FindAll(c => c is TRSFXCommand s && s.SoundID == (short)TR2SFX.LaraWade)
                .ForEach(c => (c as TRSFXCommand).SoundID = (short)TR1SFX.LaraWade);
        }

        tr1Lara.Animations[(int)InjAnim.UWToStand].FrameEnd = 31; // Default is 33, but for some reason this causes Hair to cause a crash - Item_GetFrames div by 0?

        // Step left into surf swim left
        AddChange(tr1Lara, 65, 48, 0, 25, 143, 0);
        // Step right into surf swim right
        AddChange(tr1Lara, 67, 49, 0, 25, 144, 0);
        // Surf swim left into step left
        AddChange(tr1Lara, 143, 22, 0, 45, 65, 0);
        // Surf swim right into step right
        AddChange(tr1Lara, 144, 21, 0, 45, 67, 0);

        // Change Lara's underwater lever animation to match TR2.
        tr1Lara.Animations[129].Commands.Add(new TREmptyHandsCommand());
    }

    static void ImportWetFeet(TRModel tr1Lara, TR1Level level)
    {
        TR2Level animLevel = _control2.Read($"Resources/{TR2LevelNames.GW}");
        TR2SoundEffect wetFeet2 = animLevel.SoundEffects[TR2SFX.LaraWetFeet];
        TR2SoundEffect wade2 = animLevel.SoundEffects[TR2SFX.LaraWade];

        TR1SoundEffect wetFeet1 = new()
        {
            Chance = wetFeet2.Chance,
            Mode = TR1SFXMode.Restart,
            Pan = wetFeet2.Pan,
            RandomizePitch = wetFeet2.RandomizePitch,
            RandomizeVolume = wetFeet2.RandomizeVolume,
            Volume = wetFeet2.Volume,
            Samples = new(),
        };

        for (int i = 0; i < wetFeet2.SampleCount; i++)
        {
            wetFeet1.Samples.Add(File.ReadAllBytes($"Resources/TR1/WetFeet/{i}.wav"));
        }

        TR1SoundEffect wade1 = new()
        {
            Chance = wade2.Chance,
            Mode = TR1SFXMode.Restart,
            Pan = wade2.Pan,
            RandomizePitch = wade2.RandomizePitch,
            RandomizeVolume = wade2.RandomizeVolume,
            Volume = wade2.Volume,
            Samples = new()
            {
                File.ReadAllBytes("Resources/TR1/Wade/0.wav"),
            },
        };

        level.SoundEffects[TR1SFX.LaraWetFeet] = wetFeet1;
        level.SoundEffects[TR1SFX.LaraWade] = wade1;

        // Add the wet feet commands wherever there are regular feet commands.
        var excludeFeetAnims = new List<TRAnimation>
        {
            tr1Lara.Animations[42], // climb 3-click
            tr1Lara.Animations[50], // climb 2-click
            tr1Lara.Animations[51], // climb 2-click-end
            tr1Lara.Animations[97], // hang-to-climb-on
        };
        foreach (TRAnimation anim in tr1Lara.Animations.Except(excludeFeetAnims))
        {
            List<TRSFXCommand> feetCmds = anim.Commands
                .FindAll(c => c is TRSFXCommand s && s.SoundID == 0)
                .Cast<TRSFXCommand>()
                .ToList();
            foreach (TRSFXCommand cmd in feetCmds)
            {
                cmd.Environment = TRSFXEnvironment.Land;
                anim.Commands.Add(new TRSFXCommand
                {
                    SoundID = (short)TR1SFX.LaraWetFeet,
                    FrameNumber = cmd.FrameNumber,
                    Environment = TRSFXEnvironment.Water,
                });
            }
        }

        // Splashing and water exit
        {
            // FREEFALL_LAND
            TRAnimation anim = tr1Lara.Animations[24];
            anim.Commands.ForEach(c => (c as TRSFXCommand).Environment = TRSFXEnvironment.Land);
            anim.Commands.Add(new TRSFXCommand
            {
                SoundID = (short)TR1SFX.LaraWetFeet,
                FrameNumber = 0,
                Environment = TRSFXEnvironment.Water,
            });
            anim.Commands.Add(new TRSFXCommand
            {
                SoundID = (short)TR1SFX.LaraSplash,
                FrameNumber = 3,
                Environment = TRSFXEnvironment.Water,
            });

            // FREEFALL_LAND_DEATH
            anim = tr1Lara.Animations[25];
            anim.Commands.Add(new TRSFXCommand
            {
                SoundID = (short)TR1SFX.LaraSplash,
                FrameNumber = 2,
                Environment = TRSFXEnvironment.Water,
            });

            // STAND_TO_JUMP_UP
            anim = tr1Lara.Animations[26];
            anim.Commands.Add(new TRSFXCommand
            {
                SoundID = (short)TR1SFX.LaraWetFeet,
                FrameNumber = 15,
                Environment = TRSFXEnvironment.Water,
            });

            // JUMP_UP_LAND
            anim = tr1Lara.Animations[31];
            anim.Commands.ForEach(c => (c as TRSFXCommand).Environment = TRSFXEnvironment.Land);
            anim.Commands.Add(new TRSFXCommand
            {
                SoundID = (short)TR1SFX.LaraWetFeet,
                FrameNumber = 0,
                Environment = TRSFXEnvironment.Water,
            });

            // WALLSWITCH_DOWN
            anim = tr1Lara.Animations[63];
            anim.Commands.Add(new TRSFXCommand
            {
                SoundID = (short)TR1SFX.LaraWade,
                FrameNumber = 8,
                Environment = TRSFXEnvironment.Water,
            });

            // WALLSWITCH_UP
            anim = tr1Lara.Animations[64];
            anim.Commands.Add(new TRSFXCommand
            {
                SoundID = (short)TR1SFX.LaraWade,
                FrameNumber = 10,
                Environment = TRSFXEnvironment.Water,
            });

            // JUMP_BACK_START
            anim = tr1Lara.Animations[74];
            anim.Commands.Add(new TRSFXCommand
            {
                SoundID = (short)TR1SFX.LaraExitWater,
                FrameNumber = 2,
                Environment = TRSFXEnvironment.Water,
            });

            // JUMP_FORWARD_START
            anim = tr1Lara.Animations[76];
            anim.Commands.Add(new TRSFXCommand
            {
                SoundID = (short)TR1SFX.LaraExitWater,
                FrameNumber = 1,
                Environment = TRSFXEnvironment.Water,
            });

            // JUMP_LEFT_START
            anim = tr1Lara.Animations[78];
            anim.Commands.Add(new TRSFXCommand
            {
                SoundID = (short)TR1SFX.LaraExitWater,
                FrameNumber = 2,
                Environment = TRSFXEnvironment.Water,
            });

            // JUMP_RIGHT_START
            anim = tr1Lara.Animations[80];
            anim.Commands.Add(new TRSFXCommand
            {
                SoundID = (short)TR1SFX.LaraExitWater,
                FrameNumber = 2,
                Environment = TRSFXEnvironment.Water,
            });

            // LAND
            anim = tr1Lara.Animations[82];
            (anim.Commands[2] as TRSFXCommand).FrameNumber = 0;
            anim.Commands.Add(new TRSFXCommand
            {
                SoundID = (short)TR1SFX.LaraSplash,
                FrameNumber = 5,
                Environment = TRSFXEnvironment.Water,
            });

            // JUMP_UP_START
            anim = tr1Lara.Animations[91];
            anim.Commands.Add(new TRSFXCommand
            {
                SoundID = (short)TR1SFX.LaraExitWater,
                FrameNumber = 2,
                Environment = TRSFXEnvironment.Water,
            });

            // USE_KEY
            anim = tr1Lara.Animations[131];
            anim.Commands.Add(new TRSFXCommand
            {
                SoundID = (short)TR1SFX.LaraWade,
                FrameNumber = 10,
                Environment = TRSFXEnvironment.Water,
            });
            anim.Commands.Add(new TRSFXCommand
            {
                SoundID = (short)TR1SFX.LaraWade,
                FrameNumber = 133,
                Environment = TRSFXEnvironment.Water,
            });

            // RUN_DEATH
            anim = tr1Lara.Animations[133];
            anim.Commands.Add(new TRSFXCommand
            {
                SoundID = (short)TR1SFX.LaraSplash,
                FrameNumber = 9,
                Environment = TRSFXEnvironment.Water,
            });

            // STAND_DEATH
            anim = tr1Lara.Animations[138];
            anim.Commands.Add(new TRSFXCommand
            {
                SoundID = (short)TR1SFX.LaraSplash,
                FrameNumber = 30,
                Environment = TRSFXEnvironment.Water,
            });

            // BOULDER_DEATH
            anim = tr1Lara.Animations[139];
            anim.Commands.Add(new TRSFXCommand
            {
                SoundID = (short)TR1SFX.LaraSplash,
                FrameNumber = 11,
                Environment = TRSFXEnvironment.Water,
            });

            // DEATH_JUMP
            anim = tr1Lara.Animations[145];
            anim.Commands.Add(new TRSFXCommand
            {
                SoundID = (short)TR1SFX.LaraSplash,
                FrameNumber = 10,
                Environment = TRSFXEnvironment.Water,
            });

            // ROLL_CONTINUE
            anim = tr1Lara.Animations[147];
            (anim.Commands[1] as TRSFXCommand).Environment = TRSFXEnvironment.Land;
            anim.Commands.Add(new TRSFXCommand
            {
                SoundID = (short)TR1SFX.LaraSplash,
                FrameNumber = 1,
                Environment = TRSFXEnvironment.Water,
            });

            // SWANDIVE_ROLL
            anim = tr1Lara.Animations[151];
            anim.Commands.Add(new TRSFXCommand
            {
                SoundID = (short)TR1SFX.LaraSplash,
                FrameNumber = 1,
                Environment = TRSFXEnvironment.Water,
            });

            // SWANDIVE_DEATH
            anim = tr1Lara.Animations[155];
            anim.Commands.Add(new TRSFXCommand
            {
                SoundID = (short)TR1SFX.LaraSplash,
                FrameNumber = 1,
                Environment = TRSFXEnvironment.Water,
            });
        }
    }

    private static void ImportTR2Jumping(TRModel tr1Lara, TRModel tr2Lara)
    {
        // Replicate TR2's responsive jumping state change ranges. The engine will
        // handle the new responsive state based on the player's setting, picking
        // either TR2's ranges or OG delayed jumping. Target state becomes "jump"
        // when the animation is reached, so responsive state is just a placeholder.
        IEnumerable<TRStateChange> runToJumpChanges = tr2Lara.Animations[0].Changes
            .Where(s => s.StateID == 3);

        foreach (TRStateChange change in runToJumpChanges)
        {
            TRStateChange copy = change.Clone();
            copy.StateID = (ushort)InjState.Responsive;
            tr1Lara.Animations[0].Changes.Add(copy);
        }
    }

    private static void ImportTR2Gliding(TRModel tr1Lara, TRModel tr2Lara)
    {
        Dictionary<int, InjAnim> map = new()
        {
            [198] = InjAnim.SwimToHuddle,
            [199] = InjAnim.SwimToSprawl,
            [200] = InjAnim.SwimToMedium,
        };

        foreach (int tr2Idx in map.Keys)
        {
            TRAnimation anim = tr2Lara.Animations[tr2Idx];
            tr1Lara.Animations.Add(anim);
        }

        TRAnimation swimAnim = tr1Lara.Animations[86];
        TRStateChange glideChange = swimAnim.Changes.Find(c => c.StateID == 18);
        TRStateChange responsiveGlideChange = new()
        {
            StateID = (ushort)InjState.Responsive,
            Dispatches = new(),
        };
        int index = swimAnim.Changes.IndexOf(glideChange);
        swimAnim.Changes.Insert(index + 1, responsiveGlideChange);

        // Duplicate the original dispatches, but the lowest frame range needs incresing by 3.
        TRAnimDispatch ogDispatch = glideChange.Dispatches.Find(d => d.Low == 0).Clone();
        ogDispatch.High = 5;
        responsiveGlideChange.Dispatches.Add(ogDispatch);
        responsiveGlideChange.Dispatches.Add(glideChange.Dispatches.Find(d => d.Low != 0).Clone());

        List<TRAnimDispatch> extraDispatches = tr2Lara.Animations[86].Changes.Find(c => c.StateID == 18)
            .Dispatches.FindAll(d => d.NextAnimation != 87);
        foreach (TRAnimDispatch dispatch in extraDispatches.Select(d => d.Clone()))
        {
            dispatch.NextAnimation = (short)map[dispatch.NextAnimation];
            responsiveGlideChange.Dispatches.Add(dispatch);
        }

        // Not essential, but easier to read in WADTool
        responsiveGlideChange.Dispatches.Sort((d1, d2) => d1.Low.CompareTo(d2.Low));
    }

    static void ResetLevel(TR1Level level)
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
            [TR1Type.Lara] = level.Models[TR1Type.Lara],
        };
    }

    private static void ExportLaraWAD(TR1Level level)
    {
        // Generate the injection's effect on a regular level to allow TRLE builders to utilise
        // the new animations while also being able to edit the defaults. This is a stripped back
        // level file that can be opened in WADTool.
        List<TRObjectTexture> originalInfos = new(level.ObjectTextures);
        Dictionary<ushort, ushort> texMap = new();
        level.Models[TR1Type.Lara].Meshes
            .SelectMany(m => m.TexturedFaces)
            .Select(f => f.Texture)
            .Distinct()
            .ToList().ForEach(t => texMap[t] = (ushort)texMap.Count);

        TR1TexturePacker packer = new(level);
        var laraRegions = packer.GetMeshRegions(level.Models[TR1Type.Lara].Meshes)
            .Values.SelectMany(v => v);
        
        level.Images8 = new() { new() { Pixels = new byte[TRConsts.TPageSize] } };
        level.ObjectTextures.Clear();

        packer = new(level);
        packer.AddRectangles(laraRegions);
        packer.Pack(true);

        level.ObjectTextures.AddRange(texMap.Keys.Select(t => originalInfos[t]));
        level.Models[TR1Type.Lara].Meshes
            .SelectMany(m => m.TexturedFaces)
            .ToList().ForEach(f => f.Texture = texMap[f.Texture]);

        ExportZip(level);
        using ZipArchive archive = ZipFile.Open(_wadZipPath, ZipArchiveMode.Update);
        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            // Prevent the zip changing despite the contents having not. C# provides no way to do this on create.
            entry.LastWriteTime = _wadZipPlaceholderDate;
        }
    }

    private static void ExportZip(TR1Level level)
    {
        using FileStream stream = new(_wadZipPath, FileMode.Create);
        using ZipArchive zip = new(stream, ZipArchiveMode.Create);

        {
            using MemoryStream ms = new();
            _control1.Write(level, ms);
            byte[] phdRaw = ms.ToArray();
            ZipArchiveEntry entry = zip.CreateEntry("lara_anim_ext.phd", CompressionLevel.Optimal);
            using Stream zipStream = entry.Open();
            zipStream.Write(phdRaw, 0, phdRaw.Length);
        }

        {
            XmlDocument doc = new();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", null, null);
            doc.InsertBefore(xmlDeclaration, doc.DocumentElement);

            XmlElement wadSoundsElement = doc.CreateElement("WadSounds");
            doc.AppendChild(wadSoundsElement);
            XmlElement soundsElement = doc.CreateElement("SoundInfos");
            wadSoundsElement.AppendChild(soundsElement);

            foreach (var (id, sfx) in level.SoundEffects)
            {
                soundsElement.AppendChild(SFXToXML(doc, id, sfx));

                for (int i = 0; i < sfx.Samples.Count; i++)
                {
                    ZipArchiveEntry entry = zip.CreateEntry($"SFX/{id}_s{i}.wav", CompressionLevel.Optimal);
                    using Stream zipStream = entry.Open();
                    zipStream.Write(sfx.Samples[i], 0, sfx.Samples[i].Length);
                }
            }

            {
                using StringWriter stringWriter = new();
                using var xmlTextWriter = XmlWriter.Create(stringWriter, new()
                {
                    Indent = true,
                    IndentChars = string.Empty.PadLeft(4),
                });
                doc.WriteTo(xmlTextWriter);
                xmlTextWriter.Flush();
                byte[] xmlRaw = Encoding.UTF8.GetBytes(stringWriter.GetStringBuilder().ToString());

                ZipArchiveEntry entry = zip.CreateEntry("SFX/wet-feet.xml", CompressionLevel.Optimal);
                using Stream zipStream = entry.Open();
                zipStream.Write(xmlRaw, 0, xmlRaw.Length);
            }
        }
    }

    private static XmlElement SFXToXML(XmlDocument doc, TR1SFX id, TR1SoundEffect sfx)
    {
        XmlElement root = doc.CreateElement("WadSoundInfo");
        root.AppendChild(CreateElement(doc, "Id", (int)id));
        root.AppendChild(CreateElement(doc, "Name", id.ToString().ToUpper()));
        root.AppendChild(CreateElement(doc, "Volume", TraitToPerc(sfx.Volume)));
        root.AppendChild(CreateElement(doc, "Chance", sfx.Chance == 0 ? 100 : TraitToPerc(sfx.Chance)));
        root.AppendChild(CreateElement(doc, "DisablePanning", !sfx.Pan));
        root.AppendChild(CreateElement(doc, "RandomizePitch", sfx.RandomizePitch));
        root.AppendChild(CreateElement(doc, "RandomizeVolume", sfx.RandomizeVolume));
        root.AppendChild(CreateElement(doc, "LoopBehaviour", "None"));

        XmlElement samples = doc.CreateElement("Samples");
        root.AppendChild(samples);
        for (int i = 0; i < sfx.Samples.Count; i++)
        {
            XmlElement sample = doc.CreateElement("WadSample");
            samples.AppendChild(sample);
            sample.AppendChild(CreateElement(doc, "FileName", $"{id}_s{i}.wav"));
        }

        root.AppendChild(CreateElement(doc, "Global", true));
        root.AppendChild(CreateElement(doc, "Indexed", true));

        return root;
    }

    private static XmlElement CreateElement(XmlDocument doc, string name, object value)
    {
        XmlElement element = doc.CreateElement(name);
        string valueStr = value.ToString();
        if (value is bool)
        {
            valueStr = valueStr.ToLower();
        }

        element.InnerText = valueStr;
        return element;
    }

    private static int TraitToPerc(int trait)
        => (int)Math.Ceiling(100 * ((double)trait / short.MaxValue));
}
