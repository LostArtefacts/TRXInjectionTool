using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.FD;

public class TR1PyramidFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        InjectionData data = InjectionData.Create(InjectionType.FDFix, "pyramid_fd");
        data.FloorEdits = new()
        {
            MakeMusicOneShot(36, 4, 5),
            CreateSecretTriggerFix(),
        };

        return new() { data };
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
}
