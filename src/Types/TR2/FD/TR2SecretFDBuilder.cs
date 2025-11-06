using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.FD;

public class TR2SecretFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        var result = new List<InjectionData>();
        foreach (var (levelName, binName) in _tr2NameMap)
        {
            var level = _control2.Read($"Resources/{levelName}");
            var edits = DeleteSecretTriggers(level);
            if (edits.Count == 0)
            {
                continue;
            }

            var data = InjectionData.Create(TRGameVersion.TR2, InjectionType.General, $"{binName}_secret_fd");
            CreateDefaultTests(data, levelName);
            data.FloorEdits.AddRange(edits);
            if (levelName == TR2LevelNames.BARTOLI)
            {
                // Make the Jade dragon disappear after flip map
                data.FloorEdits.Add(MakeTrigger(level, 143, 4, 1, new()
                {
                    Mask = 31,
                    OneShot = true,
                    Actions = [new() { Parameter = 125 }],
                }));
            }
            result.Add(data);
        }

        return result;
    }

    private static List<TRFloorDataEdit> DeleteSecretTriggers(TR2Level level)
    {
        var result = new List<TRFloorDataEdit>();
        for (short r = 0; r < level.Rooms.Count; r++)
        {
            var room = level.Rooms[r];
            for (ushort x = 1; x < room.NumXSectors - 1; x++)
            {
                for (ushort z = 1; z < room.NumZSectors - 1; z++)
                {
                    var sector = room.GetSector(x, z, TRUnit.Sector);
                    if (sector.FDIndex == 0)
                    {
                        continue;
                    }

                    var entries = level.FloorData[sector.FDIndex];
                    var trigger = entries.OfType<FDTriggerEntry>().FirstOrDefault();
                    if (trigger == null || !trigger.Actions.Any(a => a.Action == FDTrigAction.SecretFound))
                    {
                        continue;
                    }

                    trigger.Actions.RemoveAll(a => a.Action == FDTrigAction.SecretFound);
                    result.Add(new()
                    {
                        RoomIndex = r,
                        X = x,
                        Z = z,
                        Fixes =
                        [
                            trigger.Actions.Count == 0
                                ? new FDTrigDelete()
                                : MakeTrigFix(sector, level.FloorData),
                        ],
                    });
                }
            }
        }

        return result;
    }
}
