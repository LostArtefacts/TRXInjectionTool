using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.FD;

public class TR1StrongholdFDBuilder : FDBuilder
{
    private static readonly List<short> _windyRooms
        = new() { 4, 13 };

    public override List<InjectionData> Build()
    {
        TR1Level stronghold = _control1.Read($"Resources/{TR1LevelNames.STRONGHOLD}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.FDFix, "stronghold_fd");
        CreateDefaultTests(data, TR1LevelNames.STRONGHOLD);
        data.FloorEdits = new()
        {
            MakeMusicOneShot(4, 17, 6),
            MakeMusicOneShot(4, 17, 7),
            MakeMusicOneShot(4, 17, 8),
            MakeMusicOneShot(4, 17, 9),
            MakeMusicOneShot(13, 13, 7),
            MakeMusicOneShot(13, 13, 8),
            MakeMusicOneShot(17, 1, 7),
            MakeMusicOneShot(20, 5, 2),
            MakeMusicOneShot(25, 3, 4),
        };
        CreatePortalCrashFix(data);
        data.FloorEdits.AddRange(AddRoomFlags(_windyRooms, TRRoomFlag.Wind, stronghold.Rooms));

        return new() { data };
    }

    private static void CreatePortalCrashFix(InjectionData data)
    {
        // Fix the broken portals between rooms 74 and 12, which can cause a crash otherwise.
        TRVertex vertexShift = new() { Y = TRConsts.Step1 };
        List<TRVertex> changes = Enumerable.Repeat(vertexShift, 4).ToList();

        for (ushort portal = 1; portal < 4; portal++)
        {
            data.VisPortalEdits.Add(new()
            {
                BaseRoom = 74,
                LinkRoom = 12,
                PortalIndex = portal,
                VertexChanges = changes,
            });
        }

        // Fix the z-fighting on textures around the portal too.
        ushort[] vertices = new ushort[]
        {
            582, 583, 584, 585,
            592, 593, 594, 595,
            702, 703, 704, 705,
        };
        foreach (ushort vertex in vertices)
        {
            data.RoomEdits.Add(new TRRoomVertexMove
            {
                RoomIndex = 74,
                VertexIndex = vertex,
                VertexChange = vertexShift,
            });
        }
    }
}
