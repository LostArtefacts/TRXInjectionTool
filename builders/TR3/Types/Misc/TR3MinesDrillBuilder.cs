using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Misc;

public class TR3MinesDrillBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        // If Lara hits a drillbit on the minecart, it's actually the static mesh on the
        // wall that deals the damage. OG has a degenerate hitbox though, so adjust it to
        // avoid it becoming non-collidable.
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.RXTECH}");
        var mesh = level.StaticMeshes[TR3Type.SceneryBase + 34];
        mesh.CollisionBox.MaxX++;
        mesh.CollisionBox.MaxY++;
        mesh.CollisionBox.MaxZ++;

        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.General, "drill_collision");
        data.StaticMeshEdits.Add(new()
        {
            TypeID = 34,
            Mesh = mesh,
        });

        return [data];
    }
}
