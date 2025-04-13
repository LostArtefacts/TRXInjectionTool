using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.FD;

public  class TR2VegasFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level vegas = _control2.Read($"Resources/{TR2LevelNames.VEGAS}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.FDFix, "vegas_fd");
        CreateDefaultTests(data, TR2LevelNames.VEGAS);

        // Add additional bird monster triggers near the start and flip-on(1) triggers to avoid
        // softlock (re-entering stage area turns off the flip-map, nothing else can turn it back on).
        foreach (ushort z in new ushort[] { 1, 2 })
        {
            data.FloorEdits.Add(MakeTrigger(vegas, 27, 4, z, new()
            {
                Mask = 31,
                Actions = new()
                {
                    new() { Parameter = 6, },
                }
            }));
        }

        for (ushort x = 2; x < 10; x++)
        {
            data.FloorEdits.Add(MakeTrigger(vegas, 33, x, 11, new()
            {
                Mask = 31,
                Actions = new()
                {
                    new() { Action = FDTrigAction.FlipOn, Parameter = 1, },
                }
            }));
        }

        foreach (ushort x in new ushort[] { 5, 6 })
        {
            data.FloorEdits.Add(MakeTrigger(vegas, 35, x, 11, new()
            {
                Mask = 31,
                Actions = new()
                {
                    new() { Action = FDTrigAction.FlipOn, Parameter = 1, },
                }
            }));
        }

        foreach (ushort x in new ushort[] { 2, 5 })
        {
            data.FloorEdits.Add(MakeTrigger(vegas, 72, x, 1, new()
            {
                Mask = 31,
                Actions = new()
                {
                    new() { Action = FDTrigAction.FlipOff, Parameter = 0, },
                }
            }));
        }

        return new() { data };
    }
}
