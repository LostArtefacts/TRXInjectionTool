using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Lara;

public class TR1LaraAnimBuilder : InjectionBuilder
{
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
    };

    enum InjState : int
    {
        Twist = 57,
        UWRoll = 58,
        Wade = 59,
    };

    public override List<InjectionData> Build()
    {
        List<InjectionData> dataGroup = new();

        {
            TR1Level caves = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
            TR2Level wall = _control2.Read($"Resources/{TR2LevelNames.GW}");
            ResetLevel(caves);

            TRModel tr1Lara = caves.Models[TR1Type.Lara];
            TRModel tr2Lara = wall.Models[TR2Type.Lara];
            ImportJumpTwist(tr1Lara, tr2Lara);
            ImportUWRoll(tr1Lara);
            ImportWading(tr1Lara, tr2Lara);
            ImportWetFeet(tr1Lara, caves);

            // This can be opened in WADTool for debugging what ends up in the game itself.
            _control1.Write(caves, "Output/ExtendedLaraAnims.phd");

            InjectionData data = InjectionData.Create(caves, InjectionType.LaraAnims, "lara_animations");
            dataGroup.Add(data);
        }

        {
            // TR2-style jumping is separate as the anim ranges are replaced in this case,
            // and we have to respect the config option for OG jumping.
            InjectionData data = InjectionData.Create(InjectionType.LaraJumps, "lara_jumping");
            ImportTR2Jumping(data);
            dataGroup.Add(data);
        }

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
        tr1Lara.Animations[17].Changes.Add(new()
        {
            StateID = (ushort)InjState.Twist,
            Dispatches = new()
            {
                new()
                {
                    Low = 0,
                    High = 4,
                    NextAnimation = (short)InjAnim.RunJumpRollStart,
                    NextFrame = 0,
                }
            }
        });
        // Running jump left forward
        tr1Lara.Animations[19].Changes.Add(new()
        {
            StateID = (ushort)InjState.Twist,
            Dispatches = new()
            {
                new()
                {
                    Low = 0,
                    High = 4,
                    NextAnimation = (short)InjAnim.RunJumpRollStart,
                    NextFrame = 0,
                }
            }
        });
        // Standing jump back
        tr1Lara.Animations[75].Changes.Add(new()
        {
            StateID = (ushort)InjState.Twist,
            Dispatches = new()
            {
                new()
                {
                    Low = 0,
                    High = 4,
                    NextAnimation = (short)InjAnim.JumpBackRollStart,
                    NextFrame = 0,
                }
            }
        });
        // Standing jump forward
        tr1Lara.Animations[77].Changes.Add(new()
        {
            StateID = (ushort)InjState.Twist,
            Dispatches = new()
            {
                new()
                {
                    Low = 0,
                    High = 4,
                    NextAnimation = (short)InjAnim.JumpFwdRollStart,
                    NextFrame = 0,
                }
            }
        });
        // Freefall to somersault
        tr1Lara.Animations[153].Changes.Add(new()
        {
            StateID = (ushort)InjState.Twist,
            Dispatches = new()
            {
                new()
                {
                    Low = 0,
                    High = 9,
                    NextAnimation = (short)InjAnim.Somersault,
                    NextFrame = 0,
                }
            }
        });
    }

    static void ImportUWRoll(TRModel tr1Lara)
    {
        // This is a trimmed down anim set, only the two we're interested in are present.
        // See PR 1272/1276
        TR1Level animLevel = _control1.Read("Resources/TR1/Lara/uw_roll.phd");
        TRModel tr2Lara = animLevel.Models[TR1Type.Lara];

        TRAnimation uwRollStart = tr2Lara.Animations[0];
        TRAnimation uwRollEnd = tr2Lara.Animations[1];

        uwRollStart.NextAnimation = (ushort)InjAnim.UWRollEnd;
        uwRollEnd.NextAnimation = 108;
        tr1Lara.Animations.Add(uwRollStart);
        tr1Lara.Animations.Add(uwRollEnd);

        uwRollStart.StateID = (ushort)InjState.UWRoll;
        uwRollEnd.StateID = (ushort)InjState.UWRoll;

        (uwRollStart.Commands.Find(c => c is TRSFXCommand) as TRSFXCommand).Environment = TRSFXEnvironment.Any;

        int[] changeAnims = new int[] { 86, 87, 108 }; // Swimming, gliding and UW idle
        foreach (int changeAnim in changeAnims)
        {
            tr1Lara.Animations[changeAnim].Changes.Add(new()
            {
                StateID = (ushort)InjState.UWRoll,
                Dispatches = new()
                {
                    new()
                    {
                        Low = 0,
                        High = (short)(tr1Lara.Animations[changeAnim].FrameEnd + 1),
                        NextAnimation = (short)InjAnim.UWRollStart,
                        NextFrame = 0,
                    }
                }
            });
        }
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

        // Additional state changes
        {
            // Step left into surf swim left
            TRAnimation anim = tr1Lara.Animations[65];
            anim.Changes.Add(new()
            {
                StateID = 48,
                Dispatches = new()
                {
                    new()
                    {
                        Low = 0,
                        High = 25,
                        NextAnimation = 143,
                        NextFrame = 0,
                    }
                }
            });

            // Step right into surf swim right
            anim = tr1Lara.Animations[67];
            anim.Changes.Add(new()
            {
                StateID = 49,
                Dispatches = new()
                {
                    new()
                    {
                        Low = 0,
                        High = 25,
                        NextAnimation = 144,
                        NextFrame = 0,
                    }
                }
            });

            // Surf swim left into step left
            anim = tr1Lara.Animations[143];
            anim.Changes.Add(new()
            {
                StateID = 22,
                Dispatches = new()
                {
                    new()
                    {
                        Low = 0,
                        High = 45,
                        NextAnimation = 65,
                        NextFrame = 0,
                    }
                }
            });

            // Surf swim right into step right
            anim = tr1Lara.Animations[144];
            anim.Changes.Add(new()
            {
                StateID = 21,
                Dispatches = new()
                {
                    new()
                    {
                        Low = 0,
                        High = 45,
                        NextAnimation = 67,
                        NextFrame = 0,
                    }
                }
            });
        }
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
        foreach (TRAnimation anim in tr1Lara.Animations)
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

    private static void ImportTR2Jumping(InjectionData data)
    {
        // Replace the ranges in which Lara is permitted to jump.
        data.AnimRangeEdits.Add(new()
        {
            ModelID = (uint)TR1Type.Lara,
            AnimIndex = 0,
            Ranges = new()
            {
                new()
                {
                    ChangeOffset = 2,
                    RangeOffset = 0,
                    Low = 11,
                    High = 22,
                },
                new()
                {
                    ChangeOffset = 2,
                    RangeOffset = 1,
                    Low = 0,
                    High = 11,
                }
            }
        });
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
}
