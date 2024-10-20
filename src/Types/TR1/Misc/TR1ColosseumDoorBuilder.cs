using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Misc;

public class TR1ColosseumDoorBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level colosseum = _control1.Read($@"Resources\{TR1LevelNames.COLOSSEUM}");
        TRModel door = colosseum.Models[TR1Type.Door2];

        ResetLevel(colosseum);

        colosseum.Models[TR1Type.Door2] = door;
        door.Animations[1].Frames[0].Rotations[0].Y = door.Animations[1].Frames[1].Rotations[0].Y;

        InjectionData data = InjectionData.Create(colosseum, InjectionType.General, "colosseum_door", true);
        return new() { data };
    }
}
