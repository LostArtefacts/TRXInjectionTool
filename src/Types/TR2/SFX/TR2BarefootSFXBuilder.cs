using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.SFX;

public class TR2BarefootSFXBuilder : InjectionBuilder
{
    public override string ID => "barefoot_sfx";

    public override List<InjectionData> Build()
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.GW}");
        var data = InjectionData.Create(TRGameVersion.TR2, InjectionType.PS1SFX, ID);

        var feet = level.SoundEffects[TR2SFX.LaraFeet];
        data.SFX.Add(new()
        {
            ID = (short)TR2SFX.LaraFeet,
            Chance = feet.Chance,
            Characteristics = feet.GetFlags(),
            Volume = feet.Volume,
            Data = [.. Enumerable.Range(0, 4).Select(i => File.ReadAllBytes($"Resources/TR2/Barefoot/{i}.wav"))],
        });

        return [data];
    }
}
