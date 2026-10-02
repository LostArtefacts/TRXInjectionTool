using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR4.FD;

public class TR4SethFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR4, InjectionType.FDFix, "seth_fd");
        CreateDefaultTests(data, $"TR4/{TR4LevelNames.SETH}");
        data.FloorEdits.AddRange(FixSlot1Camera());

        return [data];
    }

    private static IEnumerable<TRFloorDataEdit> FixSlot1Camera()
    {
        var level = _control4.Read($"Resources/TR4/{TR4LevelNames.SETH}");
        var trigger = GetTrigger(level, 0, 2, 2).Clone() as FDTriggerEntry;
        trigger.TrigType = FDTrigType.Dummy;
        trigger.Actions.RemoveAll(
            a => a.Action != FDTrigAction.Camera && a.Action != FDTrigAction.LookAtItem);

        foreach (var room in new[] { 95, 104 })
        {
            var z = (ushort)(room == 95 ? 2 : 1);
            for (ushort x = 1; x < 4; x++)
            {
                yield return MakeTrigger(level, (short)room, x, z, trigger);
            }
        }
    }
}
