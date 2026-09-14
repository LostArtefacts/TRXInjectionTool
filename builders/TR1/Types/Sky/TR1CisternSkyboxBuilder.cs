using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Sky;

public class TR1CisternSkyboxBuilder : TR1ColosseumSkyboxBuilder
{
    private static readonly List<short> _skyRooms = [3, 47, 52, 74];

    public override string ID => "cistern_skybox";

    public override List<InjectionData> Build()
    {
        var level = CreateBaseLevel();
        var data = InjectionData.Create(level, InjectionType.Skybox, ID);
        CreateDefaultTests(data, TR1LevelNames.CISTERN);

        var cistern = _control1.Read($"Resources/{TR1LevelNames.CISTERN}");
        data.FloorEdits.AddRange(FDBuilder.AddRoomFlags(_skyRooms, TRRoomFlag.Skybox, cistern.Rooms));

        return [data];
    }
}
