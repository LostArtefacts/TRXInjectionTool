using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRImageControl;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types.TR3.Lara;

public class TR3CutsceneBuilder : InjectionBuilder
{
    private static readonly List<CutSetup> _setups =
    [
        new(TR3LevelNames.JUNGLE_CUT, 16384, [TR3Type.CutsceneActor1], postAction: AmendJungleCut),
        new(TR3LevelNames.RUINS_CUT, 16384, [TR3Type.CutsceneActor1, TR3Type.CutsceneActor8]),
        new(TR3LevelNames.COASTAL_CUT, 16384, [TR3Type.CutsceneActor1]),
        new(TR3LevelNames.CRASH_CUT, 16384, []),
        new(TR3LevelNames.THAMES_CUT, -16384, [TR3Type.CutsceneActor7], postAction: AmendThamesCut),
        new(TR3LevelNames.LUDS_CUT, 16384, [], postAction: AmendLudsCut),
        new(TR3LevelNames.NEVADA_CUT, 16384, []),
        new(TR3LevelNames.HSC_CUT, 16384, [], postAction: AmendHSCCut),
        new(TR3LevelNames.ANTARC_CUT, 16384, [TR3Type.CutsceneActor8]),
        new(TR3LevelNames.TINNOS_CUT, 16384, [TR3Type.CutsceneActor1, TR3Type.CutsceneActor4]),
    ];

    public override string ID => "tr3_cutscenes";

    public override List<InjectionData> Build()
    {
        return
        [
            .. _setups.Select(s => s.CreateData()),
            CreateCut3ShellData(),
        ];
    }

    private static void AmendJungleCut(InjectionData data)
    {
        // Room1 has incorrect portals into room 0 on the south wall (room 0 is north), which can
        // cause Room_GetSector to loop infinitely.
        var cut = _control3.Read($"Resources/TR3/{TR3LevelNames.JUNGLE_CUT}");
        var room1 = cut.Rooms[1];
        for (ushort x = 9; x < room1.NumXSectors; x++)
        {
            for (ushort z = 0; z < 3; z++)
            {
                var sector = room1.GetSector(x, z, TRUnit.Sector);
                if (sector.FDIndex == 0
                    || !cut.FloorData[sector.FDIndex].Any(e => e is FDPortalEntry p && p.Room == 0))
                {
                    continue;
                }

                data.FloorEdits.Add(new()
                {
                    RoomIndex = 1,
                    X = x,
                    Z = z,
                    Fixes = [new FDPortalOverwrite()],
                });
            }
        }

        // Lift the inside of Tony's box off the floor to see the base
        data.MeshEdits.Add(new()
        {
            ModelID = (uint)TR3Type.CutsceneActor1,
            VertexEdits = [.. Enumerable.Range(0, 9).Select(idx => new TRVertexEdit
            {
                Index = (short)idx,
                Change = new() { Y = -2 },
            })]
        });
    }

    private static void AmendThamesCut(InjectionData data)
    {
        // Rooms 18-20 are mostly inside so remove the wind flag to avoid rain there.
        var cut = _control3.Read($"Resources/TR3/{TR3LevelNames.THAMES_CUT}");
        data.FloorEdits.AddRange(FDBuilder.RemoveRoomFlags([18,19,20], TRRoomFlag.Wind, cut.Rooms));
    }

    private static void AmendLudsCut(InjectionData data)
    {
        // Prevents Lara walking on thin air.
        var cut = _control3.Read($"Resources/TR3/{TR3LevelNames.LUDS_CUT}");
        var faces = new int[] { 0,7,14,18 };
        data.RoomEdits.AddRange(faces.SelectMany(f => cut.Rooms[0].Mesh.Rectangles[f].Vertices)
            .Distinct()
            .Select(v => new TRRoomVertexMove
            {
                VertexIndex = v,
                VertexChange = new() { Y = -1536 },
            }));
    }

    private static void AmendHSCCut(InjectionData data)
    {
        // Remove the draw guns command from Lara for the drink can, now handled in Lua.
        data.AnimCommands.Clear();
        data.Animations[0].NumAnimCommands = 0;
    }

    private static InjectionData CreateCut3ShellData()
    {
        var jungle = _control3.Read($"Resources/TR3/{TR3LevelNames.JUNGLE}");
        var palette = jungle.Palette16.Select(c => c.ToColor()).ToList();
        CreateModelLevel(jungle, TR3Type.YellowShellCasing_H);
        TRFaceConverter.ConvertFlatFaces(jungle, palette);
        return InjectionData.Create(jungle, InjectionType.General, "cut3_shell");
    }

    private class CutSetup(string levelName, short laraAngle,
        List<TR3Type> hideShadowTargets, Action<InjectionData> postAction = null)
    {
        private static readonly List<TR3Type> _actors =
        [
            TR3Type.Lara, TR3Type.CutsceneActor1, TR3Type.CutsceneActor2, TR3Type.CutsceneActor3,
            TR3Type.CutsceneActor4, TR3Type.CutsceneActor5, TR3Type.CutsceneActor6,
            TR3Type.CutsceneActor7, TR3Type.CutsceneActor8, TR3Type.CutsceneActor9,
        ];

        public string LevelName { get; set; } = new(levelName);
        public short LaraAngle { get; init; } = laraAngle;
        public List<TR3Type> HideShadowTargets { get; set; } = hideShadowTargets;
        public Action<InjectionData> PostAction { get; set; } = postAction;

        public InjectionData CreateData()
        {
            var level = _control3.Read($"Resources/TR3/{LevelName}");
            var actors = _actors.Where(level.Models.ContainsKey).ToArray();
            foreach (var type in actors)
            {
                var model = level.Models[type];
                var endAnim = model.Animations[^1];
                endAnim.NextAnimation = (ushort)(model.Animations.Count - 1);
                endAnim.NextFrame = (ushort)endAnim.FrameEnd;
                model.MeshTrees.Clear();
                model.Meshes.Clear();
            }

            var laraEdit = RotateLara(level);
            HideShadows(level);

            if (levelName == TR3LevelNames.LUDS_CUT)
            {
                FixLudsLaraFrames(level.Models[TR3Type.Lara]);
            }

            CreateModelLevel(level, actors);
            level.SoundEffects.Clear();
            level.Images16.Clear();
            level.Images8.Clear();

            var data = InjectionData.Create(level, InjectionType.General,
                $"{Path.GetFileNameWithoutExtension(levelName).ToLower()}_setup");
            if (laraEdit != null)
            {
                data.ItemPosEdits.Add(laraEdit);
            }

            PostAction?.Invoke(data);
            return data;
        }

        private void HideShadows(TR3Level level)
        {
            foreach (var type in HideShadowTargets)
            {
                level.Models[type].Animations[0].Commands.Add(new TRFXCommand
                {
                    EffectID = 59,
                    FrameNumber = 1,
                });
            }

            if (LevelName == TR3LevelNames.TINNOS_CUT)
            {
                // Hide Willard's shadow when he falls into the pit
                level.Models[TR3Type.CutsceneActor5].Animations[8].Commands.Add(new TRFXCommand
                {
                    EffectID = 59,
                    FrameNumber = 343,
                });
                // Spider Willard hidden at the start, unhide when he shows up
                level.Models[TR3Type.CutsceneActor1].Animations[9].Commands.Add(new TRFXCommand
                {
                    EffectID = 58,
                    FrameNumber = 245,
                });
            }
        }

        private TRItemPosEdit RotateLara(TR3Level level)
        {
            var laraIdx = level.Entities.FindIndex(e => e.TypeID == TR3Type.Lara);
            if (level.Entities[laraIdx].Angle != LaraAngle)
            {
                return ItemBuilder.SetAngle(level, (short)laraIdx, LaraAngle);
            }
            return null;
        }

        private static void FixLudsLaraFrames(TRModel lara)
        {
            // Avoid Lara being frozen and suddenly coming to life. Texture changes
            // are done in tandem in AmendLudsCut so Lara isn't floating.
            var anim = lara.Animations[0];
            for (int i = 0; i < 50; i++)
            {
                anim.Frames[i].OffsetX = 5300;
                anim.Frames[i].Bounds.MaxX += 919;
                anim.Frames[i].Bounds.MinX += 919;
            }

            const int shift = 1439;
            for (int i = 46; i >= 0; i--)
            {
                anim.Frames[96 - i] = anim.Frames[122 + 46 - i].Clone();
                var frame = anim.Frames[96 - i];
                frame.OffsetX += shift;
                frame.Bounds.MaxX += shift;
                frame.Bounds.MinX += shift;
                frame.Rotations[14] = new();
            }
        }
    }
}
