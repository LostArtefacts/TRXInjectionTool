using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.FD;

public class TR3AIPatrolBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        return [.. new[] { TR3LevelNames.HSC, TR3LevelNames.AREA51 }
            .Select(DuplicateAIPatrol)];
    }

    private static InjectionData DuplicateAIPatrol(string levelName)
    {
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.General, $"{_tr3NameMap[levelName]}_patrol");
        CreateDefaultTests(data, $"TR3/{levelName}");

        var level = _control3.Read($"Resources/TR3/{levelName}");
        foreach (var item in level.Entities.Where(e => e.TypeID == TR3Type.AIPatrol1_N))
        {
            var room = level.Rooms[item.Room];
            data.FloorEdits.Add(new()
            {
                RoomIndex = item.Room,
                X = (ushort)((item.X - room.Info.X) / TRConsts.Step4),
                Z = (ushort)((item.Z - room.Info.Z) / TRConsts.Step4),
                Fixes = [new FDTrigItem
                {
                    Item = new()
                    {
                        TypeID = (TR1Type)(int)TR3Type.AIPatrol1_N,
                        Room = item.Room,
                        X = item.X,
                        Y = item.Y,
                        Z = item.Z,
                        Angle = item.Angle,
                    },
                }],
            });
        }

        return data;
    }
}
