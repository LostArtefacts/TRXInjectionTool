using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.FD;

public class TR2CutsceneFDBuilder : FDBuilder
{
    private static readonly List<string> _cutLevels = new()
    {
        TR2LevelNames.GW_CUT,
        TR2LevelNames.OPERA_CUT,
        TR2LevelNames.DA_CUT,
        TR2LevelNames.XIAN_CUT,
    };

    public override List<InjectionData> Build()
    {
        List<InjectionData> cutData = new();

        foreach (string cutLevel in _cutLevels)
        {
            TR2Level cutscene = _control2.Read($"Resources/{cutLevel}");
            InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.General,
                $"{Path.GetFileNameWithoutExtension(cutLevel).ToLower()}_fd");
            CreateDefaultTests(data, cutLevel);
            cutData.Add(data);

            data.FloorEdits.AddRange(cutscene.Rooms.Select((r, i) =>
                new TRFloorDataEdit
                {
                    RoomIndex = (short)i,
                    Fixes = new()
                    {
                        new FDRoomProperties
                        {
                            Flags = r.Flags | TRRoomFlag.Skybox,
                        }
                    },
                }));
        }

        return cutData;
    }
}
