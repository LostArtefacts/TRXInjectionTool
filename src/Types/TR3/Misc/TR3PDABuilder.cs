using TRImageControl;
using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Misc;

public class TR3PDABuilder : InjectionBuilder, IPublisher
{
    public override string ID => "tr3_pda";

    public override List<InjectionData> Build()
    {
        var level = CreateLevel();
        var data = InjectionData.Create(level, InjectionType.General, "pda_model");
        return [data];
    }

    private static TR3Level CreateLevel()
    {
        var pdaLevel = CreatePDALevel();
        var level = _control3.Read($"Resources/{TR3LevelNames.JUNGLE}");
        ResetLevel(level);

        level.Models[TR3Type.Map_H] = pdaLevel.Models[TR1Type.Map_M_U].Clone();
        level.Palette = [.. pdaLevel.Palette];
        level.ObjectTextures.AddRange(pdaLevel.ObjectTextures.Select(o => o.Clone()));
        var tile = new TRImage(TRConsts.TPageWidth, TRConsts.TPageHeight);
        tile.Import(new("Resources/TR3/pda.png"), new(0, 0));
        level.Images16.Add(new() { Pixels = tile.ToRGB555() });

        GenerateImages8(level, [.. level.Palette.Select(c => c.ToTR1Color())]);

        return level;
    }

    public TRLevelBase Publish()
        => CreateLevel();

    public string GetPublishedName()
        => "pda.tr2";
}
