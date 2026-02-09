using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Lara;

public class TR3CutsceneBuilder : InjectionBuilder
{
    private static readonly List<CutSetup> _setups =
    [
        new(TR3LevelNames.JUNGLE_CUT, 16384, [TR3Type.CutsceneActor1], AmendJungleCut),
        new(TR3LevelNames.RUINS_CUT, 16384, [TR3Type.CutsceneActor1, TR3Type.CutsceneActor8]),
        new(TR3LevelNames.COASTAL_CUT, 16384, [TR3Type.CutsceneActor1]),
        new(TR3LevelNames.CRASH_CUT, 16384, []),
        new(TR3LevelNames.THAMES_CUT, -16384, [TR3Type.CutsceneActor7]),
        new(TR3LevelNames.LUDS_CUT, 16384, []),
        new(TR3LevelNames.NEVADA_CUT, 16384, []),
        new(TR3LevelNames.HSC_CUT, 16384, [], AmendHSCCut),
        new(TR3LevelNames.ANTARC_CUT, 16384, [TR3Type.CutsceneActor8]),
        new(TR3LevelNames.TINNOS_CUT, 16384, [TR3Type.CutsceneActor1, TR3Type.CutsceneActor4]),
    ];

    public override string ID => "tr3_cutscenes";

    public override List<InjectionData> Build()
    {
        return [.. _setups.Select(s => s.CreateData())];
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

    private static void AmendHSCCut(InjectionData data)
    {
        // Remove the draw guns command from Lara for the drink can, now handled in Lua.
        data.AnimCommands.Clear();
        data.Animations[0].NumAnimCommands = 0;
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
    }
}
