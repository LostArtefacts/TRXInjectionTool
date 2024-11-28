using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.FD;

public class TR2WreckFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level wreck = _control2.Read($"Resources/{TR2LevelNames.DORIA}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.FDFix, "wreck_fd");

        // Flood room 98 where the unreachable shark is.
        data.FloorEdits.Add(new()
        {
            RoomIndex = 98,
            Fixes = new()
            {
                new FDRoomProperties
                {
                    Flags = wreck.Rooms[98].Flags | TRRoomFlag.Water,
                }
            }
        });

        return new() { data };
    }
}
