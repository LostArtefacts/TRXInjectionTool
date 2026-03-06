using TRLevelControl;
using TRLevelControl.Model;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Actions;

public class TRAnimEdit
{
    public uint ModelID { get; set; }
    public int AnimIdx { get; set; }
    public TRAnimation Anim { get; set; }

    public void Serialize(TRLevelWriter writer, TRGameVersion version)
    {
        writer.Write((int)ModelID, TRObjectType.Game, version);
        writer.Write(AnimIdx);
        writer.Write(Anim.Speed);
    }
}
