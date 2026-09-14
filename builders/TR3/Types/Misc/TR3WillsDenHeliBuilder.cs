using System.Diagnostics;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Misc;

public class TR3WillsDenHeliBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.LAIR}");
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.General, "willsden_heli");

        // The helicopter cannot update its room because the original position means portal traversal is
        // impossible. Force the room num instead and disable an out-of-bounds portal in the opposite
        // direction to prevent it reverting to the original room.
        var heliIdx = (short)level.Entities.FindIndex(e => e.TypeID == TR3Type.Animating4);
        Debug.Assert(heliIdx != -1);
        data.ItemPosEdits.Add(ItemBuilder.MoveToRoom(level, heliIdx, 10));
        data.FloorEdits.Add(new()
        {
            RoomIndex = 15,
            X = 7,
            Z = 5,
            Fixes = [new FDPortalOverwrite()],
        });

        return [data];
    }
}
