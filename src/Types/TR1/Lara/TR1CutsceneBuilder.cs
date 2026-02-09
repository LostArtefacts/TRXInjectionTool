using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Lara;

public class TR1CutsceneBuilder : InjectionBuilder
{
    public override string ID => "tr1_cutscenes";

    public override List<InjectionData> Build()
    {
        return
        [
            CreateCut1Data(),
            CreateCut2Data(),
            CreateCut3Data(),
            CreateCut4Data(),
        ];
    }

    private static TR1Level CreateBaseLevel()
    {
        var level = _control1.Read($"Resources/{TR1LevelNames.ATLANTIS}");
        CreateModelLevel(level, TR1Type.Lara, TR1Type.LaraPistolAnim_H);
        return level;
    }

    private static InjectionData CreateCommonCutData(string levelName, int laraX, int laraZ, short laraRot)
    {
        var level = CreateBaseLevel();
        var cut = _control1.Read($"Resources/{levelName}");

        level.Models[TR1Type.Lara].Animations = cut.Models[TR1Type.CutsceneActor1].Animations;
        if (levelName == TR1LevelNames.QUALOPEC_CUT)
        {
            level.Models[TR1Type.Lara].Animations[0].Commands.Add(new TRFXCommand
            {
                EffectID = (short)TR1FX.DrawRightGun,
                FrameNumber = 1,
            });

            for (var type = TR1Type.CutsceneActor2; type < TR1Type.CutsceneActor4; type++)
            {
                var model = level.Models[type] = cut.Models[type];
                model.Animations[0].NextFrame = (ushort)model.Animations[0].FrameEnd;
                model.MeshTrees.Clear();
                model.Meshes.Clear();
            }
        }

        level.Models[TR1Type.Lara].Animations[0].NextFrame = (ushort)level.Models[TR1Type.Lara].Animations[0].FrameEnd;

        var data = InjectionData.Create(level, InjectionType.General,
            $"{Path.GetFileNameWithoutExtension(levelName).ToLower()}_setup");

        var lara = cut.Entities[0];
        lara.TypeID = TR1Type.Lara;
        lara.X = laraX;
        lara.Z = laraZ;
        lara.Angle = laraRot;

        data.ItemPosEdits.Add(new()
        {
            Index = 0,
            Item = lara,
        });
        data.ItemFlagEdits.Add(new()
        {
            Index = 0,
            Item = lara,
        });

        return data;
    }

    private static InjectionData CreateCut1Data()
        => CreateCommonCutData(TR1LevelNames.QUALOPEC_CUT, 36668, 63180, -23312);

    private static InjectionData CreateCut2Data()
        => CreateCommonCutData(TR1LevelNames.TIHOCAN_CUT, 51962, 53760, 16380);

    private static InjectionData CreateCut3Data()
    {
        var level = CreateBaseLevel();
        var cut = _control1.Read($"Resources/{TR1LevelNames.MINES_CUT}");

        // Lara needs to be present, so we just give her empty frames and have her hidden
        level.Models[TR1Type.Lara].Animations =
        [
            new()
            {
                FrameRate = 1,
                FrameEnd = cut.Models[TR1Type.CutsceneActor2].Animations[0].FrameEnd,
                Accel = new(),
                Speed = new(),
                Commands =
                [
                    new TRFXCommand
                    {
                        // Replace old GF flip map sequence
                        EffectID = (short)TR1FX.FlipMap,
                        FrameNumber = 1,
                    },
                    new TRFXCommand
                    {
                        EffectID = (short)TR1FX.HideItem,
                        FrameNumber = 1,
                    }
                ],
                Frames = [.. cut.Models[TR1Type.CutsceneActor2].Animations[0].Frames.Select(f => new TRAnimFrame
                {
                    Bounds = new()
                    {
                        MinX = -105,
                        MaxX = 106,
                        MinY = -767,
                        MaxY = 22,
                        MinZ = -65,
                        MaxZ = 91,
                    },
                    OffsetY = -431,
                    Rotations = [.. Enumerable.Repeat(0, 15).Select(r => new TRAnimFrameRotation())],
                })],
            }
        ];

        // Hide scion's shadow
        level.Models[TR1Type.CutsceneActor3] = cut.Models[TR1Type.CutsceneActor3];
        level.Models[TR1Type.CutsceneActor3].Animations[0].Commands.Add(new TRFXCommand
        {
            EffectID = (short)TR1FX.ShadowOff,
            FrameNumber = 1,
        });

        var data = InjectionData.Create(level, InjectionType.General, "cut3_setup", removeMeshData: true);

        var lara = cut.Entities[2].Clone() as TR1Entity;
        lara.TypeID = TR1Type.Lara;
        lara.Angle = 16384;
        level.Entities.Add(lara);

        data.FloorEdits.Add(new()
        {
            RoomIndex = lara.Room,
            X = 3,
            Z = 5,
            Fixes =
            [
                new FDTrigItem
                {
                    Item = lara,
                }
            ],
        });

        return data;
    }

    private static InjectionData CreateCut4Data()
    {
        var level = CreateBaseLevel();
        var cut = _control1.Read($"Resources/{TR1LevelNames.ATLANTIS_CUT}");

        level.Models[TR1Type.Lara].Animations = cut.Models[TR1Type.CutsceneActor1].Animations;

        // Fix drifty Lara
        var lockFrame = level.Models[TR1Type.Lara].Animations[0].Frames[318];
        for (int i = 319; i < 380; i++)
        {
            level.Models[TR1Type.Lara].Animations[0].Frames[i] = lockFrame.Clone();
        }

        var data = InjectionData.Create(level, InjectionType.General, "cut4_setup");

        var lara = cut.Entities[1];
        lara.TypeID = TR1Type.Lara;
        lara.Angle = 16384;

        data.ItemPosEdits.Add(new()
        {
            Index = 1,
            Item = lara,
        });
        data.ItemFlagEdits.Add(new()
        {
            Index = 1,
            Item = lara,
        });

        // Rather than have the scion as an actor, make it a normal item that's active to begin with.
        // Otherwise its animation is all over the place without the hardcoded TR1 behaviour to not
        // set its position to the camera's, like every other actor.
        data.ObjectTypeEdits.Add(new()
        {
            BaseType = (int)TR1Type.CutsceneActor4,
            TargetType = (int)TR1Type.ScionPiece4_S_P,
        });
        var scion = cut.Entities[4];
        scion.CodeBits = TRConsts.FullMask;
        data.ItemFlagEdits.Add(new()
        {
            Index = 4,
            Item = scion,
        });

        return data;
    }
}
