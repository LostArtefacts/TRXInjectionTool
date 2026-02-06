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
            .. CreateFuseFix(level),
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

    private static IEnumerable<TRFloorDataEdit> CreateFuseFix(TR1Level level)
    {
        // Don't one-shot the fuse at the switch
        var switchTrig = GetTrigger(level, 89, 1, 2);
        switchTrig.Actions.RemoveAll(a => a.Parameter == 42);
        yield return MakeTrigger(level, 89, 1, 2, switchTrig);

        // Trigger it in the fliped room on the way back instead
        yield return MakeTrigger(level, 90, 1, 9, new()
        {
            Mask = 31,
            Actions = [new() { Parameter = 42 }],
        });
    }
}
