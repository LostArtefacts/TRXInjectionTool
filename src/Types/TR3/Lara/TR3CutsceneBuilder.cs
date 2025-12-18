using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Lara;

public class TR3CutsceneBuilder : InjectionBuilder
{
    public override string ID => "tr3_cutscenes";

    public override List<InjectionData> Build()
    {
        return
        [
            CreateCut6Data(),
            CreateCut2Data(),
        ];
    }

    private static InjectionData CreateCommonCutData(string levelName, short laraRot)
    {
        var cut = _control3.Read($"Resources/TR3/{levelName}");
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.General,
            $"{Path.GetFileNameWithoutExtension(levelName).ToLower()}_setup");

        var laraIdx = cut.Entities.FindIndex(e => e.TypeID == TR3Type.Lara);
        data.ItemPosEdits.Add(ItemBuilder.SetAngle(cut, (short)laraIdx, laraRot));

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

    private static InjectionData CreateCut2Data()
        => CreateCommonCutData(TR3LevelNames.THAMES_CUT, -16384);
}
