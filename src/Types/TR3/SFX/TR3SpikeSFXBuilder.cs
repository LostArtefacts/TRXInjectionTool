using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.SFX;

public class TR3SpikeSFXBuilder : InjectionBuilder
{
    private static readonly Dictionary<string, TR3SFX> _sfxMap = new()
    {
        [TR3LevelNames.COASTAL] = TR3SFX.ShivaSword2,
        [TR3LevelNames.MADUBU] = TR3SFX.LaraGetout,
    };

    public override List<InjectionData> Build()
        => [.. _sfxMap.Select(kvp => AddSpikeSFX(kvp.Key, kvp.Value))];

    private static InjectionData AddSpikeSFX(string levelName, TR3SFX sfx)
    {
        var level = _control3.Read($"Resources/TR3/{levelName}");
        var spikes = level.Models[TR3Type.TeethSpikesOrBarbedWire];
        ResetLevel(level);
        level.Models[TR3Type.TeethSpikesOrBarbedWire] = spikes;

        spikes.Animations[0].Commands.Add(new TRSFXCommand
        {
            FrameNumber = 1,
            SoundID = (short)sfx,
        });

        var data = InjectionData.Create(level, InjectionType.General, $"{_tr3NameMap[levelName]}_spike_sfx");
        data.Animations.Clear();
        data.AnimFrames.Clear();
        data.AnimChanges.Clear();
        data.AnimDispatches.Clear();
        data.Models.Clear();

        data.AnimCmdEdits.Add(new()
        {
            TypeID = (int)TR3Type.TeethSpikesOrBarbedWire,
            RawCount = data.AnimCommands.Count,
            TotalCount = spikes.Animations[0].Commands.Count,
        });

        return data;
    }
}
