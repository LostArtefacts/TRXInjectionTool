using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.FD;

public class TR1MinesFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.FDFix, "mines_fd");
        CreateDefaultTests(data, TR1LevelNames.MINES);
        data.FloorEdits = new()
        {
            MakeMusicOneShot(86, 1, 5),
            MakeMusicOneShot(86, 2, 5),
        };
        data.FloorEdits.AddRange(CreateCabinFlipmapFix());

        return new() { data };
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
}
