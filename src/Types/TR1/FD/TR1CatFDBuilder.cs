using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.FD;

public class TR1CatFDBuilder : FDBuilder
{
    private static readonly List<short> _windyRooms
        = new() { 0, 2, 4, 23, 31, 74, 75, 77, 98, 101, 102, 105 };

    public override List<InjectionData> Build()
    {
        TR1Level cat = _control1.Read($"Resources/{TR1LevelNames.CAT}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.FDFix, "cat_fd");
        CreateDefaultTests(data, TR1LevelNames.CAT);
        data.FloorEdits = new()
        {
            MakeMusicOneShot(14, 1, 1),
            MakeMusicOneShot(14, 1, 2),
            MakeMusicOneShot(98, 3, 5),
            MakeMusicOneShot(100, 3, 2),
            MakeMusicOneShot(100, 3, 3),
            MakeMusicOneShot(100, 3, 4),
        };
        data.FloorEdits.AddRange(AddRoomFlags(_windyRooms, TRRoomFlag.Wind, cat.Rooms));

        return new() { data };
    }
}
