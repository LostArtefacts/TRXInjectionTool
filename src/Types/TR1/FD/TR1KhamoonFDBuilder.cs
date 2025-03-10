using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.FD;

public class TR1KhamoonFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level khamoon = _control1.Read($"Resources/{TR1LevelNames.KHAMOON}");
        List<InjectionData> dataGroup = new();

        {
            InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.FDFix, "khamoon_fd");
            CreateDefaultTests(data, TR1LevelNames.KHAMOON);
            data.FloorEdits = CreateTrapdoorTriggers(khamoon);
            dataGroup.Add(data);
        }

        {
            InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.Item, "khamoon_mummy");
            CreateDefaultTests(data, TR1LevelNames.KHAMOON);
            data.FloorEdits = CreatePsxMummy(khamoon);
            dataGroup.Add(data);
        }

        return dataGroup;
    }

    private static List<TRFloorDataEdit> CreateTrapdoorTriggers(TR1Level khamoon)
    {
        List<TRFloorDataEdit> edits = new()
        {
            // Convert the triggers in rooms 31 and 34 to dummy type.
            MakeTrigger(khamoon, 31, 2, 5, new()
            {
                TrigType = FDTrigType.Dummy,
                Mask = TRConsts.FullMask,
                Actions = new()
                {
                    new() { Parameter = 33 },
                },
            }),
            MakeTrigger(khamoon, 31, 3, 5, new()
            {
                TrigType = FDTrigType.Dummy,
                Mask = TRConsts.FullMask,
                Actions = new()
                {
                    new() { Parameter = 34 },
                },
            }),

            MakeTrigger(khamoon, 34, 1, 7, new()
            {
                TrigType = FDTrigType.Dummy,
                Mask = TRConsts.FullMask,
                Actions = new()
                {
                    new() { Parameter = 40 },
                },
            }),
            MakeTrigger(khamoon, 34, 2, 7, new()
            {
                TrigType = FDTrigType.Dummy,
                Mask = TRConsts.FullMask,
                Actions = new()
                {
                    new() { Parameter = 42 },
                },
            }),
            MakeTrigger(khamoon, 34, 1, 8, new()
            {
                TrigType = FDTrigType.Dummy,
                Mask = TRConsts.FullMask,
                Actions = new()
                {
                    new() { Parameter = 39 },
                },
            }),
            MakeTrigger(khamoon, 34, 2, 8, new()
            {
                TrigType = FDTrigType.Dummy,
                Mask = TRConsts.FullMask,
                Actions = new()
                {
                    new() { Parameter = 41 },
                },
            }),

            // Add dummy triggers in rooms 10/18 in case the player reaches
            // room 26 before the flipmap below.
            MakeTrigger(khamoon, 10, 5, 5, new()
            {
                TrigType = FDTrigType.Dummy,
                Mask = TRConsts.FullMask,
                Actions = new()
                {
                    new() { Parameter = 33 },
                },
            }),
            MakeTrigger(khamoon, 18, 6, 5, new()
            {
                TrigType = FDTrigType.Dummy,
                Mask = TRConsts.FullMask,
                Actions = new()
                {
                    new() { Parameter = 34 },
                },
            }),
        };

        return edits;
    }

    private static List<TRFloorDataEdit> CreatePsxMummy(TR1Level khamoon)
    {
        // Add the mummy from the PS version and trigger it.
        TR1Entity mummy = new()
        {
            TypeID = TR1Type.NonShootingAtlantean_N,
            Room = 25,
            X = 43520,
            Y = -4096,
            Z = 34304,
            Intensity = -1,
            Flags = 256,
        };
        khamoon.Entities.Add(mummy);

        List<TRFloorDataEdit> edits = new()
        {
            new()
            {
                RoomIndex = 25,
                Fixes = new()
                {
                    new FDTrigItem
                    {
                        Item = mummy,
                    }
                },
            }
        };

        short[] trigRooms = new short[] { 20, 29 };
        foreach (short room in trigRooms)
        {
            edits.Add(MakeTrigger(khamoon, room, 3, 8, new()
            {
                Mask = TRConsts.FullMask,
                Actions = new()
                {
                    new() { Parameter = (short)(khamoon.Entities.Count - 1) },
                }
            }));
        }

        return edits;
    }
}
