using System.Diagnostics;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.FD;

public class TR1MinesFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR1, InjectionType.FDFix, "mines_fd");
        CreateDefaultTests(data, TR1LevelNames.MINES);

        var level = _control1.Read($"Resources/{TR1LevelNames.MINES}");
        data.FloorEdits =
        [
            MakeMusicOneShot(86, 1, 5),
            MakeMusicOneShot(86, 2, 5),
            .. CreateCabinFlipmapFix(),
            .. CreatePistolsFix(level),
        ];

        return [data];
    }

    private static List<TRFloorDataEdit> CreateCabinFlipmapFix()
    {
        // Fix flipmap issues if the player goes to the lever beyond cowboy first then returns to the cabin.
        List<TRFloorDataEdit> edits = new();

        for (ushort z = 2; z < 4; z++)
        {
            edits.Add(new()
            {
                RoomIndex = 85,
                X = 2,
                Z = z,
                Fixes = new()
                {
                    new FDTrigParamFix
                    {
                        ActionType = FDTrigAction.FlipOn,
                        OldParam = 0,
                        NewParam = 3,
                    },
                },
            });

            edits.Add(new()
            {
                RoomIndex = 85,
                X = 3,
                Z = z,
                Fixes = new()
                {
                    new FDTrigParamFix
                    {
                        ActionType = FDTrigAction.FlipOff,
                        OldParam = 0,
                        NewParam = 3,
                    },
                },
            });
        }

        return edits;
    }

    private static IEnumerable<TRFloorDataEdit> CreatePistolsFix(TR1Level level)
    {
        for (short r = 96; r < 99; r++)
        {
            var room = level.Rooms[r];
            for (ushort x = 1; x < room.NumXSectors - 1; x++)
            {
                for (ushort z = 1; z < room.NumZSectors - 1; z++)
                {
                    var sector = room.GetSector(x, z, TRUnit.Sector);
                    if (sector.FDIndex == 0
                        || level.FloorData[sector.FDIndex].OfType<FDTriggerEntry>().FirstOrDefault() is not FDTriggerEntry trigger)
                    {
                        continue;
                    }

                    Debug.Assert(trigger.Actions.Count == 1);
                    Debug.Assert(level.Entities[trigger.Actions[0].Parameter].TypeID == TR1Type.Pistols_S_P);
                    trigger.OneShot = false;
                    yield return MakeTrigger(level, r, x, z, trigger);
                }
            }
        }
    }
}
