using TRDataControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.SFX;

public class TR2GoonSFXBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level rig = _control2.Read($"Resources/{TR2LevelNames.RIG}");

        List<TR2Type> goons = new()
        {
            TR2Type.StickWieldingGoon1,
            TR2Type.Gunman1,
        };

        List<TR2SFX> soundIDs = new();
        TR2DataProvider tr2Data = new();
        foreach (TR2Type type in goons)
        {
            TRModel model = rig.Models[type];
            soundIDs.AddRange(model.Animations
                .SelectMany(a => a.Commands.Where(c => c is TRSFXCommand))
                .Select(s => (TR2SFX)((TRSFXCommand)s).SoundID));

            TR2Type alias = TR2TypeUtilities.GetAliasForLevel(TR2LevelNames.RIG, type);
            soundIDs.AddRange(tr2Data.GetHardcodedSounds(alias));
        }

        soundIDs.RemoveAll(s => !rig.SoundEffects.ContainsKey(s));

        List<InjectionData> result = new();
        Dictionary<string, string> targetLevels = new()
        {
            [TR2LevelNames.FATHOMS] = "fathoms_goon_sfx",
            [TR2LevelNames.DORIA] = "wreck_goon_sfx",
            [TR2LevelNames.LQ] = "living_deck_goon_sfx",
        };

        foreach (var (lvl, binName) in targetLevels)
        {
            TR2Level level = _control2.Read($"Resources/{lvl}");
            TRDictionary<TR2SFX, TR2SoundEffect> effects = new();
            soundIDs
                .Where(s => !level.SoundEffects.ContainsKey(s))
                .Distinct()
                .ToList()
                .ForEach(s => effects[s] = rig.SoundEffects[s]);

            ResetLevel(level);
            level.SoundEffects = effects;
            
            result.Add(InjectionData.Create(level, InjectionType.General, binName));
        }

        result.Add(FixColdWarShark());
        return result;
    }

    private static InjectionData FixColdWarShark()
    {
        TR2Level fathoms = _control2.Read($"Resources/{TR2LevelNames.FATHOMS}");
        TRModel shark = fathoms.Models[TR2Type.Shark];
        ResetLevel(fathoms);
        fathoms.Models[TR2Type.Shark] = shark;

        InjectionData data = InjectionData.Create(fathoms, InjectionType.General, "shark_sfx", true);
        data.Animations.Clear();
        data.AnimFrames.Clear();
        data.AnimChanges.Clear();
        data.AnimDispatches.Clear();
        data.Models.Clear();

        for (int i = 0; i < shark.Animations.Count; i++)
        {
            int count = shark.Animations[i].Commands.Count(c => c is TRSFXCommand);
            if (count > 0)
            {
                data.AnimCmdEdits.Add(new()
                {
                    TypeID = (int)TR2Type.Shark,
                    AnimIndex = i,
                    RawCount = 3,
                    TotalCount = count,
                });
            }
        }

        return data;
    }
}
