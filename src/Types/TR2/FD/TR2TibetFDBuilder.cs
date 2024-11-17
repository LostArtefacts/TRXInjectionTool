using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.FD;

public class TR2TibetFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level tibet= _control2.Read($"Resources/{TR2LevelNames.TIBET}");

        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.FDFix, "tibet_fd");
        data.FloorEdits.AddRange(FixDrawbridgeFlipmap(tibet));

        return new() { data };
    }

    private static List<TRFloorDataEdit> FixDrawbridgeFlipmap(TR2Level tibet)
    {
        List<TRFloorDataEdit> edits = new()
        {
            // Add dummy triggers for the drawbridges in (flipped) room 96
            MakeTrigger(tibet, 96, 6, 15, new()
            {
                TrigType = FDTrigType.Dummy,
                Mask = TRConsts.FullMask,
                Actions = new()
                {
                    new() { Parameter = 38 },
                },
            }),
            MakeTrigger(tibet, 96, 7, 15, new()
            {
                TrigType = FDTrigType.Dummy,
                Mask = TRConsts.FullMask,
                Actions = new()
                {
                    new() { Parameter = 38 },
                },
            }),
            MakeTrigger(tibet, 96, 8, 15, new()
            {
                TrigType = FDTrigType.Dummy,
                Mask = TRConsts.FullMask,
                Actions = new()
                {
                    new() { Parameter = 39 },
                },
            }),
            MakeTrigger(tibet, 96, 9, 15, new()
            {
                TrigType = FDTrigType.Dummy,
                Mask = TRConsts.FullMask,
                Actions = new()
                {
                    new() { Parameter = 39 },
                },
            }),
        };

        // Copy the key trigger across too
        TRRoomSector sector = tibet.Rooms[63].GetSector(10, 14, TRUnit.Sector);
        edits.Add(MakeTrigger(tibet, 96, 10, 14, tibet.FloorData[sector.FDIndex].Find(e => e is FDTriggerEntry) as FDTriggerEntry));

        // And the music trigger beside the bridge
        sector = tibet.Rooms[64].GetSector(5, 15, TRUnit.Sector);
        edits.Add(MakeTrigger(tibet, 143, 5, 15, tibet.FloorData[sector.FDIndex].Find(e => e is FDTriggerEntry) as FDTriggerEntry));

        return edits;
    }
}
