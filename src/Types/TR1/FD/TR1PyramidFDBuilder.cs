using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.FD;

public class TR1PyramidFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.FDFix, "pyramid_fd");
        CreateDefaultTests(data, TR1LevelNames.PYRAMID);

        data.FloorEdits.Add(MakeMusicOneShot(36, 4, 5));
        data.FloorEdits.Add(CreateSecretTriggerFix());
        data.FloorEdits.AddRange(FixEmberTrigger());

        return [data];
    }

    private static TRFloorDataEdit CreateSecretTriggerFix()
    {
        // Fix the index on the final secret trigger.
        return new()
        {
            RoomIndex = 64,
            X = 2,
            Z = 1,
            Fixes = new()
            {
                new FDTrigParamFix
                {
                    ActionType = FDTrigAction.SecretFound,
                    OldParam = 0,
                    NewParam = 2,
                },
            },
        };
    }

    private static List<TRFloorDataEdit> FixEmberTrigger()
    {
        // Ember emitter 12 is on the same timed trigger as that for the secret trapdoors.
        // TRX now allows switching off these emitters, so replace with separate triggers
        // nearby instead.
        var level = _control1.Read($"Resources/{TR1LevelNames.PYRAMID}");
        var result = new List<TRFloorDataEdit>();

        var trig = GetTrigger(level, 18, 3, 4);
        trig.Actions.RemoveAll(a => a.Parameter == 12);
        result.Add(MakeTrigger(level, 18, 3, 4, trig));

        foreach (var (x, z) in new(ushort, ushort)[] { (4, 4), (4, 3), (3, 3) })
        {
            result.Add(MakeTrigger(level, 18, x, z, new()
            {
                Mask = 31,
                Actions = [new() { Parameter = 12 }],
            }));
        }

        return result;
    }
}
