using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.FD;

public class TR1ToQFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.FDFix, "qualopec_fd");
        data.FloorEdits = new()
        {
            // We make the track in this corridor one shot and "nullify" the
            // nearby antipad to play the Caves ambience (which won't play),
            // so this in effect prevents the actual ambience being killed.
            MakeMusicOneShot(25, 5, 1),
            new()
            {
                RoomIndex = 25,
                X = 3,
                Z = 1,
                Fixes = new()
                {
                    new FDTrigParamFix
                    {
                        ActionType = FDTrigAction.PlaySoundtrack,
                        OldParam = 17,
                        NewParam = 5,
                    },
                },
            },
        };

        return new() { data };
    }
}
