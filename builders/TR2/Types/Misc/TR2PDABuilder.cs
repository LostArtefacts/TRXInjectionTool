using TRImageControl;
using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Misc;

public class TR2PDABuilder : InjectionBuilder, IPublisher
{
    public override string ID => "tr2_pda";

    public override List<InjectionData> Build()
    {
        var level = CreateLevel();
        var data = InjectionData.Create(level, InjectionType.General, "pda_model");
        return new() { data };
    }

    private static TR2Level CreateLevel()
    {
        var pdaLevel = CreatePDALevel();
        var wall = _control2.Read($"Resources/{TR2LevelNames.GW}");
        ResetLevel(wall);

        wall.Models[TR2Type.Map_M_U] = pdaLevel.Models[TR1Type.Map_M_U].Clone();
        wall.Palette = new(pdaLevel.Palette);
        wall.ObjectTextures.AddRange(pdaLevel.ObjectTextures.Select(o => o.Clone()));
        var tile = new TRImage(TRConsts.TPageWidth, TRConsts.TPageHeight);
        tile.Import(new("Resources/TR2/pda.png"), new(0, 0));
        wall.Images16.Add(new() { Pixels = tile.ToRGB555() });

        GenerateImages8(wall, wall.Palette.Select(c => c.ToTR1Color()).ToList());

        return wall;
    }

    public TRLevelBase Publish()
        => CreateLevel();

    public string GetPublishedName()
        => "pda.tr2";
}
