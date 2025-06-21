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
            if (!edits.Any())
            {
                continue;
            }

            var data = InjectionData.Create(TRGameVersion.TR2, InjectionType.General, $"{binName}_secret_fd");
            CreateDefaultTests(data, levelName);
            data.FloorEdits.AddRange(edits);
            result.Add(data);
        }

        return result;
    }

    private static IEnumerable<TRFloorDataEdit> DeleteSecretTriggers(TR2Level level)
    {
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

                    var fixes = level.FloorData[sector.FDIndex]
                        .OfType<FDTriggerEntry>()
                        .Where(t => t.Actions.Any(a => a.Action == FDTrigAction.SecretFound))
                        .Select(t => new TRFloorDataEdit
                            {
                                RoomIndex = r,
                                X = x,
                                Z = z,
                                Fixes = t.Actions.Where(a => a.Action == FDTrigAction.SecretFound)
                                    .Select(a => new FDTrigParamFix
                                    {
                                        ActionType = a.Action,
                                        OldParam = a.Parameter,
                                        NewParam = -1,
                                    }).Cast<FDFix>().ToList(),
                            });
                    foreach (var fix in fixes)
                    {
                        yield return fix;
                    }
                }
            }
        }
    }
}
