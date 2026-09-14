using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.FD;

public class TR2WreckFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.DORIA}");
        var data = InjectionData.Create(TRGameVersion.TR2, InjectionType.FDFix, "wreck_fd");
        CreateDefaultTests(data, TR2LevelNames.DORIA);

        data.FloorEdits.Add(FixDrySharkRoom(level));
        data.FloorEdits.AddRange(FixCollapsibleTileTriggers(level));

        return [data];
    }

    private static TRFloorDataEdit FixDrySharkRoom(TR2Level level)
    {
        // Flood room 98 where the unreachable shark is.
        return new()
        {
            RoomIndex = 98,
            Fixes = [new FDRoomProperties
            {
                Flags = level.Rooms[98].Flags | TRRoomFlag.Water,
            }],
        };
    }

    private static IEnumerable<TRFloorDataEdit> FixCollapsibleTileTriggers(TR2Level level)
    {
        // Room 70 has triggers for collapsible tiles, two of which also hold one-shot music
        // triggers. Jumping over these means the tiles can never break. Move the music into
        // a heavy trigger activated by the barrels instead.
        for (ushort z = 1; z < 3; z++)
        {
            var trigger = GetTrigger(level, 70, 2, z).Clone() as FDTriggerEntry;
            trigger.Actions.RemoveAll(t => t.Action == FDTrigAction.PlaySoundtrack);
            trigger.OneShot = false;
            yield return MakeTrigger(level, 70, 2, z, trigger);
        }

        yield return MakeTrigger(level, 69, 2, 2, new FDTriggerEntry
        {
            Mask = 31,
            TrigType = FDTrigType.HeavyTrigger,
            OneShot = true,
            Actions = [new()
            {
                Action = FDTrigAction.PlaySoundtrack,
                Parameter = 32,
            }],
        });

        {
            // The second row of triggers is missing one of the barrels as a target.
            var trigger = GetTrigger(level, 70, 1, 2).Clone() as FDTriggerEntry;
            trigger.Actions.Add(new() { Parameter = 123 });
            yield return MakeTrigger(level, 70, 1, 2, trigger);
        }
    }
}
