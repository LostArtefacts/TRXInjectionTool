using System.Drawing;
using TRLevelControl;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;

namespace TRXInjectionTool.Util;

public static class CrystalUtils
{
    public static readonly Color Blue = Color.FromArgb(188, 220, 220);
    public static readonly Color Purple = Color.FromArgb(64, 64, 252);

    public static Dictionary<string, List<TR1Entity>> GetLocations<T>(string path, T type)
    {
        var locations = JsonUtils.DeserializeFile<Dictionary<string, List<Location>>>(path);
        var crystals = new Dictionary<string, List<TR1Entity>>();
        foreach (var (levelName, levelLocs) in locations)
        {
            crystals[levelName] = [.. levelLocs.Select(l => new TR1Entity
            {
                TypeID = (TR1Type)(uint)(object)type,
                X = l.X,
                Y = l.Y,
                Z = l.Z,
                Room = l.Room,
                Intensity = -1,
            })];
        }

        // The location list is managed through trview, so ensure it is reformatted
        // to avoid line bloat. This assumes the given path matches repo structure.
        JsonUtils.Serialize(locations, $"../../{path}");
        return crystals;
    }

    public static IEnumerable<TRFloorDataEdit> ConvertItems(List<TR1Entity> items, Func<short, TRRoomInfo> getRoomInfo)
    {
        return items.Select(item => new TRFloorDataEdit
        {
            RoomIndex = item.Room,
            X = (ushort)((item.X - getRoomInfo(item.Room).X) / TRConsts.Step4),
            Z = (ushort)((item.Z - getRoomInfo(item.Room).Z) / TRConsts.Step4),
            Fixes = [new FDTrigItem
            {
                Item = item,
            }],
        });
    }
}
