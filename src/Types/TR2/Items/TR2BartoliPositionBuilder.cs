using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Items;

public class TR2BartoliPositionBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level lair = _control2.Read($"Resources/{TR2LevelNames.LAIR}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.General, "lair_bartolipos");
        CreateDefaultTests(data, TR2LevelNames.LAIR);

        TR2Entity bartoli = lair.Entities[43];        
        data.ItemEdits.Add(new()
        {
            Index = 43,
            Item = new()
            {
                Angle = bartoli.Angle,
                X = bartoli.X - TRConsts.Step2,
                Y = bartoli.Y,
                Z = bartoli.Z - TRConsts.Step2,
                Room = bartoli.Room,
            },
        });

        return new() { data };
    }
}
