using TRImageControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TRX.Misc;

public class CarpetBuilder : InjectionBuilder
{
    public override string ID => "inv_background";

    private record Setup(int TypeID, Func<TR2Level, int, TRLevelBase> Convert)
    {
        public int TypeID { get; set; } = TypeID;
        public Func<TR2Level, int, TRLevelBase> Convert { get; set; } = Convert;
    }

    private static readonly List<Setup> _converters =
    [
        new(293, ConvertToTR1),
        new(430, ConvertToTR3),
    ];

    public override List<InjectionData> Build()
    {
        var baseLevel = _control2.Read($"Resources/{TR2LevelNames.GW}");
        CreateModelLevel(baseLevel, TR2Type.MenuBackground_H);
        return [.. _converters.Select(converter =>
        {
            var convLevel = converter.Convert(baseLevel, converter.TypeID);
            var data = InjectionData.Create(convLevel, InjectionType.General, ID);
            data.Images.AddRange(baseLevel.Images16.Select(i =>
            {
                var img = new TRImage(i.Pixels);
                return new TRTexImage32 { Pixels = img.ToRGBA() };
            }));
            return data;
        })];
    }

    private static TR1Level ConvertToTR1(TR2Level level, int typeID)
    {
        var caves = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        ResetLevel(caves);
        caves.Models[(TR1Type)typeID] = level.Models[TR2Type.MenuBackground_H];
        caves.ObjectTextures.AddRange(level.ObjectTextures);
        return caves;
    }

    private static TR3Level ConvertToTR3(TR2Level level, int typeID)
    {
        var jungle = _control3.Read($"Resources/TR3/{TR3LevelNames.JUNGLE}");
        ResetLevel(jungle);
        jungle.Models[(TR3Type)typeID] = level.Models[TR2Type.MenuBackground_H];
        jungle.ObjectTextures.AddRange(level.ObjectTextures);
        return jungle;
    }
}
