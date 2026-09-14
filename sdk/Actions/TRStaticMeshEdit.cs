using TRLevelControl;
using TRLevelControl.Model;

namespace TRXInjectionTool.Actions;

public class TRStaticMeshEdit
{
    public int TypeID { get; set; }
    public TRStaticMesh Mesh { get; set; }

    public void Serialize(TRLevelWriter writer)
    {
        writer.Write(TypeID);
        writer.Write((byte)(Mesh.NonCollidable ? 0 : 1));
        writer.Write((byte)(Mesh.Visible ? 1 : 0));
        writer.Write(Mesh.CollisionBox);
        writer.Write(Mesh.VisibilityBox);
    }
}
