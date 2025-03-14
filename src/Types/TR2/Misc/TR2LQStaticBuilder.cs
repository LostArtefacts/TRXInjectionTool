using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Misc;

public class TR2LQStaticBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level living = _control2.Read($"Resources/{TR2LevelNames.LQ}");
        TRStaticMesh mesh = living.StaticMeshes[TR2Type.SceneryBase + 16];
        mesh.CollisionBox = new();
        mesh.NonCollidable = true;

        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.General, "seaweed_collision");
        data.StaticMeshEdits.Add(new()
        {
            TypeID = 16,
            Mesh = mesh,
        });

        return new() { data };
    }
}
