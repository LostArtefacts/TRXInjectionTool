using System.Diagnostics;
using System.Drawing;
using TRDataControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Lara;

public class TR1LaraFlareBuilder : InjectionBuilder, IPublisher
{
    public override string ID => "lara_flares";

    public override List<InjectionData> Build()
    {
        return
        [
            CreateData(CreateLevel(false), ID),
            CreateData(CreateLevel(true), "lara_gym_flares"),
        ];
    }

    private static TR1Level CreateLevel(bool gym)
    {
        var level = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
        var lara = level.Models[TR1Type.Lara];
        ResetLevel(level, 1);
        level.Models[TR1Type.Lara] = lara;

        var dataDir = "Resources/TR1/Lara";
        if (gym)
        {
            var gymDir = dataDir + "/Gym";
            foreach (var dep in new TR1DataProvider().GetDependencies(TR1Type.LaraFlareAnim_H))
            {
                var file = Path.Combine(dataDir, TR1TypeUtilities.GetName(dep) + ".trb");
                File.Copy(file, Path.Combine(gymDir, Path.GetFileName(file)), true);
            }
            dataDir = gymDir;
        }

        new TR1DataImporter
        {
            Level = level,
            DataFolder = dataDir,
            TypesToImport = [TR1Type.LaraFlareAnim_H],
        }.Import();
        level.Models.Remove(TR1Type.Lara);
        
        if (gym)
        {
            var col = Color.FromArgb(220, 160, 100);
            var i = level.Palette.FindIndex(c => c.Red == col.R && c.Green == col.G && c.Blue == col.B);
            Debug.Assert(i > 0);
            level.Models[TR1Type.LaraFlareAnim_H].Meshes[13].ColouredRectangles[0].Texture = (ushort)i;
        }

        return level;
    }

    private static InjectionData CreateData(TR1Level level, string id)
    {
        var data = InjectionData.Create(level, InjectionType.General, id);

        var sfxLevel = _control3.Read($"Resources/{TR3LevelNames.JUNGLE}");
        var soundMap = new Dictionary<TR3SFX, short>
        {
            [TR3SFX.LaraFlareIgnite] = 257,
            [TR3SFX.LaraFlareBurn] = 258,
        };

        foreach (var (tr2Id, tr1Id) in soundMap)
        {
            var sfx = sfxLevel.SoundEffects[tr2Id];
            data.SFX.Add(new()
            {
                ID = tr1Id,
                Chance = sfx.Chance,
                Characteristics = 1 << 2, // mode=wait
                Volume = 13106,
                SampleOffset = sfx.SampleID,
            });
            data.SFX[^1].LoadSFX(TRGameVersion.TR3);
        }

        return data;
    }

    public string GetPublishedName()
        => "flares.phd";

    public TRLevelBase Publish()
        => CreateLevel(false);
}
