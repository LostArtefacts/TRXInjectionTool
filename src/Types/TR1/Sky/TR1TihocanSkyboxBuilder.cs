using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Sky;

public class TR1TihocanSkyboxBuilder : TR1ColosseumSkyboxBuilder
{
    private static readonly List<short> _skyRooms = [17, 38, 109];

    public override string ID => "tihocan_skybox";

    public override List<InjectionData> Build()
    {
        var level = CreateBaseLevel();
        var data = InjectionData.Create(level, InjectionType.Skybox, ID);
        CreateDefaultTests(data, TR1LevelNames.TIHOCAN);

        var tihocan = _control1.Read($"Resources/{TR1LevelNames.TIHOCAN}");
        data.FloorEdits.AddRange(FDBuilder.AddRoomFlags(_skyRooms, TRRoomFlag.Skybox, tihocan.Rooms));

        return [data];
    }
}
