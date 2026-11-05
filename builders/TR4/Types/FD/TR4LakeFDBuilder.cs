using System.Numerics;
using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR4.FD;

public class TR4LakeFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR4, InjectionType.FDFix, "lake_fd");
        CreateDefaultTests(data, $"TR4/{TR4LevelNames.LAKE}");
        data.FloorEdits.AddRange(FixCrocodileTriggers());

        return [data];
    }

    private static IEnumerable<TRFloorDataEdit> FixCrocodileTriggers()
    {
        // Room 155 contains just one trigger gate but 4 triggers in total expect one. Inject
        // the additional gates and trigger them from room 28.
        var level = _control4.Read($"Resources/TR4/{TR4LevelNames.LAKE}");

        {
            var room155 = level.Rooms[155];
            const ushort z = 4;
            foreach (var vec in new Vector2[]
            {
                new(2, 6),
                new(4, 3),
                new(5, 0),
            })
            {
                var gate = new TR1Entity
                {
                    TypeID = (TR1Type)(uint)TR4Type.TriggerTriggerer,
                    Room = 155,
                    X = room155.Info.X + (int)vec.X * TRConsts.Step4 + TRConsts.Step2,
                    Z = room155.Info.Z + z * TRConsts.Step4 + TRConsts.Step2,
                    Y = (int)vec.Y * TRConsts.Step1,
                };

                yield return new()
                {
                    RoomIndex = gate.Room,
                    X = (ushort)vec.X,
                    Z = z,
                    Fixes = [new FDTrigItem { Item = gate }],
                };
            }
        }

        for (ushort z = 1; z < 3; z++)
        {
            var trigger = GetTrigger(level, 28, 2, z);
            for (var i = 0; i < 3; i++)
            {
                trigger.Actions.Add(new()
                {
                    Parameter = (short)(level.Entities.Count + i),
                });
            }
            yield return MakeTrigger(level, 28, 2, z, trigger);
        }
    }
}
