using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Items;

public class TR2OperaItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level opera = _control2.Read($"Resources/{TR2LevelNames.OPERA}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.ItemRotation, "opera_itemrots");
        CreateDefaultTests(data, TR2LevelNames.OPERA);

        data.ItemPosEdits = new()
        {
            SetAngle(opera, 118, 16384),
            SetAngle(opera, 82, 16384),
            // Move the switch to the correct room
            new()
            {
                Index = 57,
                Item = new()
                {
                    X = 72192,
                    Y = 7936,
                    Z = 50688,
                    Angle = 16384,
                    Room = 7,
                    Intensity = -1,
                },
            }
        };

        return new() { data };
    }
}
