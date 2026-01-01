using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.FD;

public class TR2GymFDBuilder : FDBuilder
{
    public override string ID => "gym_fd";

    public override List<InjectionData> Build()
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.ASSAULT}");
        var data = InjectionData.Create(TRGameVersion.TR2, InjectionType.FDFix, ID);
        CreateDefaultTests(data, TR2LevelNames.ASSAULT);
        data.FloorEdits.AddRange(FixZipTriggers(level));
        data.FloorEdits.Add(RemoveInitialSpeechTrigger(level));

        return new() { data };
    }

    private static TRFloorDataEdit RemoveInitialSpeechTrigger(TR2Level level)
    {
        // Remove the opening voice line for Lara: for LostArtefacts/TRX#2822,
        // we play different tracks conditionally via LUA.
        var trig = GetTrigger(level, 16, 4, 6);
        trig.Actions.RemoveAll(a => a.Action == FDTrigAction.PlaySoundtrack);
        return MakeTrigger(level, 16, 4, 6, trig);
    }

    private static IEnumerable<TRFloorDataEdit> FixZipTriggers(TR2Level level)
    {
        var trigger = new FDTriggerEntry
        {
            Mask = 31,
            Actions = new()
            {
                new() { Parameter = (short)level.Entities
                    .FindIndex(e => e.TypeID == TR2Type.ZiplineHandle) },
            },
        };
        for (ushort z = 1; z < 4; z++)
        {
            yield return MakeTrigger(level, 42, 3, z, trigger);
        }
    }
}
