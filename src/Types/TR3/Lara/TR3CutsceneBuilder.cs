using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Lara;

public class TR3CutsceneBuilder : InjectionBuilder
{
    private static readonly List<TR3Type> _actors =
    [
        TR3Type.Lara, TR3Type.CutsceneActor1, TR3Type.CutsceneActor2, TR3Type.CutsceneActor3,
        TR3Type.CutsceneActor4, TR3Type.CutsceneActor5, TR3Type.CutsceneActor6,
        TR3Type.CutsceneActor7, TR3Type.CutsceneActor8, TR3Type.CutsceneActor9,
    ];

    public override string ID => "tr3_cutscenes";

    public override List<InjectionData> Build()
    {
        return
        [
            CreateCut6Data(),
            CreateCut9Data(),
            CreateCut1Data(),
            CreateCut4Data(),
            CreateCut2Data(),
            CreateCut5Data(),
            CreateCut11Data(),
            CreateCut7Data(),
            CreateCut8Data(),
            CreateCut3Data(),
            CreateCut12Data(),
        ];
    }

    private static InjectionData CreateBaseData(string levelName)
    {
        var level = _control3.Read($"Resources/TR3/{levelName}");
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

        CreateModelLevel(level, actors);
        level.SoundEffects.Clear();
        level.Images16.Clear();
        level.Images8.Clear();

        return InjectionData.Create(level, InjectionType.General,
            $"{Path.GetFileNameWithoutExtension(levelName).ToLower()}_setup");
    }

    private static InjectionData CreateCommonCutData(string levelName, short laraRot)
    {
        var cut = _control3.Read($"Resources/TR3/{levelName}");
        var data = CreateBaseData(levelName);

        var laraIdx = cut.Entities.FindIndex(e => e.TypeID == TR3Type.Lara);
        if (cut.Entities[laraIdx].Angle != laraRot)
        {
            data.ItemPosEdits.Add(ItemBuilder.SetAngle(cut, (short)laraIdx, laraRot));
        }

        return data;
    }

    private static InjectionData CreateCut6Data()
    {
        var data = CreateCommonCutData(TR3LevelNames.JUNGLE_CUT, 16384);

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

        return data;
    }

    private static InjectionData CreateCut9Data()
        => CreateCommonCutData(TR3LevelNames.RUINS_CUT, 16384);

    private static InjectionData CreateCut1Data()
        => CreateCommonCutData(TR3LevelNames.COASTAL_CUT, 16384);

    private static InjectionData CreateCut4Data()
        => CreateCommonCutData(TR3LevelNames.CRASH_CUT, 16384);

    private static InjectionData CreateCut2Data()
        => CreateCommonCutData(TR3LevelNames.THAMES_CUT, -16384);

    private static InjectionData CreateCut5Data()
        => CreateCommonCutData(TR3LevelNames.ALDWYCH_CUT, 16384);

    private static InjectionData CreateCut11Data()
        => CreateCommonCutData(TR3LevelNames.LUDS_CUT, 16384);

    private static InjectionData CreateCut7Data()
        => CreateCommonCutData(TR3LevelNames.NEVADA_CUT, 16384);

    private static InjectionData CreateCut8Data()
        => CreateCommonCutData(TR3LevelNames.HSC_CUT, 16384);

    private static InjectionData CreateCut3Data()
        => CreateCommonCutData(TR3LevelNames.ANTARC_CUT, 16384);

    private static InjectionData CreateCut12Data()
        => CreateCommonCutData(TR3LevelNames.TINNOS_CUT, 16384);
}
