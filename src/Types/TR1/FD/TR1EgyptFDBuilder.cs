using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.FD;

public class TR1EgyptFDBuilder : FDBuilder
{
    private static readonly List<short> _windyRooms
        = new() { 19, 20, 22, 23, 74, 75, 76, 77, 78, 79, 80 };

    public override List<InjectionData> Build()
    {
        TR1Level egypt = _control1.Read($"Resources/{TR1LevelNames.EGYPT}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.FDFix, "egypt_fd");
        CreateDefaultTests(data, TR1LevelNames.EGYPT);
        data.FloorEdits = new()
        {
            MakeMusicOneShot(0, 3, 3),
            MakeMusicOneShot(0, 4, 3),
            MakeMusicOneShot(0, 5, 3),
            MakeMusicOneShot(19, 10, 7),
            MakeMusicOneShot(22, 6, 4),
            MakeMusicOneShot(22, 6, 5),
            MakeMusicOneShot(22, 7, 4),
            MakeMusicOneShot(22, 7, 5),
            MakeMusicOneShot(61, 5, 1),
            MakeMusicOneShot(93, 2, 2),
        };
        data.FloorEdits.AddRange(AddRoomFlags(_windyRooms, TRRoomFlag.Wind, egypt.Rooms));

        return new() { data };
    }
}
